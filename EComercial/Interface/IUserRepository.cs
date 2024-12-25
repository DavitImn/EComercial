using EComercial.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EComercial.Interface
{
    internal interface IUserRepository
    {
        public void RegisterNewCustomer();
        public Customer Login();

    }
}
