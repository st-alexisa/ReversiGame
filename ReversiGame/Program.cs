using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReversiGame
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var startForm = new StartForm();
            Application.Run(startForm);
            if (startForm.Field != null)
                Application.Run(new FieldForm(startForm.Field, startForm.CurrentTurn, startForm.GameMode));
        }
    }
}
