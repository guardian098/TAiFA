using Lexems;

using Parser;

using VirtualMachine;

namespace Interpreter.AcceptanceTests;

public class InterpreterTest
{
    [Theory]
    [MemberData(nameof(GetDocumentationProgramData))]
    public void ExecuteProgramFromDocumentation(string source, string expected)
    {
        Assert.Equal(expected, Run(source));
    }

    [Theory]
    [MemberData(nameof(GetProgramData))]
    public void ExecuteProgram(string source, string expected)
    {
        Assert.Equal(expected, Run(source));
    }

    [Theory]
    [MemberData(nameof(GetProgramWithInputData))]
    public void ExecuteProgramWithInput(string source, string input, string expected)
    {
        Assert.Equal(expected, Run(source, input));
    }

    [Theory]
    [MemberData(nameof(GetInvalidLexemeData))]
    public void InvalidLexemeThrowsLexerException(string source)
    {
        Assert.Throws<LexerException>(() => Run(source));
    }

    [Theory]
    [MemberData(nameof(GetInvalidSyntaxData))]
    public void InvalidSyntaxThrowsParserException(string source)
    {
        Assert.Throws<ParserException>(() => Run(source));
    }

    [Theory]
    [MemberData(nameof(GetRuntimeErrorData))]
    public void RuntimeErrorThrowsVirtualMachineException(string source)
    {
        Assert.Throws<VirtualMachineException>(() => Run(source));
    }

    [Theory]
    [MemberData(nameof(GetRuntimeErrorWithInputData))]
    public void RuntimeErrorWithInputThrowsVirtualMachineException(string source, string input)
    {
        Assert.Throws<VirtualMachineException>(() => Run(source, input));
    }

    public static TheoryData<string, string> GetDocumentationProgramData()
    {
        return new TheoryData<string, string>
        {
            {
                "void main() { print(7 / 2); print(7 % 2); print(abs(-5)); print(sqrt(16)); print(min(3, 5)); print(max(3, 5)); }",
                "3\n1\n5\n4\n3\n5\n"
            },
        };
    }

    public static TheoryData<string, string> GetProgramData()
    {
        return new TheoryData<string, string>
        {
            { "void main() { print(min(sqrt(16), abs(-5))); }", "4\n" },
            { "void main() { print(-(-5)); }", "5\n" },
            { "void main() { print(-(5 + 3)); }", "-8\n" },
            { "void main() { 2 + 3; print(6); }", "6\n" },
            { "void main() { 7 + 3; }", "" },
            { "void main() { print(1 + 2 * 3 - 4 / 2); }", "5\n" },
            { "void main() { print(10 + 7 * (2 * 3 + 4) + 5 + -5); }", "80\n" },
        };
    }

    public static TheoryData<string, string, string> GetProgramWithInputData()
    {
        return new TheoryData<string, string, string>
        {
            { "void main() { print(readInt()); }", "42", "42\n" },
            { "void main() { print(readInt() + readInt()); }", "2 3", "5\n" },
            { "void main() { print(readInt() - readInt()); }", "10\n3\n", "7\n" },
            { "void main() { print(max(readInt(), readInt())); }", "4 9", "9\n" },
            { "void main() { print(-readInt()); }", "5", "-5\n" },
            { "void main() { readInt(); print(1); }", "5", "1\n" },
            { "void main() { print(readInt()); print(readInt()); }", "1\n2\n", "1\n2\n" },
        };
    }

    public static TheoryData<string> GetInvalidLexemeData()
    {
        return new TheoryData<string>
        {
            { "void main() { print(2147483648); }" },
            { "void main() { print(-2147483648); }" },
        };
    }

    public static TheoryData<string> GetInvalidSyntaxData()
    {
        return new TheoryData<string>
        {
            { "void main() { print(1; }" },
            { "void main() { print(); }" },
            { "void main() { min(3 5); }" },
            { "void main() print(1);" },
            { "void main() { } void main() { }" },
            { "void main() { readInt(1); }" },
            { "void main() { readInt; }" },
        };
    }

    public static TheoryData<string> GetRuntimeErrorData()
    {
        return new TheoryData<string>
        {
            { "void main() { print(10 / 0); }" },
            { "void main() { print(10 % 0); }" },
            { "void main() { print(sqrt(-1)); }" },
            { "void main() { print(2147483647 + 1); }" },
            { "void main() { print(-2147483647 - 2); }" },
        };
    }

    public static TheoryData<string, string> GetRuntimeErrorWithInputData()
    {
        return new TheoryData<string, string>
        {
            { "void main() { print(readInt()); }", "" },
            { "void main() { print(readInt()); }", "abc" },
            { "void main() { print(readInt()); }", "2147483648" },
            { "void main() { print(readInt() + readInt()); }", "1" },
            { "void main() { print(10 / readInt()); }", "0" },
        };
    }

    public static string Run(string source, string input = "")
    {
        IReadOnlyList<Token> tokens = Lexer.Tokenize(source);
        List<Instruction> instructions = new Parser.Parser(tokens).ParseProgram();

        using StringWriter output = new StringWriter();
        using StringReader reader = new StringReader(input);
        new VirtualMachine.VirtualMachine(instructions, output, reader).Run();

        return output.ToString().Replace("\r\n", "\n");
    }
}