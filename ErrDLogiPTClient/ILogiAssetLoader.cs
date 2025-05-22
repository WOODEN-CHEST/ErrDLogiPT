using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

public interface ILogiAssetLoader
{
    // Methods.
    void LoadAssetDefinitions();
    void SetAssetRootPaths(string[] resourcePackDirNames);
}