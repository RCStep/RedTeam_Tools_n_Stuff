using System;
using System.Runtime.InteropServices;

namespace DeleteonReboot
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: DeleteonReboot.exe <path_to_file>");
                Environment.Exit(1);
            }

            string file = args[0];

            if (!NativeMethods.MoveFileEx(file, null, MoveFileFlags.DelayUntilReboot))
            {
                Console.Error.WriteLine("Unable to schedule file for deletion: " + file);
            }
            else
            {
                Console.WriteLine("Scheduled for deletion on system reboot: " + file);
            }
        }
    }
}

[Flags]
internal enum MoveFileFlags
{
    None = 0,
    ReplaceExisting = 1,
    CopyAllowed = 2,
    DelayUntilReboot = 4,
    WriteThrough = 8,
    CreateHardlink = 16,
    FailIfNotTrackable = 32,
}

internal static class NativeMethods
{
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool MoveFileEx(
        string lpExistingFileName,
        string lpNewFileName,
        MoveFileFlags dwFlags);
}
