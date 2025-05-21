using GHEngine.GameFont;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

public static class FontInitializer
{
    // Static methods
    public static void InitializeFont(GHFontFamily family, GHFontProperties properties)
    {
        IEnumerable<char> BaseCharacters = Enumerable.Range(' ', '~').Select(value => (char)value);
        family.LoadFontCharacters(properties, BaseCharacters);
    }
}