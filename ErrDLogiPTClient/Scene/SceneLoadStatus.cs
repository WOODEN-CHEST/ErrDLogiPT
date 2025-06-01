using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

/// <summary>
/// Simple status to determine if the scene is loaded.
/// </summary>
public enum SceneLoadStatus
{
    /// <summary>
    /// The scene is currently not loaded and isn't loading.
    /// </summary>
    NotLoaded,

    /// <summary>
    /// The scene is currently being loaded, but hasn't finished yet.
    /// </summary>
    Loading,

    /// <summary>
    /// The scene has finished loading and is ready to be switched to.
    /// </summary>
    FinishedLoading
}