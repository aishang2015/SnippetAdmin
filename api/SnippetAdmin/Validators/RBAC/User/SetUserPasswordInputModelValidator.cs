using FluentValidation;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Endpoint.Models.RBAC.User;

namespace SnippetAdmin.Models.RBAC.User
{
	public class SetUserPasswordInputModelValidator : AbstractValidator<SetUserPasswordInputModel>
	{
		public SetUserPasswordInputModelValidator()
		{
			RuleFor(x => x.Password).NotEmpty().ConfirmMessage(MessageConstant.USER_ERROR_0007);
			RuleFor(x => x.Password).MaximumLength(20).ConfirmMessage(MessageConstant.USER_ERROR_0008);

			RuleFor(x => x.ConfirmPassword).NotEmpty().ConfirmMessage(MessageConstant.USER_ERROR_0009);
		}
	}
}