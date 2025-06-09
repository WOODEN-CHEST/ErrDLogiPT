using GHEngine;
using GHEngine.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

public class ClickDetector : ITimeUpdatable
{
    // Fields.
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            if (!value)
            {
                IsTargeted = false;
            }
        }
    }

    public RectangleF ElementBounds
    {
        get => _bounds;
        set
        {
            _bounds = value;
            UpdateIsTargeted();
        }
    }

    public ElementClickMethod ClickMethod { get; set; } = ElementClickMethod.ActivateOnFullClick;

    public bool IsTargeted
    {
        get => _isTargeted;
        set => _isTargeted = value;
    }


    public event EventHandler<ClickDetectorClickStartEventArgs>? ClickStart;
    public event EventHandler<ClickDetectorClickEndEventArgs>? ClickEnd;
    public event EventHandler<ClickDetectorScrollEventArgs>? Scroll;
    public event EventHandler<ClickDetectorHoverStartEventArgs>? HoverStart;
    public event EventHandler<ClickDetectorHoverEndEventArgs>? HoverEnd;


    // Private fields.
    private readonly IUserInput _input;

    private TimeSpan _clickDuration = TimeSpan.Zero;
    private Vector2 _clickStartPosition = Vector2.Zero;
    private UIElementClickType _currentClickType = UIElementClickType.None;

    private DeltaValue<bool> _isHovered = new(false);
    private bool _isTargeted = false;

    private bool _isEnabled = true;

    private RectangleF _bounds = new(0f, 0f, 1f, 1f);


    // Constructors.
    public ClickDetector(IUserInput input)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
    }


    // Private methods.
    private void UpdateIsTargeted()
    {
        _isTargeted = IsEnabled && IsPositionOverClickArea(_input.VirtualMousePositionCurrent);
    }

    private void OnButtonClick(UIElementClickType type)
    {
        TimeSpan ClickDuration;
        Vector2 ClickStartLocation;
        Vector2 CurrentClickLocation = _input.VirtualMousePositionCurrent;

        if (ClickMethod == ElementClickMethod.ActivateOnFullClick)
        {
            ClickDuration = _clickDuration;
            ClickStartLocation = _clickStartPosition;
        }
        else
        {
            ClickDuration = TimeSpan.Zero;
            ClickStartLocation = CurrentClickLocation;
        }

        ResetClickData();

        ClickEnd?.Invoke(this, new(this, ClickStartLocation, CurrentClickLocation, type, ClickDuration, _isTargeted));
    }

    private UIElementClickType TestSimpleClick(Func<MouseButton[], bool> mousePredicate, Func<Keys[], bool> keyPredicate)
    {
        if (mousePredicate.Invoke(new MouseButton[] { MouseButton.Left } ))
        {
            return UIElementClickType.Left;
        }
        if (mousePredicate.Invoke(new MouseButton[] { MouseButton.Middle }))
        {
            return UIElementClickType.Middle;
        }
        if (mousePredicate.Invoke(new MouseButton[] { MouseButton.Right }))
        {
            return UIElementClickType.Right;
        }
        if (keyPredicate.Invoke(new Keys[] { Keys.Enter }))
        {
            return UIElementClickType.Enter;
        }
        return UIElementClickType.None;
    }

    private UIElementClickType TestMethodOnClick()
    {
        if (!_isTargeted)
        {
            return UIElementClickType.None;
        }
        return TestSimpleClick(_input.WereMouseButtonsJustPressed, _input.WereKeysJustPressed); 
    }

    private UIElementClickType TestMethodOnRelease()
    {
        if (!_isTargeted)
        {
            return UIElementClickType.None;
        }
        return TestSimpleClick(_input.WereMouseButtonsJustReleased, _input.WereKeysJustReleased);
    }

    private bool IsButtonDown(UIElementClickType type)
    {
        return type switch
        {
            UIElementClickType.Left => _input.AreMouseButtonsDown(MouseButton.Left),
            UIElementClickType.Middle => _input.AreMouseButtonsDown(MouseButton.Middle),
            UIElementClickType.Right => _input.AreMouseButtonsDown(MouseButton.Right),
            UIElementClickType.Enter => _input.AreKeysDown(Keys.Enter),
            _ => throw new ArgumentException($"Invalid click type: {type} ({(int)type})", nameof(type))
        };
    }

    private void ResetClickData()
    {
        _currentClickType = UIElementClickType.None;
        _clickStartPosition = Vector2.Zero;
        _clickDuration = TimeSpan.Zero;
    }

    private UIElementClickType TestMethodOnFullClick(IProgramTime time)
    {
        if (_currentClickType == UIElementClickType.None)
        {
            _clickDuration = TimeSpan.Zero;
            _currentClickType = TestMethodOnClick();
            if (_currentClickType != UIElementClickType.None)
            {
                _clickStartPosition = _input.VirtualMousePositionCurrent;
                ClickStart?.Invoke(this, new(this, _clickStartPosition, _currentClickType));
            }
            return UIElementClickType.None;
        }

        _clickDuration += time.PassedTime;

        UIElementClickType ReleaseClickType = TestMethodOnRelease();
        if (!IsButtonDown(_currentClickType))
        {
            return _currentClickType;
        }
        return UIElementClickType.None;
    }

    private void TestButtonClick(IProgramTime time)
    {
        UIElementClickType ClickType = ClickMethod switch
        {
            ElementClickMethod.ActivateOnClick => TestMethodOnClick(),
            ElementClickMethod.ActivateOnRelease => TestMethodOnRelease(),
            ElementClickMethod.ActivateOnFullClick => TestMethodOnFullClick(time),
            _ => throw new InvalidOperationException(
                $"Invalid click method found for basic menu button: {ClickMethod} ({(int)ClickMethod})")
        };

        if (ClickType != UIElementClickType.None)
        {
            OnButtonClick(ClickType);
        }
    }

    private void UpdateScrollValue()
    {
        if (_isTargeted && (_input.MouseScrollChangeAmount != 0))
        {
            Scroll?.Invoke(this, new(this, _input.MouseScrollChangeAmount));
        }
    }

    private void OnButtonTargetUpdate(IProgramTime time)
    {
        if (_isHovered.Current && !_isHovered.Previous)
        {
            HoverStart?.Invoke(this, new(this));
        }
    }

    private void OnButtonNotTargetUpdate()
    {
        if (!_isHovered.Current && _isHovered.Previous)
        {
            HoverEnd?.Invoke(this, new(this));
        }
    }


    // Methods.
    public bool IsPositionOverClickArea(Vector2 position)
    {
        Vector2 BottomLeft = new(ElementBounds.X, ElementBounds.Y);
        Vector2 TopRight = new(ElementBounds.X + ElementBounds.Width, ElementBounds.Y + ElementBounds.Height);
        return GHMath.IsPointInArea(position, BottomLeft, TopRight, 0f);
    }

    public void ForceStartClick(UIElementClickType clickType)
    {
        if (clickType == UIElementClickType.None)
        {
            return;
        }

        _currentClickType = clickType;
        _clickStartPosition = _input.VirtualMousePositionCurrent;
        ClickStart?.Invoke(this, new(this, _input.VirtualMousePositionCurrent, clickType));
    }

    public void ForceEndClick()
    {
        Vector2 InputPosition = _input.VirtualMousePositionCurrent;
        ClickEnd?.Invoke(this, new(this,
            _clickStartPosition,
            InputPosition,
            _currentClickType, 
            _clickDuration, 
            IsPositionOverClickArea(InputPosition)));
    }


    // Inherited methods
    public void Update(IProgramTime time)
    {
        if (IsEnabled)
        {
            UpdateIsTargeted();
            TestButtonClick(time);
            UpdateScrollValue();
        }
        
        _isHovered.SetValue(_isTargeted);

        if (_isTargeted)
        {
            OnButtonTargetUpdate(time);
        }
        else
        {
            OnButtonNotTargetUpdate();
        }
    }
}