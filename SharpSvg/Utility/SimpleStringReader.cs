using System;
using System.Collections.Generic;

namespace Peracto.Svg.Utility
{

  public static class ArgumentParser
  {
    public static IEnumerable<string> Parse(string text)
    {
      var r = new SimpleStringReader(text);

      if (r.PeekNext() == ',')
        yield return string.Empty;

      while (!r.Eof)
      {
        if (!r.TryReadNumber(out var value))
        {
          break;
        }

        yield return value;

        if (r.PeekNext() != ',') continue;
        r.Consume();
        while (r.PeekNext() == ',')
          yield return string.Empty;
      }
    }
  }

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

    public char PeekNext()
    {
      if (Eof) return '\0';
      SkipWhiteSpace();
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

    public bool TryReadNumber(out string value)
    {
      var s = Position;
      var ch = Peek();

      if (char.IsWhiteSpace(ch))
      {
        ch = PeekNext();
      }

      if (ch == '+' || ch == '-')
      {
        Consume();
        ch = Peek();
      }

      if (!char.IsDigit(ch))
      {
        value = string.Empty;
        return false;
      }

      while (char.IsDigit(Peek())) Consume();
      if (Peek() == '.')
      {
        Consume();
        while (char.IsDigit(Peek()))
          Consume();
      }

      if (Peek() == '%') Consume();

      value = _text.Substring(s, Position - s);
      return true;
    }

    public string GetString(int p)
    {
      return _text.Substring(p, (Position - p));
    }
  }
}
