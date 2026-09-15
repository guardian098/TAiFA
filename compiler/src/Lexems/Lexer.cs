using System.Globalization;
using System.IO;
using System.Text;

namespace Lexems;

public sealed class Lexer
{
    private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
    {
        { "bool", TokenType.BoolKeyword },
        { "int", TokenType.IntKeyword },
        { "string", TokenType.StringKeyword },
        { "void", TokenType.VoidKeyword },
        { "null", TokenType.NullKeyword },
        { "struct", TokenType.StructKeyword },
        { "true", TokenType.TrueKeyword },
        { "false", TokenType.FalseKeyword },
        { "if", TokenType.IfKeyword },
        { "else", TokenType.ElseKeyword },
        { "elif", TokenType.ElifKeyword },
        { "while", TokenType.WhileKeyword },
        { "exception", TokenType.ExceptionKeyword }
    };

    private readonly TextScanner _scanner;

    public Lexer(string source)
    {
        _scanner = new TextScanner(source);
    }

    public static IReadOnlyList<Token> Tokenize(string source)
    {
        Lexer lexer = new Lexer(source);
        return lexer.ParseTokens();
    }

    public static IReadOnlyList<Token> TokenizeFile(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        string source = File.ReadAllText(path, Encoding.UTF8);
        return Tokenize(source);
    }

    // При первой ошибке разбор останавливается.
    private IReadOnlyList<Token> ParseTokens()
    {
        List<Token> tokens = new List<Token>();

        while (true)
        {
            Token? error = SkipWhitespaceAndComments();
            if (error is not null)
            {
                tokens.Add(error);
                return tokens;
            }

            if (_scanner.IsEnd())
            {
                tokens.Add(new Token(TokenType.EndOfFile));
                return tokens;
            }

            Token token = ParseToken();
            tokens.Add(token);

            if (token.Type == TokenType.Error)
            {
                return tokens;
            }
        }
    }

    private Token? SkipWhitespaceAndComments()
    {
        while (true)
        {
            SkipWhitespace();

            if (_scanner.Peek() == '/' && _scanner.Peek(1) == '*')
            {
                Token? error = SkipComment();
                if (error is not null)
                {
                    return error;
                }

                continue;
            }

            return null;
        }
    }

    private void SkipWhitespace()
    {
        while (IsWhitespace(_scanner.Peek()))
        {
            _scanner.Advance();
        }
    }

    // Комментарий завершается на первом */, вложенные комментарии не поддерживаются.
    private Token? SkipComment()
    {
        int start = _scanner.Position;

        _scanner.Advance();
        _scanner.Advance();

        while (!_scanner.IsEnd())
        {
            if (_scanner.Peek() == '*' && _scanner.Peek(1) == '/')
            {
                _scanner.Advance();
                _scanner.Advance();
                return null;
            }

            _scanner.Advance();
        }

        return CreateError(start);
    }

    private Token ParseToken()
    {
        char c = _scanner.Peek();

        if (IsIdentifierStart(c))
        {
            return ParseIdentifierOrKeyword();
        }

        if (char.IsAsciiDigit(c))
        {
            return ParseIntLiteral();
        }

        if (c == '"')
        {
            return ParseStringLiteral();
        }

        int start = _scanner.Position;

        switch (c)
        {
            case '+':
                _scanner.Advance();
                if (_scanner.Peek() == '+')
                {
                    _scanner.Advance();
                    return new Token(TokenType.Increment);
                }

                return new Token(TokenType.Plus);

            case '-':
                _scanner.Advance();
                if (_scanner.Peek() == '-')
                {
                    _scanner.Advance();
                    return new Token(TokenType.Decrement);
                }

                return new Token(TokenType.Minus);

            case '*':
                _scanner.Advance();
                return new Token(TokenType.Star);

            case '/':
                _scanner.Advance();
                return new Token(TokenType.Slash);

            case '=':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.Equal);
                }

                return new Token(TokenType.Assign);

