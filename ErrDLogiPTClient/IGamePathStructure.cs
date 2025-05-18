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
    string LogRoot { get; }
    string LogArchiveRoot { get; }
    string LatestLogPath { get; }
}