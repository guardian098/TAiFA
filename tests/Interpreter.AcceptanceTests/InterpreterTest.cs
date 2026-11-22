using Lexems;

using Parser;

using VirtualMachine;

namespace Interpreter.AcceptanceTests;

public class InterpreterTest
{
    [Theory]
    [InlineData("void main() { print(7 / 2); print(7 % 2); print(abs(-5)); print(sqrt(16)); print(min(3, 5)); print(max(3, 5)); }", "3\n1\n5\n4\n3\n5\n")]
    public void ExecuteProgramFromDocumentation(string source, string expected)
    {
        Assert.Equal(expected, Run(source));
    }

    [Theory]
    [InlineData("void main() { print(min(sqrt(16), abs(-5))); }", "4\n")]
    [InlineData("void main() { print(-(-5)); }", "5\n")]
    [InlineData("void main() { print(-(5 + 3)); }", "-8\n")]
    [InlineData("void main() { 2 + 3; print(6); }", "6\n")]
    [InlineData("void main() { 7 + 3; }", "")]
    [InlineData("void main() { print(1 + 2 * 3 - 4 / 2); }", "5\n")]
    [InlineData("void main() { print(10 + 7 * (2 * 3 + 4) + 5 + -5); }", "80\n")]
    public void ExecuteProgram(string source, string expected)
    {
        Assert.Equal(expected, Run(source));
    }

    [Theory]
    [InlineData("void main() { print(readInt()); }", "42", "42\n")]
    [InlineData("void main() { print(readInt() + readInt()); }", "2 3", "5\n")]
    [InlineData("void main() { print(readInt() - readInt()); }", "10\n3\n", "7\n")]
    [InlineData("void main() { print(max(readInt(), readInt())); }", "4 9", "9\n")]
    [InlineData("void main() { print(-readInt()); }", "5", "-5\n")]
    [InlineData("void main() { readInt(); print(1); }", "5", "1\n")]
    [InlineData("void main() { print(readInt()); print(readInt()); }", "1\n2\n", "1\n2\n")]
    public void ExecuteProgramWithInput(string source, string input, string expected)
    {
        Assert.Equal(expected, Run(source, input));
    }

    [Theory]
    [InlineData("void main() { print(2147483648); }")]
    [InlineData("void main() { print(-2147483648); }")]
    public void InvalidLexemeThrowsLexerException(string source)
    {
        Assert.Throws<LexerException>(() => Run(source));
    }

    [Theory]
    [InlineData("void main() { print(1; }")]
    [InlineData("void main() { print(); }")]
    [InlineData("void main() { min(3 5); }")]
    [InlineData("void main() print(1);")]
    [InlineData("void main() { } void main() { }")]
    [InlineData("void main() { readInt(1); }")]
    [InlineData("void main() { readInt; }")]
    public void InvalidSyntaxThrowsParserException(string source)
    {
        Assert.Throws<ParserException>(() => Run(source));
    }

    [Theory]
    [InlineData("void main() { print(10 / 0); }")]
    [InlineData("void main() { print(10 % 0); }")]
    [InlineData("void main() { print(sqrt(-1)); }")]
    [InlineData("void main() { print(2147483647 + 1); }")]
    [InlineData("void main() { print(-2147483647 - 2); }")]
    public void RuntimeErrorThrowsVirtualMachineException(string source)
    {
        Assert.Throws<VirtualMachineException>(() => Run(source));
    }

    [Theory]
    [InlineData("void main() { print(readInt()); }", "")]
    [InlineData("void main() { print(readInt()); }", "abc")]
    [InlineData("void main() { print(readInt()); }", "2147483648")]
    [InlineData("void main() { print(readInt() + readInt()); }", "1")]
    [InlineData("void main() { print(10 / readInt()); }", "0")]
    public void RuntimeErrorWithInputThrowsVirtualMachineException(string source, string input)
    {
        Assert.Throws<VirtualMachineException>(() => Run(source, input));
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