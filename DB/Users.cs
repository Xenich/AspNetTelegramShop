using System;
using System.Collections.Generic;

namespace TelegramShopAsp.DB
{
    public partial class Users
    {
        public Users()
        {
            Cart = new HashSet<Cart>();
        }

        public int Id { get; set; }
        public int TelegramId { get; set; }
        public int ChatId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Adress { get; set; }
        public int? CityId { get; set; }
        public int? Region { get; set; }
        public int? PreviousMessage { get; set; }
        public int? CurrentMessage { get; set; }
        public int TypeId { get; set; }
        public int StateId { get; set; }

        public Cities City { get; set; }
        public States State { get; set; }
        public UserTypes Type { get; set; }
        public ICollection<Cart> Cart { get; set; }
    }
}
