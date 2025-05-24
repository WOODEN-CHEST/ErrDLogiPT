using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public interface ISceneComponentHolder
{
    // Fields.
    IEnumerable<ISceneComponent> Components { get; }
    int ComponentCount { get; }


    // Methods.
    void AddComponent(ISceneComponent component);
    ISceneComponent GetComponent(int index);
    void InsertComponent(ISceneComponent component, int index);
    void RemoveComponent(ISceneComponent component);
    void ClearComponents();
}