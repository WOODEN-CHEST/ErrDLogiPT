using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

public interface ILogiAssetManager
{
    // Methods.
    void LoadAssetDefinitions();
    void SetAsserRootPaths(string[] resourcePackDirNames);
}