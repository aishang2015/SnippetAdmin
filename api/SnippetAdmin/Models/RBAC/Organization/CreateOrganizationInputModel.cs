using FluentValidation;
using SnippetAdmin.Constants;
using SnippetAdmin.Core;

namespace SnippetAdmin.Models.RBAC.Organization
{
    public class CreateOrganizationInputModel
    {
        public int? UpId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Icon { get; set; }

        public string IconId { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }
    }

    public class CreateOrganizationInputModelValidator : AbstractValidator<CreateOrganizationInputModel>
    {
        public CreateOrganizationInputModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().ConfirmMessage(MessageConstant.ORGANIZATION_ERROR_0001);
            RuleFor(x => x.Name).MaximumLength(50).ConfirmMessage(MessageConstant.ORGANIZATION_ERROR_0002);
        }
    }
}