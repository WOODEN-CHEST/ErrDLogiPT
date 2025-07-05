using ErrDLogiPTClient.Scene.InGame;
using ErrDLogiPTClient.Service;
using System;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.MainMenu;

public class MainMenuScene : SceneBase
{
    // Protected fields.
    protected MainMenuUIExecutor UIExecutor => _uiExecutor;


    // Private fields.
    private MainMenuUIExecutor _uiExecutor;


    // Constructors.
    public MainMenuScene(IGenericServices services) : base(services) { }



    // Protected methods.
    protected virtual MainMenuUIExecutor CreateUIExecutor()
    {
        return new(this, SceneServices);
    }

    protected virtual InGameScene CreateInGameScene(InGameOSCreateOptions osCreationOptions)
    {
        return new InGameScene(GlobalGameServices, osCreationOptions);
    }

    protected virtual void HandleRequestToBootIntoOS(InGameOSCreateOptions options)
    {
        IGameScene NextScene = CreateInGameScene(options);

        ISceneExecutor SceneExecutor = SceneServices.GetRequired<ISceneExecutor>();
        SceneExecutor.ScheduleNextSceneSet(NextScene);
        SceneExecutor.ScheduleJumpToNextScene(true);
    }


    // Private methods.
    private void OnRequestBootIntoOSEvent(object? sender, UIRequestBootIntoOSEventArgs args)
    {
        args.AddSuccessAction(() =>
        {
            HandleRequestToBootIntoOS(args.Options);
        });
    }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        base.HandleLoadPreComponent();

        _uiExecutor = CreateUIExecutor();
        _uiExecutor.RequestBootIntoOS += OnRequestBootIntoOSEvent;
        AddComponent(_uiExecutor);
    }
}