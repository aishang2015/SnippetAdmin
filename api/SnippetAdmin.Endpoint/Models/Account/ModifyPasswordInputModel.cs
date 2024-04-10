using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnippetAdmin.Endpoint.Models.Account
{
	public record ModifyPasswordInputModel
	{
		public string OldPassword { get; set; } = null!;

		public string NewPassword { get; set; } = null!;
    }
}
