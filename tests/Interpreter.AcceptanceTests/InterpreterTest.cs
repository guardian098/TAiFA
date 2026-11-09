using Lexems;
using Parser;
using VirtualMachine;

namespace Interpreter.AcceptanceTests;

public class InterpreterTest
{
    [Theory]
    [InlineData("void main() { print(7 / 2); print(7 % 2); print(abs(-5)); print(sqrt(16)); print(min(3, 5)); print(max(3, 5)); }",  "3\n1\n5\n4\n3\n5\n")]
    public void ExecuteProgrammFromDocumentation(string source, string expected)
    {
        Assert.Equal(Run(source), expected);
    }

    [Theory]
    [InlineData("void main() { print(min(sqrt(16), abs(-5))); }", "4\n")]
    [InlineData("void main() { print(-(-5)); }", "5\n")]
    [InlineData("void main() { print(-(5 + 3)); }", "-8\n")]
    [InlineData("void main() { 2 + 3; print(6); }", "6\n")]
    [InlineData("void main() { 7 + 3; }", "")]
    [InlineData("void main() { print(1 + 2 * 3 - 4 / 2); }", "5\n")]
    public void ExecuteProgramm(string source, string expected)
    {
        Assert.Equal(Run(source), expected);
    }

    [Theory]
    [InlineData("void main() { print(1; }")]
    [InlineData("void main() { print(); }")]
    [InlineData("void main() { min(3 5); }")]
    [InlineData("void main() print(1);")]
    [InlineData("void main() { } void main() { }")]
    public void InvalidSyntaxThrowsParserException(string source)
    {
        Assert.Throws<ParserException>(() => Run(source));
    }

    [Theory]
    [InlineData("void main() { print(10 / 0); }")]
    [InlineData("void main() { print(10 % 0); }")]
    [InlineData("void main() { print(sqrt(-1)); }")]
    [InlineData("void main() { print(2147483648); }")]
    [InlineData("void main() { print(-2147483648); }")]

    public void RuntimeErrorThrowsVirtualMachineException(string source)
    {
        Assert.Throws<VirtualMachineException>(() => Run(source));
    }

    public static string Run(string source)
    {
        IReadOnlyList<Token> tokens = Lexer.Tokenize(source);
        List<Instruction> instructions = new Parser.Parser(tokens).ParseProgram();

        using StringWriter output = new StringWriter();
        new VirtualMachine.VirtualMachine(instructions, output).Run();

        return output.ToString().Replace("\r\n", "\n");
    }
}