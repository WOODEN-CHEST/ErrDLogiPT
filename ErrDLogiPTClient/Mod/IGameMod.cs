using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Mod;

public interface IGameMod
{
    // Methods.
    void OnGameLoad(IGameServices services);
    void OnGameClose(IGameServices services);
}