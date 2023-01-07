using FluentValidation;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Endpoint.Models.RBAC.Role;

namespace SnippetAdmin.Models.RBAC.Role
{
	public class AddOrUpdateRoleInputModelValidator : AbstractValidator<AddOrUpdateRoleInputModel>
	{
		public AddOrUpdateRoleInputModelValidator()
		{
			RuleFor(x => x.Name).NotEmpty().ConfirmMessage(MessageConstant.ROLE_ERROR_0001);
			RuleFor(x => x.Name).MaximumLength(20).ConfirmMessage(MessageConstant.ROLE_ERROR_0002);
			RuleFor(x => x.Remark).MaximumLength(200).ConfirmMessage(MessageConstant.ROLE_ERROR_0006);
		}
	}
}