﻿using ErrDLogiPTClient.Scene.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public interface ISceneFactoryProvider
{
    IUIElementFactory GetUIElementFactory(IGameScene scene);
}