using Microsoft.EntityFrameworkCore;
using SusidyTut.Web.Data;
using SusidyTut.Web.Models;
using SusidyTut.Web.Models.Enums;
using SusidyTut.Web.Models.ViewModels;

namespace SusidyTut.Web.Services;

/// <summary>Реалізація запитів на знайомство.</summary>
public class ContactRequestService(ApplicationDbContext db, IMatchingService matching) : IContactRequestService
{
    /// <summary>ДВ-04: ліміт запитів за 24 години.</summary>
    public const int DailyLimit = 10;

    /// <inheritdoc />
    public async Task<CandidateDetails?> GetCandidateDetailsAsync(Profile me, int profileId)
    {
        var other = await matching.GetViewableProfileAsync(me, profileId);
        if (other is null)
            return null;

        var request = await FindBetweenAsync(me.UserId, other.UserId);
        var state = RequestStates.From(request, me.UserId);
        // ФК-15: контакти лише при «Прийнято»
        var showContacts = state == RequestState.Accepted;

        return new CandidateDetails
        {
            ProfileId = other.Id,
            UserId = other.UserId,
            Name = other.Name,
            Age = other.Age,
            Gender = other.Gender,
            PreferredGender = other.PreferredGender,
            City = other.City?.Name ?? string.Empty,
            University = other.University,
            Housing = other.Housing,
            BudgetMin = other.BudgetMin,
            BudgetMax = other.BudgetMax,
            SleepSchedule = other.SleepSchedule,
            Cleanliness = other.Cleanliness,
            Guests = other.Guests,
            Noise = other.Noise,
            Smoking = other.Smoking,
            Pets = other.Pets,
            About = other.About,
            Criteria = matching.GetCriteria(me, other),
            Compatibility = matching.CalculateCompatibility(me, other),
            RequestState = state,
            RequestId = request?.Id,
            IncomingMessage = state == RequestState.ReceivedPending ? request!.Message : null,
            Telegram = showContacts ? other.Telegram : null,
            Phone = showContacts ? other.Phone : null
        };
    }

    /// <inheritdoc />
    public async Task<SendResult> SendAsync(Profile sender, string receiverId, string? message)
    {
        // ФК-10: анкета відправника активна
        if (!sender.IsActive)
            return SendResult.ProfileInactive;

        // ФК-10: одержувач — кандидат
        if (!await matching.IsCandidateAsync(sender, receiverId))
            return SendResult.NotCandidate;

        // ДВ-04: один запит на пару
        if (await FindBetweenAsync(sender.UserId, receiverId) is not null)
            return SendResult.AlreadyExists;

        // ДВ-04: ковзні 24 години
        var since = DateTime.UtcNow.AddHours(-24);
        var sentToday = await db.ContactRequests.CountAsync(r => r.SenderId == sender.UserId && r.CreatedAt > since);
        if (sentToday >= DailyLimit)
            return SendResult.LimitReached;

        db.ContactRequests.Add(new ContactRequest
        {
            SenderId = sender.UserId,
            ReceiverId = receiverId,
            Message = string.IsNullOrWhiteSpace(message) ? null : message.Trim(),
            Status = RequestStatus.Pending,
            CreatedAt = DateTime.UtcNow
        });

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Дублікат при подвійному натисканні
            return SendResult.AlreadyExists;
        }

        return SendResult.Sent;
    }

    /// <inheritdoc />
    public async Task<RespondResult> RespondAsync(int requestId, string userId, bool accept)
    {
        var request = await db.ContactRequests
            .Include(r => r.Sender)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        if (request is null || request.Sender!.IsBlocked)
            return RespondResult.NotFound;

        // ДВ-04: відповідає лише одержувач
        if (request.ReceiverId != userId)
            return RespondResult.Forbidden;

        // ДВ-04: кінцеві статуси не змінюються
        if (request.Status != RequestStatus.Pending)
            return RespondResult.NotPending;

        request.Status = accept ? RequestStatus.Accepted : RequestStatus.Declined;
        request.RespondedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return RespondResult.Success;
    }

    /// <inheritdoc />
    public async Task<RequestsOverview> GetOverviewAsync(Profile me)
    {
        // ФК-14: без запитів із заблокованими
        var requests = await db.ContactRequests.AsNoTracking()
            .Where(r => (r.SenderId == me.UserId && !r.Receiver!.IsBlocked)
                        || (r.ReceiverId == me.UserId && !r.Sender!.IsBlocked))
            .OrderByDescending(r => r.CreatedAt)
            .ThenByDescending(r => r.Id)
            .ToListAsync();

        var otherIds = requests
            .Select(r => r.SenderId == me.UserId ? r.ReceiverId : r.SenderId)
            .Distinct()
            .ToList();
        var others = await db.Profiles.AsNoTracking()
            .Where(p => otherIds.Contains(p.UserId))
            .ToDictionaryAsync(p => p.UserId);

        RequestRow? ToRow(ContactRequest r, string otherId) =>
            others.TryGetValue(otherId, out var p)
                ? new RequestRow
                {
                    RequestId = r.Id,
                    OtherProfileId = p.Id,
                    OtherName = p.Name,
                    Compatibility = matching.CalculateCompatibility(me, p),
                    Message = r.Message,
                    CreatedAt = r.CreatedAt,
                    Status = r.Status
                }
                : null;

        var incoming = requests.Where(r => r.ReceiverId == me.UserId)
            .Select(r => ToRow(r, r.SenderId)).OfType<RequestRow>().ToList();
        var sent = requests.Where(r => r.SenderId == me.UserId)
            .Select(r => ToRow(r, r.ReceiverId)).OfType<RequestRow>().ToList();

        // ФК-15: контакти лише з прийнятих запитів
        var contacts = requests
            .Where(r => r.Status == RequestStatus.Accepted)
            .OrderByDescending(r => r.RespondedAt)
            .Select(r => r.SenderId == me.UserId ? r.ReceiverId : r.SenderId)
            .Where(others.ContainsKey)
            .Select(id => others[id])
            .Select(p => new ContactRow { ProfileId = p.Id, Name = p.Name, Telegram = p.Telegram, Phone = p.Phone })
            .ToList();

        return new RequestsOverview { Incoming = incoming, Sent = sent, Contacts = contacts };
    }

    /// <inheritdoc />
    public Task<int> CountPendingIncomingAsync(string userId) =>
        db.ContactRequests.CountAsync(r =>
            r.ReceiverId == userId && r.Status == RequestStatus.Pending && !r.Sender!.IsBlocked);

    private Task<ContactRequest?> FindBetweenAsync(string a, string b) =>
        db.ContactRequests.AsNoTracking().FirstOrDefaultAsync(r =>
            (r.SenderId == a && r.ReceiverId == b) || (r.SenderId == b && r.ReceiverId == a));
}
