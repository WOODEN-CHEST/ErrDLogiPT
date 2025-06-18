using ErrDLogiPTClient.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Wrapper;

public class WrappedServiceGamePathStructure : ServiceWrapper<IGamePathStructure>, IGamePathStructure
{
    // Fields.
    public string ExecutableRoot => ServiceObject.ExecutableRoot;
    public string UserDataRoot => ServiceObject.UserDataRoot;
    public string LogRoot => ServiceObject.LogRoot;
    public string LogArchiveRoot => ServiceObject.LogArchiveRoot;
    public string LatestLogPath => ServiceObject.LatestLogPath;
    public string AssetRoot => ServiceObject.AssetRoot;
    public string AssetDefRoot => ServiceObject.AssetDefRoot;
    public string AssetValueRoot => ServiceObject.AssetValueRoot;
    public string ModRoot => ServiceObject.ModRoot;
    public string ResourcePackRoot => ServiceObject.ResourcePackRoot;



    // Constructors.
    public WrappedServiceGamePathStructure(IGenericServices services) : base(services) { }
}