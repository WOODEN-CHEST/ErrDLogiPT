using GHEngine.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

public class LogInitializer
{
    // Methods.
    public ILogger InitializeLogger(IGamePathStructure structure)
    {
        ArgumentNullException.ThrowIfNull(structure, nameof(structure));

        Directory.CreateDirectory(structure.LogRoot);

// To not clutter the log archive directory while debugging.
#if !DEBUG 
        if (File.Exists(structure.LatestLogPath))
        {
            ILogArchiver Archiver = new GHLogArchiver();
            Directory.CreateDirectory(structure.LogArchiveRoot);
            Archiver.Archive(structure.LogArchiveRoot, structure.LatestLogPath);
        }
#endif
        ILogger Logger = new GHLogger(structure.LatestLogPath);
        return Logger;
    }
}