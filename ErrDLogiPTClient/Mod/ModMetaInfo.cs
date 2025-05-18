using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Mod;

public record class ModMetaInfo
{
    // Static fields.
    public const string DEFAULT_NAME = "No name provided";
    public const string DEFAULT_DESCRIPTION = "No description provided";
    public const string DEFAULT_ENTRY_POINT = "";


    // Fields.
    public required string Name { get; init; } = DEFAULT_NAME;
    public required string Description { get; init; } = DEFAULT_DESCRIPTION;
    public required string EntryPoint { get; init; } = DEFAULT_ENTRY_POINT;
}