﻿using FluentValidation;
using SnippetAdmin.Constants;
using SnippetAdmin.Core;

namespace SnippetAdmin.Models.Common
{
    public class PageSizeInputModel
    {
        public int Page { get; set; }

        public int Size { get; set; }

        public int TakeCount { get => Size; }

        public int SkipCount { get => Size * (Page - 1); }
    }

    public class PageSizeInputModelValidator : AbstractValidator<PageSizeInputModel>
    {
        public PageSizeInputModelValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0).ConfirmMessage(MessageConstant.SYSTEM_COMMON_001);
            RuleFor(x => x.Size).GreaterThan(0).ConfirmMessage(MessageConstant.SYSTEM_COMMON_002);
        }
    }

}
