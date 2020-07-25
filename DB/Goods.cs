using System;
using System.Collections.Generic;

namespace TelegramShopAsp.DB
{
    public partial class Goods
    {
        public Goods()
        {
            CartGoods = new HashSet<CartGoods>();
            SellerGoods = new HashSet<SellerGoods>();
        }

        public string UID { get; set; }
        public string Name { get; set; }
        public string ParentUID { get; set; }
        public bool Disabled { get; set; }
        public bool IsBasic { get; set; }
        public int UnitId { get; set; }

        public Unit Unit { get; set; }
        public ICollection<CartGoods> CartGoods { get; set; }
        public ICollection<SellerGoods> SellerGoods { get; set; }
    }
}
