using Peracto.Svg.Utility;

namespace Peracto.Svg.Text
{
  public enum FontStretch
  {
    Normal,
    Wider,
    Narrower,
    Condensed,
    Expanded,
    [ExternalName("ultra-condensed")] UltraCondensed,
    [ExternalName("extra-condensed")] ExtraCondensed,
    [ExternalName("semi-condensed")] SemiCondensed,
    [ExternalName("semi-expanded")] SemiExpanded,
    [ExternalName("extra-expanded")] ExtraExpanded,
    [ExternalName("ultra-expanded")] UltraExpanded,
    Inherit
  }
}