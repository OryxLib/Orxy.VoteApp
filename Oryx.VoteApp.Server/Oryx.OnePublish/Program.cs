using System;
using System.IO;
using System.Threading.Tasks;
using Tamir.SharpSsh;

namespace Oryx.OnePublish
{
    class Program
    {
        static void Main(string[] args)
        {
            //SshConnectionInfo input = Util.GetInput();
            SshShell shell = new SshShell("101.132.130.133", "root");
            shell.Password = "Adminqwer!@#$";

            //This statement must be prior to connecting
            shell.RedirectToConsole();

            Console.Write("Connecting...");
            shell.Connect();
            Console.WriteLine("OK");

            while (shell.ShellOpened)
            {
                System.Threading.Thread.Sleep(500);
            }
            Console.Write("Disconnecting...");
            shell.Close();
            Console.WriteLine("OK");

            Console.ReadKey();
        }
    }
}
