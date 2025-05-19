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
    public IEnumerable<ModPackage> Mods => _mods;
    public int ModCount => _mods.Length;


    // Private fields.
    private readonly IGameServices _services;
    private ModPackage[] _mods;


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

        foreach (ModPackage Mod in _mods)
        {
            try
            {
                Mod.EntryPointObject.OnGameLoad(_services);
            }
            catch (Exception e)
            {
                _services.Logger?.Error($"Unhandled exception in mod \"{Mod.Name}\" in OnGameLoad call: {e}");
            }
        }
    }

    public void OnGameClose()
    {
        foreach (ModPackage Mod in _mods)
        {
            try
            {
                Mod.EntryPointObject.OnGameClose(_services);
            }
            catch (Exception e)
            {
                _services.Logger?.Error($"Unhandled exception in mod \"{Mod.Name}\" in OnGameClose call: {e}");
            }
        }
    }
}