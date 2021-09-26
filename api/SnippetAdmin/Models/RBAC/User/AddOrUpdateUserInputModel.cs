﻿using FluentValidation;
using SnippetAdmin.Constants;
using SnippetAdmin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnippetAdmin.Models.RBAC.User
{
    public class AddOrUpdateUserInputModel
    {
        public int? Id { get; set; }

        public string UserName { get; set; }

        public string RealName { get; set; }

        public int Gender { get; set; }

        public string PhoneNumber { get; set; }

        public int[] Roles { get; set; }
    }

    public class AddOrUpdateUserInputModelValidator : AbstractValidator<AddOrUpdateUserInputModel>
    {
        public AddOrUpdateUserInputModelValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().ConfirmMessage(MessageConstant.USER_ERROR_0002);
            RuleFor(x => x.UserName).MaximumLength(20).ConfirmMessage(MessageConstant.USER_ERROR_0003);
            RuleFor(x => x.UserName).Matches("^[A-Za-z0-9]+$").ConfirmMessage(MessageConstant.USER_ERROR_0004);

            RuleFor(x => x.RealName).NotEmpty().ConfirmMessage(MessageConstant.USER_ERROR_0005);
            RuleFor(x => x.RealName).MaximumLength(20).ConfirmMessage(MessageConstant.USER_ERROR_0006);
        }
    }
}
