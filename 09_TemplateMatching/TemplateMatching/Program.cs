using System;
using System.Windows.Forms;

namespace TemplateMatching
{
    internal static class Program
    {
        /// <summary>
        /// 應用程式進入點。
        /// 注意：.NET Framework 沒有 ApplicationConfiguration.Initialize()（那是 .NET 6+ 專屬），
        /// 要用傳統的 EnableVisualStyles + SetCompatibleTextRenderingDefault。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
