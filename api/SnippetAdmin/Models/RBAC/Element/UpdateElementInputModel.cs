using FluentValidation;
using SnippetAdmin.Constants;
using SnippetAdmin.Core;

namespace SnippetAdmin.Models.RBAC.Element
{
    public class UpdateElementInputModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Type { get; set; }

        public string Identity { get; set; }

        public string AccessApi { get; set; }
    }

    public class UpdateElementInputModelValidator : AbstractValidator<UpdateElementInputModel>
    {
        public UpdateElementInputModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().ConfirmMessage(MessageConstant.ELEMENT_ERROR_0001);
            RuleFor(x => x.Name).MaximumLength(50).ConfirmMessage(MessageConstant.ELEMENT_ERROR_0002);
            RuleFor(x => x.Type).NotEmpty().ConfirmMessage(MessageConstant.ELEMENT_ERROR_0003);
            RuleFor(x => x.Identity).NotEmpty().ConfirmMessage(MessageConstant.ELEMENT_ERROR_0004);
            RuleFor(x => x.Identity).MaximumLength(80).ConfirmMessage(MessageConstant.ELEMENT_ERROR_0005);
            RuleFor(x => x.Identity).Matches("^[A-Za-z0-9-_]+$").ConfirmMessage(MessageConstant.ELEMENT_ERROR_0006);
        }
    }
}