using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CompanyEmployees.Presentation.ModelBinders;

public class ArrayModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (!bindingContext.ModelMetadata.IsEnumerableType)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        if (valueResult == ValueProviderResult.None)
        {
            bindingContext.Result = ModelBindingResult.Success(null);
            return Task.CompletedTask;
        }

        var providedValue = valueResult.ToString();

        var genericType = bindingContext.ModelType.GenericTypeArguments[0];

        var converter = TypeDescriptor.GetConverter(genericType);

        var objectArray = providedValue.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => converter.ConvertFromInvariantString(x)).ToArray();

        var guidArray = Array.CreateInstance(genericType, objectArray.Length);

        objectArray.CopyTo(guidArray, 0);

        bindingContext.Model = guidArray;

        bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
        return Task.CompletedTask;

    }
}