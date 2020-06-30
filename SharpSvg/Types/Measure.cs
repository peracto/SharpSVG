using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Peracto.Svg.Types
{
    public struct Measure
    {
        private static readonly Regex MeasureRegex = new Regex(@"^\s*([+-]?(?=\.\d|\d)(?:\d+)?(?:\.?\d*)(?:[eE][+-]?\d+)?)([a-z%]+)?\s*$");

        public static readonly Measure None = new Measure(0f, MeasureUnit.None, MeasureUsage.None);
        public static readonly Measure One = new Measure(1f, MeasureUnit.User, MeasureUsage.None);
        public static readonly Measure ZeroH = new Measure(0f, MeasureUnit.User, MeasureUsage.Horizontal);
        public static readonly Measure ZeroV = new Measure(0f, MeasureUnit.User, MeasureUsage.Vertical);
        public static readonly Measure HundredPercent = new Measure(100f, MeasureUnit.Percentage, MeasureUsage.None);

        public static readonly Measure HundredPercentH =
            new Measure(100f, MeasureUnit.Percentage, MeasureUsage.Horizontal);

        public static readonly Measure HundredPercentV =
            new Measure(100f, MeasureUnit.Percentage, MeasureUsage.Vertical);

        public static readonly Measure
            FiftyPercentH = new Measure(50f, MeasureUnit.Percentage, MeasureUsage.Horizontal);

        public static readonly Measure FiftyPercentV = new Measure(50f, MeasureUnit.Percentage, MeasureUsage.Vertical);
        public static readonly Measure TwelvePoint = new Measure(12f, MeasureUnit.Point, MeasureUsage.Vertical);

        public float Value { get; }
        public MeasureUsage Usage { get; }
        public MeasureUnit Unit { get; }

        public override bool Equals(object obj)
        {
            return obj is Measure measure && Equals(measure);
        }

        public bool Equals(Measure other)
        {
            return Unit == other.Unit &&
                   Usage == other.Usage &&
                   (Math.Abs(Value - other.Value) < float.Epsilon);
        }

        public override int GetHashCode()
        {
            return (1000000007 * Unit.GetHashCode()) + (1000000009 * Value.GetHashCode());
        }

        public override string ToString()
        {
            return Unit == MeasureUnit.None
                ? "none"
                : $"{Value.ToString(CultureInfo.InvariantCulture)}{GetUnitName()}";
        }


        public static Measure Parse(string value, MeasureUsage measureUsage)
        {
            return TryParse(value, measureUsage, out var m) ? m : Measure.None;
        }

        public static bool TryParse(string value, MeasureUsage measureUsage, out Measure measure)
        {
            var unit = value.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(unit))
            {
                measure = new Measure(0f, MeasureUnit.User, measureUsage);
                return true;
            }

            if (unit == "none")
            {
                measure = Measure.None;
                return true;
            }

            var match = MeasureRegex.Match(value);

            //  Console.WriteLine($"Matching {value} to Measure {match.Success}");

            if (!match.Success)
            {
                measure = Measure.None;
                return false;
            }

            measure = new Measure(
                float.Parse(match.Groups[1].Value),
                TranslateToUnitType(match.Groups[2].Value),
                measureUsage
            );

            return true;
        }


        private string GetUnitName()
        {
            switch (Unit)
            {
                case MeasureUnit.None: return "none";
                case MeasureUnit.Pixel: return "px";
                case MeasureUnit.Point: return "pt";
                case MeasureUnit.Inch: return "in";
                case MeasureUnit.Centimeter: return "cm";
                case MeasureUnit.Millimeter: return "mm";
                case MeasureUnit.Percentage: return "%";
                case MeasureUnit.Em: return "em";
                case MeasureUnit.Ex: return "em";
                case MeasureUnit.User: return "";
                case MeasureUnit.Pica: return "pc";
                case MeasureUnit.Unknown: return "";
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private static MeasureUnit TranslateToUnitType(string type)
        {
            switch (type)
            {
                case "": return MeasureUnit.User;
                case "mm": return MeasureUnit.Millimeter;
                case "cm": return MeasureUnit.Centimeter;
                case "in": return MeasureUnit.Inch;
                case "px": return MeasureUnit.Pixel;
                case "pt": return MeasureUnit.Point;
                case "pc": return MeasureUnit.Pica;
                case "%": return MeasureUnit.Percentage;
                case "em": return MeasureUnit.Em;
                case "ex": return MeasureUnit.Ex;
                default: return MeasureUnit.Unknown;
            }
        }

        public Measure(float value, MeasureUnit unit, MeasureUsage renderType)
        {
            Unit = unit;
            Value = value;
            Usage = renderType;
        }
    }
}