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
    // Private static fields.
    private const string ARCHIVE_EXTENSION = ".zip";
    private const string META_FILE_NAME = "meta.json";
    private const string ASSEMBLY_EXTENSION = ".dll";


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
    private IGameMod? LoadSingleMod(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                return LoadModFromDirectory(path);
            }
            else if (File.Exists(path) && (Path.GetExtension(path) == ARCHIVE_EXTENSION))
            {
                return LoadModFromArchive(path);
            }
            else
            {
                _logger?.Warning("Unknown file \"path\" in mod directory. " +
                    "Expected directory or zip archive.");
            }
        }
        catch (Exception e)
        {
            _logger?.Error($"Failed to load mod at path \"{path}\": {e.Message}");
        }
        return null;
    }

    private IGameMod? LoadModFromDirectory(string dirPath)
    {
        string MetaInfoPath = Path.Combine(dirPath, META_FILE_NAME);
        if (!File.Exists(MetaInfoPath))
        {
            throw new ModLoadException($"No meta info \"{META_FILE_NAME}\" file found in mod directory.");
        }
        ModMetaInfo? MetaInfo = _metaReader.ReadMetaInfo(File.OpenRead(MetaInfoPath))
            ?? throw new ModLoadException($"Failed to read mod meta info from directory.");

        List<Assembly> LoadedAssemblies = new();
        foreach (string AssemblyFile in Directory.GetFiles(dirPath, $"*{ASSEMBLY_EXTENSION}", SearchOption.TopDirectoryOnly))
        {
            LoadedAssemblies.Add(AssemblyLoadContext.Default.LoadFromAssemblyPath(AssemblyFile));
        }

        return CreateModFromAssembly(LoadedAssemblies, LoadedAssemblies.Count, MetaInfo.EntryPoint);
    }

    private IGameMod? LoadModFromArchive(string archivePath)
    {
        using ZipArchive Archive = new(File.OpenRead(archivePath));

        using Stream MetaInfoStream = Archive.GetEntry(META_FILE_NAME)?.Open()
            ?? throw new ModLoadException($"No meta info \"{META_FILE_NAME}\" file found in archive");
        ModMetaInfo? MetaInfo = _metaReader.ReadMetaInfo(MetaInfoStream);
        if (MetaInfo == null)
        {
            throw new ModLoadException($"Failed to read mod meta info from archive.");
        }

        List<Assembly> LoadedAssemblies = new();
        foreach (ZipArchiveEntry Entry in Archive.Entries.Where(entry => entry.Name.EndsWith(ASSEMBLY_EXTENSION)))
        {
            using Stream AssemblyStream = Entry.Open();
            LoadedAssemblies.Add(AssemblyLoadContext.Default.LoadFromStream(AssemblyStream));
        }

        return CreateModFromAssembly(LoadedAssemblies, LoadedAssemblies.Count, MetaInfo.EntryPoint);
    }

    private IGameMod CreateModFromAssembly(IEnumerable<Assembly> modAssemblies, int assemblyCount, string entryPoint)
    {
        if (assemblyCount == 0)
        {
            throw new ModLoadException($"No mod assemblies found in mod.");
        }

        Assembly[] AssembliesWithEntryPoint = modAssemblies
            .Where(assembly => assembly.GetType(entryPoint) != null)
            .ToArray();

        if (AssembliesWithEntryPoint.Length > 1)
        {
            throw new ModLoadException($"Multiple assemblies with entry point \"{entryPoint}\" found, " +
                $"expected only one.");
        }
        if (AssembliesWithEntryPoint.Length < 0)
        {
            throw new ModLoadException($"No assemblies with entry point \"{entryPoint}\" found.");
        }

        Type ModType = AssembliesWithEntryPoint[0].GetType(entryPoint)!;

        if (!ModType.IsAssignableFrom(typeof(IGameMod)))
        {
            throw new ModLoadException($"Entry point \"{entryPoint}\" does not implement IGameMod interface.");
        }

        ConstructorInfo? EmptyConstructor = ModType.GetConstructor(Array.Empty<Type>());

        if (EmptyConstructor == null)
        {
            throw new ModLoadException($"Entry point \"{entryPoint}\" does not have a default constructor.");
        }
        return (IGameMod)EmptyConstructor.Invoke(null);
    }

    // Inherited methods.
    public IGameMod[] LoadMods(string modsRootDirPath)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(modsRootDirPath, nameof(modsRootDirPath));

        if (!Directory.Exists(modsRootDirPath))
        {
            return Array.Empty<IGameMod>();
        }

        try
        {
            List<IGameMod> Mods = new();
            foreach (string Entry in Directory.GetFileSystemEntries(modsRootDirPath))
            {
                IGameMod? Mod = LoadSingleMod(Entry);
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
        return Array.Empty<IGameMod>();
    }
}