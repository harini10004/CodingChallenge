using OrderManagement.dao;
using OrderManagement.Entity;

namespace OrderManagement
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IOrderManagementRepository repository = new OrderProcessor();
            while (true)
            {
                Console.WriteLine("\n=== Order Management System ===");
                Console.WriteLine("1. Register User");
                Console.WriteLine("2. Add Product (Admin only)");
                Console.WriteLine("3. Place Order");
                Console.WriteLine("4. Cancel Order");
                Console.WriteLine("5. View All Products");
                Console.WriteLine("6. View My Orders");
                Console.WriteLine("7. Exit");

                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            User user = InputUI.GetUserInput("User");
                            repository.CreateUser(user);
                            Console.WriteLine("User registered successfully.");
                            break;

                        case "2":
                            User admin = InputUI.GetUserInput("Admin");
                            Product product = InputUI.GetProductInput();
                            repository.CreateProduct(admin, product);
                            Console.WriteLine("Product added successfully.");
                            break;

                        case "3":
                            User orderUser = InputUI.GetUserInput("User");
                            List<Product> orderList = InputUI.GetOrderInput();
                            repository.CreateOrder(orderUser, orderList);
                            Console.WriteLine("Order placed successfully.");
                            break;

                        case "4":
                            Console.Write("Enter your User ID: ");
                            int userId = int.Parse(Console.ReadLine());

                            Console.Write("Enter Order ID to cancel: ");
                            int orderId = int.Parse(Console.ReadLine());

                            repository.CancelOrder(userId, orderId);
                            Console.WriteLine("Order cancelled successfully.");
                            break;

                        case "5":
                            List<Product> products = repository.GetAllProducts();
                            Console.WriteLine("\nAll Products:");
                            foreach (var p in products)
                            {
                                Console.WriteLine($"{p.ProductId} - {p.ProductName} - {p.Price} - Qty: {p.QuantityInStock} - Type: {p.Type}");
                            }
                            break;

                        case "6":
                            User searchUser = InputUI.GetUserInput("User");
                            List<Product> orders = repository.GetOrderByUser(searchUser);
                            Console.WriteLine("\nYour Orders:");
                            foreach (var p in orders)
                            {
                                Console.WriteLine($"{p.ProductId} - {p.ProductName} - {p.Price} - Qty: {p.QuantityInStock} - Type: {p.Type}");
                            }
                            break;

                        case "7":
                            Console.WriteLine("Exiting...");
                            return;

                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                  throw new OrderException("Error: " + ex.Message);
                }
            }
        }
    }
}
