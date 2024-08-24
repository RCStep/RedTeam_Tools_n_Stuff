using System;
using System.Diagnostics;
 
namespace explorer_open
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                Console.WriteLine("[+] explorer_open.exe open/select \"path to folder or file\"");
                Console.WriteLine("[+] open: explorer window opens to the folder or the target file executed");
                Console.WriteLine("[+] select: explorer window opens with folder or target file selected/highlighted");
                return;
            }

            if (args[0] == "open")
            {
                string p = args[1];

                string path = string.Format("/e, \"{0}\"", p);

                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "explorer";
                info.Arguments = path;
                Process.Start(info);
                Console.WriteLine("[+] Explorer window should be open to the folder or the target ran: {0}", p);

                return;
            }

            if (args[0] == "select")
            {
                string p = args[1];

                string path = string.Format("/e, /select, \"{0}\"", p);

                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "explorer";
                info.Arguments = path;
                Process.Start(info);
                Console.WriteLine("[+] Explorer window should be open and the file or folder selected/highlighted: {0}", p);

                return;
            }
        }
    }
}
