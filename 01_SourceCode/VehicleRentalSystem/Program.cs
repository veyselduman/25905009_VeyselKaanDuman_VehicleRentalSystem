using System;
using System.Windows.Forms;
using VehicleRentalSystem.Forms;
using VehicleRentalSystem.Services;

namespace VehicleRentalSystem
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            RentalManager manager = new RentalManager();
            Application.Run(new LoginForm(manager));
        }
    }
}
