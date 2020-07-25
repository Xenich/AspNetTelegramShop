using System;
using System.Collections.Generic;

namespace TelegramShopAsp.DB
{
    public partial class Cities
    {
        public Cities()
        {
            Buyers = new HashSet<Buyers>();
            Sellers = new HashSet<Sellers>();
        }

        public string UID { get; set; }
        public string Name { get; set; }
        public string CountryUID { get; set; }

        public Countries CountryU { get; set; }
        public ICollection<Buyers> Buyers { get; set; }
        public ICollection<Sellers> Sellers { get; set; }
    }
}
