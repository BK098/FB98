using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace FB98.Shared.Utils.Extensions
{
	public class ModelBinderExtensions : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			if (bindingContext == null)
			{
				throw new ArgumentNullException(nameof(bindingContext));
			}

			var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

			if (valueProviderResult == ValueProviderResult.None)
			{
				bindingContext.Result = ModelBindingResult.Failed();
				return Task.CompletedTask;
			}

			try
			{
				var value = valueProviderResult.FirstValue;
				var result = JsonConvert.DeserializeObject(value, bindingContext.ModelType);
				bindingContext.Result = ModelBindingResult.Success(result);
			}
			catch (Exception ex)
			{
				bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"Invalid JSON format: {ex.Message}");
				bindingContext.Result = ModelBindingResult.Failed();
			}

			return Task.CompletedTask;
		}
	}
}