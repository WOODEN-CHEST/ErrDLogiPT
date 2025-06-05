using GHEngine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

public class ElementColorCalculator : ITimeUpdatable
{
    // Fields.
    public Color NormalColor { get; set; } = Color.White;
    public Color HoverColor { get; set; } = Color.White;
    public Color ClickColor { get; set; } = Color.White;
    public Color FinalColor { get; private set; } = Color.White;
    public TimeSpan HoverFadeDuration
    {
        get => _hoverFadeDuration;
        set
        {
            if (value.Ticks < 0)
            {
                throw new ArgumentException("Hover fade duration must be >= 0", nameof(value));
            }
            _hoverFadeDuration = value;
        }
    }
    public TimeSpan ClickFadeDuration
    {
        get => _clickFadeDuration;
        set
        {
            if (value.Ticks < 0)
            {
                throw new ArgumentException("Click fade duration must be >= 0", nameof(value));
            }
            _clickFadeDuration = value;
        }
    }


    // Private static fields.
    private const float FADE_FACTOR_MIN = 0f;
    private const float FADE_FACTOR_MAX = 1f;


    // Private fields.
    private bool _wasClickStarted = false;
    private bool _isHovering = false;
    private TimeSpan _hoverFadeDuration = TimeSpan.Zero;
    private TimeSpan _clickFadeDuration = TimeSpan.Zero;
    private float _hoverFadeFactor = FADE_FACTOR_MIN;
    private float _clickFadeFactor = FADE_FACTOR_MIN;

    // Methods.
    public void OnHoverStart()
    {
        _isHovering = true;
    }

    public void OnHoverEnd()
    {
        _isHovering = false;
    }

    public void OnClickStart()
    {
        _wasClickStarted = true;
    }

    public void OnClickEnd()
    {
        _clickFadeFactor = _isHovering ? FADE_FACTOR_MAX : _clickFadeFactor;
        _wasClickStarted = false;
    }


    // Private methods.
    private void UpdateColorFadeFactors(IProgramTime time)
    {
        /* Snapping to max or min fade factors if the timespan is short is so that there is no division by zero. */
        if (_isHovering && _wasClickStarted)
        {
            _clickFadeFactor = FADE_FACTOR_MAX;
        }
        else if (_clickFadeDuration < time.PassedTime)
        {
            _clickFadeFactor = 0f;
        }
        else
        {
            _clickFadeFactor = Math.Clamp(
                _clickFadeFactor - (float)(time.PassedTime.TotalSeconds / ClickFadeDuration.TotalSeconds),
                FADE_FACTOR_MIN,
                FADE_FACTOR_MAX);
        }

        if (_hoverFadeDuration < time.PassedTime)
        {
            _hoverFadeFactor = _isHovering ? FADE_FACTOR_MAX : FADE_FACTOR_MIN;
        }
        else
        {
            float Step = _isHovering ? 1f : -1f;
            _hoverFadeFactor = Math.Clamp(
                _hoverFadeFactor + (Step * ((float)(time.PassedTime.TotalSeconds / HoverFadeDuration.TotalSeconds))),
                FADE_FACTOR_MIN,
                FADE_FACTOR_MAX);
        }
    }

    private void UpdateFinalColor()
    {
        FloatColor ColorStage1 = FloatColor.InterpolateRGB(NormalColor, HoverColor, _hoverFadeFactor);
        FloatColor ColorStage2 = FloatColor.InterpolateRGB(ColorStage1, ClickColor, _clickFadeFactor);
        FinalColor = ColorStage2;
    }


    // Inherited methods.
    public void Update(IProgramTime time)
    {
        UpdateColorFadeFactors(time);
        UpdateFinalColor();
    }
}