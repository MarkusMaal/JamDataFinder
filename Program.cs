using System.Reflection;

namespace JamDataFinder
{
    public abstract class Program
    {
        public static int Main(string[] args)
        {
            var processor = new ArgProcessor(args);
            if (!processor.Validate())
            {
                Console.WriteLine("Invalid arguments. Please pass --help to see correct usage.");
                return 1;
            }

            switch (processor.Mode)
            {
                case Modes.Help:
                    ShowHelp();
                    break;
                case Modes.Version:
                    Console.WriteLine($"Version: {System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}\n.NET runtime version: {Environment.Version}");
                    break;
                case Modes.FindHd:
                    DataSearchers.FindHd(processor.InputFile!, processor.OutFile!);
                    break;
                case Modes.FindBd:
                    DataSearchers.FindBd(processor.InputFile!, processor.HdFile!, processor.OutFile!);
                    break;
                case Modes.FindSq:
                    DataSearchers.FindSeq(processor.InputFile!, processor.OutFile!);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return 0;
        }

        private static void ShowHelp()
        {
            Console.WriteLine("""
                              JamDataFinder
                              Usage: JamDataFinder [args] -i [input] -o [output]

                              --help [-h]           - Show command line usage
                              --version [-v]        - Show version information

                              --find-hd [-fh]       - Search for JAM header files
                              --find-bd [-fb]       - Search for JAM body data (requires --hd-file)
                              --find-seq [-fs]      - Search for JAM sequences

                              --input [-i]          - Input file (not required for -h and -v)
                              --output [-o]         - Output directory (not required for -h and -v)
                              --hd-file [-hf]       - Header file location
                              """);
        }
    }
}