using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EComercial.Core;
using EComercial.DbDataContext;
using EComercial.Interface;
using EComercial.Validators;
using EComercial.Enum;
using EComercial.Services;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EComercial.Repository
{
    internal class UserRepository : IUserRepository
    {
        private readonly CustomerValidator validationRules = new CustomerValidator();
        private readonly DataContext _context = new DataContext();
        EmailService EmailService = new EmailService();

        private static string GetPassword()
        {
            StringBuilder input = new StringBuilder();
            while (true)
            {
                int x = Console.CursorLeft;
                int y = Console.CursorTop;
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input.Remove(input.Length - 1, 1);
                    Console.SetCursorPosition(x - 1, y);
                    Console.Write(" ");
                    Console.SetCursorPosition(x - 1, y);
                }
                else if (key.Key != ConsoleKey.Backspace)
                {
                    input.Append(key.KeyChar);
                    Console.Write("*");
                }
            }
            return input.ToString();
        }
        public Customer Login()
        {
            Console.Write("Enter User Name: ");
            Console.WriteLine();
            string userName = Console.ReadLine();

            Console.Write("Enter User Password: ");
            string userPassword = GetPassword();

            var user = _context.customers.FirstOrDefault(x => x.Name == userName);

            if (user == null)
            {
                return null;
            }

            bool isPassworCorrect = BCrypt.Net.BCrypt.Verify(userPassword, user.Password);

            if (user == null || !isPassworCorrect)
            {
                return null;
            }
            if (!user.IsActive)
            {
                Console.WriteLine("Account is not active. Please activate your account using the code sent to your email.");

                Console.WriteLine("Would You Like To Activate Your Account? \n" +
                                  "Y - YES \n" +
                                  "N - NO");

                var inputKey = Console.ReadKey(true).Key;

                if (inputKey == ConsoleKey.Y)
                {
                    if (DateTime.UtcNow >= user.CodeExpirationTime)
                    {
                        Console.WriteLine("Activation code has expired.");
                        Console.WriteLine("A new activation code will be sent to your email.");

                        var random = new Random();
                        user.ActivationCode = random.Next(100000, 999999).ToString();
                        user.CodeExpirationTime = DateTime.UtcNow.AddMinutes(5);

                        _context.customers.Update(user);
                        _context.SaveChanges();

                        EmailService.SendActivationCode(user, user.ActivationCode);

                        Console.WriteLine("Enter the new activation code from your email:");
                        string newCode = Console.ReadLine();
                        ActivateUser(user.Id, newCode); 
                    }
                    else
                    {
                        Console.WriteLine("Enter the activation code from your email:");
                        string code = Console.ReadLine();
                        ActivateUser(user.Id, code); 
                    }
                }
                else
                {
                    Console.WriteLine("Goodbye!");
                    return null;
                }
            }

            if (user.IsActive)
            {
                Console.WriteLine("Your account is active.");
            }
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(userPassword, user.Password);

            if (isPasswordCorrect)
            {
                return user;
            }
            return null;
        }
        public void RegisterNewCustomer()
        {
            var customerValidator = new CustomerValidator();
            RoleEnum roles = new RoleEnum();

            Console.Write("Enter User Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter User Email: ");
            string email = Console.ReadLine();

            Console.Write("Enter User Password: ");
            string password = Console.ReadLine();

            string passwordHesh = BCrypt.Net.BCrypt.HashPassword(password);

            Console.WriteLine("Chose The Role: " +
                "1 - System Admin" +
                "2 - Manager" +
                "3 - Customer");

            var keyInput = Console.ReadKey(true).Key;

            if (keyInput == ConsoleKey.D1 || keyInput == ConsoleKey.NumPad1)
            {
                roles = RoleEnum.SystemAdmin;
            }
            if (keyInput == ConsoleKey.D2 || keyInput == ConsoleKey.NumPad2)
            {
                roles = RoleEnum.Manager;
            }
            if (keyInput == ConsoleKey.D3 || keyInput == ConsoleKey.NumPad3)
            {
                roles = RoleEnum.LoggedUser;
            }

       //     var mailcheck = _context.customers.FirstOrDefault(x => x.Email == email);
       //
       //     if (mailcheck != null)
       //     {
       //         Console.WriteLine("ALlready have this mail");
       //         Console.WriteLine($"this email allready exist {mailcheck.Email}");
       //
       //         return;
       //     }

            Customer customer = new Customer()
            {
                Name = name,
                Email = email,
                Password = password,
                IsActive = false, 
                Role = roles

            };

            var validationResult = customerValidator.Validate(customer);
            if (validationResult.IsValid)
            {
                var random = new Random();
                customer.ActivationCode = random.Next(100000, 999999).ToString();
                customer.CodeExpirationTime = DateTime.UtcNow.AddMinutes(5);
                customer.Password = passwordHesh;

                _context.customers.Add(customer);
                _context.SaveChanges();

                EmailService.SendActivationCode(
                    customer,
                    customer.ActivationCode
                );

                Console.WriteLine($"Welcome New Customer: {customer.Name}  Your Role Is {customer.Role}");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Validation Error Occurred");
                foreach (var error in validationResult.Errors)
                {
                    Console.WriteLine($"Error - {error.PropertyName}, Message - {error.ErrorMessage}");
                }
                Console.ReadKey();
            }
        }
        public bool ActivateUser(int userId, string code)
        {
            var user = _context.customers.FirstOrDefault(u => u.Id == userId);
            if (user == null || user.IsActive) return false;
            if (user.ActivationCode == code && DateTime.UtcNow <= user.CodeExpirationTime)
            {
                user.IsActive = true;
                user.ActivationCode = null;
                user.CodeExpirationTime = null;

                _context.customers.Update(user);
                _context.SaveChanges();
                Console.WriteLine("User Activited !!!");
                return true;
            }
            else if(user.ActivationCode != code) 
            {
                Console.WriteLine("Wrong Code");
            }
            return false;
        }
    }
}
