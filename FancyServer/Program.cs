using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Forms;

using FancyLibrary.Bridges;


namespace FancyServer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new NoForm());
        }

        public static void Test() {
            PipeClient server = new PipeClient();
            PipeServer client = new PipeServer();
            ResXResourceReader res = new ResXResourceReader("");
        }
    }
}
