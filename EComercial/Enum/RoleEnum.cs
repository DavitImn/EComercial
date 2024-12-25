using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EComercial.Enum
{
    internal enum RoleEnum
    {
        SystemAdmin = 1, // system admin role , can see the analystic and logs 
        Manager = 2,  // manager - CRUD operations with products and CRUD Operations with Categories
        LoggedUser = 3, // all functions of Guest + add item in cart , and buy , or delete from cart 
        Guest = 4  // can just chack product what in sale
    }
}
