using System;
using System.IO;

namespace SetAttributes
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1) {

                Console.WriteLine("Usage: SetAttributes.exe <add/remove> <check,archive,notcontentindexed,normal,readonly,system> <path to directory or file>");
                Environment.Exit(1);
            }
            
            string path = args[2];

            //if (!File.Exists(path))
            //{
            //    Console.WriteLine("Target File not found, possible Directory selected: " + path);
            //}

            try
            {

                FileAttributes attributes = File.GetAttributes(path);
                Console.WriteLine("[+] Attributes Currently Set to: " + attributes);

                if ((args[0].Equals("check", StringComparison.OrdinalIgnoreCase)))
                {
                    string check_path = args[1];
                    FileInfo fi = new FileInfo(check_path);
                    //FileAttributes attributes = File.GetAttributes(check_path);

                    Console.WriteLine("[+] Attributes Currently Set to: " + attributes);
                }


                if ((args[1].Equals("hidden", StringComparison.OrdinalIgnoreCase)))
                {
                    FileInfo fi = new FileInfo(path);
                    //FileAttributes attributes = File.GetAttributes(path);

                    //Console.WriteLine("[+] Attributes Currently Set to: " + attributes);

                    if ((!fi.Attributes.HasFlag(FileAttributes.Hidden)) && (args[0].Equals("add", StringComparison.OrdinalIgnoreCase)))
                    {
                        fi.Attributes |= FileAttributes.Hidden;
                        Console.WriteLine("[+] Hidden attribute set for: " + path);
                    }
                    else
                    {
                        //Console.WriteLine("[*] Hidden attribute already on: " + path);
                    }

                    if ((fi.Attributes.HasFlag(FileAttributes.Hidden)) && (args[0].Equals("remove", StringComparison.OrdinalIgnoreCase)))
                    {
                        
                        attributes = RemoveAttribute(attributes, FileAttributes.Hidden);
                        File.SetAttributes(path, attributes);
                        Console.WriteLine("[+] Hidden attribute removed for: " + path);
                    }
                    else
                    {
                        //Console.WriteLine("Hidden attribute not on: " + path);
                    }

                }

                if ((args[1].Equals("readonly", StringComparison.OrdinalIgnoreCase)))
                {
                    FileInfo fi = new FileInfo(path);
                    //FileAttributes attributes = File.GetAttributes(path);

                    if ((!fi.Attributes.HasFlag(FileAttributes.ReadOnly)) && (args[0].Equals("add", StringComparison.OrdinalIgnoreCase)))
                    {
                        fi.Attributes |= FileAttributes.ReadOnly;
                        Console.WriteLine("[+] ReadOnly attribute set for: " + path);
                    }
                    else
                    {
                        //Console.WriteLine("[*] ReadOnly attribute already on: " + path);
                    }

                    if ((fi.Attributes.HasFlag(FileAttributes.ReadOnly)) && (args[0].Equals("remove", StringComparison.OrdinalIgnoreCase)))
                    {
                        attributes = RemoveAttribute(attributes, FileAttributes.ReadOnly);
                        File.SetAttributes(path, attributes);
                        Console.WriteLine("[+] ReadOnly attribute removed for: " + path);
                    }
                    else
                    {
                        //Console.WriteLine("ReadOnly attribute not on: " + path);
                    }

                }

                if ((args[1].Equals("notcontentindexed", StringComparison.OrdinalIgnoreCase)))
                {
                    FileInfo fi = new FileInfo(path);
                    //FileAttributes attributes = File.GetAttributes(path);

                    if ((!fi.Attributes.HasFlag(FileAttributes.NotContentIndexed)) && (args[0].Equals("add", StringComparison.OrdinalIgnoreCase)))
                    {
                        fi.Attributes |= FileAttributes.NotContentIndexed;
                        Console.WriteLine("[+] NotContentIndexed attribute set for: " + path);
                    }
                    else
                    {
                        //Console.WriteLine("[*] NotContentIndexed attribute already on: " + path);
                    }

                    if ((fi.Attributes.HasFlag(FileAttributes.NotContentIndexed)) && (args[0].Equals("remove", StringComparison.OrdinalIgnoreCase)))
                    {
                        attributes = RemoveAttribute(attributes, FileAttributes.NotContentIndexed);
                        File.SetAttributes(path, attributes);
                        Console.WriteLine("[+] NotContentIndexed attribute removed for: " + path);
                    }
                    else
                    {
                        //Console.WriteLine("NotContentIndexed attribute not on: " + path);
                    }

                }

                if ((args[1].Equals("archive", StringComparison.OrdinalIgnoreCase)))
                {
                    FileInfo fi = new FileInfo(path);
                    //FileAttributes attributes = File.GetAttributes(path);

                    if ((!fi.Attributes.HasFlag(FileAttributes.Archive)) && (args[0].Equals("add", StringComparison.OrdinalIgnoreCase)))
                    {
                        fi.Attributes |= FileAttributes.Archive;
                        Console.WriteLine("[+] Archive attribute set for: " + path);
                    }
                    else
                    {
                        //Console.WriteLine("[*] Archive attribute already on: " + path);
                    }

                    if ((fi.Attributes.HasFlag(FileAttributes.Archive)) && (args[0].Equals("remove", StringComparison.OrdinalIgnoreCase)))
                    {
                        attributes = RemoveAttribute(attributes, FileAttributes.Archive);
                        File.SetAttributes(path, attributes);
                        Console.WriteLine("[+] Archive attribute removed for: " + path);
                    }
                    else
                    {
                        //Console.WriteLine("Archive attribute not on: " + path);
                    }

                }

                if ((args[1].Equals("system", StringComparison.OrdinalIgnoreCase)))
                {
                    FileInfo fi = new FileInfo(path);
                    //FileAttributes attributes = File.GetAttributes(path);

                    if ((!fi.Attributes.HasFlag(FileAttributes.System)) && (args[0].Equals("add", StringComparison.OrdinalIgnoreCase)))
                    {
                        fi.Attributes |= FileAttributes.System;
                        Console.WriteLine("[+] System attribute set for: " + path);
                    }
                    else
                    {
                        //Console.WriteLine("[*] System attribute already on: " + path);
                    }

                    if ((fi.Attributes.HasFlag(FileAttributes.System)) && (args[0].Equals("remove", StringComparison.OrdinalIgnoreCase)))
                    {
                        attributes = RemoveAttribute(attributes, FileAttributes.System);
                        File.SetAttributes(path, attributes);
                        Console.WriteLine("[+] System attribute removed for: " + path);
                    }
                    else
                    {
                        //Console.WriteLine("System attribute not on: " + path);
                    }

                }

                if ((args[1].Equals("normal", StringComparison.OrdinalIgnoreCase)))
                {
                    FileInfo fi = new FileInfo(path);
                    //FileAttributes attributes = File.GetAttributes(path);

                    if ((!fi.Attributes.HasFlag(FileAttributes.Normal)) && (args[0].Equals("add", StringComparison.OrdinalIgnoreCase)))
                    {
                        fi.Attributes = FileAttributes.Normal;
                        Console.WriteLine("[+] Normal attributes set for: " + path);
                    }
                    else
                    {
                        //Console.WriteLine("Normal attribute already on: " + path);
                    }

                    if ((fi.Attributes.HasFlag(FileAttributes.Normal)) && (args[0].Equals("remove", StringComparison.OrdinalIgnoreCase)))
                    {
                        Console.WriteLine("[*] Cannot remove Normal attributes");

                        //attributes = RemoveAttribute(attributes, FileAttributes.Normal);
                        //File.SetAttributes(path, attributes);
                        //Console.WriteLine("Normal attribute removed for: " + path);
                    }
                    else
                    {
                        //Console.WriteLine("System attribute not on: " + path);
                    }
                }

                /*if (args[0].Equals("dir", StringComparison.OrdinalIgnoreCase))
                {
                    DirectoryInfo di = new DirectoryInfo(path);

                    if (!di.Attributes.HasFlag(FileAttributes.Hidden))
                    {
                        di.Attributes |= FileAttributes.Hidden;
                        Console.WriteLine("Hidden attribute set on directory: " + path);
                    }
                    else
                    {
                        Console.WriteLine("Hidden attribute already on directory: " + path);
                    }
                }*/

            FileAttributes new_attributes = File.GetAttributes(path);
            Console.WriteLine("[+] Attributes Now Set to: " + new_attributes);

            }

            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.ToString());
            }
        }

        private static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }
    }
}
