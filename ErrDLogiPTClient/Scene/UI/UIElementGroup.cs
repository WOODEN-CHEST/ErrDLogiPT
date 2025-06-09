using ErrDLogiPTClient.Scene.UI.Button;
using ErrDLogiPTClient.Scene.UI.Slider;
using GHEngine;
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
    // Fields.
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            foreach (IUIElement Element in _elements)
            {
                Element.IsEnabled = _isEnabled;
            }
        }
    }

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;
            foreach (IUIElement Element in _elements)
            {
                Element.IsVisible = _isVisible;
            }
        }
    }


    // Private fields.
    private readonly HashSet<IUIElement> _elements = new();
    private bool _isEnabled = false;
    private bool _isVisible = false;


    // Methods.
    public void Add(params IUIElement[] elements)
    {
        ArgumentNullException.ThrowIfNull(elements, nameof(elements));

        foreach (IUIElement Element in elements)
        {
            _elements.Add(Element);
            Element.IsEnabled = IsEnabled;
            Element.IsVisible = IsVisible;
        }
    }

    public void Remove(params IUIElement[] elements)
    {
        ArgumentNullException.ThrowIfNull(elements, nameof(elements));

        foreach (IUIElement Element in elements)
        {
            _elements.Add(Element);
            Element.IsEnabled = IsEnabled;
            Element.IsVisible = IsVisible;
        }
    }

    public IEnumerator<IUIElement> GetEnumerator()
    {
        return _elements.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Update(IProgramTime time)
    {
        if (!IsEnabled)
        {
            return;
        }

        foreach (IUIElement Element in _elements)
        {
            Element.Update(time);
        }
    }
}