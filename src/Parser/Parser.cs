using Lexems;
using VirtualMachine;
using System.Collections.Generic;

namespace Parser;

public sealed class Parser
{
    private readonly IReadOnlyList<Token> _tokens;
    private readonly List<Instruction> _instructions;
    private int _index;

    private Token _current;

    public Parser(IReadOnlyList<Token> tokens)
    {
        _tokens = tokens;
        _instructions = new List<Instruction>();
        _index = 0;
        _current = tokens[_index];
    }


    public List<Instruction> ParseProgram() { return _instructions; }
    private void ParseStatement() { }
    private void ParsePrintStatement() { }
    private void ParseExpressionStatement() { }
    private void ParseAdditiveExpression() { }
    private void ParseMultiplicativeExpression() { }
    private void ParseUnaryExpression() { }
    private void ParsePrimaryExpression() { }
}
