using System;
using System.Collections.Generic;

namespace TelegramShopAsp.DB
{
    public partial class Buyers
    {
        public Buyers()
        {
            Cart = new HashSet<Cart>();
        }

        public int TelegramId { get; set; }
        public int? ChatId { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string adress { get; set; }
        public string CityUID { get; set; }
        public int? Region { get; set; }
        public int? PreviousMessage { get; set; }
        public string currentItemEdit { get; set; }
        public string StateUID { get; set; }
        public bool isActive { get; set; }

        public Cities CityU { get; set; }
        public ICollection<Cart> Cart { get; set; }
    }
}
