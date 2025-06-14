﻿using ErrDLogiPTClient.Scene.Sound;
using GHEngine;
using GHEngine.Audio.Source;
using GHEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Button;

public interface IBasicButton : IUIElement
{
    // Fields.
    bool IsDisabledOnClick { get; set; }
    Vector2 Position { get; set; }
    Color ButtonColor { get; set; }
    Color HoverColor { get; set; }
    Color ClickColor { get; set; }
    TimeSpan HoverFadeDuration { get; set; }
    TimeSpan ClickFadeDuration { get; set; }
    string Text { get; set; }
    Vector2 TextPadding { get; set; }
    bool IsTextShadowEnabled { get; set; }
    float TextShadowBrightness { get; set; }
    Vector2 ShadowOffset { get; set; }
    float Length { get; set; }
    float Scale { get; set; }
    bool IsTargeted { get; set; }
    float Volume { get; set; }
    IEnumerable<IPreSampledSound> ClickSounds { get; set; }
    IEnumerable<IPreSampledSound> HoverStartSounds { get; set; }
    IEnumerable<IPreSampledSound> HoverEndSounds { get; set; }
    LogiSoundCategory SoundCategory { get; set; }
    ElementClickMethod ClickMethod { get; set; }
    Action<BasicButtonMainClickArgs>? MainClickAction { get; set; }
    Action<BasicButtonMainHoverStartArgs>? MainHoverStartAction { get; set; }
    Action<BasicButtonMainHoverEndArgs>? MainHoverEndAction { get; set; }
    RectangleF ButtonBounds { get; }
    IEnumerable<UIElementClickType> DetectedClickTypes { get; set; }

    event EventHandler<BasicButtonClickStartEventArgs>? ClickStart;
    event EventHandler<BasicButtonClickEndEventArgs>? ClickEnd;
    event EventHandler<BasicButtonHoverStartEventArgs>? HoverStart;
    event EventHandler<BasicButtonHoverEndEventArgs>? HoverEnd;
    event EventHandler<BasicButtonPlaySoundEventArgs>? PlaySound;
    event EventHandler<BasicButtonScrollEventArgs>? Scroll;


    // Methods.
    bool IsPositionOverButton(Vector2 position);
}