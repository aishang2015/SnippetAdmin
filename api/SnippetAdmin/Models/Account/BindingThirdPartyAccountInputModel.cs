using FluentValidation;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Extensions;

namespace SnippetAdmin.Models.Account
{
    public record BindingThirdPartyAccountInputModel(
        string UserName,
        string Password,
        string ThirdPartyType,              // 第三方登录类型
        string ThirdPartyInfoCacheKey);     // 第三方获取的信息缓存

    public class BindingThirdPartyAccountInputModelValidator : AbstractValidator<BindingThirdPartyAccountInputModel>
    {
        public BindingThirdPartyAccountInputModelValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().ConfirmMessage(MessageConstant.ACCOUNT_ERROR_0002);
            RuleFor(x => x.Password).NotEmpty().ConfirmMessage(MessageConstant.ACCOUNT_ERROR_0003);
            RuleFor(x => x.ThirdPartyType).NotEmpty().ConfirmMessage(MessageConstant.ACCOUNT_ERROR_0005);
            RuleFor(x => x.ThirdPartyInfoCacheKey).NotEmpty().ConfirmMessage(MessageConstant.ACCOUNT_ERROR_0006);
        }
    }
}