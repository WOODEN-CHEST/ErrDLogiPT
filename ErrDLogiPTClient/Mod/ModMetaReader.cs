using GHEngine.IO.JSON;
using GHEngine.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Mod;

public class ModMetaReader
{
    // Private static fields.
    private const string KEY_NAME = "name";
    private const string KEY_DESCRIPTION = "description";

    // Private fields.
    private readonly ILogger? _logger;

    
    // Constructors.
    public ModMetaReader(ILogger? logger)
    {
        _logger = logger;
    }

    // Methods.
    public ModMetaInfo? ReadMetaInfo(Stream metaInfoStream)
    {
        try
        {
            using StreamReader reader = new StreamReader(metaInfoStream, Encoding.UTF8, leaveOpen: true);
            object? RootJSON = new JSONDeserializer().Deserialize(reader.ReadToEnd());
            return DeconstructJSONMeta(RootJSON);
        }
        catch (JSONDeserializeException e)
        {
            _logger?.Error($"Failed to deserialize mod meta info JSON: {e.Message}");
        }
        catch (JSONSchemaException e)
        {
            _logger?.Error($"Invalid mod JSON meta info data: {e.Message}");
        }
        catch (IOException e)
        {
            _logger?.Error($"IOException while trying to read a mod's JSON meta info: {e}");
        }
        return null;
    }


    // Private methods.
    private ModMetaInfo DeconstructJSONMeta(object? jsonRoot)
    {
        if (jsonRoot is not JSONCompound Compound)
        {
            throw new JSONSchemaException("Root JSON object in mod meta info is not a compound object.");
        }

        string Name = Compound.GetVerified<string>(KEY_NAME);
        string Description = Compound.GetVerified<string>(KEY_DESCRIPTION);

        return new ModMetaInfo()
        {
            Name = Name,
            Description = Description
        };
    }
}