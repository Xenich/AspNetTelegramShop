using System;
using System.Collections.Generic;

namespace TelegramShopAsp.DB
{
    public partial class UserTypes
    {
        public UserTypes()
        {
            Users = new HashSet<Users>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Users> Users { get; set; }
    }
}
