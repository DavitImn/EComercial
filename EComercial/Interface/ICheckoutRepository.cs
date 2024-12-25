using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EComercial.Interface
{
    internal interface ICheckoutRepository
    {
        public void Checkout(int customerId);
    }
}
