using ErrDLogiPTClient.Scene.Sound;
using ErrDLogiPTClient.Scene.UI.Button;
using GHEngine;
using GHEngine.Audio.Source;
using GHEngine.Frame;
using GHEngine.Frame.Animation;
using GHEngine.Frame.Item;
using GHEngine.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ErrDLogiPTClient.Scene.UI.Dropdown;

public class DefaultBasicDropdownList<T> : IBasicDropdownList<T>
{
    // Fields.
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (value == _isEnabled)
            {
                return;
            }
            _isEnabled = value;
            UpdateEntryButtonStates();
        }
    }

    public bool IsVisible { get; set; } = true;

    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            UpdateElementPositions();
            UpdateEntryButtonStates();
        }
    }
    public int MaxDisplayedElementCount
    {
        get => _maxDisplayedElementCount;
        set => _maxDisplayedElementCount = Math.Max(value, 0);
    }

    public int MaxSelectedElementCount
    {
        get => _maxSelectedElementCount;
        set
        {
            _maxSelectedElementCount = Math.Max(value, 0);
            _minSelectedElementCount = Math.Min(value, _minSelectedElementCount);
            EnsureSelectedElementCount(MaxSelectedElementCount);
        }
    }

    public int MinSelectedElementCount
    {
        get => _minSelectedElementCount;
        set
        {
            _minSelectedElementCount = Math.Max(value, 0);
            _maxSelectedElementCount = Math.Max(value, _maxSelectedElementCount);
            EnsureSelectedElementCount(MaxSelectedElementCount);
        }
    }

    public int ElementCount => _entries.Count;

    public IEnumerable<DropdownListElement<T>> Elements => _entries.Select(entry => entry.Element);

    public Color ValueDisplayColor
    {
        get => _displayNormalColor;
        set => _displayNormalColor = value;
    }

    public Color ValueDisplayHoverColor
    {
        get => _displayHoverColor;
        set => _displayHoverColor = value;
    }

    public Color ValueDisplayChangeColor
    {
        get => _displayChangeColor;
        set => _displayChangeColor = value;
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

    public TimeSpan ElementPopupDelay { get; set; } = TimeSpan.FromSeconds(0.025d);

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

    public float Volume
    {
        get => _volume;
        set
        {
            _volume = value;
            UpdateAllEntryProperties();
        }
    }

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
        set
        {
            _soundCategory = value ?? throw new ArgumentNullException(nameof(value));
            UpdateAllEntryProperties();
        }
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
            UpdateElementPositions();
            UpdateEntryButtonStates();
            UpdateArrowIndicatorProperties();
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
            UpdateElementPositions();
            UpdateEntryButtonStates();
            UpdateArrowIndicatorProperties();

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
            _scrollIndex = Math.Clamp(value, 0, Math.Max(0, _entries.Count - _clampedMaxPopupElementCount));
            UpdateElementPositions();
            UpdateEntryButtonStates();
        }
    }

    public bool IsTargeted { get; set; } = false;
    public int SelectedElementCount => _selectedElements.Count;
    public IEnumerable<DropdownListElement<T>> SelectedElements => _selectedElements;

    public event EventHandler<BasicDropdownExpandStartEventArgs<T>>? ExpandStart;
    public event EventHandler<BasicDropdownExpandFinishEventArgs<T>>? ExpandFinish;
    public event EventHandler<BasicDropdownContractStartEventArgs<T>>? ContractStart;
    public event EventHandler<BasicDropdownContractFinishEventArgs<T>>? ContractFinish;
    public event EventHandler<BasicDropdownSelectionUpdateEventArgs<T>>? SelectionUpdate;
    public event EventHandler<BasicDropdownPlaySoundEventArgs<T>>? PlaySound;




    // Private static fields.
    private const float COLOR_FADE_FACTOR_MIN = 0f;
    private const float COLOR_FADE_FACTOR_MAX = 1f;
    private const string DISPLAY_TEXT_NO_ELEMENTS = "...";
    private const float POSITION_MIDDLE = 0.5f;


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
    private int _clampedMaxPopupElementCount = 0;

    private bool _isTextShadowEnabled = true;

    private IPreSampledSound[] _clickSounds = Array.Empty<IPreSampledSound>();
    private IPreSampledSound[] _hoverStartSounds = Array.Empty<IPreSampledSound>();
    private IPreSampledSound[] _hoverEndSounds = Array.Empty<IPreSampledSound>();
    private LogiSoundCategory _soundCategory = LogiSoundCategory.UI;
    private float _volume = 1f;

    private List<DropdownListElement<T>> _selectedElements = new();

    private Vector2 _position = Vector2.Zero;

    private ButtonClickMethod _clickMethod = ButtonClickMethod.ActivateOnFullClick;

    private int _minSelectedElementCount = 1;
    private int _maxSelectedElementCount = 1;
    private int _maxDisplayedElementCount = int.MaxValue;

    private bool _isEnabled = true;

    private readonly SpriteItem _nearIndicator;
    private readonly SpriteItem _farIndicator;


    // Constructors.
    public DefaultBasicDropdownList(IUserInput input,
        Func<IBasicButton> buttonCreator,
        ISpriteAnimation arrowIndicatorAnimation)
    {
        ArgumentNullException.ThrowIfNull(input, nameof(input));
        ArgumentNullException.ThrowIfNull(buttonCreator, nameof(buttonCreator));
        ArgumentNullException.ThrowIfNull(arrowIndicatorAnimation, nameof(arrowIndicatorAnimation));

        _input = input;
        _buttonCreator = buttonCreator;

        _displayButton = _buttonCreator.Invoke();
        InitDisplayButton();

        _nearIndicator = new(arrowIndicatorAnimation.CreateInstance());
        _farIndicator = new(arrowIndicatorAnimation.CreateInstance());
        InitArrowIndicator(_nearIndicator);
        InitArrowIndicator(_farIndicator);
        UpdateArrowIndicatorProperties();
        UpdateArrowIndicatorPositions();
        UpdateArrowIndicatorVisibility();
        
        UpdateElementPositions();
        UpdateDisplayButtonProperties();
    }


    // Private methods.
    private int GetMovementYStep()
    {
        return _position.Y >= POSITION_MIDDLE ? -1 : 1;
    }
    
    private void InitArrowIndicator(SpriteItem item)
    {
        item.IsVisible = false;
        item.Origin = new Vector2(0.5f);
    }

    private void UpdateArrowIndicatorProperties()
    {
        const float INDICATOR_SCALE = 0.75f;
        foreach (SpriteItem Indicator in new SpriteItem[] { _farIndicator, _nearIndicator })
        {
            Indicator.Size = new Vector2(Indicator.FrameSize.X / Indicator.FrameSize.Y * _scale, _scale) * INDICATOR_SCALE;
        }

        if (_position.X > POSITION_MIDDLE)
        {
            _nearIndicator.Rotation = 0f;
            _farIndicator.Rotation = MathF.PI;
        }
        else
        {
            _nearIndicator.Rotation = MathF.PI;
            _farIndicator.Rotation = 0f;
        }
    }

    private void UpdateArrowIndicatorVisibility()
    {
        _nearIndicator.IsVisible = _scrollIndex > 0;
        _farIndicator.IsVisible = _scrollIndex < ElementCount - _clampedMaxPopupElementCount;
    }

    private void UpdateArrowIndicatorPositions()
    {
        int YStep = GetMovementYStep();
        RectangleF ButtonBounds = _displayButton.ButtonBounds;

        _nearIndicator.Position = _displayButton.Position
            + new Vector2(0f, ButtonBounds.Height * (-YStep));

        _farIndicator.Position = _displayButton.Position 
            + new Vector2(0f, (ButtonBounds.Height * (YStep) * (_popupElementCount + 1)));
    }

    private void InitDisplayButton()
    {
        _displayButton.IsEnabled = false;
    }

    private void InitializeEntry(DropdownEntry entry)
    {
        entry.Button.Initialize();
        entry.Button.DetectedClickTypes = new UIElementClickType[] 
        { 
            UIElementClickType.Left,
            UIElementClickType.Middle,
            UIElementClickType.Right,
        };
        UpdateSingleEntryProperties(entry);
        entry.Button.MainClickAction = (args) => OnElementClickEvent(entry, args);
        entry.Button.PlaySound += OnElementCreateSoundEvent;
    }

    private void DeinitializeEntry(DropdownEntry entry)
    {
        entry.Button.Deinitialize();
        entry.Button.PlaySound -= OnElementCreateSoundEvent;
    }

    private void UpdateDisplayButtonProperties()
    {
        _displayButton.IsTextShadowEnabled = IsTextShadowEnabled;
        _displayButton.Length = Length;
        _displayButton.Scale = Scale;

        if (_selectedElements.Count == 0)
        {
            _displayButton.Text = DISPLAY_TEXT_NO_ELEMENTS;
        }
        else
        {
            _displayButton.Text = $"{_selectedElements[0].DisplayName}" +
                $"{(_selectedElements.Count > 1 ? '+' : string.Empty)}";
        }
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
                ? entry.Element.SelectedColorOverride ?? DefaultElementSelectedColor
                : entry.Element.NormalColorOverride ?? DefaultElementColor;
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
        Button.Volume = Volume;
    }

    private void GenericUpdate(IProgramTime time)
    {
        float PassedTimeSeconds = (float)time.PassedTime.TotalSeconds;

        if (_hoverColorDuration < time.PassedTime)
        {
            _hoverColorFactor = IsTargeted ? COLOR_FADE_FACTOR_MAX : COLOR_FADE_FACTOR_MIN;
        }
        else
        {
            _hoverColorFactor = Math.Clamp(
                _hoverColorFactor + (IsTargeted ? 1f : -1f) * (PassedTimeSeconds / (float)HoverColorDuration.TotalSeconds),
                COLOR_FADE_FACTOR_MIN, 
                COLOR_FADE_FACTOR_MAX);
        }

        if (_valueChangeColorDuration < time.PassedTime)
        {
            _valueChangeColorFactor = COLOR_FADE_FACTOR_MAX;
        }
        else
        {
            _valueChangeColorFactor = Math.Clamp(
                _valueChangeColorFactor - (PassedTimeSeconds / (float)ValueChangeColorDuration.TotalSeconds),
                COLOR_FADE_FACTOR_MIN,
                COLOR_FADE_FACTOR_MAX);
        }

        UpdateScrollPosition();
        UpdateDisplayButtonColor(time);
        AnimateElementPopup(time);
    }

    private void AnimateElementPopup(IProgramTime time)
    {
        if (TrySkipPopupAnimation())
        {
            return;
        }

        _elementPopupTimer += time.PassedTime;
        int OldPopupElementCount = _popupElementCount;
        int TargetMaxCount = IsTargeted ? _clampedMaxPopupElementCount : 0;
        int Step = IsTargeted ? 1 : -1;

        while (_elementPopupTimer >= ElementPopupDelay && _popupElementCount != TargetMaxCount)
        {
            _popupElementCount += Step;
            _elementPopupTimer -= ElementPopupDelay;
        }

        if (OldPopupElementCount != _popupElementCount)
        {
            TryRaiseAnimationEvents(OldPopupElementCount);
            UpdateEntryButtonStates();
            UpdateArrowIndicatorPositions();
        }
    }

    private bool TrySkipPopupAnimation()
    {
        if (!IsTargeted && _popupElementCount <= 0)
        {
            ScrollIndex = 0;
            _elementPopupTimer = TimeSpan.Zero;
            return true;
        }

        if (IsTargeted && _popupElementCount >= _clampedMaxPopupElementCount)
        {
            _elementPopupTimer = TimeSpan.Zero;
            return true;
        }
        return false;
    }

    private void TryRaiseAnimationEvents(int oldPopupElementCount)
    {
        CancellableEventBase? TargetEvent = null;

        if (oldPopupElementCount == 0)
        {
            BasicDropdownExpandStartEventArgs<T> ExpandStartEvent = new(this);
            ExpandStart?.Invoke(this, ExpandStartEvent);
            TargetEvent = ExpandStartEvent;
        }
        else if (_popupElementCount == _clampedMaxPopupElementCount)
        {
            BasicDropdownExpandFinishEventArgs<T> ExpandFinishEvent = new(this);
            ExpandFinish?.Invoke(this, ExpandFinishEvent);
            TargetEvent = ExpandFinishEvent;
        }
        else if (_popupElementCount == 0)
        {
            BasicDropdownContractFinishEventArgs<T> ContractFinishEvent = new(this);
            ContractFinish?.Invoke(this, ContractFinishEvent);
            TargetEvent = ContractFinishEvent;
        }
        else if (oldPopupElementCount == _clampedMaxPopupElementCount)
        {
            BasicDropdownContractStartEventArgs<T> ContractStartEvent = new(this);
            ContractStart?.Invoke(this, ContractStartEvent);
            TargetEvent = ContractStartEvent;
        }

        if (TargetEvent != null)
        {
            if (TargetEvent.IsCancelled)
            {
                _popupElementCount = oldPopupElementCount;
            }
            TargetEvent.ExecuteActions();
        }
    }

    private void UpdateElementPositions()
    {
        Vector2 ButtonPosition = _position;
        _displayButton.Position = ButtonPosition;
        int YStep = GetMovementYStep();

        List<DropdownEntry> VisibleEntries = GetEntriesInSelectionRange();
        bool IsOOBButtonReached = false;

        for (int i = 0; i < VisibleEntries.Count; i++)
        {
            DropdownEntry TargetEntry = VisibleEntries[i];
            ButtonPosition += new Vector2(0f, YStep * TargetEntry.Button.ButtonBounds.Height);
            TargetEntry.Button.Position = ButtonPosition;

            bool WasOOBReachedPreviously = IsOOBButtonReached;
            IsOOBButtonReached = IsOOBButtonReached
                    || i >= MaxDisplayedElementCount
                    || !IsButtonInBounds(TargetEntry.Button);

            if (!WasOOBReachedPreviously && IsOOBButtonReached)
            {
                _clampedMaxPopupElementCount = i;
            }
        }

        if (!IsOOBButtonReached)
        {
            _clampedMaxPopupElementCount = Math.Min(MaxDisplayedElementCount, VisibleEntries.Count);
        }
    }

    private void UpdateEntryButtonStates()
    {
        int MinIndex = _scrollIndex;
        int MaxIndexExclusive = _scrollIndex + _popupElementCount;
        for (int i = 0; i < _entries.Count; i++)
        {
            DropdownEntry Entry = _entries[i];
            Entry.Button.IsTargeted = false;
            if ((i >= MinIndex) && (i < MaxIndexExclusive))
            {
                Entry.Button.IsVisible = true;
                Entry.Button.IsEnabled = IsEnabled && Entry.Element.IsSelectable;
            }
            else
            {
                Entry.Button.IsVisible = false;
                Entry.Button.IsEnabled = false;
            }
        }
        UpdateArrowIndicatorVisibility();
    }

    private bool IsButtonInBounds(IBasicButton button)
    {
        float SCREEN_POSITION_MAX = 1f - button.ButtonBounds.Y;
        float SCREEN_POSITION_MIN = 0f + button.ButtonBounds.Y;

        RectangleF Bounds = button.ButtonBounds;
        Vector2 CornerMin = new(Bounds.X, Bounds.Y);
        Vector2 CornerMax = CornerMin + new Vector2(Bounds.Width, Bounds.Height);

        return CornerMin.X >= SCREEN_POSITION_MIN
            && CornerMin.Y >= SCREEN_POSITION_MIN
            && CornerMax.X <= SCREEN_POSITION_MAX
            && CornerMax.Y <= SCREEN_POSITION_MAX;
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
        if (!IsTargeted || _input.MouseScrollChangeAmount == 0)
        {
            return;
        }

        ScrollIndex = _input.MouseScrollChangeAmount > 0 ? ScrollIndex - 1 : ScrollIndex + 1;
    }

    private List<DropdownEntry> GetEntriesInSelectionRange()
    {
        List<DropdownEntry> VisibleEntries = new();
        long MaxIndexExclusive = Math.Min(_entries.Count, (long)_scrollIndex + (long)MaxDisplayedElementCount);
        for (int i = _scrollIndex; i < MaxIndexExclusive; i++)
        {
            VisibleEntries.Add(_entries[i]);
        }
        return VisibleEntries;
    }

    private void OnElementClickEvent(DropdownEntry entry, BasicButtonMainClickArgs args)
    {
        if (!entry.Element.IsSelectable)
        {
            return;
        }

        if (args.ClickType == UIElementClickType.Middle)
        {
            OnClickDeselectAllElements();
        }
        else if (args.ClickType == UIElementClickType.Left)
        {
            OnClickSelectElement(entry);
        }
        else if (args.ClickType == UIElementClickType.Right)
        {
            OnClickDeselectElement(entry);
        }

        UpdateDisplayButtonProperties();
    }

    private void OnClickDeselectAllElements()
    {
        EnsureSelectedElementCount(MinSelectedElementCount);
    }

    private void OnClickSelectElement(DropdownEntry entry)
    {
        SetIsElementSelectedInternal(entry.Element, true, true);
    }

    private void OnClickDeselectElement(DropdownEntry entry)
    {
        SetIsElementSelectedInternal(entry.Element, false, true);
    }

    private void EnsureSelectedElementCount(int countLimit)
    {
        int ClampedCountLimit = Math.Max(0, countLimit);
        while (_selectedElements.Count > ClampedCountLimit)
        {
            DropdownListElement<T> Element = _selectedElements[0];
            _selectedElements.RemoveAt(0);
            UpdateSingleEntryProperties(GetEntryByElement(Element)!);
        }
    }

    private DropdownEntry? GetEntryByElement(DropdownListElement<T> element)
    {
        return _entries.Where(entry => entry.Element == element).FirstOrDefault();
    }

    private void SetIsElementSelectedInternal(DropdownListElement<T> element, bool isSelected, bool raiseEvent)
    {
        DropdownEntry? Entry = GetEntryByElement(element);
        if (Entry == null)
        {
            return;
        }

        (var NewElementList, var ModifiedElements) = GetElementLists(element, isSelected);
        
        BasicDropdownSelectionUpdateEventArgs<T>? EventArgs = null;
        if (raiseEvent)
        {
            EventArgs = new(this, NewElementList);
            SelectionUpdate?.Invoke(this, EventArgs);

            if (EventArgs.IsCancelled)
            {
                EventArgs.ExecuteActions();
                return;
            }
        }

        _selectedElements.Clear();
        _selectedElements.AddRange(NewElementList);
        foreach (var Element in ModifiedElements)
        {
            UpdateSingleEntryProperties(GetEntryByElement(Element)!);
        }
        EventArgs?.ExecuteActions();
    }

    private (List<DropdownListElement<T>> NewElementList, List<DropdownListElement<T>> ModifiedElements) GetElementLists(
        DropdownListElement<T> elementModified, bool isSelected)
    {
        List<DropdownListElement<T>> NewElementList = new(_selectedElements);
        List<DropdownListElement<T>> ModifiedElements = new();

        if (isSelected && !NewElementList.Contains(elementModified))
        {
            int TargetElementCount = Math.Max(0, MaxSelectedElementCount - 1);
            while (NewElementList.Count > TargetElementCount)
            {
                ModifiedElements.Add(NewElementList[0]);
                NewElementList.RemoveAt(0);
            }
            NewElementList.Add(elementModified);
            ModifiedElements.Add(elementModified);
        }
        else if (NewElementList.Count > MinSelectedElementCount)
        {
            NewElementList.Remove(elementModified);
            ModifiedElements.Add(elementModified);
        }

        return (NewElementList, ModifiedElements);
    }

    private void OnElementCreateSoundEvent(object? sender, BasicButtonSoundEventArgs args)
    {
        BasicDropdownPlaySoundEventArgs<T> EventArgs = new(this, args.Sound, args.Origin);
        if (EventArgs.IsCancelled)
        {
            args.IsCancelled = true;
        }

        EventArgs.ExecuteActions();
    }


    // Inherited methods.
    public void Initialize()
    {
        _displayButton.Initialize();
    }

    public void Deinitialize()
    {
        _displayButton.Deinitialize();
        ClearElements();
    }

    public void SetIsElementSelected(DropdownListElement<T> element, bool isSelected)
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));
        SetIsElementSelectedInternal(element, isSelected, true);
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

        UpdateElementPositions();
        UpdateEntryButtonStates();
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
        _selectedElements.Remove(Entry.Element);
        DeinitializeEntry(Entry);

        UpdateElementPositions();
        UpdateEntryButtonStates();
        UpdateDisplayButtonProperties();
        UpdateArrowIndicatorPositions();
        UpdateArrowIndicatorVisibility();
    }

    public void SetElements(IEnumerable<DropdownListElement<T>>? elements)
    {
        ClearElements();

        foreach (DropdownListElement<T> Element in elements ?? Enumerable.Empty<DropdownListElement<T>>())
        {
            DropdownEntry Entry = new(_buttonCreator.Invoke(), Element);
            InitializeEntry(Entry);
            _entries.Add(Entry);
        }

        UpdateElementPositions();
        UpdateEntryButtonStates();
        UpdateArrowIndicatorPositions();
        UpdateArrowIndicatorVisibility();
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
        _selectedElements.Clear();
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
        IsTargeted = IsEnabled && (IsTargeted && _input.VirtualMousePositionCurrent == _input.VirtualMousePositionPrevious
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

        _nearIndicator.Render(renderer, time);
        _farIndicator.Render(renderer, time);
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

        for (int i = _scrollIndex; i < _scrollIndex + _clampedMaxPopupElementCount; i++)
        {
            DropdownEntry TargetEntry = _entries[i];
            if (TargetEntry.Button.IsPositionOverButton(position))
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