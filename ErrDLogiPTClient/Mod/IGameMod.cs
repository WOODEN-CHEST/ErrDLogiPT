using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Mod;

public interface IGameMod
{
    // Methods.
    void OnStart(GameServices services);
    void OnEnd(GameServices services);
}