using EComercial.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EComercial.Interface
{
    internal interface IProductRepository
    {
        public void AddProduct(int userId);  
        public void UpdateProduct(int userId);
        public void RemoveProduct(int userId);
        public void GetAllProducts();
        public void GetProductByCategory();
        public void SearchProductByName();


    }
}
