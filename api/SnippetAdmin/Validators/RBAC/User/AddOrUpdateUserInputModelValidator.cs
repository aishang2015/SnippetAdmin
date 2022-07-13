using FluentValidation;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Endpoint.Models.RBAC.User;

namespace SnippetAdmin.Models.RBAC.User
{
    public class AddOrUpdateUserInputModelValidator : AbstractValidator<AddOrUpdateUserInputModel>
    {
        public AddOrUpdateUserInputModelValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().ConfirmMessage(MessageConstant.USER_ERROR_0002);
            RuleFor(x => x.UserName).MaximumLength(20).ConfirmMessage(MessageConstant.USER_ERROR_0003);
            RuleFor(x => x.UserName).Matches("^[A-Za-z0-9]+$").ConfirmMessage(MessageConstant.USER_ERROR_0004);

            RuleFor(x => x.RealName).NotEmpty().ConfirmMessage(MessageConstant.USER_ERROR_0005);
            RuleFor(x => x.RealName).MaximumLength(20).ConfirmMessage(MessageConstant.USER_ERROR_0006);
        }
    }
}