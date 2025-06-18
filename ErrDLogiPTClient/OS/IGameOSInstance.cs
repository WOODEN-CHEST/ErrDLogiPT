using ErrDLogiPTClient.Service;
using GHEngine;
using GHEngine.Frame.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.OS;

public interface IGameOSInstance : ITimeUpdatable, IRenderableItem
{
    // Fields.
    IGameOSDefinition Definition { get; }
    IGenericServices Services { get; }


    // Methods.
    void Start();
    void ShutDown();
    void Restart();
}