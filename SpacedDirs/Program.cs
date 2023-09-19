using System;
using System.Runtime.InteropServices;

namespace SpacedDirs
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {

                Console.WriteLine("Usage: SpacedDirs.exe <create/delete> <directory>");
                Console.WriteLine("Directory create/delete tool that forces acceptance of trailing blank spaces.");
                Console.WriteLine("Wrap directory path including spaces in quotes:");
                Console.WriteLine("SpacedDirs.exe create \"C:\\windows \" then run SpacedDirs.exe create \"C:\\windows \\system32\"");
                Console.WriteLine("SpacedDirs.exe delete \"C:\\windows \\system32\" then run SpacedDirs.exe delete \"C:\\windows \"");
                Console.WriteLine("Can only create/delete the end-most directory at a time. The directory must be empty and not occupied/locked to delete.");
                Environment.Exit(1);
            }

            string path = args[1];
            try
            {
                if ((args[0].Equals("create", StringComparison.OrdinalIgnoreCase)))
                {
                    string directorypath = @"\\?\" + path + @"\";
                    CreateFolder(directorypath);
                    Console.WriteLine("[+] Created Folder: \"" + path + "\"");
                }

                if ((args[0].Equals("delete", StringComparison.OrdinalIgnoreCase)))
                {
                    string directorypath = @"\\?\" + path + @"\";
                    DeleteFolder(directorypath);
                    Console.WriteLine("[+] Deleted Folder: \"" + path + "\"");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.ToString());
            }
            
        }

        //If the value of SECURITY_ATTRIBUTES is NULL, the object is assigned the default security descriptor associated with the access token of the calling process
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto, BestFitMapping = false)]
        internal static extern bool CreateDirectory(String path, SECURITY_ATTRIBUTES lpSecurityAttributes);

        [StructLayout(LayoutKind.Sequential)]
        public class SECURITY_ATTRIBUTES
        {
            internal int nLength = 0;
            // don't remove null, or it will fail to set the default ACL, making the folder inaccessible and non-removeable
            // unsafe is available if compile with /unsafe flag, https://docs.microsoft.com/en-us/dotnet/csharp/misc/cs0227
            public IntPtr? pSecurityDescriptor = null;
            //or
            //internal unsafe byte* pSecurityDescriptor = null;
            internal int bInheritHandle = 0;
            public IntPtr lpSecurityDescriptor;
        }

        public static bool CreateFolder(string path)
        {
            var lpSecurityAttributes = new SECURITY_ATTRIBUTES();
            var security = new System.Security.AccessControl.DirectorySecurity();
            lpSecurityAttributes.nLength = Marshal.SizeOf(lpSecurityAttributes);
            byte[] src = security.GetSecurityDescriptorBinaryForm();
            IntPtr dest = Marshal.AllocHGlobal(src.Length);
            Marshal.Copy(src, 0, dest, src.Length);
            lpSecurityAttributes.lpSecurityDescriptor = dest;
            return CreateDirectory(path, lpSecurityAttributes);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool RemoveDirectory(string lpPathName);

        public static bool DeleteFolder(string path)
        {
            return RemoveDirectory(path);
        }

    }
}
