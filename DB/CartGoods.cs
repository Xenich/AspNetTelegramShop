using System;
using System.Collections.Generic;

namespace TelegramShopAsp.DB
{
    public partial class CartGoods
    {
        public string CartUid { get; set; }
        public string GoodUid { get; set; }
        public int Id { get; set; }
        public string Quantity { get; set; }

        public Cart CartU { get; set; }
        public Goods GoodU { get; set; }
    }
}
