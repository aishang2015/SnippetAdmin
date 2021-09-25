using FluentValidation;
using SnippetAdmin.Constants;
using SnippetAdmin.Core;

namespace SnippetAdmin.Models.RBAC.Role
{
    public class AddOrUpdateRoleInputModel
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string Remark { get; set; }

        public int[] Rights { get; set; }
    }


    public class AddOrUpdateRoleInputModelValidator : AbstractValidator<AddOrUpdateRoleInputModel>
    {
        public AddOrUpdateRoleInputModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().ConfirmMessage(MessageConstant.ROLE_ERROR_0001);
            RuleFor(x => x.Name).MaximumLength(20).ConfirmMessage(MessageConstant.ROLE_ERROR_0002);
            RuleFor(x => x.Code).NotEmpty().ConfirmMessage(MessageConstant.ROLE_ERROR_0003);
            RuleFor(x => x.Code).MaximumLength(40).ConfirmMessage(MessageConstant.ROLE_ERROR_0004);
            RuleFor(x => x.Code).Matches("^[A-Za-z0-9-_]+$").ConfirmMessage(MessageConstant.ROLE_ERROR_0005);
            RuleFor(x => x.Remark).MaximumLength(200).ConfirmMessage(MessageConstant.ROLE_ERROR_0006);
        }
    }
}
