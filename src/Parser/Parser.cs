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
            throw new LexerException($"Lexical error: invalid lexeme '{token.Value}'.");
        }
    }

    _tokens = tokens;
    _instructions = new List<Instruction>();
    _index = 0;
    _current = _tokens[_index];
}

    /// <summary>
    /// Разбирает программу.
    /// Правила:
    ///     program = main_function ;
    ///     main_function = "void", "main", "(", ")", block ;
    ///     block = "{", { statement }, "}" ;
    /// </summary>
    public List<Instruction> ParseProgram()
    {
        Match(TokenType.VoidKeyword);

        Token mainToken = Match(TokenType.Identifier);
        if (mainToken.Value?.ToString() != "main")
        {
            throw new ParserException($"Unexpected identifier '{mainToken.Value}' where expected 'main'");
        }

        Match(TokenType.LeftParenthesis);
        Match(TokenType.RightParenthesis);
        Match(TokenType.LeftBrace);

        while (_current.Type != TokenType.RightBrace && _current.Type != TokenType.EndOfFile)
        {
            ParseStatement();
        }

        Match(TokenType.RightBrace);
        Match(TokenType.EndOfFile);

        return _instructions;
    }

    /// <summary>
    /// Разбирает инструкцию.
    /// Правило:
    ///     statement = print_statement | expression_statement ;
    /// </summary>
    private void ParseStatement()
    {
        if (_current.Type == TokenType.Identifier && _current.Value?.ToString() == "print")
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
        ParseAdditiveExpression();
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
        ParseAdditiveExpression();
        Match(TokenType.Semicolon);

        _instructions.Add(new Instruction(OpCode.Pop));
    }

    /// <summary>
    /// Разбирает операторы сложения и вычитания.
    /// Правила:
    ///     expression = additive_expression ;
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
    /// Выполняет разбор унарного плюса и минуса.
    /// Правило:
    ///     unary_expression = { "+" | "-" }, primary_expression ;
    /// </summary>
    private void ParseUnaryExpression()
    {
        if (_current.Type == TokenType.Plus)
        {
            Advance();
            ParseUnaryExpression();
            return;
        }

        if (_current.Type == TokenType.Minus)
        {
            Advance();
            ParseUnaryExpression();
            _instructions.Add(new Instruction(OpCode.Neg));
            return;
        }

        ParsePrimaryExpression();
    }

    /// <summary>
    /// Разбирает элементарные выражения.
    /// Правила:
    ///     primary_expression = integer_literal
    ///         | "(", expression, ")"
    ///         | sqrt_call
    ///         | abs_call
    ///         | min_call
    ///         | max_call ;
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
                ParseAdditiveExpression();
                Match(TokenType.RightParenthesis);
                break;

            case TokenType.Identifier:
                {
                    string functionName = _current.Value?.ToString() ?? string.Empty;
                    Advance();

                    switch (functionName)
                    {
                        case "sqrt":
                            Match(TokenType.LeftParenthesis);
                            ParseAdditiveExpression();
                            Match(TokenType.RightParenthesis);
                            _instructions.Add(new Instruction(OpCode.Sqrt));
                            break;

                        case "abs":
                            Match(TokenType.LeftParenthesis);
                            ParseAdditiveExpression();
                            Match(TokenType.RightParenthesis);
                            _instructions.Add(new Instruction(OpCode.Abs));
                            break;

                        case "min":
                            Match(TokenType.LeftParenthesis);
                            ParseAdditiveExpression();
                            Match(TokenType.Comma);
                            ParseAdditiveExpression();
                            Match(TokenType.RightParenthesis);
                            _instructions.Add(new Instruction(OpCode.Min));
                            break;

                        case "max":
                            Match(TokenType.LeftParenthesis);
                            ParseAdditiveExpression();
                            Match(TokenType.Comma);
                            ParseAdditiveExpression();
                            Match(TokenType.RightParenthesis);
                            _instructions.Add(new Instruction(OpCode.Max));
                            break;

                        default:
                            throw new ParserException(
                                $"Unexpected identifier '{functionName}' where expected one of: sqrt, abs, min, max"
                            );
                    }
                }

                break;

            default:
                throw new ParserException(
                    $"Unexpected lexeme {_current} where expected IntLiteral, LeftParenthesis or Identifier"
                );
        }
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