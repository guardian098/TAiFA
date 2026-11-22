using System.Diagnostics;

namespace Interpreter.AcceptanceTests;

public class InterpreterProcessTest
{
    private const int TimeoutMilliseconds = 30000;

    [Fact]
    public void ValidProgramPrintsToStdoutAndReturnsZero()
    {
        (int exitCode, string stdout, string stderr) = RunSource(
            "void main() { print(7 / 2); print(7 % 2); print(abs(-5)); print(sqrt(16)); print(min(3, 5)); print(max(3, 5)); }");

        Assert.Equal(0, exitCode);
        Assert.Equal("3\n1\n5\n4\n3\n5\n", stdout);
        Assert.Empty(stderr);
    }

    [Theory]
    [InlineData("void main() { print(readInt()); }", "42\n", "42\n")]
    [InlineData("void main() { print(readInt() + readInt()); }", "2 3\n", "5\n")]
    [InlineData("void main() { print(readInt() - readInt()); }", "10\r\n3\r\n", "7\n")]
    [InlineData("void main() { print(max(readInt(), readInt())); }", "  4\n\n 9", "9\n")]
    [InlineData("void main() { print(readInt()); }", "-2147483648", "-2147483648\n")]
    [InlineData("void main() { readInt(); print(1); }", "5", "1\n")]
    public void ProgramReadsIntegersFromStdin(string source, string stdin, string expectedStdout)
    {
        (int exitCode, string stdout, string stderr) = RunSource(source, stdin);

        Assert.Equal(0, exitCode);
        Assert.Equal(expectedStdout, stdout);
        Assert.Empty(stderr);
    }

    [Theory]
    [InlineData("void main() { print(2147483648); }", "Lexical error")]
    [InlineData("void main() { print(1; }", "Syntax error")]
    [InlineData("void main() { print(readInt(1)); }", "Syntax error")]
    [InlineData("void main() { print(10 / 0); }", "Runtime error")]
    [InlineData("void main() { print(2147483647 + 1); }", "Runtime error")]
    public void ErrorsAreReportedToStderrWithNonZeroExitCode(string source, string expectedMessage)
    {
        (int exitCode, string stdout, string stderr) = RunSource(source);

        Assert.Equal(1, exitCode);
        Assert.Empty(stdout);
        Assert.Contains(expectedMessage, stderr);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   \n")]
    [InlineData("abc")]
    [InlineData("12abc")]
    [InlineData("2147483648")]
    public void InvalidStdinIsReportedAsRuntimeError(string stdin)
    {
        (int exitCode, string stdout, string stderr) = RunSource("void main() { print(readInt()); }", stdin);

        Assert.Equal(1, exitCode);
        Assert.Empty(stdout);
        Assert.Contains("Runtime error", stderr);
    }

    [Fact]
    public void OutputPrintedBeforeRuntimeErrorIsKept()
    {
        (int exitCode, string stdout, string stderr) = RunSource("void main() { print(1); print(10 / 0); }");

        Assert.Equal(1, exitCode);
        Assert.Equal("1\n", stdout);
        Assert.Contains("Runtime error", stderr);
    }

    [Fact]
    public void OutputPrintedBeforeInputErrorIsKept()
    {
        (int exitCode, string stdout, string stderr) = RunSource("void main() { print(1); print(readInt()); }");

        Assert.Equal(1, exitCode);
        Assert.Equal("1\n", stdout);
        Assert.Contains("Runtime error", stderr);
    }

    [Fact]
    public void MissingArgumentReturnsNonZeroExitCode()
    {
        (int exitCode, string stdout, string stderr) = RunInterpreter(string.Empty);

        Assert.Equal(1, exitCode);
        Assert.Empty(stdout);
        Assert.Contains("Usage", stderr);
    }

    [Fact]
    public void MissingFileReturnsNonZeroExitCode()
    {
        string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".ppp");

        (int exitCode, string stdout, string stderr) = RunInterpreter(string.Empty, path);

        Assert.Equal(1, exitCode);
        Assert.Empty(stdout);
        Assert.Contains("not found", stderr);
    }

    private static (int ExitCode, string StdOut, string StdErr) RunSource(string source, string stdin = "")
    {
        string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".ppp");
        File.WriteAllText(path, source);

        try
        {
            return RunInterpreter(stdin, path);
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static (int ExitCode, string StdOut, string StdErr) RunInterpreter(string stdin, params string[] args)
    {
        ProcessStartInfo info = new ProcessStartInfo("dotnet")
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        info.ArgumentList.Add(Path.Combine(AppContext.BaseDirectory, "interpreter.dll"));
        foreach (string arg in args)
        {
            info.ArgumentList.Add(arg);
        }

        using Process process = Process.Start(info)
            ?? throw new InvalidOperationException("Failed to start interpreter process.");

        Task<string> stdoutTask = process.StandardOutput.ReadToEndAsync();
        Task<string> stderrTask = process.StandardError.ReadToEndAsync();

        try
        {
            process.StandardInput.Write(stdin);
            process.StandardInput.Close();
        }
        catch (IOException)
        {
            // Интерпретатор мог завершиться, не дочитав ввод (например, из-за ошибки разбора).
        }

        if (!process.WaitForExit(TimeoutMilliseconds))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("Interpreter process did not finish in time.");
        }

        process.WaitForExit();

        string stdout = stdoutTask.Result.Replace("\r\n", "\n");
        string stderr = stderrTask.Result;

        return (process.ExitCode, stdout, stderr);
    }
}