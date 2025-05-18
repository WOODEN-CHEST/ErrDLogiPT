using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

public class DefaultGamePathStructure : IGamePathStructure
{
    // Static fields.
    public const string DIR_NAME_LOGS = "logs";
    public const string DIR_NAME_LOG_ARCHIVE = "old";
    public const string DIR_NAME_LOG_NAME = "latest.log";
    public const string DIR_NAME_ASSET_ROOT = "asset";
    public const string DIR_NAME_ASSET_DEF_ROOT = "definition";
    public const string DIR_NAME_MOD_ROOT = "asset";
    public const string DIR_NAME_USER_DATA = "err_d_logi_pt";


    // Fields.
    public virtual string ExecutableRoot { get; init; }
    public virtual string LogRoot { get; init; }
    public virtual string LogArchiveRoot { get; init; }
    public virtual string LatestLogPath { get; init; }
    public virtual string AssetRoot { get; init; }
    public virtual string AssetDefRoot { get; init; }
    public virtual string AssetValueRoot { get; init; }
    public virtual string ModRoot { get; init; }
    public virtual string UserDataRoot { get; init; }


    // Constructors.
    public DefaultGamePathStructure(string executableDirPath)
    {
        ExecutableRoot = executableDirPath ?? throw new ArgumentNullException(nameof(executableDirPath));

        UserDataRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DIR_NAME_USER_DATA);

        LogRoot = Path.Combine(UserDataRoot, DIR_NAME_LOGS);
        LogArchiveRoot = Path.Combine(LogRoot, DIR_NAME_LOG_ARCHIVE);
        LatestLogPath = Path.Combine(LogRoot, DIR_NAME_LOG_NAME);

        AssetRoot = Path.Combine(ExecutableRoot, DIR_NAME_ASSET_ROOT);
        AssetDefRoot = Path.Combine(AssetRoot, DIR_NAME_ASSET_DEF_ROOT);
        AssetValueRoot = AssetRoot;

        ModRoot = Path.Combine(UserDataRoot, DIR_NAME_MOD_ROOT);
    }
}