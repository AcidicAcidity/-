using System;
using System.Windows.Forms;
using ReservationApp.Forms; // Добавлена необходимая директива для доступа к MainForm

namespace ReservationApp
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Console.WriteLine("Программа запускается...");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm()); // ИСПРАВЛЕНО: Запуск MainForm
        }
    }
}