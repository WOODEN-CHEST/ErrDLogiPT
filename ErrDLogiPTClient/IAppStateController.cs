using GHEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

public interface IAppStateController : ITimeUpdatable
{
    // Fields.
    bool CanSwitchFullScreen { get; set; }
    bool IsRestartScheduled { get; }


    // Fields.
    void Exit();
    void Restart();
}