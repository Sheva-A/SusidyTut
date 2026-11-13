using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SusidyTut.Web.Infrastructure;

/// <summary>Обрізає пробіли в рядках; порожній рядок стає null.</summary>
public class TrimStringModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (result == ValueProviderResult.None)
            return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, result);

        var value = result.FirstValue;
        // Паролі не чіпаємо
        if (value is not null && bindingContext.ModelMetadata.DataTypeName != nameof(DataType.Password))
            value = value.Trim();

        bindingContext.Result = ModelBindingResult.Success(string.IsNullOrEmpty(value) ? null : value);
        return Task.CompletedTask;
    }
}

/// <summary>Підключає TrimStringModelBinder для всіх рядків.</summary>
public class TrimStringModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context) =>
        context.Metadata.ModelType == typeof(string) ? new TrimStringModelBinder() : null;
}
