using GHEngine.Assets.Def;
using GHEngine.IO;
using GHEngine.Logging;
using GHEngine.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

public interface IGameServices
{
    IFrameExecutor FrameExecutor { get; set; }
    ILogger? Logger { get; set; }
    IUserInput Input { get; set; }
    IDisplay Display { get; set; }
    IAssetDefinitionCollection Assets { get; set; }

}