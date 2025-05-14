using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Entity
{
    public class Clothing : Product
    {
        public string Size { get; set; }
        public string Color { get; set; }

        public Clothing(int productId, string productName, string description, double price, int quantity, string type, string size, string color)
            : base(productId, productName, description, price, quantity, type)
        {
            Size = size;
            Color = color;
        }
    }
}
