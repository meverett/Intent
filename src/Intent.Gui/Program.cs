using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Intent.Gui
{
    static class Program
    {
        /// <summary>
        /// Current application instance.
        /// </summary>
        public static AppForm Current { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Current = new AppForm();
            Application.Run(Current);
        }
    }
}
