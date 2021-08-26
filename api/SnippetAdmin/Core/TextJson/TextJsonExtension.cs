using Microsoft.Extensions.DependencyInjection;

namespace SnippetAdmin.Core.TextJson
{
    public static class TextJsonExtension
    {
        /// <summary>
        /// 配置System.Text.Json
        /// </summary>
        public static IMvcBuilder AddTextJsonOptions(this IMvcBuilder builder)
        {
            builder.AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            });
            return builder;
        }
    }
}