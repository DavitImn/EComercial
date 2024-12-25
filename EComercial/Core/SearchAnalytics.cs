using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EComercial.Core
{
    internal class SearchAnalytics
    {
        public int Id { get; set; }
        public string SearchTerm { get; set; }
        public int SearchCount { get; set; }
        public DateTime LastSearchedAt { get; set; }

    }
}
