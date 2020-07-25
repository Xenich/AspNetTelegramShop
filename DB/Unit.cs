using System;
using System.Collections.Generic;

namespace TelegramShopAsp.DB
{
    public partial class Unit
    {
        public Unit()
        {
            Goods = new HashSet<Goods>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Name234 { get; set; }
        public string Name5 { get; set; }

        public ICollection<Goods> Goods { get; set; }
    }
}
