namespace Peracto.Svg.Utility
{
  public class SimpleStringReader
  {
    private readonly string _text;

    public SimpleStringReader(string text)
    {
      _text = text;
      Eof = string.IsNullOrWhiteSpace(text);
      Position = 0;
    }

    public bool Eof { get; private set; }

    public int Position { get; private set; }

    public char Peek()
    {
      return Eof ? '\0' : _text[Position];
    }

    public char Read()
    {
      var ch = _text[Position++];
      Eof = Position >= _text.Length;
      return ch;
    }

    public void Consume()
    {
      if (Eof) return;
      Position++;
      Eof = Position >= _text.Length;
    }

    public void SkipWhiteSpace()
    {
      while (char.IsWhiteSpace(Peek())) Consume();
    }

    public string GetString(int p)
    {
      return _text.Substring(p, (Position - p));
    }
  }
}
