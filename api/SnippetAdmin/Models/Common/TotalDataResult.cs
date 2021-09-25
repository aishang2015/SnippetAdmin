using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnippetAdmin.Models.Common
{
    public class TotalDataResult<T>
    {
        public int Total { get; set; }

        public IEnumerable<T> Data { get; set; }
    }
}
