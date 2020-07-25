using System;
using System.Collections.Generic;

namespace TelegramShopAsp.DB
{
    public partial class Sellers
    {
        public Sellers()
        {
            SellerGoods = new HashSet<SellerGoods>();
        }

        public int TelegramId { get; set; }
        public int? ChatId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Adress { get; set; }
        public string CityUID { get; set; }
        public int? Region { get; set; }
        public int? PreviousMessage { get; set; }
        public int? CurrentMessage { get; set; }
        public string StateUID { get; set; }
        public string CurrentItemEdit { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string Description { get; set; }

        public Cities CityU { get; set; }
        public ICollection<SellerGoods> SellerGoods { get; set; }
    }
}
