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
    [InlineData("")]
    [InlineData("void foo() { }")]
    [InlineData("void main(1) { }")]
    [InlineData("void main() {")]
    [InlineData("void main() { } 1")]
    [InlineData("void main() { foo(1); }")]
    [InlineData("void main() { print; }")]
    [InlineData("void main() { 1 + ; }")]
    [InlineData("void main() { 1 2; }")]
    [InlineData("void main() { (1; }")]
    [InlineData("void main() { print(1 2); }")]
    [InlineData("void main() { print(sqrt()); }")]
    [InlineData("void main() { min(1, 2, 3); }")]
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
    [InlineData("void main() { print(2147483648); }")]
    [InlineData("void main() { print(123abc); }")]
    [InlineData("void main() { print(1 @ 2); }")]
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

    private static List<Instruction> Parse(string code)
    {
        List<Token> tokens = new List<Token>(Lexer.Tokenize(code));
        Parser parser = new Parser(tokens);
        return parser.ParseProgram();
    }
}