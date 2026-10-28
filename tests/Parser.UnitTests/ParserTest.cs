using System.Collections.Generic;

using Lexems;
using Parser;
using VirtualMachine;

namespace Parser.UnitTests;

public class ParserTest
{

    [Theory]
    [MemberData(nameof(GetValidProgramStructureData))]
    public void CanParseValidProgramStructure(string code, List<Instruction> expected)
    {
        List<Instruction> actual = Parse(code);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetInvalidProgramStructureData))]
    public void CanReportProgramStructureErrors(string code)
    {
        Assert.Throws<ParserException>(() => Parse(code));
    }

    [Theory]
    [MemberData(nameof(GetValidStatementsData))]
    public void CanParseValidStatements(string code, List<Instruction> expected)
    {
        List<Instruction> actual = Parse(code);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetInvalidStatementsData))]
    public void CanReportStatementErrors(string code)
    {
        Assert.Throws<ParserException>(() => Parse(code));
    }

    [Theory]
    [MemberData(nameof(GetArithmeticExpressionsData))]
    public void CanParseArithmeticExpressions(string code, List<Instruction> expected)
    {
        List<Instruction> actual = Parse(code);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetUnaryOperatorsData))]
    public void CanParseUnaryOperators(string code, List<Instruction> expected)
    {
        List<Instruction> actual = Parse(code);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetBuiltinFunctionsData))]
    public void CanParseBuiltinFunctions(string code, List<Instruction> expected)
    {
        List<Instruction> actual = Parse(code);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetInvalidBuiltinFunctionsData))]
    public void CanReportBuiltinFunctionErrors(string code)
    {
        Assert.Throws<ParserException>(() => Parse(code));
    }

    public static TheoryData<string, List<Instruction>> GetValidProgramStructureData()
    {
        return new TheoryData<string, List<Instruction>>
        {

            {
                "void main() { }",
                [

                ]
            },
        };
    }

    public static TheoryData<string> GetInvalidProgramStructureData()
    {
        return new TheoryData<string>
        {
            { "main() {}" },
            { "void main() { print(1);" },
            { "void main() print(1);" },
            { "int main() { }" },
            { "void main() { } void main() { }" },
        };
    }

    public static TheoryData<string, List<Instruction>> GetValidStatementsData()
    {
        return new TheoryData<string, List<Instruction>>
        {
            {
                "void main() { print(42); }",
                [
                    new Instruction(OpCode.Push, 42),
                    new Instruction(OpCode.Print)
                ]
            },
            {
                "void main() { print(2 + 3); }",
                [
                    new Instruction(OpCode.Push, 2),
                    new Instruction(OpCode.Push, 3),
                    new Instruction(OpCode.Add),
                    new Instruction(OpCode.Print)
                ]
            },
            {
                "void main() { 7 + 3; }",
                [
                    new Instruction(OpCode.Push, 7),
                    new Instruction(OpCode.Push, 3),
                    new Instruction(OpCode.Add),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { print(1); print(2); }",
                [
                    new Instruction(OpCode.Push, 1),
                    new Instruction(OpCode.Print),
                    new Instruction(OpCode.Push, 2),
                    new Instruction(OpCode.Print)
                ]
            },
        };
    }

    public static TheoryData<string> GetInvalidStatementsData()
    {
        return new TheoryData<string>
        {
            { "void main() { print(1) }" },
            { "void main() { print 1); }" },
            { "void main() { print(1; }" },
            { "void main() { print(); }" },
        };
    }

    public static TheoryData<string, List<Instruction>> GetArithmeticExpressionsData()
    {
        return new TheoryData<string, List<Instruction>>
        {
            {
                "void main() { 1 + 2; }",
                [
                    new Instruction(OpCode.Push, 1),
                    new Instruction(OpCode.Push, 2),
                    new Instruction(OpCode.Add),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { 5 - 3; }",
                [
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Push, 3),
                    new Instruction(OpCode.Sub),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { 3 * 4; }",
                [
                    new Instruction(OpCode.Push, 3),
                    new Instruction(OpCode.Push, 4),
                    new Instruction(OpCode.Mul),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { 20 / 5; }",
                [
                    new Instruction(OpCode.Push, 20),
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Div),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { 17 % 5; }",
                [
                    new Instruction(OpCode.Push, 17),
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Mod),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { 10 + 2 * 5; }",
                [
                    new Instruction(OpCode.Push, 10),
                    new Instruction(OpCode.Push, 2),
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Mul),
                    new Instruction(OpCode.Add),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { (10 + 2) * 5; }",
                [
                    new Instruction(OpCode.Push, 10),
                    new Instruction(OpCode.Push, 2),
                    new Instruction(OpCode.Add),
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Mul),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { 10 - 3 - 2; }",
                [
                    new Instruction(OpCode.Push, 10),
                    new Instruction(OpCode.Push, 3),
                    new Instruction(OpCode.Sub),
                    new Instruction(OpCode.Push, 2),
                    new Instruction(OpCode.Sub),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { 100 / 10 / 2; }",
                [
                    new Instruction(OpCode.Push, 100),
                    new Instruction(OpCode.Push, 10),
                    new Instruction(OpCode.Div),
                    new Instruction(OpCode.Push, 2),
                    new Instruction(OpCode.Div),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { 1 + 2 * 3 - 4 / 2; }",
                [
                    new Instruction(OpCode.Push, 1),
                    new Instruction(OpCode.Push, 2),
                    new Instruction(OpCode.Push, 3),
                    new Instruction(OpCode.Mul),
                    new Instruction(OpCode.Add),
                    new Instruction(OpCode.Push, 4),
                    new Instruction(OpCode.Push, 2),
                    new Instruction(OpCode.Div),
                    new Instruction(OpCode.Sub),
                    new Instruction(OpCode.Pop)
                ]
            },
        };
    }

    public static TheoryData<string, List<Instruction>> GetUnaryOperatorsData()
    {
        return new TheoryData<string, List<Instruction>>
        {
            {
                "void main() { -5; }",
                [
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Neg),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { +5; }",
                [
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { - -5; }",
                [
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Neg),
                    new Instruction(OpCode.Neg),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { -(-5); }",
                [
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Neg),
                    new Instruction(OpCode.Neg),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { -(5 + 3); }",
                [
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Push, 3),
                    new Instruction(OpCode.Add),
                    new Instruction(OpCode.Neg),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { -sqrt(16); }",
                [
                    new Instruction(OpCode.Push, 16),
                    new Instruction(OpCode.Sqrt),
                    new Instruction(OpCode.Neg),
                    new Instruction(OpCode.Pop)
                ]
            },
        };
    }

    public static TheoryData<string, List<Instruction>> GetBuiltinFunctionsData()
    {
        return new TheoryData<string, List<Instruction>>
        {
            {
                "void main() { sqrt(16); }",
                [
                    new Instruction(OpCode.Push, 16),
                    new Instruction(OpCode.Sqrt),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { abs(-5); }",
                [
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Neg),
                    new Instruction(OpCode.Abs),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { min(3, 5); }",
                [
                    new Instruction(OpCode.Push, 3),
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Min),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { max(3, 5); }",
                [
                    new Instruction(OpCode.Push, 3),
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Max),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { min(sqrt(16), abs(-5)); }",
                [
                    new Instruction(OpCode.Push, 16),
                    new Instruction(OpCode.Sqrt),
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Neg),
                    new Instruction(OpCode.Abs),
                    new Instruction(OpCode.Min),
                    new Instruction(OpCode.Pop)
                ]
            },
            {
                "void main() { sqrt(16) + 1; }",
                [
                    new Instruction(OpCode.Push, 16),
                    new Instruction(OpCode.Sqrt),
                    new Instruction(OpCode.Push, 1),
                    new Instruction(OpCode.Add),
                    new Instruction(OpCode.Pop)
                ]
            },
        };
    }

    public static TheoryData<string> GetInvalidBuiltinFunctionsData()
    {
        return new TheoryData<string>
        {
            { "void main() { sqrt 16); }" },
            { "void main() { min(3 5); }" },
        };
    }

    private static List<Instruction> Parse(string code)
    {
        List<Token> tokens = new List<Token>(Lexer.Tokenize(code));
        Parser parser = new Parser(tokens);
        return parser.ParseProgram();
    }
}