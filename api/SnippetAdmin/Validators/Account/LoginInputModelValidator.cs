using FluentValidation;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Endpoint.Models.Account;

namespace SnippetAdmin.Validators.Account
{
    public class LoginInputModelValidator : AbstractValidator<LoginInputModel>
    {
        public LoginInputModelValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().ConfirmMessage(MessageConstant.ACCOUNT_ERROR_0002);
            RuleFor(x => x.Password).NotEmpty().ConfirmMessage(MessageConstant.ACCOUNT_ERROR_0003);
        }
    }
}
