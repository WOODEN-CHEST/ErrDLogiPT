using ErrDLogiPTClient.Mod;
using ErrDLogiPTClient.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Wrapper;

public class WrappedServiceModManager : ServiceWrapper<IModManager>, IModManager
{
    // Fields.
    public IEnumerable<ModPackage> Mods => ServiceObject.Mods;

    public int ModCount => ServiceObject.ModCount;


    // Constructors.
    public WrappedServiceModManager(IGenericServices services) : base(services) { }


    // Inherited methods.
    public void DeinitializeMods(IGenericServices services)
    {
        ServiceObject.InitializeMods(services);
    }

    public void InitializeMods(IGenericServices services)
    {
        ServiceObject.DeinitializeMods(services);
    }

    public void LoadMods(string modRootDirPath)
    {
        ServiceObject.LoadMods(modRootDirPath);
    }
}