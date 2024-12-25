using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EComercial.Enum;

namespace EComercial.Core
{
    internal class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email {  get; set; }
        public string Password { get; set; }
        public RoleEnum Role { get; set; }
        public bool IsActive { get; set; }
        public string? ActivationCode { get; set; }
        public DateTime? CodeExpirationTime { get; set; }

        public int CartItemId {  get; set; }
        public List<CartItem> CartItems { get; set; }
    }
}
