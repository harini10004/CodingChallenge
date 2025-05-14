using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Entity;

namespace OrderManagement.dao
{
    public class OrderProcessor : IOrderManagementRepository
    {
        public void CreateUser(User user)
        {
            using (SqlConnection conn = DBUtility.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO Users VALUES (@UserId, @Username, @Password, @Role)", conn);
                cmd.Parameters.AddWithValue("@UserId", user.UserId);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@Role", user.Role);
                cmd.ExecuteNonQuery();
            }
        }

        public void CreateProduct(User user, Product product)
        {
            if (user.Role != "Admin")
                throw new UnauthorizedAccessException("Only Admin users can add products.");

            using (SqlConnection conn = DBUtility.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Products (ProductId, ProductName, Description, Price, QuantityInStock, Type, Brand, WarrantyPeriod, Size, Color)
                    VALUES (@ProductId, @ProductName, @Description, @Price, @Quantity, @Type, @Brand, @WarrantyPeriod, @Size, @Color)", conn);

                cmd.Parameters.AddWithValue("@ProductId", product.ProductId);
                cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                cmd.Parameters.AddWithValue("@Description", product.Description);
                cmd.Parameters.AddWithValue("@Price", product.Price);
                cmd.Parameters.AddWithValue("@Quantity", product.QuantityInStock);
                cmd.Parameters.AddWithValue("@Type", product.Type);

                // Fill type-specific fields
                if (product is Electronics e)
                {
                    cmd.Parameters.AddWithValue("@Brand", e.Brand);
                    cmd.Parameters.AddWithValue("@WarrantyPeriod", e.WarrantyPeriod);
                    cmd.Parameters.AddWithValue("@Size", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Color", DBNull.Value);
                }
                else if (product is Clothing c)
                {
                    cmd.Parameters.AddWithValue("@Brand", DBNull.Value);
                    cmd.Parameters.AddWithValue("@WarrantyPeriod", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Size", c.Size);
                    cmd.Parameters.AddWithValue("@Color", c.Color);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Brand", DBNull.Value);
                    cmd.Parameters.AddWithValue("@WarrantyPeriod", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Size", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Color", DBNull.Value);
                }

                cmd.ExecuteNonQuery();
            }
        }

        public void CreateOrder(User user, List<Product> products)
        {
            using (SqlConnection conn = DBUtility.GetConnection())
            {
                SqlCommand checkUser = new SqlCommand("SELECT COUNT(*) FROM Users WHERE UserId = @UserId", conn);
                checkUser.Parameters.AddWithValue("@UserId", user.UserId);
                int exists = (int)checkUser.ExecuteScalar();

                if (exists == 0)
                    CreateUser(user);  // Insert new user if not exists

                SqlCommand insertOrder = new SqlCommand("INSERT INTO Orders (UserId) OUTPUT INSERTED.OrderId VALUES (@UserId)", conn);
                insertOrder.Parameters.AddWithValue("@UserId", user.UserId);
                int orderId = (int)insertOrder.ExecuteScalar();

                foreach (var product in products)
                {
                    SqlCommand insertDetail = new SqlCommand("INSERT INTO OrderDetails (OrderId, ProductId, Quantity) VALUES (@OrderId, @ProductId, @Quantity)", conn);
                    insertDetail.Parameters.AddWithValue("@OrderId", orderId);
                    insertDetail.Parameters.AddWithValue("@ProductId", product.ProductId);
                    insertDetail.Parameters.AddWithValue("@Quantity", product.QuantityInStock); // using QuantityInStock as ordered qty
                    insertDetail.ExecuteNonQuery();
                }
            }
        }

        public void CancelOrder(int userId, int orderId)
        {
            using (SqlConnection conn = DBUtility.GetConnection())
            {
                SqlCommand checkUser = new SqlCommand("SELECT COUNT(*) FROM Users WHERE UserId = @UserId", conn);
                checkUser.Parameters.AddWithValue("@UserId", userId);
                if ((int)checkUser.ExecuteScalar() == 0)
                    throw new UserNotFoundException("User not found");

                SqlCommand checkOrder = new SqlCommand("SELECT COUNT(*) FROM Orders WHERE OrderId = @OrderId AND UserId = @UserId", conn);
                checkOrder.Parameters.AddWithValue("@OrderId", orderId);
                checkOrder.Parameters.AddWithValue("@UserId", userId);
                if ((int)checkOrder.ExecuteScalar() == 0)
                    throw new OrderNotFoundException("Order not found");

                SqlCommand deleteDetails = new SqlCommand("DELETE FROM OrderDetails WHERE OrderId = @OrderId", conn);
                deleteDetails.Parameters.AddWithValue("@OrderId", orderId);
                deleteDetails.ExecuteNonQuery();

                SqlCommand deleteOrder = new SqlCommand("DELETE FROM Orders WHERE OrderId = @OrderId", conn);
                deleteOrder.Parameters.AddWithValue("@OrderId", orderId);
                deleteOrder.ExecuteNonQuery();
            }
        }

        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection conn = DBUtility.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Products", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string type = reader["Type"].ToString();

                    if (type == "Electronics")
                    {
                        products.Add(new Electronics(
                            Convert.ToInt32(reader["ProductId"]),
                            reader["ProductName"].ToString(),
                            reader["Description"].ToString(),
                            Convert.ToDouble(reader["Price"]),
                            Convert.ToInt32(reader["QuantityInStock"]),
                            type,
                            reader["Brand"].ToString(),
                            Convert.ToInt32(reader["WarrantyPeriod"])
                        ));
                    }
                    else if (type == "Clothing")
                    {
                        products.Add(new Clothing(
                            Convert.ToInt32(reader["ProductId"]),
                            reader["ProductName"].ToString(),
                            reader["Description"].ToString(),
                            Convert.ToDouble(reader["Price"]),
                            Convert.ToInt32(reader["QuantityInStock"]),
                            type,
                            reader["Size"].ToString(),
                            reader["Color"].ToString()
                        ));
                    }
                    else
                    {
                        products.Add(new Product(
                            Convert.ToInt32(reader["ProductId"]),
                            reader["ProductName"].ToString(),
                            reader["Description"].ToString(),
                            Convert.ToDouble(reader["Price"]),
                            Convert.ToInt32(reader["QuantityInStock"]),
                            type
                        ));
                    }
                }

                reader.Close();
            }

