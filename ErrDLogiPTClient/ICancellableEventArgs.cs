using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

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