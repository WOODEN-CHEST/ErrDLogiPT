using ErrDLogiPTClient.Scene.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public class DefaultSceneFactoryProvider : ISceneFactoryProvider
{
    public IUIElementFactory GetUIElementFactory(IGameScene scene)
    {
        return new DefaultUIElementFactory(scene.SceneServices);
    }
}