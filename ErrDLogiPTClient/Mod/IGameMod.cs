using GHEngine.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Mod;

public interface IGameMod
{
    // Fields.
    public ILogger? Logger { get; set; }


    // Methods.
    void OnStart(GameServices services);
    void OnEnd(GameServices services);
}