            case '!':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.NotEqual);
                }

                return new Token(TokenType.Not);

            case '<':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.LessOrEqual);
                }

                return new Token(TokenType.Less);

            case '>':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.GreaterOrEqual);
                }

                return new Token(TokenType.Greater);

            case '&':
                _scanner.Advance();
                if (_scanner.Peek() == '&')
                {
                    _scanner.Advance();
                    return new Token(TokenType.And);
                }

                return CreateError(start);

            case '|':
                _scanner.Advance();
                if (_scanner.Peek() == '|')
                {
                    _scanner.Advance();
                    return new Token(TokenType.Or);
                }

                return CreateError(start);

            case '.':
                _scanner.Advance();
                return new Token(TokenType.Dot);

            case ',':
                _scanner.Advance();
                return new Token(TokenType.Comma);

            case ';':
                _scanner.Advance();
                return new Token(TokenType.Semicolon);

            case '(':
                _scanner.Advance();
                return new Token(TokenType.LeftParenthesis);

            case ')':
                _scanner.Advance();
                return new Token(TokenType.RightParenthesis);

            case '[':
                _scanner.Advance();
                return new Token(TokenType.LeftBracket);

            case ']':
                _scanner.Advance();
                return new Token(TokenType.RightBracket);

            case '{':
                _scanner.Advance();
                return new Token(TokenType.LeftBrace);

            case '}':
                _scanner.Advance();
                return new Token(TokenType.RightBrace);
        }

        _scanner.Advance();
        return CreateError(start);
    }

    private Token ParseIdentifierOrKeyword()
    {
        StringBuilder text = new StringBuilder();

        while (IsIdentifierPart(_scanner.Peek()))
        {
            text.Append(_scanner.Peek());
            _scanner.Advance();
        }

        string value = text.ToString();

        if (Keywords.TryGetValue(value, out TokenType keywordType))
        {
            return new Token(keywordType);
        }

        return new Token(TokenType.Identifier, value);
    }

    // Если сразу после цифр идёт идентификатор, лексема считается ошибочной целиком.
    private Token ParseIntLiteral()
    {
        int start = _scanner.Position;
        StringBuilder digits = new StringBuilder();

        while (char.IsAsciiDigit(_scanner.Peek()))
        {
            digits.Append(_scanner.Peek());
            _scanner.Advance();
        }

        if (IsIdentifierStart(_scanner.Peek()))
        {
            while (IsIdentifierPart(_scanner.Peek()))
            {
                _scanner.Advance();
            }

            return CreateError(start);
        }

        string text = digits.ToString();

        if (text.Length > 1 && text[0] == '0')
        {
            return CreateError(start);
        }

        if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
        {
            return CreateError(start);
        }

        return new Token(TokenType.IntLiteral, value);
    }

    private Token ParseStringLiteral()
    {
        int start = _scanner.Position;

        _scanner.Advance();

        StringBuilder value = new StringBuilder();

        while (true)
        {
            if (_scanner.IsEnd())
            {
                return CreateError(start);
            }

            char c = _scanner.Peek();

            if (c == '"')
            {
                _scanner.Advance();
                return new Token(TokenType.StringLiteral, value.ToString());
            }

            if (c is '\n' or '\r')
            {
                return CreateError(start);
            }

            if (c == '\\')
            {
                if (!TryAppendEscape(value))
                {
                    return CreateError(start);
                }
            }
            else
            {
                value.Append(c);
                _scanner.Advance();
            }
        }
    }

    private bool TryAppendEscape(StringBuilder value)
    {
        _scanner.Advance();

        if (_scanner.IsEnd())
        {
            return false;
        }

        char escape = _scanner.Peek();

        switch (escape)
        {
            case '"':
                value.Append('"');
                _scanner.Advance();
                return true;

            case '\\':
                value.Append('\\');
                _scanner.Advance();
                return true;

            case 'n':
                value.Append('\n');
                _scanner.Advance();
                return true;

            case 'r':
                value.Append('\r');
                _scanner.Advance();
                return true;

            case 't':
                value.Append('\t');
                _scanner.Advance();
                return true;

            case 'f':
                value.Append('\f');
                _scanner.Advance();
                return true;

            default:
                _scanner.Advance();
                return false;
        }
    }

    private Token CreateError(int start)
    {
        string text = _scanner.Source[start.._scanner.Position];
        return new Token(TokenType.Error, text);
    }

    private static bool IsIdentifierStart(char c)
    {
        return char.IsAsciiLetter(c) || c == '_';
    }

    private static bool IsIdentifierPart(char c)
    {
        return IsIdentifierStart(c) || char.IsAsciiDigit(c);
    }

    private static bool IsWhitespace(char c)
    {
        return c is ' ' or '\t' or '\n' or '\r' or '\f';
    }
}