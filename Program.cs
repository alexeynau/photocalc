using System;
using System.Windows.Forms;

namespace PhotoOrderCalculator
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Проверяем, не запущено ли уже приложение
            bool createdNew;
            using var mutex = new System.Threading.Mutex(true, "PhotoOrderCalculator_SingleInstance", out createdNew);
            
            if (!createdNew)
            {
                MessageBox.Show("Приложение уже запущено!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            Application.Run(new TrayApplicationContext());
        }
    }
}
