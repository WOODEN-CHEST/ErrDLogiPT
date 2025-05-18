using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

public interface IGamePathStructure
{
    // Fields.
    string ExecutableRoot { get; }
    string UserDataRoot { get; }
    string LogRoot { get; }
    string LogArchiveRoot { get; }
    string LatestLogPath { get; }
    string AssetRoot { get; }
    string AssetDefRoot { get; }
    string AssetValueRoot { get; }
    string ModRoot { get; }
}