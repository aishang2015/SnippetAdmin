using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using SnippetAdmin.Core;
using System.Reflection;

namespace SnippetAdmin.Core
{
    public static class FluentValidationExtension
    {
        public static IMvcBuilder AddFluentValidation(this IMvcBuilder builder)
        {
            builder.AddFluentValidation(configuration =>
            {
                configuration.ValidatorOptions.CascadeMode = CascadeMode.Stop;
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