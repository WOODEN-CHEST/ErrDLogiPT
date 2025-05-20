using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Mod;

public interface IModPathStructure
{
    string Root { get; }
    string MetaInfo { get; }
    string[] Assemblies { get; }
    string? Icon { get; }
    string? AssetRoot { get; }
    string? AssetDefRoot { get; }
}