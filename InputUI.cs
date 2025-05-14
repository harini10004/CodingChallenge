using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Entity;

namespace OrderManagement
{
    public static class InputUI
    {
        public static User GetUserInput(string role)
        {
            Console.Write("User ID: ");
            int userId = int.Parse(Console.ReadLine());

            Console.Write("Username: ");
            string username = Console.ReadLine();

            Console.Write("Password: ");
            string password = Console.ReadLine();

            return new User(userId, username, password, role);
        }

        public static Product GetProductInput()
        {
            Console.Write("Product ID: ");
            int productId = int.Parse(Console.ReadLine());

            Console.Write("Name: ");
            string name = Console.ReadLine();

            Console.Write("Description: ");
            string desc = Console.ReadLine();

            Console.Write("Price: ");
            double price = double.Parse(Console.ReadLine());

            Console.Write("Quantity: ");
            int qty = int.Parse(Console.ReadLine());

            Console.Write("Type (Electronics/Clothing): ");
            string type = Console.ReadLine();

            if (type == "Electronics")
            {
                Console.Write("Brand: ");
                string brand = Console.ReadLine();

                Console.Write("Warranty Period: ");
                int warranty = int.Parse(Console.ReadLine());

                return new Electronics(productId, name, desc, price, qty, type, brand, warranty);
            }
            else if (type == "Clothing")
            {
                Console.Write("Size: ");
                string size = Console.ReadLine();

                Console.Write("Color: ");
                string color = Console.ReadLine();

                return new Clothing(productId, name, desc, price, qty, type, size, color);
            }
            else
            {
                return new Product(productId, name, desc, price, qty, type);
            }
        }

        public static List<Product> GetOrderInput()
        {
            List<Product> products = new List<Product>();

            Console.Write("Enter number of products to order: ");
            int count = int.Parse(Console.ReadLine());

            for (int i = 0; i < count; i++)
            {
                Console.Write("Enter Product ID: ");
                int pid = int.Parse(Console.ReadLine());

                Console.Write("Enter Quantity: ");
                int qty = int.Parse(Console.ReadLine());

                Product p = new Product(pid, "", "", 0, qty, "");
                products.Add(p);
            }

            return products;
        }
    }
}
