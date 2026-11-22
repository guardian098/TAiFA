using System.Collections.Generic;

using Lexems;
using Parser;
using VirtualMachine;

using Xunit;

namespace Parser.UnitTests;

public class ParserInputTest
{
    [Theory]
    [MemberData(nameof(GetReadIntData))]
    public void CanParseReadInt(string code, List<Instruction> expected)
    {
        List<Instruction> actual = Parse(code);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("void main() { readInt; }")]
    [InlineData("void main() { readInt(; }")]
    [InlineData("void main() { readInt(); ")]
    [InlineData("void main() { readInt(1); }")]
    [InlineData("void main() { readInt() }")]
    [InlineData("void main() { print(readInt(); }")]
    [InlineData("void main() { readint(); }")]
    public void CanReportReadIntErrors(string code)
    {
        Assert.Throws<ParserException>(() => Parse(code));
    }

    public static TheoryData<string, List<Instruction>> GetReadIntData()
    {
        return new TheoryData<string, List<Instruction>>
        {
            {
                "void main() { print(readInt()); }",
                [
                    new Instruction(OpCode.Read),
                    new Instruction(OpCode.Print)
                ]
            },
            {
                "void main() { readInt(); }",
                [
                    new Instruction(OpCode.Read),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { print(readInt() + readInt() * 2); }",
                [
                    new Instruction(OpCode.Read),
                    new Instruction(OpCode.Read),
                    new Instruction(OpCode.Push, 2),
                    new Instruction(OpCode.Mul),
                    new Instruction(OpCode.Add),
                    new Instruction(OpCode.Print)
                ]
            },
            {
                "void main() { -readInt(); }",
                [
                    new Instruction(OpCode.Read),
                    new Instruction(OpCode.Neg),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { min(readInt(), 5); }",
                [
                    new Instruction(OpCode.Read),
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Min),
                    new Instruction(OpCode.Pop)
                ]
            },
        };
    }

    private static List<Instruction> Parse(string code)
    {
        List<Token> tokens = new List<Token>(Lexer.Tokenize(code));
        Parser parser = new Parser(tokens);
        return parser.ParseProgram();
    }
}