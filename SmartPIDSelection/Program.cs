using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace SmartPIDSelection
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Provide a process name for PID selection");
                return;
            }

            string pname = args[0];
            int processPID = SmartFindPID(pname);
            Console.WriteLine("SmartPIDSelection: {0}", processPID);
        }

        [DllImport("advapi32")]
        public static extern bool OpenProcessToken(
        IntPtr ProcessHandle,
        int DesiredAccess,
        ref IntPtr TokenHandle);

        [DllImport("kernel32")]
        public static extern bool CloseHandle(IntPtr handle);

        private static int SmartFindPID(string process)
        {
            int pid = 0;

            if (string.IsNullOrEmpty(process))
            {
                return pid;
            }

            else if (Regex.IsMatch(process, @"^\d+"))
            {
                pid = int.Parse(process);
                Console.WriteLine("[*] Finding an appropriate PID for process name: {0}", pid);

                return pid;
            }

            else
            {
                string username = WindowsIdentity.GetCurrent().Name;
                Console.WriteLine("[*] Running User Identity: \"{0}\" ", username);

                int session = Process.GetCurrentProcess().SessionId;
                Console.WriteLine("[*] Running User Session is: \"{0}\" ", session);

                Process[] processes = Process.GetProcessesByName(process);

                int minId = int.MaxValue;

                foreach (Process proc in processes)
                {
                    try
                    {
                        if (proc.Id < minId)
                        {
                            string ppiduser = null;

                            if (proc.SessionId == session)
                            {
                                IntPtr procToken = IntPtr.Zero;
                                Process hprocess = proc;

                                try
                                {

                                    OpenProcessToken(hprocess.Handle, 8, ref procToken);

                                    WindowsIdentity wi = new WindowsIdentity(procToken);
                                    ppiduser = wi.Name;

                                    CloseHandle(procToken);
                                    CloseHandle(hprocess.Handle);

                                }
                                catch (Exception ex)
                                {
                                    //Console.WriteLine("[-] Could not open a handle to PID: {0}, Error: {1}", pid, ex);
                                }
                            }

                            if (username == ppiduser)
                            {
                                minId = proc.Id;
                                pid = proc.Id;

                                Console.WriteLine("[*] Found Process: \"{0}\" at PID: {1} for Session: {2} running as user {3}", process, pid, session, ppiduser);
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        //Console.WriteLine("[-] Error: {0]", ex);
                    }
                }

                Console.WriteLine("[+] Identified process: \"{0}\" with lowest PID: {1} for Session: {2} User: {3}", process, pid, session, username);

                return pid;
            }
        }
    }
}
