using EComercial.Core;
using EComercial.Enum;
using System;
using System.Collections.Generic;
using EComercial.Interface;
using EComercial.Repository;
using EComercial.Services;
using EComercial.DbDataContext;

public class MenuNavigator
{
    private Customer? _loggedInUser;
    private readonly DataContext _context;
    private readonly IUserRepository _user = new UserRepository();
    private readonly IProductRepository _product = new ProductRepository();
    private readonly CartItemRepository _cartItem = new CartItemRepository();
    private readonly ICheckoutRepository _checkout = new CheckoutRepository();
    private readonly LoggingService _logger = new LoggingService();

    public void RefreshMenu()
    {
        Console.Clear();
        ShowLoggedUserMenu();
    }

    public void StartMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Welcome to the E-Commerce Platform!");

            if (_loggedInUser == null)
            {
                ShowGuestMenu();
            }
            else
            {
                switch (_loggedInUser.Role)
                {
                    case RoleEnum.SystemAdmin:
                        ShowAdminMenu();
                        break;
                    case RoleEnum.Manager:
                        ShowManagerMenu();
                        break;
                    case RoleEnum.LoggedUser:
                        ShowLoggedUserMenu();
                        break;
                    case RoleEnum.Guest:
                        ShowGuestMenu();
                        break;
                }
            }
        }
    }


    private void ShowGuestMenu()
    {
        string[] guestMenuOptions = { "Get All Products", "Search Product by Name", "Search Product by Category", "Login", "Sign Up", "Exit" };
        int selectedIndex = ShowMenu("Guest Menu", guestMenuOptions);

        switch (guestMenuOptions[selectedIndex])
        {
            case "Get All Products":
                Console.WriteLine("Displaying all products...");
                _product.GetAllProducts();
                _logger.LogInfo("Guest viewed all products");
                break;
            case "Search Product by Name":
                Console.WriteLine("Searching product by name...");
                _product.SearchProductByName();
                _logger.LogInfo("Guest searched for a product by name.");
                break;
            case "Search Product by Category":
                Console.WriteLine("Searching product by category...");
                _product.GetProductByCategory();
                _logger.LogInfo("Guest searched for a product by category.");
                break;
            case "Login":
                _loggedInUser = _user.Login();
                if (_loggedInUser != null)
                {
                    _logger.LogUserLogin(_loggedInUser.Id.ToString());
                }
                else
                {
                    Console.WriteLine("Invalid credentials. Press any key to try again...");
                    Console.ReadKey();
                }
                break;
            case "Sign Up":
                Console.WriteLine("Registr New User");
                _user.RegisterNewCustomer();
                _logger.LogInfo("New user registration attempted.");
                break;
            case "Exit":
                Environment.Exit(0);
                _logger.LogInfo("Application exited by guest.");
                break;
        }
    }


    private void ShowAdminMenu()
    {
        string[] adminMenuOptions = { "View Logs", "View Analytics", "Logout", "Exit" };
        int selectedIndex = ShowMenu("System Admin Menu", adminMenuOptions);

        _logger.LogInfo($"ADMIN {_loggedInUser.Name} Login In {DateTime.UtcNow}");
        switch (adminMenuOptions[selectedIndex])
        {
            case "View Logs":
                Console.WriteLine("Viewing logs...");
                _logger.LogInfo($"ADMIN {_loggedInUser.Name} Viewing logs.");
                break;
            case "View Analytics":
                Console.WriteLine("Viewing analytics...");
                _logger.LogInfo($"ADMIN {_loggedInUser.Name} Viewing analytics.");
                break;
            case "Logout":
                _logger.LogInfo($"ADMIN {_loggedInUser.Name} Login Out {DateTime.UtcNow}");
                Logout();
                break;
            case "Exit":
                Environment.Exit(0);
                _logger.LogInfo($"Application exited by {_loggedInUser.Name} ADMIN.");
                break;
        }
    }


    private void ShowManagerMenu()
    {
        string[] managerMenuOptions = { "Add Product", "Update Product", "Remove Product", "Get All Products", "Search Product by Name", "Search Product by Category", "Logout", "Exit" };
        int selectedIndex = ShowMenu("Manager Menu", managerMenuOptions);

        _logger.LogInfo($"Manager {_loggedInUser.Name} Login In {DateTime.UtcNow}");
        switch (managerMenuOptions[selectedIndex])
        {
            case "Add Product":
                Console.WriteLine("Adding product...\n");
                _product.AddProduct(_loggedInUser.Id);
                break;
            case "Update Product":
                Console.WriteLine("Updating product...\n");
                _product.UpdateProduct(_loggedInUser.Id);
                break;
            case "Remove Product":
                Console.WriteLine("Removing product...\n");
                _product.RemoveProduct(_loggedInUser.Id);
                break;
            case "Get All Products":
                Console.WriteLine("Displaying all products...");
                _product.GetAllProducts();
                _logger.LogInfo($"Manager {_loggedInUser.Name} viewed all products In Time: {DateTime.UtcNow}");
                break;
            case "Search Product by Name":
                Console.WriteLine("Searching product by name...\n");
                _product.SearchProductByName();
                _logger.LogInfo($"Manager {_loggedInUser.Name} Searching product by name In Time: {DateTime.UtcNow}");
                break;
            case "Search Product by Category":
                Console.WriteLine("Searching product by category...\n");
                _product.GetProductByCategory();
                _logger.LogInfo($"Manager {_loggedInUser.Name} Searching product by category In Time: {DateTime.UtcNow}");
                break;
            case "Logout":
                _logger.LogInfo($"Manager {_loggedInUser.Name} Logout In Time: {DateTime.UtcNow}");
                Logout();
                break;
            case "Exit":
                Environment.Exit(0);
                _logger.LogInfo($"Manager {_loggedInUser.Name} Close Application In Time: {DateTime.UtcNow}");
                break;
        }
    }


    private void ShowLoggedUserMenu()
    {
        string[] loggedUserMenuOptions = { "Get All Products", "Search Product by Name", "Search Product by Category", "View Cart", "Add to Cart", "Remove From Cart", "Checkout", "Logout", "Exit" };
        int selectedIndex = ShowMenu("Logged User Menu", loggedUserMenuOptions);

        switch (loggedUserMenuOptions[selectedIndex])
        {
            case "Get All Products":
                Console.Clear();
                Console.WriteLine("Displaying all products...");
                _product.GetAllProducts();
                _logger.LogInfo($"User: {_loggedInUser.Name} viewed all products In Time: {DateTime.UtcNow}");
                break;
            case "Search Product by Name":
                Console.WriteLine("Searching product by name...\n");
                _product.SearchProductByName();
                _logger.LogInfo($"User: {_loggedInUser.Name} Searching product by name In Time: {DateTime.UtcNow}");
                break;
            case "Search Product by Category":
                Console.WriteLine("Searching product by category...\n");
                _product.GetProductByCategory();
                _logger.LogInfo($"User: {_loggedInUser.Name} Searching product by category In Time: {DateTime.UtcNow}");
                break;
            case "View Cart":
                Console.WriteLine("Viewing cart...");
                _cartItem.CartedItems(_loggedInUser.Id);
                break;
            case "Add to Cart":
                Console.WriteLine("Adding item to cart...");
                _cartItem.AddItemToCart(_loggedInUser.Id);
                break;
            case "Remove From Cart":
                _cartItem.RemoveItemFromCart(_loggedInUser.Id);
                break;
            case "Checkout":
                Console.WriteLine("Checking out...");
                _checkout.Checkout(_loggedInUser.Id);
                _cartItem.ClearCartAfterCheckout(_loggedInUser.Id);
                RefreshMenu();
                break;
            case "Logout":
                _logger.LogInfo($"User: {_loggedInUser.Name}  Id:{_loggedInUser.Id} Logout at: {DateTime.UtcNow} ");
                Logout();
                break;
            case "Exit":
                Environment.Exit(0);
                _logger.LogInfo($"User: {_loggedInUser.Name}  Id:{_loggedInUser.Id} Close Application at: {DateTime.UtcNow} ");
                break;
        }
    }


    private void Logout()
    {
        Console.WriteLine("Logging out...");
        _loggedInUser = null;
        Console.WriteLine("You have been logged out. Press any key to continue...");
        Console.ReadKey();
    }


    private int ShowMenu(string title, string[] options)
    {
        int selectedIndex = 0;

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"=== {title} ===");
            Console.WriteLine("Use the arrow keys to navigate and press Enter to select.\n");

            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($" > {options[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"   {options[i]}");
                }
            }

            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex == 0) ? options.Length - 1 : selectedIndex - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex == options.Length - 1) ? 0 : selectedIndex + 1;
                    break;
                case ConsoleKey.Enter:
                    return selectedIndex;
            }
        }
    }
}
