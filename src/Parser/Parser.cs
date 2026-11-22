using System.Globalization;

using Lexems;
using VirtualMachine;

namespace Parser;

public sealed class Parser
{
    private readonly IReadOnlyList<Token> _tokens;
    private readonly List<Instruction> _instructions;
    private int _index;

    private Token _current;

    public Parser(IReadOnlyList<Token> tokens)
    {
        ArgumentNullException.ThrowIfNull(tokens);

        if (tokens.Count == 0)
        {
            throw new ParserException("Token list cannot be empty.");
        }

        foreach (Token token in tokens)
        {
            if (token.Type == TokenType.Error)
            {
                throw new LexerException($"Invalid lexeme '{token.Value}'.");
            }
        }

        _tokens = tokens;
        _instructions = new List<Instruction>();
        _index = 0;
        _current = _tokens[_index];
    }

    /// <summary>
    /// Разбирает программу.
    /// Правило:
    ///     program = main_function ;
    /// </summary>
    public List<Instruction> ParseProgram()
    {
        ParseMainFunction();
        Match(TokenType.EndOfFile);

        return _instructions;
    }

    /// <summary>
    /// Разбирает главную функцию.
    /// Правило:
    ///     main_function = "void", "main", "(", ")", block ;
    /// </summary>
    private void ParseMainFunction()
    {
        Match(TokenType.VoidKeyword);

        Token mainToken = Match(TokenType.Identifier);
        if (mainToken.Value?.ToString() != "main")
        {
            throw new ParserException($"Unexpected identifier '{mainToken.Value}' where expected 'main'");
        }

        Match(TokenType.LeftParenthesis);
        Match(TokenType.RightParenthesis);
        ParseBlock();
    }

    /// <summary>
    /// Разбирает блок инструкций.
    /// Правило:
    ///     block = "{", { statement }, "}" ;
    /// </summary>
    private void ParseBlock()
    {
        Match(TokenType.LeftBrace);

        while (_current.Type != TokenType.RightBrace && _current.Type != TokenType.EndOfFile)
        {
            ParseStatement();
        }

        Match(TokenType.RightBrace);
    }

    /// <summary>
    /// Разбирает инструкцию.
    /// Правило:
    ///     statement = print_statement | expression_statement ;
    /// </summary>
    private void ParseStatement()
    {
        if (IsIdentifier("print"))
        {
            ParsePrintStatement();
        }
        else
        {
            ParseExpressionStatement();
        }
    }

    /// <summary>
    /// Разбирает инструкцию вывода.
    /// Правило:
    ///     print_statement = "print", "(", expression, ")", ";" ;
    /// </summary>
    private void ParsePrintStatement()
    {
        Advance();
        Match(TokenType.LeftParenthesis);
        ParseExpression();
        Match(TokenType.RightParenthesis);
        Match(TokenType.Semicolon);

        _instructions.Add(new Instruction(OpCode.Print));
    }

    /// <summary>
    /// Разбирает инструкцию-выражение.
    /// Правило:
    ///     expression_statement = expression, ";" ;
    /// </summary>
    private void ParseExpressionStatement()
    {
        ParseExpression();
        Match(TokenType.Semicolon);

        _instructions.Add(new Instruction(OpCode.Pop));
    }

    /// <summary>
    /// Разбирает выражение.
    /// Правило:
    ///     expression = additive_expression ;
    /// </summary>
    private void ParseExpression()
    {
        ParseAdditiveExpression();
    }

    /// <summary>
    /// Разбирает операторы сложения и вычитания.
    /// Правило:
    ///     additive_expression = multiplicative_expression,
    ///         { ( "+" | "-" ), multiplicative_expression } ;
    /// </summary>
    private void ParseAdditiveExpression()
    {
        ParseMultiplicativeExpression();
        while (true)
        {
            switch (_current.Type)
            {
                case TokenType.Plus:
                    Advance();
                    ParseMultiplicativeExpression();
                    _instructions.Add(new Instruction(OpCode.Add));
                    break;

                case TokenType.Minus:
                    Advance();
                    ParseMultiplicativeExpression();
                    _instructions.Add(new Instruction(OpCode.Sub));
                    break;

                default:
                    return;
            }
        }
    }

    /// <summary>
    /// Разбирает операторы умножения, деления и взятия остатка.
    /// Правило:
    ///     multiplicative_expression = unary_expression,
    ///         { ( "*" | "/" | "%" ), unary_expression } ;
    /// </summary>
    private void ParseMultiplicativeExpression()
    {
        ParseUnaryExpression();
        while (true)
        {
            switch (_current.Type)
            {
                case TokenType.Star:
                    Advance();
                    ParseUnaryExpression();
                    _instructions.Add(new Instruction(OpCode.Mul));
                    break;

                case TokenType.Slash:
                    Advance();
                    ParseUnaryExpression();
                    _instructions.Add(new Instruction(OpCode.Div));
                    break;

                case TokenType.Percent:
                    Advance();
                    ParseUnaryExpression();
                    _instructions.Add(new Instruction(OpCode.Mod));
                    break;

                default:
                    return;
            }
        }
    }

