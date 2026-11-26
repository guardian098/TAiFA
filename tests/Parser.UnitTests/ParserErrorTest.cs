using System;
using System.Collections.Generic;

using Lexems;
using Parser;
using VirtualMachine;

using Xunit;

namespace Parser.UnitTests;

public class ParserErrorTest
{
    [Theory]
    [MemberData(nameof(GetSyntaxErrorsData))]
    public void CanReportSyntaxErrors(string code)
    {
        Assert.Throws<ParserException>(() => Parse(code));
    }

    [Fact]
    public void ReportsUnknownFunctionName()
    {
        ParserException exception = Assert.Throws<ParserException>(() => Parse("void main() { foo(1); }"));
        Assert.Contains("foo", exception.Message);
    }

    [Theory]
    [MemberData(nameof(GetLexicalErrorsData))]
    public void CanReportLexicalErrors(string code)
    {
        LexerException exception = Assert.Throws<LexerException>(() => Parse(code));
        Assert.Contains("Invalid lexeme", exception.Message);
        Assert.DoesNotContain("Lexical error", exception.Message);
    }

    [Fact]
    public void RejectsEmptyTokenList()
    {
        Assert.Throws<ParserException>(() => new Parser(new List<Token>()));
    }

    [Fact]
    public void RejectsNullTokenList()
    {
        Assert.Throws<ArgumentNullException>(() => new Parser(null!));
    }

    public static TheoryData<string> GetSyntaxErrorsData()
    {
        return new TheoryData<string>
        {
            { "" },
            { "void foo() { }" },
            { "void main(1) { }" },
            { "void main() {" },
            { "void main() { } 1" },
            { "void main() { foo(1); }" },
            { "void main() { print; }" },
            { "void main() { 1 + ; }" },
            { "void main() { 1 2; }" },
            { "void main() { (1; }" },
            { "void main() { print(1 2); }" },
            { "void main() { print(sqrt()); }" },
            { "void main() { min(1, 2, 3); }" },
        };
    }

    public static TheoryData<string> GetLexicalErrorsData()
    {
        return new TheoryData<string>
        {
            { "void main() { print(2147483648); }" },
            { "void main() { print(123abc); }" },
            { "void main() { print(1 @ 2); }" },
        };
    }

    private static List<Instruction> Parse(string code)
    {
        List<Token> tokens = new List<Token>(Lexer.Tokenize(code));
        Parser parser = new Parser(tokens);
        return parser.ParseProgram();
    }
}