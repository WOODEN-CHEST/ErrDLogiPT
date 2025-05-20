using GHEngine.Assets.Def;
using GHEngine.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Mod;

public class DefaultModLoader : IModLoader
{
    // Private fields.
    private readonly ILogger? _logger;
    private readonly ModMetaReader _metaReader;


    // Constructors.
    public DefaultModLoader(ILogger? logger)
    {
        _logger = logger;
        _metaReader = new(logger);
    }


    // Private methods.
    private ModPackage? LoadSingleMod(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                return LoadModFromDirectory(path);
            }
            else
            {
                _logger?.Warning("Unknown file \"path\" in mod directory. " +
                    "Expected directory for mod.");
            }
        }
        catch (Exception e)
        {
            _logger?.Error($"Failed to load mod at path \"{path}\": {e.Message}");
        }
        return null;
    }

    private ModPackage? LoadModFromDirectory(string dirPath)
    {
        IModPathStructure ModPathStructure = new DefaultModPathStructure(dirPath);

        if (!File.Exists(ModPathStructure.MetaInfo))
        {
            throw new ModLoadException($"No meta info \"{ModPathStructure.MetaInfo}\" file found in mod directory.");
        }
        ModMetaInfo? MetaInfo = _metaReader.ReadMetaInfo(File.OpenRead(ModPathStructure.MetaInfo))
            ?? throw new ModLoadException($"Failed to read mod meta info from directory.");

        List<Assembly> LoadedAssemblies = new();
        foreach (string AssemblyFile in ModPathStructure.Assemblies)
        {
            LoadedAssemblies.Add(AssemblyLoadContext.Default.LoadFromAssemblyPath(AssemblyFile));
        }

        IGameMod EntryPointObject = CreateModEntryPointObject(LoadedAssemblies, LoadedAssemblies.Count, MetaInfo);
        IAssetDefinitionCollection AssetDefinitions = GetModAssetDefinitions(ModPathStructure);
        return new(ModPathStructure, MetaInfo.Name, MetaInfo.Description, EntryPointObject, AssetDefinitions);
    }

    private IAssetDefinitionCollection GetModAssetDefinitions(IModPathStructure structure)
    {
        IAllAssetDefinitionConverter Reader = new JSONAllAssetDefinitionConverter(_logger);
        IAssetDefinitionCollection Definitions = new GHAssetDefinitionCollection();

        if (structure.AssetDefRoot != null)
        {
            Reader.Read(Definitions, structure.AssetDefRoot);
        }

        return Definitions;
    }

    private IGameMod CreateModEntryPointObject(IEnumerable<Assembly> modAssemblies, 
        int assemblyCount, 
        ModMetaInfo metaInfo)
    {
        if (assemblyCount == 0)
        {
            throw new ModLoadException($"No mod assemblies found in mod.");
        }

        Assembly[] AssembliesWithEntryPoint = modAssemblies
            .Where(assembly => assembly.GetType(metaInfo.EntryPoint) != null)
            .ToArray();

        if (AssembliesWithEntryPoint.Length > 1)
        {
            throw new ModLoadException($"Multiple assemblies with entry point \"{metaInfo.EntryPoint}\" found, " +
                $"expected only one.");
        }
        if (AssembliesWithEntryPoint.Length < 0)
        {
            throw new ModLoadException($"No assemblies with entry point \"{metaInfo.EntryPoint}\" found.");
        }

        Type ModType = AssembliesWithEntryPoint[0].GetType(metaInfo.EntryPoint)!;

        if (!ModType.IsAssignableFrom(typeof(IGameMod)))
        {
            throw new ModLoadException($"Entry point \"{metaInfo.EntryPoint}\" does not implement IGameMod interface.");
        }

        ConstructorInfo? EmptyConstructor = ModType.GetConstructor(Array.Empty<Type>());

        if (EmptyConstructor == null)
        {
            throw new ModLoadException($"Entry point \"{metaInfo.EntryPoint}\" does not have a default constructor.");
        }
        return (IGameMod)EmptyConstructor.Invoke(null);
    }

    // Inherited methods.
    public ModPackage[] LoadMods(string modsRootDirPath)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(modsRootDirPath, nameof(modsRootDirPath));

        if (!Directory.Exists(modsRootDirPath))
        {
            return Array.Empty<ModPackage>();
        }

        try
        {
            List<ModPackage> Mods = new();
            foreach (string Entry in Directory.GetFileSystemEntries(modsRootDirPath))
            {
                ModPackage? Mod = LoadSingleMod(Entry);
                if (Mod != null)
                {
                    Mods.Add(Mod);
                }
            }
            return Mods.ToArray();
        }
        catch (Exception e)
        {
            _logger?.Error($"Failed to load mods: {e}");
        }
        return Array.Empty<ModPackage>();
    }
}