            return products;
        }

        public List<Product> GetOrderByUser(User user)
        {
            List<Product> orderedProducts = new List<Product>();

            using (SqlConnection conn = DBUtility.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT p.* FROM Products p
                    JOIN OrderDetails od ON p.ProductId = od.ProductId
                    JOIN Orders o ON o.OrderId = od.OrderId
                    WHERE o.UserId = @UserId", conn);

                cmd.Parameters.AddWithValue("@UserId", user.UserId);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string type = reader["Type"].ToString();

                    if (type == "Electronics")
                    {
                        orderedProducts.Add(new Electronics(
                            Convert.ToInt32(reader["ProductId"]),
                            reader["ProductName"].ToString(),
                            reader["Description"].ToString(),
                            Convert.ToDouble(reader["Price"]),
                            Convert.ToInt32(reader["QuantityInStock"]),
                            type,
                            reader["Brand"].ToString(),
                            Convert.ToInt32(reader["WarrantyPeriod"])
                        ));
                    }
                    else if (type == "Clothing")
                    {
                        orderedProducts.Add(new Clothing(
                            Convert.ToInt32(reader["ProductId"]),
                            reader["ProductName"].ToString(),
                            reader["Description"].ToString(),
                            Convert.ToDouble(reader["Price"]),
                            Convert.ToInt32(reader["QuantityInStock"]),
                            type,
                            reader["Size"].ToString(),
                            reader["Color"].ToString()
                        ));
                    }
                    else
                    {
                        orderedProducts.Add(new Product(
                            Convert.ToInt32(reader["ProductId"]),
                            reader["ProductName"].ToString(),
                            reader["Description"].ToString(),
                            Convert.ToDouble(reader["Price"]),
                            Convert.ToInt32(reader["QuantityInStock"]),
                            type
                        ));
                    }
                }

                reader.Close();
            }

            return orderedProducts;
        }
    }
}
