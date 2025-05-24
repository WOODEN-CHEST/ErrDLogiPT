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

namespace ErrDLogiPTClient.Scene.MainMenu.UI;

public class MainMenuBasicButton : ITimeUpdatable, IRenderableItem
{
    // Fields.
    public bool IsVisible { get; set; }

    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            SyncPositions();
        }
    }

    public Color ButtonColor { get; set; }

    public string Text
    {
        get => _buttonText.Text;
        set
        {
            _buttonText.Components.First().Text = value;
        }
    }

    public float Length
    {
        get => _buttonLength;
        set
        {
            if (float.IsNaN(value))
            {
                throw new ArgumentException("Button length cannot be NaN");
            }
            if (float.IsInfinity(value))
            {
                throw new ArgumentException("Button length cannot be infinity");
            }
            if (value < 0f)
            {
                throw new ArgumentException("Button length cannot be < 0");
            }
            _buttonLength = value;
        }
    }


    // Private static fields.
    private const int FRAME_INDEX_LEFT_PART = 0;
    private const int FRAME_INDEX_MIDDLE_PART = 1;
    private const int FRAME_INDEX_RIGHT_PART = 2;


    // Private fields.
    private readonly IUserInput _input;
    private readonly ISceneAssetProvider _assetProvider;
    private readonly SpriteItem _leftPartSprite;
    private readonly SpriteItem _middlePartSprite;
    private readonly SpriteItem _rightPartSprite;
    private float _buttonLength;

    private Vector2 _position;

    private readonly TextBox _buttonText;



    // Constructors
    public MainMenuBasicButton(IUserInput input,
        ISceneAssetProvider? assetProvider,
        ISpriteAnimation animation,
        GHFontFamily font,
        float buttonLength)
    {
        ArgumentNullException.ThrowIfNull(input, nameof(input));
        ArgumentNullException.ThrowIfNull(animation, nameof(animation));
        ArgumentNullException.ThrowIfNull(font, nameof(font));
        _assetProvider = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));

        if (animation.FrameCount < 3)
        {
            throw new ArgumentException("basic main menu button animation must have at least 3 frames", nameof(animation));
        }

        _input = input;

        _leftPartSprite = new(animation.CreateInstance());
        _middlePartSprite = new(animation.CreateInstance());
        _rightPartSprite = new(animation.CreateInstance());

        _leftPartSprite.Animation.FrameIndex = FRAME_INDEX_LEFT_PART;
        _middlePartSprite.Animation.FrameIndex = FRAME_INDEX_MIDDLE_PART;
        _rightPartSprite.Animation.FrameIndex = FRAME_INDEX_RIGHT_PART;

        _buttonText = new TextBox()
        {
            new(font, string.Empty),
        };
        _buttonText.Origin = new(0.5f, 0.5f);
        _buttonText.FitMethod = TextFitMethod.Resize;

        Length =  buttonLength;
        SyncPositions();
    }


    // Methods.
    void Initialize()
    {
        _assetProvider.RegisterRenderedItem(_buttonText);
        _assetProvider.RegisterRenderedItem(_leftPartSprite);
        _assetProvider.RegisterRenderedItem(_middlePartSprite);
        _assetProvider.RegisterRenderedItem(_rightPartSprite);
    }

    void Deinitialize()
    {
        _assetProvider.UnregisterRenderedItem(_buttonText);
        _assetProvider.UnregisterRenderedItem(_leftPartSprite);
        _assetProvider.UnregisterRenderedItem(_middlePartSprite);
        _assetProvider.UnregisterRenderedItem(_rightPartSprite);
    }

    // Private methods.
    private void SyncPositions()
    {

    }



    // Inherited methods.
    public void Update(IProgramTime time)
    {
        
    }

    public void Render(IRenderer renderer, IProgramTime time)
    {
        
    }
}