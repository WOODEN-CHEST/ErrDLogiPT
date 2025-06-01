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
    private readonly GenericServices _services;
    private ModPackage[] _mods = Array.Empty<ModPackage>();


    // Constructors
    public DefaultModManager(GenericServices services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }


    // Inherited methods.
    public void LoadMods(string modRootDirPath)
    {
        IModLoader Loader = new DefaultModLoader(_services.Get<ILogger>());
        _mods = Loader.LoadMods(modRootDirPath);
    }

    public void InitializeMods(GenericServices services)
    {
        ILogger? BaseLogger = _services.Get<ILogger>();

        foreach (ModPackage Mod in _mods)
        {
            ModLogger? Logger = null;
            try
            {
                Logger = new(BaseLogger, Mod.Name);
                Logger.Initialize();

                Mod.EntryPointObject.Logger = Logger;
                Mod.EntryPointObject.OnStart(services);
            }
            catch (Exception e)
            {
                Logger?.Dispose();
                BaseLogger?.Error($"Unhandled exception in mod \"{Mod.Name}\" in OnGameLoad call: {e}");
            }
        }
    }

    public void DeinitializeMods(GenericServices services)
    {
        foreach (ModPackage Mod in _mods)
        {
            try
            {
                Mod.EntryPointObject.OnEnd(services);
            }
            catch (Exception e)
            {
                _services.Get<ILogger>()?.Error($"Unhandled exception in mod \"{Mod.Name}\" in OnGameClose call: {e}");
            }
        }
    }
}