using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Mod;

public class ModLoadException : Exception
{
    public ModLoadException(string message) : base(message) { }
}