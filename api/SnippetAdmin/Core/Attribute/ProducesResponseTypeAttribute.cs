using Microsoft.AspNetCore.Mvc;
using SnippetAdmin.Models;
using System;

namespace SnippetAdmin.Core.Attribute
{
    public class CommonResultResponseTypeAttribute : ProducesResponseTypeAttribute
    {
        public CommonResultResponseTypeAttribute() : base(typeof(CommonResult), 200)
        {
        }

        public CommonResultResponseTypeAttribute(Type type) : base(200)
        {
            Type = typeof(CommonResult<>).MakeGenericType(type);
        }
    }
}