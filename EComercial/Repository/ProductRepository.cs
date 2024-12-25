using EComercial.Core;
using EComercial.DbDataContext;
using EComercial.Interface;
using EComercial.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EComercial.Repository
{
    internal class ProductRepository : IProductRepository
    {
        private readonly DataContext _context = new DataContext();
        private readonly LoggingService _logger = new LoggingService();

        private int ItemCounter = 0;
        string Truncate(string value, int maxLength)
        {
            return value.Length > maxLength ? value.Substring(0, maxLength - 3) + "..." : value;
        }
        public void AddProduct(int userId)
        {
            Console.WriteLine("Enter New Product name");
            string prodName = Console.ReadLine();

            Console.WriteLine("Enter Product Description");
            string prodDescription = Console.ReadLine();

            Console.WriteLine("Enter Product Price");
            decimal prodPrice = Convert.ToDecimal(Console.ReadLine());

            Console.WriteLine("Enter Quantity");
            int prodStock = Convert.ToInt32(Console.ReadLine());

            var allCategories = _context.categories.ToList();

            foreach (var item in allCategories)
            {
                Console.WriteLine($"ID: {item.Id}   NAME: {item.Name}");
            }

            Console.WriteLine("Enter Category To add This Product In , By Id: ");
            int categoryId = Convert.ToInt32(Console.ReadLine());

            Product product = new Product()
            {
                Name = prodName,
                Description = prodDescription,
                Price = prodPrice,
                Stock = prodStock,
                CategoryId = categoryId

            };

            _context.products.Add(product);
            _context.SaveChanges();

            var manager = _context.customers.FirstOrDefault(x => x.Id == userId);
            _logger.LogInfo($"Manager: {manager.Name} Add New Item: {product.Name}  In: {DateTime.UtcNow}");
        }
        public void GetAllProducts()
        {
            const int pageSize = 20;
            int currentPage = 1;

            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{"  Item Name",-34} {"Description",-63} {"Price",-16} {"Category",-22} {"Stock",-12} {"ID",-5}");
                Console.ResetColor();

                var paginatedProducts = _context.products
                    .AsNoTracking()
                    .Include(p => p.Category)
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                if (paginatedProducts.Count == 0 && currentPage > 1)
                {
                    Console.WriteLine("No more products to display.");
                    currentPage--;
                    continue;
                }
                else if (paginatedProducts.Count == 0 && currentPage == 1)
                {
                    Console.WriteLine("No products found.");
                    break;
                }

                Console.ForegroundColor = ConsoleColor.DarkGray;
                for (int i = 0; i < 170; i++)
                {

                    if (i == 0)
                    {
                        Console.Write("╔");
                    }
                    else if (i == 169)
                    {
                        Console.Write("╗");
                    }
                    else if (i == 33 || i == 96 || i == 114 || i == 137 || i == 150)
                    {
                        Console.Write("╦");
                    }
                    else
                    {
                        Console.Write("═");
                    }

                }
                Console.ResetColor();
                Console.WriteLine();

                for (int i = 0; i < paginatedProducts.Count; i++)
                {
                    var prod = paginatedProducts[i];

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("║ ");
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"{Truncate(prod.Name, 30),-30} ");

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("║ ");
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"{Truncate(prod.Description, 60),-60} ");

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("║ ");
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write($" {prod.Price+" $",-15}");

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("║ ");
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write($"{Truncate(prod.Category?.Name ?? "Unknown", 20),-20} ");

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("║ ");
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{prod.Stock,-10} ");

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("║ ");
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"{prod.Id,-5}");

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"            ║");
                    Console.ResetColor();


                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    for (int j = 0; j < 170; j++)
                    {
                        if (i == paginatedProducts.Count - 1)
                        {
                            if (j == 0)
                            {
                                Console.Write("╚");
                            }
                            else if (j == 169)
                            {
                                Console.Write("╝");
                            }
                            
                            else if (j == 33 || j == 96 || j == 114 || j == 137 || j == 150)
                            {
                                Console.Write("╩");
                            }
                            else
                            {
                                Console.Write("═");
                            }
                        }
                        else
                        {
                            if (j == 0)
                            {
                                Console.Write("╠");
                            }
                            else if ( j == 169)
                            {
                                Console.Write("╣");
                            }
                            else if (j == 33 || j == 96 || j == 114 || j == 137 || j == 150)
                            {
                                Console.Write("╬");
                            }
                            else
                            {
                                Console.Write("═");
                            }
                        }
                    }
                    Console.ResetColor();
                    Console.WriteLine(); 
                }
                Console.WriteLine($"Press 'N' for Next Page, 'P' for Previous Page, or 'X' to Exit.                                    Page: {currentPage}");
                var inputKey = Console.ReadKey(true).Key;

                if (inputKey == ConsoleKey.N)
                {
                    currentPage++;
                }
                else if (inputKey == ConsoleKey.P)
                {
                    if (currentPage > 1)
                    {
                        currentPage--;
                    }
                }
                else if (inputKey == ConsoleKey.X)
                {
                    break;
                }
            }
        }
        public void GetProductByCategory()
        {
            const int pageSize = 20;
            int currentPage = 1;

            Console.WriteLine("Choose a Category To Get All Items From This Category: \n");

            var allCategories = _context.categories.ToList();

            foreach (var category in allCategories)
            {
                Console.WriteLine($"ID: {category.Id}   NAME: {category.Name}");
            }

            int catId = Convert.ToInt32(Console.ReadLine());

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Page {currentPage}\n");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{"Item Name",-30} {"Description",-60} {"Price",-15} {"Category",-35} {"Stock",-10} {"ID",-5}");
                Console.ResetColor();
                Console.WriteLine(new string('-', Console.BufferWidth - 1)); 

                var paginatedItemsByCategory = _context.products
                    .Include(x => x.Category)
                    .Where(c => c.CategoryId == catId)
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                if (paginatedItemsByCategory.Count == 0 && currentPage > 1)
                {
                    Console.WriteLine("No more products to display.");
                    currentPage--;
                    continue;
                }
                else if (paginatedItemsByCategory.Count == 0 && currentPage == 1)
                {
                    Console.WriteLine("No products found.");
                    break;
                }

                foreach (var prod in paginatedItemsByCategory)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"{Truncate(prod.Name, 30),-30} ");

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"{Truncate(prod.Description, 60),-60} "); 

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write($"{prod.Price,-15:C} ");

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write($"{Truncate(prod.Category?.Name ?? "Unknown", 35),-35} "); 

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{prod.Stock,-10} ");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{prod.Id,-5}");

                    Console.ResetColor();
                }

                Console.WriteLine("\nPress 'N' for Next Page, 'P' for Previous Page, or 'X' to Exit.");

                var inputKey = Console.ReadKey(true).Key;

                if (inputKey == ConsoleKey.N)
                {
                    currentPage++;
                }
                else if (inputKey == ConsoleKey.P)
                {
                    if (currentPage > 1)
                    {
                        currentPage--;
                    }
                }
                else if (inputKey == ConsoleKey.X)
                {
                    break;
                }
            }
        }
        public void RemoveProduct(int userId)
        {
            int id = 0;
            while (true)
            {
                Console.WriteLine("Search Item By Name = N \n Search Item By Category = C \n Search Item From All Products = A");

                var inputKey = Console.ReadKey(true).Key;

                if (inputKey == ConsoleKey.N)
                {
                    SearchProductByName();
                    Console.Write("Enter ID To Remove Item: ");
                    id = Convert.ToInt32(Console.ReadLine());
                    break;
                }
                else if (inputKey == ConsoleKey.C)
                {
                    GetProductByCategory();
                    Console.Write("Enter ID To Remove Item: ");
                    id = Convert.ToInt32(Console.ReadLine());
                    break;
                }
                else if (inputKey == ConsoleKey.A)
                {
                    GetAllProducts();
                    Console.Write("Enter ID To Remove Item: ");
                    id = Convert.ToInt32(Console.ReadLine());
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid Input Try Again!");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            Console.Clear();

            var deleteProd = _context.products.Include(p => p.Category).FirstOrDefault(x => x.Id == id);
            _context.products.Remove(deleteProd);
            _context.SaveChanges();

            Console.WriteLine($"Product By ID: {deleteProd.Id}    Name {deleteProd.Name}  was deleted !");

            var manager = _context.customers.FirstOrDefault(x => x.Id == userId);
            _logger.LogInfo($"Manager: {manager.Name} Delete Item: {deleteProd.Name}  In: {DateTime.UtcNow}");
        }
        public void SearchProductByName()
        {
            const int pageSize = 20;
            int currentPage = 1;

            while (true)
            {
                var topSearches = _context.searchAnalytics
                   .OrderByDescending(x => x.SearchCount)
                   .Take(3)
                   .ToList();

                Console.WriteLine("Enter 'X' to Exit or \n ");
                Console.WriteLine("Top Searches:  \n ");

                foreach (var search in topSearches)
                {
                    Console.WriteLine($"- {search.SearchTerm}: {search.SearchCount} searches");
                }

                Console.Write("\nSearch By Name: ");
                string wordSearch = Console.ReadLine();
                Console.WriteLine();

                if (wordSearch.ToLower() == "x")
                    break;

                var existingSearch = _context.searchAnalytics.FirstOrDefault(x => x.SearchTerm == wordSearch);
                if (existingSearch != null)
                {
                    existingSearch.SearchCount++;
                    existingSearch.LastSearchedAt = DateTime.Now;
                }
                else
                {
                    _context.searchAnalytics.Add(new SearchAnalytics
                    {
                        SearchTerm = wordSearch,
                        SearchCount = 1,
                        LastSearchedAt = DateTime.Now
                    });
                }
                _context.SaveChanges();

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"Page {currentPage}\n");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{"Item Name",-30} {"Description",-60} {"Price",-15} {"Category",-35} {"Stock",-10} {"ID",-5}");
                    Console.ResetColor();
                    Console.WriteLine(new string('-', Console.BufferWidth - 1));

                    var foundByName = _context.products.Include(c => c.Category)
                        .Where(n => n.Name.ToLower().Contains(wordSearch))
                        .Skip((currentPage - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                    if (foundByName.Count == 0 && currentPage > 1)
                    {
                        Console.WriteLine("No more products to display.");
                        currentPage--;
                        continue;
                    }
                    else if (foundByName.Count == 0 && currentPage == 1)
                    {
                        Console.WriteLine("No products found.");
                        break;
                    }

                    if (foundByName.Count > 0)
                    {
                        foreach (var prod in foundByName)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write($"{Truncate(prod.Name, 30),-30} ");

                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write($"{Truncate(prod.Description, 60),-60} "); 

                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.Write($"{prod.Price,-15:C} ");

                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write($"{Truncate(prod.Category?.Name ?? "Unknown", 35),-35} ");

                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write($"{prod.Stock,-10} ");

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"{prod.Id,-5}");

                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.WriteLine("No products found matching your search.");
                    }

                    Console.WriteLine("\nPress 'N' for Next Page, 'P' for Previous Page, or 'X' to Exit.");

                    var inputKey = Console.ReadKey(true).Key;

                    if (inputKey == ConsoleKey.N)
                    {
                        currentPage++;
                    }
                    else if (inputKey == ConsoleKey.P)
                    {
                        if (currentPage > 1)
                        {
                            currentPage--;
                        }
                    }
                    else if (inputKey == ConsoleKey.X)
                    {
                        break;
                    }
                }
            }
        }
        public void UpdateProduct(int userId)
        {
            int id = 0;

            while (true)
            {
                Console.WriteLine("Search Item By Name = N \n Search Item By Category = C \n Search Item From All Products = A");

                var inputKey = Console.ReadKey(true).Key;

                if (inputKey == ConsoleKey.N)
                {
                    SearchProductByName();
                    Console.Write("Enter ID To Edit Item: ");
                    id = Convert.ToInt32(Console.ReadLine());
                    break;
                }
                else if (inputKey == ConsoleKey.C)
                {
                    GetProductByCategory();
                    Console.Write("Enter ID To Edit Item: ");
                    id = Convert.ToInt32(Console.ReadLine());
                    break;
                }
                else if (inputKey == ConsoleKey.A)
                {
                    GetAllProducts();
                    Console.Write("Enter ID To Edit Item: ");
                    id = Convert.ToInt32(Console.ReadLine());
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid Input Try Again!");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            Console.Clear();

            var itemById = _context.products.Include(c => c.Category).FirstOrDefault(x => x.Id == id);

            Console.WriteLine("Enter New Product name");
            itemById.Name = Console.ReadLine();

            Console.WriteLine("Enter Product Description");
            itemById.Description = Console.ReadLine();

            Console.WriteLine("Enter Product Price");
            itemById.Price = Convert.ToDecimal(Console.ReadLine());

            Console.WriteLine("Enter Quantity");
            itemById.Stock = Convert.ToInt32(Console.ReadLine());

            _context.Update(itemById);
            _context.SaveChanges();

            Console.WriteLine("Done !!!");

            var manager = _context.customers.FirstOrDefault(x => x.Id == userId);
            _logger.LogInfo($"Manager: {manager.Name} Edit Item: {itemById.Name}  In: {DateTime.UtcNow}");


        }
    }
}
