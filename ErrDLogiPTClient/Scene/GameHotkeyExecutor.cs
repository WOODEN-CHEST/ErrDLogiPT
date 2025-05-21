using GHEngine;
using GHEngine.IO;
using GHEngine.Screen;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public class GameHotkeyExecutor : SceneComponentBase<IGameScene>
{
    // Constructors.
    public GameHotkeyExecutor(IGameScene scene, ISceneAssetProvider assetProvider, IGameServices services)
        : base(scene, assetProvider, services)
    {
    }


    // Inherited methods.
    public override void Update(IProgramTime time)
    {
        base.Update(time);

        IUserInput Input = Services.Input;

        if (Input.WereKeysJustPressed(Keys.F11)
            || (Input.WereKeysJustPressed(Keys.Enter) && Input.AreKeysDown(Keys.LeftAlt)))
        {
            IDisplay Display = Services.Display;
            Display.FullScreenSize = Display.ScreenSize;
            Display.IsFullScreen = !Display.IsFullScreen;
        }
    }
}