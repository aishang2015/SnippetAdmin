using FluentValidation;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Endpoint.Models.Account;

namespace SnippetAdmin.Validators.Account
{
	public class BindingThirdPartyAccountInputModelValidator : AbstractValidator<BindingThirdPartyAccountInputModel>
	{
		public BindingThirdPartyAccountInputModelValidator()
		{
			RuleFor(x => x.UserName).NotEmpty().ConfirmMessage(MessageConstant.ACCOUNT_ERROR_0002);
			RuleFor(x => x.Password).NotEmpty().ConfirmMessage(MessageConstant.ACCOUNT_ERROR_0003);
			RuleFor(x => x.ThirdPartyType).NotEmpty().ConfirmMessage(MessageConstant.ACCOUNT_ERROR_0005);
			RuleFor(x => x.ThirdPartyInfoCacheKey).NotEmpty().ConfirmMessage(MessageConstant.ACCOUNT_ERROR_0006);
		}
	}
}
