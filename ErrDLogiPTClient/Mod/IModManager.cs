using GHEngine.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Mod;

public interface IModManager
{
    // Fields.
    public IEnumerable<IGameMod> Mods { get; }
    int ModCount { get; }


    // Methods.
    void LoadMods();
    void OnGameClose();
}