using FluentValidation;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Extensions;

namespace SnippetAdmin.Models.System.Dic
{
    public class AddDicTypeInputModel
    {
        public string Name { get; set; }

        public string Code { get; set; }
    }

    public class AddDicTypeInputModelValidator : AbstractValidator<AddDicTypeInputModel>
    {
        public AddDicTypeInputModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().ConfirmMessage(MessageConstant.DICTIONARY_ERROR_0005);
            RuleFor(x => x.Name).MaximumLength(50).ConfirmMessage(MessageConstant.DICTIONARY_ERROR_0006);
            RuleFor(x => x.Code).NotEmpty().ConfirmMessage(MessageConstant.DICTIONARY_ERROR_0007);
            RuleFor(x => x.Code).MaximumLength(80).ConfirmMessage(MessageConstant.DICTIONARY_ERROR_0008);
            RuleFor(x => x.Code).Matches("^[A-Za-z0-9-_]+$").ConfirmMessage(MessageConstant.DICTIONARY_ERROR_0009);
        }
    }
}