    /// <summary>
    /// Разбирает унарный плюс и минус.
    /// Правило:
    ///     unary_expression = ( "+" | "-" ), unary_expression
    ///         | primary_expression ;
    /// </summary>
    private void ParseUnaryExpression()
    {
        switch (_current.Type)
        {
            case TokenType.Plus:
                Advance();
                ParseUnaryExpression();
                break;

            case TokenType.Minus:
                Advance();
                ParseUnaryExpression();
                _instructions.Add(new Instruction(OpCode.Neg));
                break;

            default:
                ParsePrimaryExpression();
                break;
        }
    }

    /// <summary>
    /// Разбирает элементарные выражения.
    /// Правило:
    ///     primary_expression = integer_literal
    ///         | "(", expression, ")"
    ///         | sqrt_call
    ///         | abs_call
    ///         | min_call
    ///         | max_call
    ///         | read_int_call ;
    /// </summary>
    private void ParsePrimaryExpression()
    {
        switch (_current.Type)
        {
            case TokenType.IntLiteral:
                {
                    int value = _current.Value!.Value is int number
                        ? number
                        : int.Parse(_current.Value.ToString(), CultureInfo.InvariantCulture);
                    Advance();
                    _instructions.Add(new Instruction(OpCode.Push, value));
                }

                break;

            case TokenType.LeftParenthesis:
                Advance();
                ParseExpression();
                Match(TokenType.RightParenthesis);
                break;

            case TokenType.Identifier:
                {
                    string functionName = _current.Value?.ToString() ?? string.Empty;

                    switch (functionName)
                    {
                        case "sqrt":
                            ParseSqrtCall();
                            break;

                        case "abs":
                            ParseAbsCall();
                            break;

                        case "min":
                            ParseMinCall();
                            break;

                        case "max":
                            ParseMaxCall();
                            break;

                        case "readInt":
                            ParseReadIntCall();
                            break;

                        default:
                            throw new ParserException(
                                $"Unexpected identifier '{functionName}' where expected one of: sqrt, abs, min, max, readInt");
                    }
                }

                break;

            default:
                throw new ParserException(
                    $"Unexpected lexeme {_current} where expected IntLiteral, LeftParenthesis or Identifier");
        }
    }

    /// <summary>
    /// Разбирает вызов функции sqrt.
    /// Правило:
    ///     sqrt_call = "sqrt", "(", expression, ")" ;
    /// </summary>
    private void ParseSqrtCall()
    {
        Advance();
        Match(TokenType.LeftParenthesis);
        ParseExpression();
        Match(TokenType.RightParenthesis);

        _instructions.Add(new Instruction(OpCode.Sqrt));
    }

    /// <summary>
    /// Разбирает вызов функции abs.
    /// Правило:
    ///     abs_call = "abs", "(", expression, ")" ;
    /// </summary>
    private void ParseAbsCall()
    {
        Advance();
        Match(TokenType.LeftParenthesis);
        ParseExpression();
        Match(TokenType.RightParenthesis);

        _instructions.Add(new Instruction(OpCode.Abs));
    }

    /// <summary>
    /// Разбирает вызов функции min.
    /// Правило:
    ///     min_call = "min", "(", expression, ",", expression, ")" ;
    /// </summary>
    private void ParseMinCall()
    {
        Advance();
        Match(TokenType.LeftParenthesis);
        ParseExpression();
        Match(TokenType.Comma);
        ParseExpression();
        Match(TokenType.RightParenthesis);

        _instructions.Add(new Instruction(OpCode.Min));
    }

    /// <summary>
    /// Разбирает вызов функции max.
    /// Правило:
    ///     max_call = "max", "(", expression, ",", expression, ")" ;
    /// </summary>
    private void ParseMaxCall()
    {
        Advance();
        Match(TokenType.LeftParenthesis);
        ParseExpression();
        Match(TokenType.Comma);
        ParseExpression();
        Match(TokenType.RightParenthesis);

        _instructions.Add(new Instruction(OpCode.Max));
    }

    /// <summary>
    /// Разбирает вызов функции readInt.
    /// Правило:
    ///     read_int_call = "readInt", "(", ")" ;
    /// </summary>
    private void ParseReadIntCall()
    {
        Advance();
        Match(TokenType.LeftParenthesis);
        Match(TokenType.RightParenthesis);

        _instructions.Add(new Instruction(OpCode.Read));
    }

    /// <summary>
    /// Проверяет, что текущая лексема — идентификатор с заданным именем.
    /// </summary>
    private bool IsIdentifier(string name)
    {
        return _current.Type == TokenType.Identifier && _current.Value?.ToString() == name;
    }

    /// <summary>
    /// Читает ожидаемую лексему либо бросает исключение, если встретит иную лексему.
    /// </summary>
    private Token Match(TokenType expected)
    {
        Token token = _current;
        if (token.Type != expected)
        {
            throw new ParserException($"Unexpected lexeme {token} where expected {expected}");
        }

        Advance();
        return token;
    }

    /// <summary>
    /// Переходит к следующему токену.
    /// </summary>
    private void Advance()
    {
        if (_index + 1 < _tokens.Count)
        {
            _index++;
            _current = _tokens[_index];
        }
    }
}