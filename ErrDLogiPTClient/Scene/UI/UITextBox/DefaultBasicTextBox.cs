using GHEngine;
using GHEngine.Frame;
using GHEngine.Frame.Animation;
using GHEngine.Frame.Item;
using GHEngine.GameFont;
using GHEngine.IO;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.UITextBox;

public class DefaultBasicTextBox : IBasicTextBox
{
    // Fields.
    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            _previousRenderAspectRatio = null;
            UpdateBounds();
        }
    }

    public bool IsTypingEnabled
    {
        get => _isTypingEnabled;
        set
        {
            _isTypingEnabled = value;
            UpdateWrappedTextBox();
        }
    }

    public float Scale
    {
        get => _scale;
        set
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentException($"Invalid scale: {value}", nameof(value));
            }
            if (value < 0f)
            {
                throw new ArgumentException($"Scale must be >= 0, got {value}", nameof(value));
            }
            _scale = value;
            UpdateSizes();
            UpdateBounds();
            _previousRenderAspectRatio = null;
        }
    }

    public Vector2 Dimensions
    {
        get => _dimensions;
        set
        {
            _dimensions = value;
            UpdateSizes();
            UpdateBounds();
            _previousRenderAspectRatio = null;
        }
    }
    
    public RectangleF Bounds { get; private set; }

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            UpdateWrappedTextBox();
        }
    }

    public bool IsVisible { get; set; } = true;
    public int ComponentCount => _wrappedTextBox.ComponentCount;

    public IEnumerable<TextComponent> Components => _wrappedTextBox.Components;

    public Color GlobalTextColor
    {
        get => _wrappedTextBox.Mask;
        set => _wrappedTextBox.Mask = value;
    }

    public float GlobalTextBrightness
    {
        get => _wrappedTextBox.Brightness;
        set => _wrappedTextBox.Brightness = value;
    }

    public float GlobalTextOpacity
    {
        get => _wrappedTextBox.Opacity;
        set => _wrappedTextBox.Opacity = value;
    }

    public Color BoxColor
    {
        get => _boxColor;
        set
        {
            _boxColor = value;

            _topLeftCorner.Mask = value;
            _topBar.Mask = value;
            _topRightCorner.Mask = value;
            _rightBar.Mask = value;
            _bottomRightCorner.Mask = value;
            _bottomBar.Mask = value;
            _bottomLeftCorner.Mask = value;
            _leftBar.Mask = value;
            _center.Mask = value;
        }
    }

    public float ScrollFactor
    {
        get => _scrollFactor;
        set
        {
            if (float.IsNaN(value))
            {
                throw new ArgumentException($"Invalid scroll factor: {value}", nameof(value));
            }
            _scrollFactor = Math.Clamp(value, ScrollFactorMin, ScrollFactorMax);
            UpdateWrappedTextBox();
        }
    }

    public float ScrollFactorMin => 0f;

    public float ScrollFactorMax => 1f;


    // Private static fields.
    private const int FRAME_INDEX_TOP_LEFT_CORNER = 0;
    private const int FRAME_INDEX_TOP_BAR = 1;
    private const int FRAME_INDEX_TOP_RIGHT_CORNER = 2;
    private const int FRAME_INDEX_RIGHT_BAR = 3;
    private const int FRAME_INDEX_BOTTOM_RIGHT_CORNER = 4;
    private const int FRAME_INDEX_BOTTOM_BAR = 5;
    private const int FRAME_INDEX_BOTTOM_LEFT_CORNER = 6;
    private const int FRAME_INDEX_LEFT_BAR = 7;
    private const int FRAME_INDEX_CENTER = 8;


    // Private fields.
    private readonly WritableTextBox _wrappedTextBox;
    private readonly ISceneAssetProvider _assetProvider;
    private readonly IUserInput _input;

    private readonly SpriteItem _topLeftCorner;
    private readonly SpriteItem _topBar;
    private readonly SpriteItem _topRightCorner;
    private readonly SpriteItem _rightBar;
    private readonly SpriteItem _bottomRightCorner;
    private readonly SpriteItem _bottomBar;
    private readonly SpriteItem _bottomLeftCorner;
    private readonly SpriteItem _leftBar;
    private readonly SpriteItem _center;

    private Vector2 _position = Vector2.Zero;
    private float _scale = 1.0f;
    private Vector2 _dimensions = Vector2.One;
    private Color _boxColor = Color.White;
    private bool _isEnabled = true;

    private bool _isTypingEnabled = false;
    private float _scrollFactor = 0f;

    private float? _previousRenderAspectRatio = null;


    // Constructors.
    public DefaultBasicTextBox(IUserInput input,
        ISceneAssetProvider assetProvider,
        ISpriteAnimation boxAnimation)
    {
        ArgumentNullException.ThrowIfNull(input, nameof(input));
        ArgumentNullException.ThrowIfNull(assetProvider, nameof(assetProvider));
        ArgumentNullException.ThrowIfNull(boxAnimation, nameof(boxAnimation));

        _input = input;
        _assetProvider = assetProvider;

        _topLeftCorner = new(boxAnimation.CreateInstance());
        _topBar = new(boxAnimation.CreateInstance());
        _topRightCorner = new(boxAnimation.CreateInstance());
        _rightBar = new(boxAnimation.CreateInstance());
        _bottomRightCorner = new(boxAnimation.CreateInstance());
        _bottomBar = new(boxAnimation.CreateInstance());
        _bottomLeftCorner = new(boxAnimation.CreateInstance());
        _leftBar = new(boxAnimation.CreateInstance());
        _center = new(boxAnimation.CreateInstance());

        _topLeftCorner.Animation.FrameIndex = FRAME_INDEX_TOP_LEFT_CORNER;
        _topBar.Animation.FrameIndex = FRAME_INDEX_TOP_BAR;
        _topRightCorner.Animation.FrameIndex = FRAME_INDEX_TOP_RIGHT_CORNER;
        _rightBar.Animation.FrameIndex = FRAME_INDEX_RIGHT_BAR;
        _bottomRightCorner.Animation.FrameIndex = FRAME_INDEX_BOTTOM_RIGHT_CORNER;
        _bottomBar.Animation.FrameIndex = FRAME_INDEX_BOTTOM_BAR;
        _bottomLeftCorner.Animation.FrameIndex = FRAME_INDEX_BOTTOM_LEFT_CORNER;
        _leftBar.Animation.FrameIndex = FRAME_INDEX_LEFT_BAR;
        _center.Animation.FrameIndex = FRAME_INDEX_CENTER;

        _topLeftCorner.Origin = new(1f, 1f);
        _topBar.Origin = new(0.5f, 1f);
        _topRightCorner.Origin = new(0f, 1f);
        _rightBar.Origin = new(0f, 0.5f);
        _bottomRightCorner.Origin = new(0f, 0f);
        _bottomBar.Origin = new(0.5f, 0f);
        _bottomLeftCorner.Origin = new(1f, 0f);
        _leftBar.Origin = new(1f, 0.5f);
        _center.Origin = new(0.5f, 0.5f);

        _wrappedTextBox = new(input);
        _wrappedTextBox.FitMethod = TextFitMethod.Resize;
        _wrappedTextBox.Origin = new(0.0f, 0.0f);
        _wrappedTextBox.IsNewlineAllowed = true;

        UpdateSizes();
        UpdateBounds();
        UpdateWrappedTextBox();
    }


    // Private methods.
    private void UpdatePositions(float aspectRatio)
    {
        _center.Position = Position;

        Vector2 HalfCenterSize = GHMath.GetWindowAdjustedVector(_center.Size / 2f, aspectRatio);

        _topLeftCorner.Position = Position + new Vector2(-HalfCenterSize.X, -HalfCenterSize.Y);
        _topBar.Position = Position + new Vector2(0f, -HalfCenterSize.Y);
        _topRightCorner.Position = Position + new Vector2(HalfCenterSize.X, -HalfCenterSize.Y);
        _rightBar.Position = Position + new Vector2(HalfCenterSize.X, 0f);
        _bottomRightCorner.Position = Position + new Vector2(HalfCenterSize.X, HalfCenterSize.Y);
        _bottomBar.Position = Position + new Vector2(0f, HalfCenterSize.Y);
        _bottomLeftCorner.Position = Position + new Vector2(-HalfCenterSize.X, HalfCenterSize.Y);
        _leftBar.Position = Position + new Vector2(-HalfCenterSize.X, 0f);

        _previousRenderAspectRatio = aspectRatio;
        UpdateWrappedTextBox();
    }

    private void UpdateSizes()
    {
        _center.Size = new Vector2(1f, 1f) * _scale * _dimensions;

        SpriteItem BorderReference = _topLeftCorner;
        Vector2 BorderSize = new Vector2(
            BorderReference.FrameSize.X / BorderReference.FrameSize.X, 1f)
            * (BorderReference.FrameSize.Y / _center.FrameSize.Y) * _scale;

        _topLeftCorner.Size = BorderSize;
        _topRightCorner.Size = BorderSize;
        _bottomRightCorner.Size = BorderSize;
        _bottomLeftCorner.Size = BorderSize;

        _topBar.Size = new(_center.Size.X, BorderSize.Y);
        _bottomBar.Size = new(_center.Size.X, BorderSize.Y);
        _leftBar.Size = new(BorderSize.X, _center.Size.Y);
        _rightBar.Size = new(BorderSize.X, _center.Size.Y);

        _wrappedTextBox.MaxSize = new(_center.Size.X, float.PositiveInfinity);
        UpdateWrappedTextBox();
    }

    private void UpdateBounds()
    {
        float AspectRatio = _input.InputAreaRatio;

        Vector2 HalfSize = GHMath.GetWindowAdjustedVector(_center.Size + _topLeftCorner.Size, AspectRatio);

        Vector2 TopLeft = _position - HalfSize;
        Vector2 BottomRight = _position + HalfSize;
        Vector2 Dimensions = BottomRight - TopLeft;

        Bounds = new(TopLeft.X, TopLeft.Y, Dimensions.X, Dimensions.Y);
    }

    private void UpdateWrappedTextBox()
    {
        _wrappedTextBox.IsTypingEnabled = IsEnabled && IsTypingEnabled;
        if (!IsEnabled)
        {
            _wrappedTextBox.IsFocused = false;
        }

        Vector2 DrawSize = _wrappedTextBox.DrawSize;
        float AbsoluteCutOffAmount = Math.Max(DrawSize.Y - _center.Size.Y, 0f);
        float RelativeCutHeight = AbsoluteCutOffAmount / DrawSize.Y;
        float RelativeDrawHeight = 1f - RelativeCutHeight;

        _wrappedTextBox.DrawBounds = new(
            0f,
            _scrollFactor * RelativeCutHeight,
            float.PositiveInfinity,
            RelativeDrawHeight);

        Vector2 HalfCenterSize = GHMath.GetWindowAdjustedVector(_center.Size / 2f, _input.InputAreaRatio);
        _wrappedTextBox.Position = new(
            _position.X - HalfCenterSize.X,
            _position.Y - (_scrollFactor * RelativeCutHeight * DrawSize.Y) - HalfCenterSize.Y);
    }

    private void TryScrollText()
    {
        if (!IsPositionOverArea(_input.VirtualMousePositionCurrent))
        {
            return;
        }

        Vector2 DrawSize = _wrappedTextBox.DrawSize;
        if ((DrawSize.X <= 0f) || (DrawSize.Y <= 0f))
        {
            return;
        }

        const float STEP_SIZE = 0.2f;
        float Step = STEP_SIZE * _scale / DrawSize.Y;
         
        if (_input.MouseScrollChangeAmount > 0)
        {
            ScrollFactor -= Step;
        }
        else if (_input.MouseScrollChangeAmount < 0)
        {
            ScrollFactor += Step;
        }
    }


    // Inherited methods.
    public void Initialize()
    {
        _assetProvider.RegisterRenderedItem(_topLeftCorner);
        _assetProvider.RegisterRenderedItem(_topBar);
        _assetProvider.RegisterRenderedItem(_topRightCorner);
        _assetProvider.RegisterRenderedItem(_rightBar);
        _assetProvider.RegisterRenderedItem(_bottomRightCorner);
        _assetProvider.RegisterRenderedItem(_bottomBar);
        _assetProvider.RegisterRenderedItem(_bottomLeftCorner);
        _assetProvider.RegisterRenderedItem(_leftBar);
        _assetProvider.RegisterRenderedItem(_center);
        _assetProvider.RegisterRenderedItem(_wrappedTextBox);
    }

    public void Deinitialize()
    {
        _assetProvider.UnregisterRenderedItem(_topLeftCorner);
        _assetProvider.UnregisterRenderedItem(_topBar);
        _assetProvider.UnregisterRenderedItem(_topRightCorner);
        _assetProvider.UnregisterRenderedItem(_rightBar);
        _assetProvider.UnregisterRenderedItem(_bottomRightCorner);
        _assetProvider.UnregisterRenderedItem(_bottomBar);
        _assetProvider.UnregisterRenderedItem(_bottomLeftCorner);
        _assetProvider.UnregisterRenderedItem(_leftBar);
        _assetProvider.UnregisterRenderedItem(_center);
        _assetProvider.UnregisterRenderedItem(_wrappedTextBox);
    }

    public void Render(IRenderer renderer, IProgramTime time)
    {
        if (!IsVisible)
        {
            return;
        }

        if ((_previousRenderAspectRatio == null) || (_previousRenderAspectRatio != renderer.AspectRatio))
        {
            UpdatePositions(renderer.AspectRatio);
        }

        _topLeftCorner.Render(renderer, time);
        _topBar.Render(renderer, time);
        _topRightCorner.Render(renderer, time);
        _rightBar.Render(renderer, time);
        _bottomRightCorner.Render(renderer, time);
        _bottomBar.Render(renderer, time);
        _bottomLeftCorner.Render(renderer, time);
        _leftBar.Render(renderer, time);
        _center.Render(renderer, time);

        _wrappedTextBox.Render(renderer, time);
    }

    public void Update(IProgramTime time)
    {
        if (IsTypingEnabled)
        {
            
        }

        TryScrollText();

        _wrappedTextBox.Update(time);
    }

    public void AddComponent(TextComponent component)
    {
        _wrappedTextBox.Add(component);
    }

    public void RemoveComponent(TextComponent component)
    {
        _wrappedTextBox.Remove(component);
    }

    public void InsertComponent(TextComponent component, int index)
    {
        _wrappedTextBox.Insert(component, index);
    }

    public void ClearComponents(TextComponent component)
    {
        _wrappedTextBox.Clear();
    }

    public void PrepareTexturesForRendering()
    {
        _wrappedTextBox.PrepareTexturesForRendering(Enumerable.Empty<char>());
    }

    public bool IsPositionOverArea(Vector2 position)
    {
        Vector2 Corner1 = new(Bounds.X, Bounds.Y);
        Vector2 Corner2 = Corner1 + new Vector2(Bounds.Width, Bounds.Height);

        return GHMath.IsPointInArea(position, Corner1, Corner2, 0f);
    }
}