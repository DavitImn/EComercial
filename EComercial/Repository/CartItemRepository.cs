using EComercial.Core;
using EComercial.DbDataContext;
using EComercial.Interface;
using EComercial.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Globalization;

internal class CartItemRepository : ICartItemRepository
{
    private readonly DataContext _context = new DataContext();
    private readonly LoggingService _logger = new LoggingService(); 

    // add item for logined user by customerId
    public void AddItemToCart(int customerId)
    {
        var user = _context.customers.FirstOrDefault(x => x.Id == customerId);
        if (user == null)
        {
            Console.WriteLine("Customer not found.");
            return;
        }
        Console.WriteLine("Enter Product Id: ");
        int prodId = Convert.ToInt32(Console.ReadLine());
        var item = _context.products.FirstOrDefault(x => x.Id == prodId);

        if (item == null)
        {
            Console.WriteLine("Product not found.");
            return;
        }
        if (user.CartItems == null)
        {
            user.CartItems = new List<CartItem>();
        }
        var existingCartItem = user.CartItems.FirstOrDefault(ci => ci.Product.Id == item.Id);


        // it will add new item in cart , if there is not existing same item , if yes it will just add one more , Case 1 phone + 1 = 2 phone in cart
        if (existingCartItem != null)
        {
            existingCartItem.Quantity += 1;
            Console.WriteLine($"Increased quantity of {item.Name} to {existingCartItem.Quantity}.");
        }
        else
        {
            user.CartItems.Add(new CartItem
            {
                Product = item,
                Quantity = 1
            });
            Console.WriteLine($"Added {item.Name} to the cart.");
        }

        _context.customers.Update(user);
        _context.SaveChanges();

        _logger.LogInfo($"User: {user.Name} User:Id {user.Id} Add Prod: {item.Name} In Cart  In Time: {DateTime.UtcNow}");
    }

    // show items in cart for Logined user By CustomerID
    public void CartedItems(int customerId)
    {
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
            Console.WriteLine("No items in the cart.");
            Console.ResetColor();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine("| Product Name          | Quantity | Total Price |");
        Console.WriteLine("--------------------------------------------------");

        foreach (var cartItem in user.CartItems)
        {
            Console.ForegroundColor = ConsoleColor.Cyan; // Use cyan for the table details
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
        Console.WriteLine($"\nTotal Price: {total} $");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\nPress Any Key To Return To Main Menu");
        Console.ResetColor();
        Console.ReadKey();
    }


    // Remove Items From Cart For Logined User By CustomerID
    public void RemoveItemFromCart(int customerId)
    {
        var user = _context.customers.FirstOrDefault(x => x.Id == customerId);
        if (user == null)
        {
            Console.WriteLine("Customer not found.");
            return;
        }


        Console.WriteLine($"\nItems in {user.Name}'s Cart:");
        if (user.CartItems == null || !user.CartItems.Any())
        {
            Console.WriteLine("Your cart is empty.");
            return;
        }

        foreach (var cartItem in user.CartItems)
        {
            Console.WriteLine($"ID: {cartItem.Product.Id}, Product: {cartItem.Product.Name}, Quantity: {cartItem.Quantity}, Price: {cartItem.Product.Price * cartItem.Quantity}");
        }


        Console.WriteLine("\nEnter the Product ID to remove from the cart: ");
        int prodId = Convert.ToInt32(Console.ReadLine());

        var itemToRemove = user.CartItems.FirstOrDefault(ci => ci.Product.Id == prodId);
        if (itemToRemove != null)
        {
            // if Quantity More then 1 it will remove Only one item and reamin other Cuantity , In Case 2 phones it will remain 1 , In case 1 it will clear the phone from list 
            if (itemToRemove.Quantity > 1)
            {
                itemToRemove.Quantity--;
                Console.WriteLine($"Decreased quantity of {itemToRemove.Product.Name} to {itemToRemove.Quantity}.");
            }
            else
            {
               
                user.CartItems.Remove(itemToRemove);
                Console.WriteLine($"Removed {itemToRemove.Product.Name} from the cart.");
            }

           
            _context.customers.Update(user);
            _context.SaveChanges();
        }
        else
        {
            Console.WriteLine("Item not found in cart.");
        }

        _logger.LogInfo($"User: {user.Name} User:Id {user.Id} Remove Prod: {itemToRemove.Product.Name} From Cart In Time: {DateTime.UtcNow}");
    }


    // Clearing items from cart after user Chackout all items ,
    public void ClearCartAfterCheckout(int customerId)
    {
      
        var user = _context.customers.Include(c => c.CartItems).FirstOrDefault(c => c.Id == customerId);

        if (user != null && user.CartItems.Any())
        {
          
            user.CartItems.Clear();
            Console.WriteLine("All items have been removed from the cart.");

            _context.customers.Update(user);
            _context.SaveChanges();
        }
        else
        {
            Console.WriteLine("Your cart is already empty.");
        }
    }

}
