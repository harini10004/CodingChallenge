using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement
{
    public class OrderException : Exception
    {
        public OrderException(string message) : base(message) { }
    }
    public class InvalidDataException : OrderException
    {
        public InvalidDataException(string message) : base(message) { }
    }
    public class UserNotFoundException : OrderException
    {
        public UserNotFoundException(string message) : base(message) { }
    }
    public class OrderNotFoundException : OrderException
    {
        public OrderNotFoundException(string message) : base(message) { }
    }
}
