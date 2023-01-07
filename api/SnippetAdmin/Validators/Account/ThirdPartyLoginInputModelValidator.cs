using FluentValidation;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Endpoint.Models.Account;

namespace SnippetAdmin.Validators.Account
{
	public class ThirdPartyLoginInputModelValidator : AbstractValidator<ThirdPartyLoginInputModel>
	{
		public ThirdPartyLoginInputModelValidator()
		{
			RuleFor(x => x.Code).NotEmpty().ConfirmMessage(MessageConstant.ACCOUNT_ERROR_0002);
			RuleFor(x => x.Source).NotEmpty().ConfirmMessage(MessageConstant.ACCOUNT_ERROR_0003);
		}
	}
}
