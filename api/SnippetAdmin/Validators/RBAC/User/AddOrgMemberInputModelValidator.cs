using FluentValidation;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Endpoint.Models.RBAC.User;

namespace SnippetAdmin.Models.RBAC.User
{

	public class AddOrgMemberInputModelValidator : AbstractValidator<AddOrgMemberInputModel>
	{
		public AddOrgMemberInputModelValidator()
		{
			RuleFor(x => x.UserIds).NotNull().ConfirmMessage(MessageConstant.USER_ERROR_0001);
			RuleFor(x => x.UserIds).Must(p => p.Length > 0).ConfirmMessage(MessageConstant.USER_ERROR_0001);
		}
	}
}