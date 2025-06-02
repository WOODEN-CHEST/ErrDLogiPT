using ErrDLogiPTClient.Scene.Sound;
using GHEngine.Audio.Source;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

public interface IBasicDropdownList<T> : IUIElement
{
    // Fields.
    Vector2 Position { get; set; }
    int MaxDisplayedElementCount { get; set; }
    int ScrollIndex { get; set; }
    int MaxSelectedElementCount { get; set; }
    int ElementCount { get; }
    IEnumerable<DropdownListElement<T>> Elements { get; }
    Color ValueDisplayColor { get; set; }
    Color ValueDisplayHoverColor { get; set; }
    Color ValueDisplayChangeColor { get; set; }
    TimeSpan HoverColorDuration { get; set; }
    TimeSpan ValueChangeColorDuration { get; set; }
    Color DefaultElementColor { get; set; }
    Color DefaultElementHoverColor { get; set; }
    Color DefaultElementClickColor { get; set; }
    Color DefaultElementSelectedColor { get; set; }
    Color DefaultElementUnavailableColor { get; set; }
    TimeSpan ElementPopupDelay { get; set; }
    bool IsTextShadowEnabled { get; set; }
    float Volume {  get; set; }
    IEnumerable<IPreSampledSound> ClickSounds { get; set; }
    IEnumerable<IPreSampledSound> HoverStartSounds { get; set; }
    IEnumerable<IPreSampledSound> HoverEndSounds { get; set; }
    LogiSoundCategory SoundCategory { get; set; }
    float Length { get; set; }
    float Scale { get; set; }
    ButtonClickMethod ClickMethod { get; set; }
    bool IsTargeted { get; set; }

    event EventHandler<EventArgs>? Expand;
    event EventHandler<EventArgs>? Contract;
    event EventHandler<EventArgs>? SelectionUpdate;
    event EventHandler<EventArgs>? PlaySound;

    // Methods.
    void SetIsElementSelected(DropdownListElement<T> element, bool isSelected);
    bool IsElementSelected(DropdownListElement<T> element);
    bool IsPositionOverList(Vector2 position);
    void SetElements(IEnumerable<DropdownListElement<T>>? elements);
    void AddElement(DropdownListElement<T>? element);
    void InsertElement(DropdownListElement<T>? element, int index);
    void RemoveElement(DropdownListElement<T> element);
    void RemoveElement(int index);
    void ClearElements();
    bool ContainsElement(DropdownListElement<T> element);
}