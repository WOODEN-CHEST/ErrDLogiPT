using GHEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public interface ISceneComponent : ITimeUpdatable
{
    void OnStart();
    void OnEnd();
    void OnLoad();
    void OnUnload();
}