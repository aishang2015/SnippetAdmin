using FluentValidation;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Endpoint.Models.RBAC.Organization;

namespace SnippetAdmin.Models.RBAC.Organization
{
    public class UpdateOrganizationInputModelValidator : AbstractValidator<UpdateOrganizationInputModel>
    {
        public UpdateOrganizationInputModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().ConfirmMessage(MessageConstant.ELEMENT_ERROR_0001);
            RuleFor(x => x.Name).MaximumLength(50).ConfirmMessage(MessageConstant.ELEMENT_ERROR_0002);
        }
    }
}