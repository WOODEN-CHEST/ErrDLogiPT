using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Mod;

public class DefaultModPathStructure : IModPathStructure
{
    // Static fields.
    private const string META_FILE_NAME = "meta.json";
    private const string ICON_FILE_NAME = "icon.png";
    private const string ASSEMBLY_EXTENSION = ".dll";
    private const string DIR_NAME_ASSET = "asset";
    private const string DIR_NAME_ASSET_DEF = "def";


    // Inherited methods.
    public string Root { get; init; }
    public string MetaInfo { get; init; }
    public string[] Assemblies { get; init; }
    public string? Icon { get; init; }
    public string? AssetRoot { get; init; }
    public string? AssetDefRoot { get; init; }


    // Constructors.
    public DefaultModPathStructure(string modRoot)
    {
        ArgumentNullException.ThrowIfNull(modRoot, nameof(modRoot));
        if (!Path.IsPathFullyQualified(modRoot))
        {
            throw new ArgumentException("Mod structure root must be fully qualified", nameof(modRoot));
        }

        Root = modRoot;
        MetaInfo = Path.Combine(modRoot, META_FILE_NAME);

        Assemblies = Directory.GetFiles(modRoot, $"*{ASSEMBLY_EXTENSION}", SearchOption.TopDirectoryOnly);

        string IconPath = Path.Combine(modRoot, ICON_FILE_NAME);
        Icon = File.Exists(IconPath) ? IconPath : null;

        string AssetRootPath = Path.Combine(modRoot, DIR_NAME_ASSET);
        AssetRoot = Directory.Exists(AssetRootPath) ? AssetRootPath : null;

        string AssetDefRootPath = Path.Combine(AssetRootPath, DIR_NAME_ASSET_DEF);
        AssetDefRoot = Directory.Exists(AssetDefRootPath) ? AssetDefRootPath : null;
    }
}