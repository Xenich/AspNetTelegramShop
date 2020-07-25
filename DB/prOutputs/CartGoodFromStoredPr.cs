using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramShopAsp.DB.prOutputs
{
    public class CartGoodFromStoredPr
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string UID { get; set; }
        public string Quantity { get; set; }
    }
}
