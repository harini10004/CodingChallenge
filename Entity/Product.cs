using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Entity
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int QuantityInStock { get; set; }
        public string Type { get; set; }

        public Product() { }

        public Product(int productId, string productName, string description, double price, int quantity, string type)
        {
            ProductId = productId;
            ProductName = productName;
            Description = description;
            Price = price;
            QuantityInStock = quantity;
            Type = type;
        }
    }
}
