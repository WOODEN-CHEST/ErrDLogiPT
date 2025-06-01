using GHEngine;
using GHEngine.Frame.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

/// <summary>
/// Base interface for all non-OS UI elements.
/// </summary>
public interface IUIElement : IRenderableItem, ITimeUpdatable
{
    // Fields.

    /// <summary>
    /// Whether the element is enabled. An element which is enabled can be interacted with by the player.
    /// A disabled element should not allow the user to interact with it in any way.
    /// </summary>
    bool IsEnabled { get; set; }


    // Methods.

    /// <summary>
    /// Initializes the element.
    /// This could include things like subscribing to events, registering rendered items, preparing values and so on.
    /// <para>This must be called once before the element is used in a scene.</para>
    /// <para>A call to this method should be followed by a call to <c>Deinitialize()</c> at some point in the future.</para>
    /// </summary>
    void Initialize();

    /// <summary>
    /// Deinitializes the element. 
    /// This could include things like unsubscribing to events, unregistering rendered items and so on.
    /// <para>This should be called once after this element is no longer needed.</para>
    /// </summary>
    void Deinitialize();
}