﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Peracto.Svg.Transform;
using Peracto.Svg.Types;
// ReSharper disable ConvertIfStatementToSwitchStatement

namespace Peracto.Svg.Converters
{
  public class TransformAttributeConverter : AttributeConverterBase<ITransform>
  {
    public TransformAttributeConverter(string name) : base(name)
    {
    }

    protected override bool TryCreate(string attributeValue, out ITransform rc)
    {
      var transforms = SplitTransforms(attributeValue)
        .Select(f => ParseFunction(f.Key, f.Value))
        .ToArray();
      rc = Simplify(transforms);
      return rc != null;
    }

    public static bool TryParse(string attributeValue, out ITransform rc)
    {
      var transforms = SplitTransforms(attributeValue)
        .Select(f => ParseFunction(f.Key, f.Value))
        .ToArray();
      rc = Simplify(transforms);
      return rc != null;
    }


    private static ITransform ParseFunction(string transformName, IReadOnlyList<string> points)
    {
      switch (transformName)
      {
        case "translate":
          if (points.Count == 1)
            return new TranslateTransform(Measure.Parse(points[0],MeasureUsage.Horizontal), Measure.ZeroV);
          else if (points.Count == 2)
            return new TranslateTransform(Measure.Parse(points[0], MeasureUsage.Horizontal), Measure.Parse(points[1], MeasureUsage.Vertical));
          else
            throw new FormatException("Translate transforms must be in the format 'translate(x [,y])'");
        case "rotate":
          if (points.Count == 1)
            return new RotateTransform(new Angle(float.Parse(points[0]), AngleUnit.Degree));
          else if (points.Count == 3)
            return new RotateTransform(new Angle(float.Parse(points[0]), AngleUnit.Degree), Measure.Parse(points[1], MeasureUsage.Horizontal), Measure.Parse(points[2], MeasureUsage.Vertical));
          else
            throw new FormatException("Rotate transforms must be in the format 'rotate(angle [cx cy ])'");
        case "scale":
          if (points.Count == 2)
            return new ScaleTransform(Measure.Parse(points[0], MeasureUsage.Horizontal), Measure.Parse(points[1], MeasureUsage.Vertical));
          else if (points.Count == 1)
            return new ScaleTransform(Measure.Parse(points[0], MeasureUsage.Horizontal), Measure.Parse(points[0], MeasureUsage.Vertical));
          else
            throw new FormatException("Scale transforms must be in the format 'scale(x [,y])'");
        case "matrix":
            if (points.Count == 6)
                return new MatrixTransform(
                    Measure.Parse(points[0], MeasureUsage.Horizontal),
                    Measure.Parse(points[1], MeasureUsage.Horizontal),
                    Measure.Parse(points[2], MeasureUsage.Horizontal),
                    Measure.Parse(points[3], MeasureUsage.Horizontal),
                    Measure.Parse(points[4], MeasureUsage.Horizontal),
                    Measure.Parse(points[5], MeasureUsage.Horizontal)
                );
          else
            throw new FormatException(
              "Matrix transforms must be in the format 'matrix(m11, m12, m21, m22, dx, dy)'");
        case "shear":
          if (points.Count == 1)
            return new ShearTransform(Measure.Parse(points[0], MeasureUsage.Horizontal), Measure.Parse(points[1], MeasureUsage.Vertical));
          else if (points.Count == 2)
            return new ShearTransform(Measure.Parse(points[0], MeasureUsage.Horizontal), Measure.Parse(points[1], MeasureUsage.Vertical));
          else
            throw new FormatException("Shear transforms must be in the format 'shear(x [,y])'");
        case "skewx":
          if (points.Count == 1)
            return new SkewTransform(Measure.Parse(points[0], MeasureUsage.Horizontal), Measure.ZeroV);
          else
            throw new FormatException("SkewX transforms must be in the format 'skewX(x)'");
        case "skewy":
          if (points.Count == 1)
            return new SkewTransform(Measure.ZeroH, Measure.Parse(points[1], MeasureUsage.Vertical));
          else
            throw new FormatException("SkewY transforms must be in the format 'skewY(y)'");
        default:
          throw new FormatException($"Unexpected transformation '{transformName}'");
      }
    }

    private static ITransform Simplify(ITransform[] transforms)
    {
      switch (transforms.Length)
      {
        case 0: return null;
        case 1: return transforms[0];
        case 2: return new MultiTransform2(transforms[0], transforms[1]);
        case 3: return new MultiTransform3(transforms[0], transforms[1], transforms[2]);
        case 4: return new MultiTransform4(transforms[0], transforms[1], transforms[2], transforms[3]);
        default: return new MultiTransform(transforms);
      }
    }

    private static IEnumerable<KeyValuePair<string, string[]>> SplitTransforms(string transforms)
    {
      var transformEnd = 0;

      for (var i = 0; i < transforms.Length; i++)
      {
        if (transforms[i] != ')') continue;
        var transform = transforms.Substring(transformEnd, i - transformEnd + 1).Trim();

        if (!string.IsNullOrEmpty(transform))
        {
          var parts = transform.Split('(', ')');
          var args = Regex.Replace(parts[1].Trim(), @"[\s,]+", ",")
            .Split(',')
            .Select(s => s.Trim());
          yield return new KeyValuePair<string, string[]>(parts[0].Trim().ToLower(), args.ToArray());
        }

        while (i < transforms.Length && !char.IsLetter(transforms[i])) i++;
        transformEnd = i;
      }
    }
  }
}