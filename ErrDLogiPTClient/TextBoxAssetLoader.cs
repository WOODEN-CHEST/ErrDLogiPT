using GHEngine.Frame.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

public static class TextBoxAssetLoader
{
    // Static methods.
    public static void LoadTextures(TextBox box)
    {
        ArgumentNullException.ThrowIfNull(box, nameof(box));
        IEnumerable<char> ExtraCharacters = Enumerable.Range(' ', '~' - ' ' + 1).Select(x => (char)x);
        box.PrepareTexturesForRendering(ExtraCharacters);
    }
}