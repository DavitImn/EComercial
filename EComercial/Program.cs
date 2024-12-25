using System;
using System.Linq;
using System.Collections.Generic;
using EComercial.Core;
using EComercial.DbDataContext;
using EComercial.Repository;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using EComercial.Interface;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EComercial
{
    class Program
    {
        #region FullScreen
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private const int SW_MAXIMIZE = 3;
        private const byte VK_RETURN = 0x0D;
        private const byte VK_ALT = 0x12;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private static void MaximizeConsole()
        {
            IntPtr consoleWindow = GetConsoleWindow();
            if (consoleWindow != IntPtr.Zero)
            {
                ShowWindow(consoleWindow, SW_MAXIMIZE);
            }
            Console.SetBufferSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
        }
        private static void ForceFullScreen()
        {
            keybd_event(VK_ALT, 0, 0, UIntPtr.Zero);
            keybd_event(VK_RETURN, 0, 0, UIntPtr.Zero);
            keybd_event(VK_RETURN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            keybd_event(VK_ALT, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }
        #endregion

        static void Main(string[] args)
        {
            MaximizeConsole();
            ForceFullScreen();

            var navigator = new MenuNavigator();
            navigator.StartMenu();
        }
    }
}
