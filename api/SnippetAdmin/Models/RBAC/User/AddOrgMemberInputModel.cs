using FluentValidation;
using SnippetAdmin.Constants;
using SnippetAdmin.Core;

namespace SnippetAdmin.Models.RBAC.User
{
    public class AddOrgMemberInputModel
    {
        public int OrgId { get; set; }

        public int[] UserIds { get; set; }

        public int[] Positions { get; set; }
    }

    public class AddOrgMemberInputModelValidator : AbstractValidator<AddOrgMemberInputModel>
    {
        public AddOrgMemberInputModelValidator()
        {
            RuleFor(x => x.UserIds).NotNull().ConfirmMessage(MessageConstant.USER_ERROR_0001);
            RuleFor(x => x.UserIds).Must(p => p.Length > 0).ConfirmMessage(MessageConstant.USER_ERROR_0001);
        }
    }
}