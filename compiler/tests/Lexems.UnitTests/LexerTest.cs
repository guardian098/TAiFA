using Lexems;

namespace Lexems.UnitTests;

public class LexerTest
{
    [Theory]
    [MemberData(nameof(GetIdentifiersAndKeywordsData))]
    public void CanTokenizeIdentifiersAndKeywords(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetLiteralsData))]
    public void CanTokenizeLiterals(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetOperatorsAndPunctuationData))]
    public void CanTokenizeOperatorsAndPunctuation(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetWhitespacesAndCommentsData))]
    public void CanSkipWhitespacesAndComments(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetLexicalErrorsData))]
    public void CanReportLexicalErrors(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);

        Assert.Equal(expected, actual);
    }

    public static TheoryData<string, List<Token>> GetIdentifiersAndKeywordsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "bool int string void",
                [
                    new Token(TokenType.BoolKeyword),
                    new Token(TokenType.IntKeyword),
                    new Token(TokenType.StringKeyword),
                    new Token(TokenType.VoidKeyword),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                "intx integer ifelse",
                [
                    new Token(TokenType.Identifier, "intx"),
                    new Token(TokenType.Identifier, "integer"),
                    new Token(TokenType.Identifier, "ifelse"),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                "Int IF While TRUE Null",
                [
                    new Token(TokenType.Identifier, "Int"),
                    new Token(TokenType.Identifier, "IF"),
                    new Token(TokenType.Identifier, "While"),
                    new Token(TokenType.Identifier, "TRUE"),
                    new Token(TokenType.Identifier, "Null"),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                "x _var count123 camelCase snake_case",
                [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Identifier, "_var"),
                    new Token(TokenType.Identifier, "count123"),
                    new Token(TokenType.Identifier, "camelCase"),
                    new Token(TokenType.Identifier, "snake_case"),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                "int x = 10; bool flag = true",
                [
                    new Token(TokenType.IntKeyword),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.IntLiteral, 10),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.BoolKeyword),
                    new Token(TokenType.Identifier, "flag"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.TrueKeyword),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                "while counter != 3 {counter++}",
                [
                    new Token(TokenType.WhileKeyword),
                    new Token(TokenType.Identifier, "counter"),
                    new Token(TokenType.NotEqual),
                    new Token(TokenType.IntLiteral, 3),
                    new Token(TokenType.LeftBrace),
                    new Token(TokenType.Identifier, "counter"),
                    new Token(TokenType.Increment),
                    new Token(TokenType.RightBrace),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                "struct Point {int x; string s;}",
                [
                    new Token(TokenType.StructKeyword),
                    new Token(TokenType.Identifier, "Point"),
                    new Token(TokenType.LeftBrace),
                    new Token(TokenType.IntKeyword),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.StringKeyword),
                    new Token(TokenType.Identifier, "s"),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.RightBrace),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                "if n == 0 {n = 1} elif n == 1 {} else {n = 2}",
                [
                    new Token(TokenType.IfKeyword),
                    new Token(TokenType.Identifier, "n"),
                    new Token(TokenType.Equal),
                    new Token(TokenType.IntLiteral, 0),
                    new Token(TokenType.LeftBrace),
                    new Token(TokenType.Identifier, "n"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.IntLiteral, 1),
                    new Token(TokenType.RightBrace),
                    new Token(TokenType.ElifKeyword),
                    new Token(TokenType.Identifier, "n"),
                    new Token(TokenType.Equal),
                    new Token(TokenType.IntLiteral, 1),
                    new Token(TokenType.LeftBrace),
                    new Token(TokenType.RightBrace),
                    new Token(TokenType.ElseKeyword),
                    new Token(TokenType.LeftBrace),
                    new Token(TokenType.Identifier, "n"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.IntLiteral, 2),
                    new Token(TokenType.RightBrace),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                "void foo(){}",
                [
                    new Token(TokenType.VoidKeyword),
                    new Token(TokenType.Identifier, "foo"),
                    new Token(TokenType.LeftParenthesis),
                    new Token(TokenType.RightParenthesis),
                    new Token(TokenType.LeftBrace),
                    new Token(TokenType.RightBrace),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                "if a=null{}",
                [
                    new Token(TokenType.IfKeyword),
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.NullKeyword),
                    new Token(TokenType.LeftBrace),
                    new Token(TokenType.RightBrace),
                    new Token(TokenType.EndOfFile)
                ]
            }
        };
    }

    public static TheoryData<string, List<Token>> GetLiteralsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "0 1234 56780 12034 1010 1200",
                [
                    new Token(TokenType.IntLiteral, 0),
                    new Token(TokenType.IntLiteral, 1234),
                    new Token(TokenType.IntLiteral, 56780),
                    new Token(TokenType.IntLiteral, 12034),
                    new Token(TokenType.IntLiteral, 1010),
                    new Token(TokenType.IntLiteral, 1200),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                "2147483647",
                [
                    new Token(TokenType.IntLiteral, 2147483647),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                """
                "" "0" "Hello, world!"
                """,
                [
                    new Token(TokenType.StringLiteral, ""),
                    new Token(TokenType.StringLiteral, "0"),
                    new Token(TokenType.StringLiteral, "Hello, world!"),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                // Исходный текст содержит escape-последовательности как пары символов.
                """
                "\"\\\n\r\t\f" "Hello\tWorld\n"
                """,
                [
                    new Token(TokenType.StringLiteral, "\"\\\n\r\t\f"),
                    new Token(TokenType.StringLiteral, "Hello\tWorld\n"),
                    new Token(TokenType.EndOfFile)
                ]
            }
        };
    }

    public static TheoryData<string, List<Token>> GetOperatorsAndPunctuationData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "x + y / (10 - z * 2)",
                [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Plus),
                    new Token(TokenType.Identifier, "y"),
                    new Token(TokenType.Slash),
                    new Token(TokenType.LeftParenthesis),
                    new Token(TokenType.IntLiteral, 10),
                    new Token(TokenType.Minus),
                    new Token(TokenType.Identifier, "z"),
                    new Token(TokenType.Star),
                    new Token(TokenType.IntLiteral, 2),
                    new Token(TokenType.RightParenthesis),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                "a == b != c || !d",
                [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Equal),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.NotEqual),
                    new Token(TokenType.Identifier, "c"),
                    new Token(TokenType.Or),
                    new Token(TokenType.Not),
                    new Token(TokenType.Identifier, "d"),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                "a <= b && b >= c && a < c > d",
                [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.LessOrEqual),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.And),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.GreaterOrEqual),
                    new Token(TokenType.Identifier, "c"),
                    new Token(TokenType.And),
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Less),
                    new Token(TokenType.Identifier, "c"),
                    new Token(TokenType.Greater),
                    new Token(TokenType.Identifier, "d"),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                "speed.x = v[0]",
                [
                    new Token(TokenType.Identifier, "speed"),
                    new Token(TokenType.Dot),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.Identifier, "v"),
                    new Token(TokenType.LeftBracket),
                    new Token(TokenType.IntLiteral, 0),
                    new Token(TokenType.RightBracket),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                "i++; j--",
                [
                    new Token(TokenType.Identifier, "i"),
                    new Token(TokenType.Increment),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.Identifier, "j"),
                    new Token(TokenType.Decrement),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                "a===b",
                [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Equal),
                    new Token(TokenType.Assign),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                "a+++b",
                [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Increment),
                    new Token(TokenType.Plus),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                "foo(x, y); {1, 2}",
                [
                    new Token(TokenType.Identifier, "foo"),
                    new Token(TokenType.LeftParenthesis),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Comma),
                    new Token(TokenType.Identifier, "y"),
                    new Token(TokenType.RightParenthesis),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.LeftBrace),
                    new Token(TokenType.IntLiteral, 1),
                    new Token(TokenType.Comma),
                    new Token(TokenType.IntLiteral, 2),
                    new Token(TokenType.RightBrace),
                    new Token(TokenType.EndOfFile)
                ]
            }
        };
    }

    public static TheoryData<string, List<Token>> GetWhitespacesAndCommentsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "x \t\r\n\fy",
                [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Identifier, "y"),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                "/* ... */ a / /* should be */ b * c /* ignored */",
                [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Slash),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.Star),
                    new Token(TokenType.Identifier, "c"),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                "int/*коммент*/x=5",
                [
                    new Token(TokenType.IntKeyword),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.IntLiteral, 5),
                    new Token(TokenType.EndOfFile)
                ]
            },
            {
                // Вложенные комментарии не допускаются: комментарий закрывается на первом */.
                "/* a /* b */ c */",
                [
                    new Token(TokenType.Identifier, "c"),
                    new Token(TokenType.Star),
                    new Token(TokenType.Slash),
                    new Token(TokenType.EndOfFile)
                ]
            }
        };
    }

    public static TheoryData<string, List<Token>> GetLexicalErrorsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "000233",
                [
                    new Token(TokenType.Error, "000233")
                ]
            },
            {
                "01",
                [
                    new Token(TokenType.Error, "01")
                ]
            },
            {
                "00",
                [
                    new Token(TokenType.Error, "00")
                ]
            },
            {
                "2147483648",
                [
                    new Token(TokenType.Error, "2147483648")
                ]
            },
            {
                "123abc",
                [
                    new Token(TokenType.Error, "123abc")
                ]
            },
            {
                "1_var",
                [
                    new Token(TokenType.Error, "1_var")
                ]
            },
            {
                "\"\\q\"",
                [
                    new Token(TokenType.Error, "\"\\q")
                ]
            },
            {
                "\"открытая строка без конца",
                [
                    new Token(TokenType.Error, "\"открытая строка без конца")
                ]
            },
            {
                "a & b",
                [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Error, "&")
                ]
            },
            {
                "a | b",
                [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Error, "|")
                ]
            },
            {
                "/* abc",
                [
                    new Token(TokenType.Error, "/* abc")
                ]
            }
        };
    }

    private static List<Token> Tokenize(string code)
    {
        return new List<Token>(Lexer.Tokenize(code));
    }
}