namespace Lexems;

public sealed class Token : IEquatable<Token>
{
    public Token(TokenType type)
    {
        Type = type;
    }

    public Token(TokenType type, string value)
        : this(type)
    {
        Value = new TokenValue(value);
    }

    public Token(TokenType type, int value)
        : this(type)
    {
        Value = new TokenValue(value);
    }

    public TokenType Type { get; }

    public TokenValue? Value { get; }

    public bool Equals(Token? other)
    {
        if (other is null)
        {
            return false;
        }

        return Type == other.Type && Equals(Value, other.Value);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Token);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Value);
    }

    public override string ToString()
    {
        if (Value is null)
        {
            return Type.ToString();
        }

        return $"{Type} ({Value})";
    }
}