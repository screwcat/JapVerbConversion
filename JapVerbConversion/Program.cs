using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;

namespace JapVerbConversion
{
    static class Program
    {
        public static string JapCharDict = Application.StartupPath + "\\Data\\" + ConfigurationManager.AppSettings["JapCharDict"];
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}
