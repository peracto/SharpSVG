using System;
using Peracto.Svg.Accessor;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Peracto.Svg.Converters;

namespace Peracto.Svg.Parser
{
  public class AttributeFactory
  {
    private readonly IDictionary<string, IElementAttributeFactory> _dict;

    public AttributeFactory()
    {
      var elements = (
        from property in typeof(AttributeDefinitions).GetFields(BindingFlags.Static | BindingFlags.Public)
        where typeof(IElementAttributeFactory).IsAssignableFrom(property.FieldType)
        select (IElementAttributeFactory) property.GetValue(null)
      );

      _dict = elements.ToDictionary(
        (k) => k.AttributeName.ToLower(),
        (v) => v
      );
    }

    public IElementAttribute Create(string elementType, string name, string value)
    {
      if (!_dict.TryGetValue(name.ToLower(), out var factory))
        factory = RegisterGeneric(name);
      try
      {
        return factory.Create(value);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private IElementAttributeFactory RegisterGeneric(string name)
    {
      var factory = new TokenAttributeConverter(name);
      _dict.Add(name.ToLower(), factory);
      return factory;
    }
  }
}