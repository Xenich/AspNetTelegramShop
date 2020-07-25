using System;
using System.Collections.Generic;

namespace TelegramShopAsp.DB
{
    public partial class Cart
    {
        public Cart()
        {
            CartGoods = new HashSet<CartGoods>();
        }

        public string Uid { get; set; }
        public int UserTelegramId { get; set; }
        public bool? IsActive { get; set; }

        public Buyers UserTelegram { get; set; }
        public ICollection<CartGoods> CartGoods { get; set; }
    }
}
