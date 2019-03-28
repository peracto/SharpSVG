using System;
using System.Collections.Generic;
using System.Linq;
using Peracto.Svg.Paths;
using Peracto.Svg.Types;
using Peracto.Svg.Utility;

namespace Peracto.Svg.Converters
{
  public class PathAttributeConverter : AttributeConverterBase<Path>
  {
    public PathAttributeConverter(string name) : base(name)
    {
    }

    private static IPathCommand CreateSegment(PxPoint cursor, IPathCommand previous, PxPoint start, char command, IList<float> args)
    {
      switch (command)
      {
        case 'm': // relative moveto
          return new MovePathCommand(cursor, args.ToPoint(0,cursor));
        case 'M': // moveto
          return new MovePathCommand(cursor, args.ToPoint(0));
        case 'l': // relative lineto
          return new LinePathCommand(cursor, args.ToPoint(0,cursor));
        case 'L': // lineto
          return new LinePathCommand(cursor, args.ToPoint(0));
        case 'h': // relative horizontal lineto
          return new LinePathCommand(cursor, new PxPoint(args[0], 0).Add(cursor));
        case 'H': // horizontal lineto
          return new LinePathCommand(cursor, new PxPoint(args[0], cursor.Y));
        case 'v': // relative vertical lineto
          return new LinePathCommand(cursor, new PxPoint(0, args[0]).Add(cursor));
        case 'V': // vertical lineto
          return new LinePathCommand(cursor, new PxPoint(cursor.X, args[0]));
        case 'q': // relative curveto
          return new QuadraticPathCommand(cursor, args.ToPoint(0,cursor), args.ToPoint(2,cursor));
        case 'Q': // curveto
          return new QuadraticPathCommand(cursor, args.ToPoint(0), args.ToPoint(2));
        case 't': // relative shorthand/smooth curveto
          return new QuadraticPathCommand(cursor, previous.ShortPoint, args.ToPoint(0,cursor));
        case 'T': // shorthand/smooth curveto
          return new QuadraticPathCommand(cursor, previous.ShortPoint, args.ToPoint(0));
        case 'c': // relative curveto
          return new CubicCurveCommand(cursor, args.ToPoint(0,cursor), args.ToPoint(2,cursor), args.ToPoint(4,cursor));
        case 'C': // curveto
          return new CubicCurveCommand(cursor, args.ToPoint(0), args.ToPoint(2), args.ToPoint(4));
        case 's': // relative shorthand/smooth curveto
          return new CubicCurveCommand(cursor, previous.ShortPoint, args.ToPoint(0,cursor), args.ToPoint(2,cursor));
        case 'S': // shorthand/smooth curveto
          return new CubicCurveCommand(cursor, previous.ShortPoint, args.ToPoint(0), args.ToPoint(2));
        case 'Z': // closepath
        case 'z': // relative closepath
          return new ClosePathCommand(cursor, start, true);
        case 'a':
          return new ArcPathCommand(cursor, args[0], args[1], args[2], Math.Abs(args[3]) > float.Epsilon,
            Math.Abs(args[4]) > float.Epsilon, args.ToPoint(5,cursor));
        case 'A':
          return new ArcPathCommand(cursor, args[0], args[1], args[2], Math.Abs(args[3]) > float.Epsilon,
            Math.Abs(args[4]) > float.Epsilon, args.ToPoint(5));
        default:
          throw new Exception();
      }
    }

    protected override bool TryCreate(string attributeValue,  out Path path)
    {
      var bounds = new XYRect();
      var segments = new List<PathSegment>();
      foreach (var ps in GetPathSegments(attributeValue))
      {
        segments.Add(ps);
        bounds = bounds.Add(ps.Bounds);
      }
      path = new Path(segments.ToArray(),bounds);
      return true;
    }

    protected IEnumerable<PathSegment> GetPathSegments(string attributeValue)
    {
      IList<IPathCommand> stack = null;
      var bounds = new XYRect();
      var cursor = new PxPoint(0, 0);
      var start = new PxPoint(0,0);
      IPathCommand previous = null;

      foreach (var command in GetCommands(attributeValue))
      {
        var c = CreateSegment(cursor, previous, start, command.Key, command.Value);
        previous = c;

        if (command.Key == 'M' || command.Key == 'm')
        {
          if (stack != null)
          {
            //If we get here, we have an existing path that has not been closed.
            // We add a dummy closepath 
            stack.Add(new ClosePathCommand(cursor, start, false));
            yield return new PathSegment(stack, bounds, false);
          }

          stack = new List<IPathCommand>();
          bounds = new XYRect();
          start = c.NextPoint;
          stack.Add(c);
        }
        else if (command.Key == 'Z' || command.Key == 'z')
        {
          if (stack != null)
          {
            stack.Add(c);
            yield return new PathSegment(stack, bounds, true);
          }

          stack = null;
          bounds = new XYRect();
        }
        else
        {
          stack?.Add(c);
        }

        cursor = c.NextPoint;
        bounds.AddPoint(cursor);
      }

      if (stack != null)
      {
        //If we get here, we have an existing path that has not been closed.
        // We add a dummy closepath 
        stack.Add(new ClosePathCommand(cursor, start, false));
        yield return new PathSegment(stack, bounds, false);
      }
    }


