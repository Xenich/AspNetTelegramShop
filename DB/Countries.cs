using System;
using System.Collections.Generic;

namespace TelegramShopAsp.DB
{
    public partial class Countries
    {
        public Countries()
        {
            Cities = new HashSet<Cities>();
        }

        public string UID { get; set; }
        public string Name { get; set; }

        public ICollection<Cities> Cities { get; set; }
    }
}
