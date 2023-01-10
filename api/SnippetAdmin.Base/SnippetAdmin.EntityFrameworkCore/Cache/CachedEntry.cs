using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnippetAdmin.EntityFrameworkCore.Cache
{
	public class CachedEntry
	{
		public object Entity { get; set; }
		public EntityState State { get; set; }
		public IEntityType Metadata { get; set; }
	}
}
