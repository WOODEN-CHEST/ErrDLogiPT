using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

/// <summary>
/// A cancellable event is one which has some action which is performed if the event isn't canceled,
/// or performs no action if the event is canceled.
/// <para>A cancellable event also supports queuing actions to be performed if the event is canceled or succeeds.</para>
/// </summary>
public interface ICancellableEventArgs
{
    // Fields.
    bool IsCancelled { get; set; }


    // Methods.
    void AddSuccessAction(Action action);
    void AddFailureAction(Action action);
    void AddPostEventAction(Action action);
    void ExecuteActions();
}