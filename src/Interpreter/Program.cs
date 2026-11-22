using Lexems;
using Parser;
using VirtualMachine;

namespace Interpreter;

internal static class Program
{
    private const int ExitSuccess = 0;
    private const int ExitFailure = 1;

    public static int Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Usage: interpreter <file-path>");
            return ExitFailure;
        }

        string sourcePath = args[0];
        if (!File.Exists(sourcePath))
        {
            Console.Error.WriteLine($"Error: source file '{sourcePath}' not found.");
            return ExitFailure;
        }

        try
        {
            // Сначала разбирается весь файл, и только потом запускается VM.
            IReadOnlyList<Token> tokens = Lexer.TokenizeFile(sourcePath);
            List<Instruction> instructions = new Parser.Parser(tokens).ParseProgram();

            // Ввод читается из stdin, вывод пишется в stdout.
            new VirtualMachine.VirtualMachine(instructions, Console.Out, Console.In).Run();
            return ExitSuccess;
        }
        catch (LexerException ex)
        {
            Console.Error.WriteLine($"Lexical error: {ex.Message}");
            return ExitFailure;
        }
        catch (ParserException ex)
        {
            Console.Error.WriteLine($"Syntax error: {ex.Message}");
            return ExitFailure;
        }
        catch (VirtualMachineException ex)
        {
            Console.Error.WriteLine($"Runtime error: {ex.Message}");
            return ExitFailure;
        }
        catch (Exception ex)
        {
            // Любая иная ошибка (например, ввода-вывода) тоже должна завершать процесс с ненулевым кодом.
            Console.Error.WriteLine($"Interpreter threw an {ex.GetType().Name}: {ex.Message}");
            return ExitFailure;
        }
    }
}