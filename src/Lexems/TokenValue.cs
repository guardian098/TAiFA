using System.Globalization;

namespace Lexems;

public sealed class TokenValue : IEquatable<TokenValue>
{
    public TokenValue(string value)
    {
        Value = value;
    }

    public TokenValue(int value)
    {
        Value = value;
    }

    public object Value { get; }

    public bool Equals(TokenValue? other)
    {
        if (other is null)
        {
            return false;
        }

        return Value.Equals(other.Value);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as TokenValue);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        if (Value is string text)
        {
            return text;
        }

        if (Value is int number)
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }

        return Value.ToString() ?? string.Empty;
    }
}