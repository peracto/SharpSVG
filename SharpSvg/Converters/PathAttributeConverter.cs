using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Peracto.Svg.Paths;
using Peracto.Svg.Types;
using Peracto.Svg.Utility;
using SharpDX.DirectWrite;

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
          return new ArcPathCommand(
            cursor, 
            args[0], 
            args[1], 
            args[2], 
            Math.Abs(args[3]) > float.Epsilon,
            Math.Abs(args[4]) > float.Epsilon, 
            args.ToPoint(5,cursor)
            );
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

    public static bool TryParse(string value, out Path path)
    {
      var bounds = new XYRect();
      var segments = new List<PathSegment>();
      foreach (var ps in GetPathSegments(value))
      {
        segments.Add(ps);
        bounds = bounds.Add(ps.Bounds);
      }
      path = new Path(segments.ToArray(), bounds);
      return true;
    }

    protected static IEnumerable<PathSegment> GetPathSegments(string attributeValue)
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
      var parser = new PathParser(text);

      float x1 = 0;
      float y1 = 0;
      float x2 = 0;
      float y2 = 0;
      float x3 = 0;
      float y3 = 0;

      var action = '\0';

      while (!parser.Eof)
      {
        if (parser.TryGetCommand(out var cmd))
          action = cmd;

        switch (char.ToUpper(action))
        {
          case 'Z':
            yield return new KeyValuePair<char, IList<float>>(action, null);
            break;
          case 'M':
            if (parser.TryGetValue(out x1, out y1))
              yield return new KeyValuePair<char, IList<float>>(action, new List<float>() {x1, y1});
            action = action == 'M' ? 'L' : 'l';
            break;
          case 'L':
            if (parser.TryGetValue(out x1, out y1))
              yield return new KeyValuePair<char, IList<float>>(action, new List<float>() {x1, y1});
            break;
          case 'T':
            if (parser.TryGetValue(out x1, out y1))
              yield return new KeyValuePair<char, IList<float>>(action, new List<float>() {x1, y1});
            break;
          case 'H':
          case 'V':
            if (parser.TryGetValue(out x1))
              yield return new KeyValuePair<char, IList<float>>(action, new List<float>() {x1});
            break;
          case 'C':
            if (parser.TryGetValue(out x1, out y1, out x2, out y2, out x3, out y3))
              yield return new KeyValuePair<char, IList<float>>(action, new List<float>() {x1, y1, x2, y2, x3, y3});
            break;
          case 'S':
          case 'Q':
            if (parser.TryGetValue(out x1, out y1, out x2, out y2))
              yield return new KeyValuePair<char, IList<float>>(action, new List<float>() {x1, y1, x2, y2});
            break;
          case 'A':
            if (parser.TryGetArcValue(out x1, out x2, out x3, out var flag1, out var flag2, out y1, out y2))
              yield return new KeyValuePair<char, IList<float>>(action,
                new List<float>() {x1, x2, x3, flag1, flag2, y1, y2});
            break;
          default:
            parser.TryGetValue(out x1);
            break;
        }
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


    private class PathParser
    {
      private readonly IEnumerator<Token> _t;

      public PathParser(string text)
      {
        _t = GetTokens(text).GetEnumerator();
        Eof = !_t.MoveNext();
      }

      public bool Eof { get; private set; }

      public bool TryGetCommand(out char cmd)
      {
        if (_t.Current.TokenType != TokenType.String)
        {
          cmd = '\0';
          return false;
        }

        cmd = _t.Current.Value[0];
        Eof = !_t.MoveNext();
        return true;
      }

      public bool TryGetValue(out float x)
      {
        if (TryGetValueRaw(out var v))
        {
          x = float.Parse(v);
          return true;
        }
        x = 0;
        return false;
      }

      private bool TryGetValueRaw(out string x)
      {
        if (_t.Current.TokenType == TokenType.Number)
        {
          x = _t.Current.Value;
          Eof = !_t.MoveNext();
          return true;
        }
        x = "";
        return false;
      }

      public bool TryGetValue(out float v1, out float v2)
      {
        if (TryGetValue(out v1) && TryGetValue(out v2))
          return true;
        v1 = 0;
        v2 = 0;
        return false;
      }

      private bool TryGetValue(out float v1, out float v2, out float v3)
      {
        if (TryGetValue(out v1) && TryGetValue(out v2) && TryGetValue(out v3))
          return true;
        v1 = 0;
        v2 = 0;
        v3 = 0;
        return false;
      }

      public bool TryGetValue(out float v1, out float v2, out float v3, out float v4)
      {
        if (TryGetValue(out v1) && TryGetValue(out v2) && TryGetValue(out v3) && TryGetValue(out v4))
          return true;
        v1 = 0;
        v2 = 0;
        v3 = 0;
        v4 = 0;
        return false;
      }

      public bool TryGetValue(out float v1, out float v2, out float v3, out float v4, out float v5, out float v6)
      {
        if (TryGetValue(out v1) && TryGetValue(out v2) && TryGetValue(out v3) && TryGetValue(out v4) && TryGetValue(out v5) && TryGetValue(out v6))
          return true;
        v1 = 0;
        v2 = 0;
        v3 = 0;
        v4 = 0;
        v5 = 0;
        v6 = 0;
        return false;
      }

      public bool TryGetArcValue(out float x1, out float x2, out float x3,
        out float flag1, out float flag2, out float y1, out float y2)
      {
        if (
          TryGetValue(out x1,out x2, out x3) &&
          TryCoordinateFlagSet(out flag1, out flag2, out y1) &&
          TryGetValue(out y2)
        ) return true;
        x1 = x2 = x3 = flag1 = flag2 = y1 = y2 = 0;
        return false;
      }

      private bool TryCoordinateFlagSet(out float flag1, out float flag2, out float value1)
      {
        flag1 = 0;
        flag2 = 0;
        value1 = 0;


        try
        {

          if (!TryGetValueRaw(out var v4)) return false;
          if (v4.StartsWith("-") || v4.StartsWith("+")) return false;
          if (v4.Length == 1)
          {
            flag1 = float.Parse(v4);
            if (!TryGetValueRaw(out v4)) return false;
            if (v4.StartsWith("-") || v4.StartsWith("+")) return false;
            if (v4.Length == 1)
            {
              flag2 = float.Parse(v4);
              v4 = "";
            }
            else
            {
              flag2 = float.Parse(v4.Substring(0, 1));
              v4 = v4.Substring(1);
            }
          }
          else
          {
            flag1 = float.Parse(v4.Substring(0, 1));
            flag2 = float.Parse(v4.Substring(1, 1));
            v4 = v4.Substring(2);
          }

          if (v4.Length > 0)
            value1 = float.Parse(v4);
          else if (!TryGetValue(out value1)) return false;
          return true;
        }
        catch (Exception ex)
        {
          throw ex;
        }
      }

      private static IEnumerable<Token> GetTokens(string text)
      {
        var rdr = new SimpleStringReader(text);

        rdr.SkipWhiteSpace();

        while (!rdr.Eof)
        {
          var ch = rdr.Peek();
          if (ch == '-' || ch == '+' || ch=='.' || char.IsDigit(ch))
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
          else if (ch == ',')
          {
            rdr.Consume();
          }
          else /*if (char.IsLetter(ch))*/
          {
            var p = rdr.Position;
            rdr.Consume();
            yield return new Token(TokenType.String, rdr.GetString(p));
          }
          /*else
            rdr.Consume();*/

          rdr.SkipWhiteSpace();
        }
      }

    }
  }
}
