using ErrDLogiPTClient;
using ErrDLogiPTClient.Scene;

bool IsRestartScheduled = false;

do
{
    using var game = new LogiGame();
    game.Run();
    IsRestartScheduled = game.LogiGameServices?.Get<ISceneExecutor>()?.IsRestartScheduled ?? false;
} while (IsRestartScheduled);
