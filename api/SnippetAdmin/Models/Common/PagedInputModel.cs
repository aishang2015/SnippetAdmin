using FluentValidation;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Extensions;

namespace SnippetAdmin.Models.Common
{
    public record PagedInputModel
    {
        public int Page { get; set; }

        public int Size { get; set; }

        public int TakeCount { get => Size; }

        public int SkipCount { get => Size * (Page - 1); }

        public SortModel[] Sorts { get; set; }


    }

    public class PagedInputModelValidator : AbstractValidator<PagedInputModel>
    {
        public PagedInputModelValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0).ConfirmMessage(MessageConstant.SYSTEM_ERROR_002);
            RuleFor(x => x.Size).GreaterThan(0).ConfirmMessage(MessageConstant.SYSTEM_ERROR_003);
        }
    }

}