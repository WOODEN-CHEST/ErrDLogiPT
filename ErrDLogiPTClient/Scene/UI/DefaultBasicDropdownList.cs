using ErrDLogiPTClient.Scene.Sound;
using GHEngine;
using GHEngine.Audio.Source;
using GHEngine.Frame;
using GHEngine.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

public class DefaultBasicDropdownList<T> : IBasicDropdownList<T>
{
    // Fields.
    public bool IsEnabled { get; set; } = true;
    public bool IsVisible { get; set; } = true;
    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            UpdateVisibleButtonProperties();
        }
    }
    public int MaxDisplayedElementCount { get; set; } = 5;
    public int MaxSelectedElementCount { get; set; } = 1;

    public int ElementCount => _entries.Count;

    public IEnumerable<DropdownListElement<T>> Elements => _entries.Select(entry => entry.Element);

    public Color ValueDisplayColor
    {
        get => _displayNormalColor;
        set
        {
            _displayNormalColor = value;
            UpdateDisplayButtonProperties();
        }
    }

    public Color ValueDisplayHoverColor
    {
        get => _displayHoverColor;
        set
        {
            _displayHoverColor = value;
            UpdateDisplayButtonProperties();
        }
    }

    public Color ValueDisplayChangeColor
    {
        get => _displayChangeColor;
        set
        {
            _displayChangeColor = value;
            UpdateDisplayButtonProperties();
        }
    }

    public TimeSpan HoverColorDuration
    {
        get => _hoverColorDuration;
        set
        {
            if (value.Ticks < 0)
            {
                throw new ArgumentException("Hover color duration must be >= 0 ticks", nameof(value));
            }
            _hoverColorDuration = value;
        }
    }

    public TimeSpan ValueChangeColorDuration
    {
        get => _valueChangeColorDuration;
        set
        {
            if (value.Ticks < 0)
            {
                throw new ArgumentException("Value change color duration must be >= 0 ticks", nameof(value));
            }
            _valueChangeColorDuration = value;
        }
    }

    public Color DefaultElementColor
    {
        get => _defaultElementColor;
        set
        {
            _defaultElementColor = value;
            UpdateAllEntryProperties();
        }
    }

    public Color DefaultElementHoverColor
    {
        get => _defaultElementHoverColor;
        set
        {
            _defaultElementHoverColor = value;
            UpdateAllEntryProperties();
        }
    }

    public Color DefaultElementClickColor
    {
        get => _defaultElementClickColor;
        set
        {
            _defaultElementClickColor = value;
            UpdateAllEntryProperties();
        }
    }
    public Color DefaultElementUnavailableColor
    {
        get => _defaultElementUnavailableColor;
        set
        {
            _defaultElementUnavailableColor = value;
            UpdateAllEntryProperties();
        }
    }
    public Color DefaultElementSelectedColor
    {
        get => _defaultElementSelectedColor;
        set
        {
            _defaultElementSelectedColor = value;
            UpdateAllEntryProperties();
        }
    }

    public TimeSpan ElementPopupDelay { get; set; } = TimeSpan.FromSeconds(0.05d);

    public bool IsTextShadowEnabled
    {
        get => _isTextShadowEnabled;
        set
        {
            _isTextShadowEnabled = value;
            UpdateDisplayButtonProperties();
            UpdateAllEntryProperties();
        }
    }

    public float Volume { get; set; }

    public IEnumerable<IPreSampledSound> ClickSounds
    {
        get => _clickSounds;
        set
        {
            _clickSounds = value.ToArray();
            UpdateAllEntryProperties();
        }
    }

    public IEnumerable<IPreSampledSound> HoverStartSounds
    {
        get => _hoverStartSounds;
        set
        {
            _hoverStartSounds = value.ToArray();
            UpdateAllEntryProperties();
        }
    }

    public IEnumerable<IPreSampledSound> HoverEndSounds
    {
        get => _hoverEndSounds;
        set
        {
            _hoverEndSounds = value.ToArray();
            UpdateAllEntryProperties();
        }
    }

    public LogiSoundCategory SoundCategory
    {
        get => _soundCategory;
        set => _soundCategory = value ?? throw new ArgumentNullException(nameof(value));
    }

    public float Length
    {
        get => _length;
        set
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentException($"Invalid length: {value}", nameof(value));
            }
            if (value < 0f)
            {
                throw new ArgumentException($"Length cannot be < 0: {value}", nameof(value));
            }

            _length = value;
            UpdateDisplayButtonProperties();
            UpdateAllEntryProperties();
            UpdateVisibleButtonProperties();
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
                throw new ArgumentException($"Length scale be < 0: {value}", nameof(value));
            }

            _scale = value;
            UpdateDisplayButtonProperties();
            UpdateAllEntryProperties();
            UpdateVisibleButtonProperties();
        }
    }
    public ButtonClickMethod ClickMethod
    {
        get => _clickMethod;
        set
        {
            _clickMethod = value;
            UpdateAllEntryProperties();
        }
    }

    public int ScrollIndex
    {
        get => _scrollIndex;
        set
        {
            if (_scrollIndex == value)
            {
                return;
            }
            _scrollIndex = Math.Clamp(value, 0, Math.Max(0, ElementCount - 1 - MaxDisplayedElementCount));
            UpdateVisibleButtonProperties();
        }
    }

    public bool IsTargeted { get; set; } = false;

    public event EventHandler<EventArgs>? Expand;
    public event EventHandler<EventArgs>? Contract;
    public event EventHandler<EventArgs>? SelectionUpdate;
    public event EventHandler<EventArgs>? PlaySound;


    // Private static fields.
    private const float COLOR_FADE_FACTOR_MIN = 0f;
    private const float COLOR_FADE_FACTOR_MAX = 1f;


    // Private fields.
    private readonly IBasicButton _displayButton;
    private readonly IUserInput _input;
    private readonly Func<IBasicButton> _buttonCreator;
    private readonly List<DropdownEntry> _entries = new();

    private Color _displayNormalColor = Color.White;
    private Color _displayHoverColor = Color.White;
    private Color _displayChangeColor = Color.White;
    private float _hoverColorFactor = 0f;
    private float _valueChangeColorFactor = 0f;
    private TimeSpan _hoverColorDuration = TimeSpan.Zero;
    private TimeSpan _valueChangeColorDuration = TimeSpan.Zero;
    private Color _defaultElementColor = Color.White;
    private Color _defaultElementHoverColor = Color.White;
    private Color _defaultElementClickColor = Color.White;
    private Color _defaultElementUnavailableColor = Color.White;
    private Color _defaultElementSelectedColor = Color.White;

    private float _length = 1f;
    private float _scale = 1f;

    private TimeSpan _elementPopupTimer = TimeSpan.Zero;
    private int _popupElementCount = 0;
    private int _scrollIndex = 0;

    private bool _isTextShadowEnabled = true;

    private IPreSampledSound[] _clickSounds = Array.Empty<IPreSampledSound>();
    private IPreSampledSound[] _hoverStartSounds = Array.Empty<IPreSampledSound>();
    private IPreSampledSound[] _hoverEndSounds = Array.Empty<IPreSampledSound>();
    private LogiSoundCategory _soundCategory = LogiSoundCategory.UI;

    private HashSet<DropdownListElement<T>> _selectedElements = new();

    private Vector2 _position = Vector2.Zero;

    private ButtonClickMethod _clickMethod = ButtonClickMethod.ActivateOnFullClick;


    // Constructors.
    public DefaultBasicDropdownList(IUserInput input,
        Func<IBasicButton> buttonCreator)
    {
        ArgumentNullException.ThrowIfNull(input, nameof(input));
        ArgumentNullException.ThrowIfNull(buttonCreator, nameof(buttonCreator));

        _input = input;
        _buttonCreator = buttonCreator;

        _displayButton = _buttonCreator.Invoke();
        InitDisplayButton();
    }


    // Private methods.
    private void InitDisplayButton()
    {
        _displayButton.IsEnabled = false;
    }

    private void InitializeEntry(DropdownEntry entry)
    {
        entry.Button.Initialize();
        UpdateSingleEntryProperties(entry);
    }

    private void DeinitializeEntry(DropdownEntry entry)
    {
        entry.Button.Deinitialize();
    }

    private void UpdateDisplayButtonProperties()
    {
        _displayButton.IsTextShadowEnabled = _isTextShadowEnabled;
        _displayButton.Length = Length;
        _displayButton.Scale = Scale;
    }

    private void UpdateAllEntryProperties()
    {
        foreach (DropdownEntry Entry in _entries)
        {
            UpdateSingleEntryProperties(Entry);
        }
    }

    private void UpdateSingleEntryProperties(DropdownEntry entry)
    {
        entry.Button.IsTextShadowEnabled = _isTextShadowEnabled;
        IBasicButton Button = entry.Button;
        if (entry.Element.IsSelectable)
        {
            Button.IsEnabled = true;
            Button.ButtonColor = IsElementSelected(entry.Element)
                ? (entry.Element.NormalColorOverride ?? DefaultElementColor)
                : (entry.Element.SelectedColorOverride ?? DefaultElementSelectedColor);
        }
        else
        {
            entry.Button.IsEnabled = false;
            entry.Button.ButtonColor = entry.Element.UnavailableColorOverride ?? DefaultElementUnavailableColor;
        }

        Button.HighlightColor = entry.Element.HoverColorOverride ?? DefaultElementHoverColor;
        Button.ClickColor = entry.Element.ClickColorOverride ?? DefaultElementClickColor;
        Button.ClickSounds = ClickSounds;
        Button.HoverStartSounds = HoverStartSounds;
        Button.HoverEndSounds = HoverEndSounds;
        Button.Length = Length;
        Button.Scale = Scale;
        Button.ClickSounds = ClickSounds;
        Button.HoverStartSounds = HoverStartSounds;
        Button.HoverEndSounds = HoverEndSounds;
        Button.ClickMethod = ClickMethod;
        Button.Text = entry.Element.DisplayName;
    }

    private void GenericUpdate(IProgramTime time)
    {
        float PassedTimeSeconds = (float)time.PassedTime.TotalSeconds;

        if (time.PassedTime < _hoverColorDuration)
        {
            _hoverColorFactor = IsTargeted ? COLOR_FADE_FACTOR_MAX : COLOR_FADE_FACTOR_MIN;
        }
        else
        {
            _hoverColorFactor = Math.Clamp(
                _hoverColorFactor + ((IsTargeted ? 1f : -1f) * (PassedTimeSeconds / (float)HoverColorDuration.TotalSeconds)),
                COLOR_FADE_FACTOR_MIN, 
                COLOR_FADE_FACTOR_MAX);
        }

        if (time.PassedTime < _valueChangeColorDuration)
        {
            _valueChangeColorFactor = COLOR_FADE_FACTOR_MAX;
        }
        else
        {
            _valueChangeColorFactor = Math.Clamp(
                _valueChangeColorFactor - (PassedTimeSeconds / (float)HoverColorDuration.TotalSeconds),
                COLOR_FADE_FACTOR_MIN,
                COLOR_FADE_FACTOR_MAX);
        }

        UpdateScrollPosition();
        UpdateDisplayButtonColor(time);
        AnimateElementPopup(time);
    }

    private void AnimateElementPopup(IProgramTime time)
    {
        if (!IsTargeted && (_popupElementCount <= 0))
        {
            ScrollIndex = 0;
            _elementPopupTimer = TimeSpan.Zero;
            return;
        }


        if (IsTargeted && (_popupElementCount >= Math.Min(MaxDisplayedElementCount, ElementCount)))
        {
            _elementPopupTimer = TimeSpan.Zero;
            return;
        }

        _elementPopupTimer += time.PassedTime;
        bool WereElementsUpdated = false;
        int TargetMaxCount = IsTargeted ? Math.Min(MaxDisplayedElementCount, ElementCount) : 0;
        int Step = IsTargeted ? 1 : -1;

        while ((_elementPopupTimer >= ElementPopupDelay) && (_popupElementCount != TargetMaxCount))
        {
            _popupElementCount += Step;
            _elementPopupTimer -= ElementPopupDelay;
            WereElementsUpdated = true;
        }

        if (WereElementsUpdated)
        {
            UpdateVisibleButtonProperties();
        }
    }

    private void UpdateVisibleButtonProperties()
    {
        const float SCREEN_MIDDLE_POSITION = 0.5f;

        Vector2 ButtonPosition = _position;
        _displayButton.Position = ButtonPosition;
        float YStep = _position.Y <= SCREEN_MIDDLE_POSITION ? 1f : -1f;
        
        List<DropdownEntry> VisibleEntries = GetEntriesInSelectionRange();
        bool IsOOBButtonReached = false;

        for (int i = 0; i < VisibleEntries.Count; i++)
        {
            DropdownEntry TargetEntry = VisibleEntries[i];
            if (!IsOOBButtonReached)
            {
                ButtonPosition += new Vector2(0f, YStep * TargetEntry.Button.ButtonBounds.Height);
                TargetEntry.Button.Position = ButtonPosition;
                IsOOBButtonReached = IsOOBButtonReached || !IsButtonInBounds(TargetEntry.Button);
            }

            if (IsOOBButtonReached)
            {
                TargetEntry.Button.IsVisible = false;
                TargetEntry.Button.IsEnabled = false;
                continue;
            }
            else
            {
                TargetEntry.Button.IsVisible = true;
                TargetEntry.Button.IsEnabled = TargetEntry.Element.IsSelectable;
            }
        }
    }

    private bool IsButtonInBounds(IBasicButton button)
    {
        const float SCREEN_POSITION_MAX = 1f;
        const float SCREEN_POSITION_MIN = 0f;

        RectangleF Bounds = button.ButtonBounds;
        Vector2 CornerMin = new(Bounds.X, Bounds.Y);
        Vector2 CornerMax = CornerMin + new Vector2(Bounds.Width, Bounds.Height);

        return (CornerMin.X >= SCREEN_POSITION_MIN)
            && (CornerMin.Y >= SCREEN_POSITION_MIN)
            && (CornerMax.X <= SCREEN_POSITION_MAX)
            && (CornerMax.Y <= SCREEN_POSITION_MAX);
    }

    private void UpdateDisplayButtonColor(IProgramTime time)
    {
        FloatColor ColorStage1 = FloatColor.InterpolateRGB(_displayNormalColor, _displayHoverColor, _hoverColorFactor);
        FloatColor ColorStage2 = FloatColor.InterpolateRGB(ColorStage1, _displayChangeColor, _valueChangeColorFactor);
        Color FinalButtonColor = ColorStage2;

        _displayButton.ButtonColor = FinalButtonColor;
    }

    private void UpdateScrollPosition()
    {
        if (!IsTargeted || (_input.MouseScrollChangeAmount == 0))
        {
            return;
        }

        ScrollIndex = _input.MouseScrollChangeAmount > 0 ? (ScrollIndex - 1) : (ScrollIndex + 1);
    }

    private List<DropdownEntry> GetEntriesInSelectionRange()
    {
        List<DropdownEntry> VisibleEntries = new();
        int MaxIndexExclusive = Math.Min(_entries.Count, _scrollIndex + MaxDisplayedElementCount);
        for (int i = _scrollIndex; i < MaxIndexExclusive; i++)
        {
            VisibleEntries.Add(_entries[i]);
        }
        return VisibleEntries;
    }


    // Inherited methods.
    public void Initialize()
    {
        _displayButton.Initialize();
    }

    public void Deinitialize()
    {
        _displayButton.Deinitialize();
    }

    public void SetIsElementSelected(DropdownListElement<T> element, bool isSelected)
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));
        if (!ContainsElement(element))
        {
            return;
        }

        if (isSelected)
        {
            _selectedElements.Add(element);
        }
        else
        {
            _selectedElements.Remove(element);
        }
    }

    public bool IsElementSelected(DropdownListElement<T> element)
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));
        return _selectedElements.Contains(element);
    }

    public void InsertElement(DropdownListElement<T>? element, int index)
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));
        DropdownEntry Entry = new(_buttonCreator.Invoke(), element);
        _entries.Insert(index, Entry);
        InitializeEntry(Entry);
        UpdateVisibleButtonProperties();
    }

    public void RemoveElement(DropdownListElement<T> element)
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));
        for (int i = 0; i < _entries.Count; i++)
        {
            if (_entries[i].Element == element)
            {
                RemoveElement(i);
                return;
            }
        }
    }

    public void RemoveElement(int index)
    {
        DropdownEntry Entry = _entries[index];
        _entries.RemoveAt(index);
        DeinitializeEntry(Entry);
    }

    public void SetElements(IEnumerable<DropdownListElement<T>>? elements)
    {
        ClearElements();
        foreach (DropdownListElement<T> Element in elements ?? Enumerable.Empty<DropdownListElement<T>>())
        {
            AddElement(Element);
        }
    }

    public void AddElement(DropdownListElement<T>? element)
    {
        InsertElement(element, _entries.Count);
    }

    public void ClearElements()
    {
        foreach (DropdownEntry entry in _entries)
        {
            DeinitializeEntry(entry);
        }

        _entries.Clear();
    }

    public bool ContainsElement(DropdownListElement<T> element)
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));

        foreach (DropdownEntry Entry in _entries)
        {
            if (Entry.Element == element)
            {
                return true;
            }
        }

        return false;
    }

    public void Update(IProgramTime time)
    {
        IsTargeted = IsEnabled && ((IsTargeted && (_input.VirtualMousePositionCurrent == _input.VirtualMousePositionPrevious))
            || IsPositionOverList(_input.VirtualMousePositionCurrent));

        GenericUpdate(time);

        _displayButton.Update(time);
        foreach (DropdownEntry Entry in _entries)
        {
            Entry.Button.Update(time);
        }
    }

    public void Render(IRenderer renderer, IProgramTime time)
    {
        if (!IsVisible)
        {
            return;
        }

        foreach (DropdownEntry Entry in _entries)
        {
            Entry.Button.Render(renderer, time);
        }

        _displayButton.Render(renderer, time);
    }

    public bool IsPositionOverList(Vector2 position)
    {
        if (_displayButton.IsPositionOverButton(position))
        {
            return true;
        }

        if (_popupElementCount <= 0)
        {
            return false;
        }

        foreach (DropdownEntry Entry in GetEntriesInSelectionRange())
        {
            if (Entry.Button.IsEnabled && Entry.Button.IsPositionOverButton(position))
            {
                return true;
            }
        }
        return false;
    }


    // Types.
    private class DropdownEntry
    {
        // Fields.
        public IBasicButton Button { get; }
        public DropdownListElement<T> Element { get; }


        // Constructors.
        public DropdownEntry(IBasicButton button, DropdownListElement<T> element)
        {
            Button = button;
            Element = element;
        }
    }
}