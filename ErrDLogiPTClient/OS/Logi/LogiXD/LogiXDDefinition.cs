using ErrDLogiPTClient.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.OS.Logi.LogiXD;

public class LogiXDDefinition : AbstractGameOsDefinition
{
    // Static fields.
    public const string NAME = "Logi XD";
    public const string DESCRIPTION = "Logi XD, released shortly after Logi 9000, is an all-around great operating system. " +
        "It is light, easy to use and has all the features you could possibly need... in 2001 at least.";
    public const string DEF_KEY = "logi_xd";
    public static readonly DateTime RELEASE_DATE = new(new DateOnly(2001, 4, 17), new TimeOnly(16, 34, 6));


    // Constructors.
    public LogiXDDefinition() : base(NAME, DESCRIPTION, RELEASE_DATE, DEF_KEY) { }


    // Inherited methods.
    protected override IGameOSInstance CreateOSInstance(IGenericServices sceneServices)
    {
        return new LogiXDSystem(this, sceneServices);
    }
}