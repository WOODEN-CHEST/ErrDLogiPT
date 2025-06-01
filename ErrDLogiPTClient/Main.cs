using ErrDLogiPTClient;

bool IsRestartScheduled = false;

do
{
    using var game = new LogiGame();
    game.Run();
    IsRestartScheduled = game.LogiGameServices?.Get<IAppStateController>()?.IsRestartScheduled ?? false;
} while (IsRestartScheduled);
