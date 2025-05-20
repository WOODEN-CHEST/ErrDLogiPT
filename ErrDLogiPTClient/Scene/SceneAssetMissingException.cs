using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public class SceneAssetMissingException : Exception
{
    public SceneAssetMissingException() { }

    public SceneAssetMissingException(string? message) : base(message) { }
}