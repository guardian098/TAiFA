namespace Lexems;

public sealed class LexerException : Exception
{
    public LexerException()
    {
    }

    public LexerException(string message)
        : base(message)
    {
    }

    public LexerException(string message, Exception inner)
        : base(message, inner)
    {
    }
}