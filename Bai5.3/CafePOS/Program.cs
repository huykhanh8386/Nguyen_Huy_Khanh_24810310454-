// Program.cs
using System;
using System.Windows.Forms;
using CafePOS.GUI;

namespace CafePOS
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormCafe());
        }
    }
}
