using FluentValidation;
using FluentValidation.AspNetCore;
using SnippetAdmin.Validators.Common;

namespace SnippetAdmin.Core.Extensions
{
	public static class FluentValidationExtension
	{
		public static IMvcBuilder AddFluentValidation(this IMvcBuilder builder)
		{
			ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
			builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
			builder.Services.AddValidatorsFromAssemblyContaining<PagedInputModelValidator>();

			return builder;
		}

		public static IRuleBuilderOptions<T, TProperty> ConfirmMessage<T, TProperty>(
			this IRuleBuilderOptions<T, TProperty> ruleBuilderOptions,
			(string, string) messageConstant)
		{
			return ruleBuilderOptions.OverridePropertyName(messageConstant.Item1)
				.WithMessage(messageConstant.Item2);
		}
	}
}