using FluentValidation;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Endpoint.Models.System.Dic;

namespace SnippetAdmin.Models.System.Dic
{
	public class AddDicValueInputModelValidator : AbstractValidator<AddDicValueInputModel>
	{
		public AddDicValueInputModelValidator()
		{
			RuleFor(x => x.Name).NotEmpty().ConfirmMessage(MessageConstant.DICTIONARY_ERROR_0010);
			RuleFor(x => x.Name).MaximumLength(50).ConfirmMessage(MessageConstant.DICTIONARY_ERROR_0011);
			RuleFor(x => x.Code).NotEmpty().ConfirmMessage(MessageConstant.DICTIONARY_ERROR_0012);
			RuleFor(x => x.Code).MaximumLength(80).ConfirmMessage(MessageConstant.DICTIONARY_ERROR_0013);
			RuleFor(x => x.Code).Matches("^[A-Za-z0-9-_]+$").ConfirmMessage(MessageConstant.DICTIONARY_ERROR_0014);
		}
	}
}
