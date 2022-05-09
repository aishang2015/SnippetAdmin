using FluentValidation;
using SnippetAdmin.Constants;
using SnippetAdmin.Core;

namespace SnippetAdmin.Models.RBAC.Organization
{
    public class UpdateOrganizationInputModel
    {
        public int? UpId { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string Type { get; set; }

        public string Icon { get; set; }

        public string IconId { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }
    }

    public class UpdateOrganizationInputModelValidator : AbstractValidator<UpdateOrganizationInputModel>
    {
        public UpdateOrganizationInputModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().ConfirmMessage(MessageConstant.ELEMENT_ERROR_0001);
            RuleFor(x => x.Name).MaximumLength(50).ConfirmMessage(MessageConstant.ELEMENT_ERROR_0002);
        }
    }
}