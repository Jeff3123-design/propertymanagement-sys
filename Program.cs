using System;
using System.Windows.Forms;
using PropertyManagementSystem.Forms;

namespace PropertyManagementSystem
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Show login form first
            Application.Run(new LoginForm());
        }
    }
}