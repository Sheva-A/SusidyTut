using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SusidyTut.Web.Models;

namespace SusidyTut.Web.Data;

/// <summary>Контекст бази даних сервісу.</summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    /// <summary>Довідник міст.</summary>
    public DbSet<City> Cities => Set<City>();

    /// <summary>Анкети.</summary>
    public DbSet<Profile> Profiles => Set<Profile>();

    /// <summary>Запити на знайомство.</summary>
    public DbSet<ContactRequest> ContactRequests => Set<ContactRequest>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(e =>
        {
            e.Property(u => u.IsBlocked).HasDefaultValue(false);
        });

        builder.Entity<City>(e =>
        {
            e.Property(c => c.Name).IsRequired().HasMaxLength(50);
            e.HasIndex(c => c.Name).IsUnique();
        });

        builder.Entity<Profile>(e =>
        {
            e.ToTable(t => t.HasCheckConstraint("CK_Profiles_Budget", "\"BudgetMin\" <= \"BudgetMax\""));
            e.Property(p => p.Name).IsRequired().HasMaxLength(50);
            e.HasIndex(p => p.UserId).IsUnique();
            e.HasIndex(p => new { p.CityId, p.IsActive });
            e.HasOne(p => p.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<Profile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(p => p.City)
                .WithMany()
                .HasForeignKey(p => p.CityId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ContactRequest>(e =>
        {
            e.ToTable(t => t.HasCheckConstraint("CK_ContactRequests_Users", "\"SenderId\" <> \"ReceiverId\""));
            e.HasIndex(r => new { r.SenderId, r.ReceiverId }).IsUnique();
            e.HasIndex(r => new { r.ReceiverId, r.Status });
            e.HasIndex(r => new { r.SenderId, r.CreatedAt });
            e.HasOne(r => r.Sender)
                .WithMany()
                .HasForeignKey(r => r.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.Receiver)
                .WithMany()
                .HasForeignKey(r => r.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
