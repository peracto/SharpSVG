using Peracto.Svg.Types;
using Peracto.Svg.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Peracto.Svg.Converters
{
  public class PointsAttributeConverter : AttributeConverterBase<PxPoint[]>
    {
        public PointsAttributeConverter(string name) : base(name)
        {
        }
        protected override bool  TryCreate(string value, out PxPoint[] rc)
        {
          Console.WriteLine(value);
          var list = ArgumentParser.Parse(value)
            .Select(s => float.Parse(s.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture))
            .ToArray();


          // Handle odd number - should be even !
          var l = list.Length % 2 == 1 ? list.Length - 1 : list.Length;

          var a = new List<PxPoint>();
          for (var i = 0; i < l; i += 2)
          {
            a.Add(new PxPoint(list[i],list[i+1]));
          }

          rc = a.ToArray();
          return true;

        }
    }
}