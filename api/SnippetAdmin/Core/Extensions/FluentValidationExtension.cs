using FluentValidation;
using FluentValidation.AspNetCore;
using SnippetAdmin.Core.Extensions;
using System.Reflection;

namespace SnippetAdmin.Core.Extensions
{
    public static class FluentValidationExtension
    {
        public static IMvcBuilder AddFluentValidation(this IMvcBuilder builder)
        {
            ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
            builder.AddFluentValidation(configuration =>
            {
                configuration.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            });

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