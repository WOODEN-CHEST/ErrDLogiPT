using GHEngine.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Mod;

public class DefaultModManager : IModManager
{
    // Fields.
    public IEnumerable<IGameMod> Mods => _mods;
    public int ModCount => _mods.Length;


    // Private fields.
    private readonly IGameServices _services;
    private IGameMod[] _mods;


    // Constructors
    public DefaultModManager(IGameServices services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }


    // Inherited methods.
    public void LoadMods()
    {
        IModLoader Loader = new DefaultModLoader(_services.Logger);
        _mods = Loader.LoadMods(_services.Structure.ModRoot);

        foreach (IGameMod Mod  in _mods)
        {
            try
            {
                Mod.OnGameLoad(_services);
            }
            catch (Exception e)
            {
                _services.Logger?.Error($"Unhandled exception in mod \"{Mod.ModName}\" in OnGameLoad call: {e}");
            }
        }
    }

    public void OnGameClose()
    {
        foreach (IGameMod Mod in _mods)
        {
            try
            {
                Mod.OnGameClose(_services);
            }
            catch (Exception e)
            {
                _services.Logger?.Error($"Unhandled exception in mod \"{Mod.ModName}\" in OnGameClose call: {e}");
            }
        }
    }
}