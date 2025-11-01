namespace JamDataFinder;


public class ArgProcessor
{
    public Modes Mode { get; set; } = Modes.Help;
    public string? InputFile { get; set; }
    public string? OutFile { get; set; }
    
    public string? HdFile { get; set; }
    
    public ArgProcessor(string[] args)
    {
        var lastArg = "";
        foreach (var arg in args)
        {
            Mode = arg switch
            {
                "--help" or "-h" => Modes.Help,
                "--version" or "-v" => Modes.Version,
                "--find-seq" or "-fs" => Modes.FindSq,
                "--find-bd" or "-fb" => Modes.FindBd,
                "--find-hd" or "-fh" => Modes.FindHd,
                _ => Mode
            };

            switch (lastArg)
            {
                case "--input":
                case "-i":
                    InputFile = arg;
                    break;
                case "--output":
                case "-o":
                    OutFile = arg;
                    break;
                case "--hd-file":
                case "-hf":
                    HdFile = arg;
                    break;
            }
            lastArg = arg;
        }
    }

    public bool Validate()
    {
        return Mode switch
        {
            Modes.FindBd => InputFile != null && File.Exists(InputFile) && OutFile != null && File.Exists(HdFile),
            Modes.FindSq or Modes.FindHd =>
                InputFile != null && File.Exists(InputFile) && OutFile != null,
            _ => true
        };
    }
}

public enum Modes
{
    Help,
    Version,
    FindSq,
    FindHd,
    FindBd
}