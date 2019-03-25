using System;

namespace Peracto.Svg.Utility
{
  public class ExternalNameAttribute : Attribute
  {
    public string Name { get; }
    public ExternalNameAttribute(string name)
    {
      Name = name;
    }
  }
}