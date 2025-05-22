using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

public abstract class CancellableEventBase : EventArgs, ICancellableEventArgs
{
    // Fields.
    public bool IsCancelled { get; set; } = false;


    // Private fields.
    private readonly List<Action> _failureActions = new();
    private readonly List<Action> _successActions = new();
    private readonly List<Action> _postActions = new();


    // Constructors.
    public CancellableEventBase() { }

    // Methods.
    public void AddSuccessAction(Action action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));
        _successActions.Add(action);
    }

    public void AddFailureAction(Action action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));
        _failureActions.Add(action);
    }

    public void AddPostEventAction(Action action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));
        _postActions.Add(action);
    }

    public void ExecuteActions()
    {
        if (IsCancelled)
        {
            foreach (Action TargetAction in _failureActions)
            {
                TargetAction.Invoke();
            }
        }
        else
        {
            foreach (Action TargetAction in _successActions)
            {
                TargetAction.Invoke();
            }
        }

        foreach (Action TargetAction in _postActions)
        {
            TargetAction.Invoke();
        }
    }
}