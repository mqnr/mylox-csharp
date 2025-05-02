namespace MyLox;

public class MyLox
{
    private static bool _hadError;

    private static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: mylox [script]");
            Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }
    }

    private static void RunFile(String path)
    {
        Run(File.ReadAllText(path));

        if (_hadError)
        {
            Environment.Exit(65);
        }
    }

    private static void RunPrompt()
    {
        string? line;
     
        Console.Write("> ");
        while ((line = Console.ReadLine()) != null)
        {
            Run(line);
            _hadError = false;
            Console.Write("> ");
        }
    }

    private static void Run(string source)
    {
        var scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
        _hadError = true;
    }
}