    private static IEnumerable<KeyValuePair<char, IList<float>>> GetCommands(string text)
    {
      foreach (var a in GetSuperCommands(text))
      {
        var k = a;
        if ((k.Key == 'M' || k.Key == 'm') && k.Value.Count > 2)
        {
          yield return new KeyValuePair<char, IList<float>>(a.Key,a.Value.Take(2).ToList());
          k = new KeyValuePair<char, IList<float>>(a.Key=='M'?'L':'l', a.Value.Skip(2).ToList());
        }

        if((a.Key=='L' || a.Key == 'l') && k.Value.Count > 2)
        {
          var list = a.Value;
          while (list.Count >= 2)
          {
            yield return new KeyValuePair<char, IList<float>>(k.Key,list.Take(2).ToList());
            list = list.Skip(2).ToList();
          }
        }
        else
        {
          yield return a;
        }
      }
    }


    private static IEnumerable<KeyValuePair<char, IList<float>>> GetSuperCommands(string text)
    {
      var command = '\0';
      IList<float> args = null;

      foreach (var t in GetTokens(text))
      {
        switch (t.TokenType)
        {
          case TokenType.String:
          {
            if (command != '\0')
            {
              yield return new KeyValuePair<char, IList<float>>(command, args);
            }

            command = t.Value[0];
            args = null;
            break;
          }
          case TokenType.Number:
          {
            if (args == null) args = new List<float>();
            args.Add(float.Parse(t.Value));
            break;
          }
          default:
            throw new ArgumentOutOfRangeException();
        }
      }

      yield return new KeyValuePair<char, IList<float>>(command, args);
    }

    private static IEnumerable<Token> GetTokens(string text)
    {
      var rdr = new SimpleStringReader(text);

      rdr.SkipWhiteSpace();

      while (!rdr.Eof)
      {
        var ch = rdr.Peek();
        if (ch == '-' || ch == '+' || char.IsDigit(ch))
        {
          var p = rdr.Position;
          rdr.Consume();
          while (char.IsDigit(rdr.Peek())) rdr.Consume();
          if (rdr.Peek() == '.')
          {
            rdr.Consume();
            while (char.IsDigit(rdr.Peek())) rdr.Consume();
          }

          ch = rdr.Peek();
          if (ch == 'e' || ch == 'E')
          {
            rdr.Consume();
            ch = rdr.Peek();
            if (ch == '-' || ch == '+')
              rdr.Consume();
            while (char.IsDigit(rdr.Peek())) rdr.Consume();
          }




          yield return new Token(TokenType.Number, rdr.GetString(p));
        }
        else if (char.IsLetter(ch))
        {
          var p = rdr.Position;
          rdr.Consume();
          yield return new Token(TokenType.String, rdr.GetString(p));
        }
        else
          rdr.Consume();

        rdr.SkipWhiteSpace();
      }
    }

    private enum TokenType
    {
      String,
      Number
    }

    private struct Token
    {
      public Token(TokenType type, string value)
      {
        TokenType = type;
        Value = value;
      }

      public TokenType TokenType { get; private set; }
      public string Value { get; private set; }
    }

    private class XYRect
    {
      private readonly  float _x1;
      private readonly float _y1;
      private readonly float _x2;
      private readonly float _y2;

      public XYRect()
      {
      }

      private XYRect(float x1, float y1, float x2, float y2)
      {
        _x1 = x1;
        _y1 = y1;
        _x2 = x2;
        _y2 = y2;
      }

      public XYRect AddPoint(PxPoint pt)
      {
        return new XYRect(
          Math.Min(_x1, pt.X),
          Math.Min(_y1, pt.Y),
          Math.Max(_x2, pt.X),
          Math.Max(_y2, pt.Y)
        );
      }

      public XYRect Add(PxRectangle r)
      {
        return new XYRect(
          Math.Min(_x1, r.X),
          Math.Min(_y1, r.Y),
          Math.Max(_x2, r.X + r.Width),
          Math.Max(_y2, r.Y + r.Height)
        );
      }

      public static   implicit operator PxRectangle(XYRect r)
      {
        return new PxRectangle(r._x1,r._y1,r._x2-r._x1,r._y2-r._y1);
      }
    }
  }



}
