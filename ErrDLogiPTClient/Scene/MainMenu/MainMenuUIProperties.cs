using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.MainMenu;

public class MainMenuUIProperties
{
    // Fields.
    public required float ButtonOffsetY { get; init; }
    public required Vector2 ButtonStartingPosition { get; init; }
    public required float ButtonScale { get; init; }
    public required float ButtonLength { get; init; }
}