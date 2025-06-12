using ErrDLogiPTClient.Scene.UI.Button;
using ErrDLogiPTClient.Scene.UI.Dropdown;
using ErrDLogiPTClient.Scene.UI.Slider;
using GHEngine;
using GHEngine.Frame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ErrDLogiPTClient.Scene.UI;

public class UIElementGroup : IEnumerable<IUIElement>, ITimeUpdatable
{
    // Static fields.
    public const float Z_INDEX_DEFAULT = 0f;


    // Fields.
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            UpdateElementProperties();
        }
    }

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;
            UpdateElementProperties();
        }
    }

    public bool AreExcludedElementsVisible
    {
        get => _areExcludedElementsVisible;
        set
        {
            _areExcludedElementsVisible = value;
            UpdateElementProperties();
        }
    }

    public bool AreExcludedElementsEnabled
    {
        get => _areExcludedElementsEnabled;
        set
        {
            _areExcludedElementsEnabled = value;
            UpdateElementProperties();
        }
    }

    public bool AreElementsExcluded
    {
        get => _areElementExcluded;
        set
        {
            _areElementExcluded = value;
            UpdateElementProperties();
        }
    }

    public int ActiveElementCount => _activeElements.Count;

    public IEnumerable<IUIElement> ActiveElements => _activeElements;



    // Private fields.
    private readonly ILayer? _renderLayer = null;
    private readonly Dictionary<IUIElement, ElementData> _elements = new();

    private bool _isEnabled = false;
    private bool _isVisible = false;

    private readonly HashSet<IUIElement> _activeElements = new();
    private bool _areElementExcluded = false;
    private bool _areExcludedElementsVisible = true;
    private bool _areExcludedElementsEnabled = false;


    // Constructors.
    public UIElementGroup() { }

    public UIElementGroup(ILayer? renderLayer)
    {
        _renderLayer = renderLayer;
    }


    // Private methods.
    private void UpdateElementProperties()
    {
        bool IsElementEnabledIfExcluded = !AreElementsExcluded || AreExcludedElementsEnabled;
        bool IsElementVisibleIfExcluded = !AreElementsExcluded || AreExcludedElementsVisible;

        foreach (var Entry in _elements)
        {
            bool IsElementActive = _activeElements.Contains(Entry.Key);

            Entry.Key.IsEnabled = IsEnabled 
                && ((Entry.Value.AbilityOverride.HasValue && Entry.Value.AbilityOverride.Value)
                || (!Entry.Value.AbilityOverride.HasValue && (IsElementEnabledIfExcluded || IsElementActive)));

            bool IsThisElementVisible = IsVisible
                && ((Entry.Value.VisibilityOverride.HasValue && Entry.Value.VisibilityOverride.Value)
                || (!Entry.Value.VisibilityOverride.HasValue && (IsElementVisibleIfExcluded || IsElementActive)));

            if (_renderLayer != null)
            {
                _renderLayer.RemoveItem(Entry.Key);
                if (IsThisElementVisible)
                {
                    _renderLayer.AddItem(Entry.Key, Entry.Value.ZIndex);
                }
            }
        }
    }


    // Methods.
    public void AddElement(IUIElement element, float zIndex = Z_INDEX_DEFAULT)
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));

        if (_elements.ContainsKey(element))
        {
            return;
        }
        _elements[element] = new(element, zIndex);

        UpdateElementProperties();
    }

    public void AddElements(float zIndex, params IUIElement[] elements)
    {
        ArgumentNullException.ThrowIfNull(elements, nameof(elements));

        foreach (IUIElement Element in elements)
        {
            _elements[Element] = new(Element, zIndex);
        }

        UpdateElementProperties();
    }

    public void RemoveElement(IUIElement element)
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));

        _elements.Remove(element);
        _activeElements.Remove(element);
        element.IsEnabled = false;
        element.IsVisible = false;
        _renderLayer?.RemoveItem(element);
    }

    public void RemoveElements(params IUIElement[] elements)
    {
        ArgumentNullException.ThrowIfNull(elements, nameof(elements));

        foreach (IUIElement Element in elements)
        {
            _elements.Remove(Element);
        }

        UpdateElementProperties();
    }

    public void SetActiveElements(params IUIElement[] elements)
    {
        ArgumentNullException.ThrowIfNull(elements, nameof(elements));

        _activeElements.Clear();
        foreach (IUIElement Element in  elements)
        {
            _activeElements.Add(Element);
        }
        UpdateElementProperties();
    }

    public void AddActiveElement(IUIElement element)
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));
        _activeElements.Add(element);
        UpdateElementProperties();
    }

    public void RemoveActiveElement(IUIElement element)
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));
        _activeElements.Remove(element);
        UpdateElementProperties();
    }

    public void ClearActiveElements()
    {
        _activeElements.Clear();
        UpdateElementProperties();
    }

    public void SetAbilityOverride(IUIElement element, bool? value)
    {
        if (_elements.TryGetValue(element, out ElementData? Data))
        {
            Data.AbilityOverride = value;
            UpdateElementProperties();
        }
    }

    public void SetVisibilityOverride(IUIElement element, bool? value)
    {
        if (_elements.TryGetValue(element, out ElementData? Data))
        {
            Data.VisibilityOverride = value;
            UpdateElementProperties();
        }
    }

    public void Initialize()
    {
        foreach (IUIElement Element in _elements.Keys)
        {
            Element.Initialize();
        }
    }

    public void Deinitialize()
    {
        foreach (IUIElement Element in _elements.Keys)
        {
            Element.Deinitialize();
        }
    }


    // Inherited methods.
    public IEnumerator<IUIElement> GetEnumerator()
    {
        return _elements.Keys.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Update(IProgramTime time)
    {
        foreach (IUIElement Element in _elements.Keys)
        {
            Element.Update(time);
        }
    }


    // Types.
    private record class ElementData(IUIElement Element, float ZIndex)
    {
        public bool? AbilityOverride { get; set; } = null;
        public bool? VisibilityOverride { get; set; } = null;
    }
}