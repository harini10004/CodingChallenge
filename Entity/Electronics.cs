using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Entity
{
    public class Electronics : Product
    {
        public string Brand { get; set; }
        public int WarrantyPeriod { get; set; }

        public Electronics(int productId, string productName, string description, double price, int quantity, string type, string brand, int warranty)
            : base(productId, productName, description, price, quantity, type)
        {
            Brand = brand;
            WarrantyPeriod = warranty;
        }
    }
}
