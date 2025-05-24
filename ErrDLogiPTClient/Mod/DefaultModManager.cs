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
    private readonly ILogger _logger;
    private ModPackage[] _mods;


    // Constructors
    public DefaultModManager(ILogger? logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    // Inherited methods.
    public void LoadMods(string modRootDirPath)
    {
        IModLoader Loader = new DefaultModLoader(_logger);
        _mods = Loader.LoadMods(modRootDirPath);
    }

    public void InitializeMods(GameServices services)
    {
        foreach (ModPackage Mod in _mods)
        {
            ModLogger? Logger = null;
            try
            {
                Logger = new(services.Logger, Mod.Name);
                Logger.Initialize();

                Mod.EntryPointObject.Logger = Logger;
                Mod.EntryPointObject.OnStart(services);
            }
            catch (Exception e)
            {
                Logger?.Dispose();
                services.Logger?.Error($"Unhandled exception in mod \"{Mod.Name}\" in OnGameLoad call: {e}");
            }
        }
    }

    public void DeinitializeMods(GameServices services)
    {
        foreach (ModPackage Mod in _mods)
        {
            try
            {
                Mod.EntryPointObject.OnEnd(services);
            }
            catch (Exception e)
            {
                services.Logger?.Error($"Unhandled exception in mod \"{Mod.Name}\" in OnGameClose call: {e}");
            }
        }
    }
}