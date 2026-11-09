namespace Lexems;

public sealed class TextScanner
{
    private readonly string _source;
    private int _position;

    public TextScanner(string source)
    {
        _source = source;
    }

    public string Source => _source;

    public int Position => _position;

    public char Peek(int offset = 0)
    {
        int index = _position + offset;

        if (index < 0 || index >= _source.Length)
        {
            return '\0';
        }

        return _source[index];
    }

    public void Advance()
    {
        if (_position < _source.Length)
        {
            _position++;
        }
    }

    public bool IsEnd()
    {
        return _position >= _source.Length;
    }
}