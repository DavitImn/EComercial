using EComercial.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EComercial.Interface
{
    internal interface ICartItemRepository
    {
        
      public void AddItemToCart(int customerId);

        public void CartedItems(int customerId);

        public void ClearCartAfterCheckout(int customerId);

        public void RemoveItemFromCart(int customerId);

    }
}
