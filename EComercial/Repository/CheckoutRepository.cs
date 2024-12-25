using EComercial.Core;
using EComercial.DbDataContext;
using EComercial.Interface;
using EComercial.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace EComercial.Repository
{
    internal class CheckoutRepository : ICheckoutRepository
    {
        private readonly DataContext _context = new DataContext();
        private readonly IProductRepository _prductRepostiry = new ProductRepository();
        private readonly Order _order = new Order();
        private readonly LoggingService _logger = new LoggingService();

        public void Checkout(int customerId)
        {
            _order.CostumerId = customerId;

            var user = _context.customers
                                .Include(c => c.CartItems)
                                .ThenInclude(ci => ci.Product)
                                .FirstOrDefault(x => x.Id == customerId);

            if (user == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Customer not found.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nItems in {user.Name}'s Cart:");
            Console.ResetColor();

            if (!user.CartItems.Any())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No items in the cart to checkout.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("| Product Name          | Quantity | Total Price |");
            Console.WriteLine("--------------------------------------------------");

            foreach (var cartItem in user.CartItems)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                string productName = cartItem.Product.Name.PadRight(20);
                string quantity = cartItem.Quantity.ToString().PadLeft(8);
                string totalPrice = (cartItem.Product.Price * cartItem.Quantity)
                                        .ToString("C", CultureInfo.GetCultureInfo("en-US"))
                                        .PadLeft(12);

                Console.WriteLine($"| {productName} | {quantity} | {totalPrice} |");
            }
            Console.WriteLine("--------------------------------------------------");
            Console.ResetColor();

            decimal total = user.CartItems.Sum(ci => ci.Product.Price * ci.Quantity);

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\nTotal Price: {total:C}");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\nIf You Would Like to Pay, Press: Y - YES");
            Console.ResetColor();

            var keyInput = Console.ReadKey(true).Key;

            if (keyInput == ConsoleKey.Y)
            {
                foreach (var item in user.CartItems.ToList())
                {
                    var product = _context.products.FirstOrDefault(p => p.Id == item.Product.Id);
                    if (product != null)
                    {
                        if (product.Stock >= item.Quantity)
                        {
                            product.Stock -= item.Quantity;
                            _context.products.Update(product);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Not enough stock for {product.Name}. Only {product.Stock} items are available.");
                            Console.ResetColor();
                            return;
                        }
                    }
                }

                _context.SaveChanges();

                user.CartItems.Clear();
                _context.customers.Update(user);
                _context.SaveChanges();

                user = _context.customers
                               .Include(c => c.CartItems)
                               .ThenInclude(ci => ci.Product)
                               .FirstOrDefault(x => x.Id == customerId);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Your purchase is complete! Thank you for shopping with us!");
                Console.WriteLine("Your cart is now empty.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("You have canceled the checkout process.");
                Console.ResetColor();
            }

            _logger.LogPaymentSuccess($"{Guid.NewGuid()}", user.Id.ToString(), total);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\nPress Any Key To Return To Main Menu");
            Console.ResetColor();
            Console.ReadKey();
        }

    }

}
