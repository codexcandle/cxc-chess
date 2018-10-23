using UnityEngine;
using System.Collections;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

using StrangeCamera.Game;
using cbc.cbcchess;

namespace cbc.cbcchess
{
	public class GameContext:SignalContext
	{
		public GameContext(MonoBehaviour contextView):base(contextView)
		{
		}

		protected override void addCoreComponents()
		{
			base.addCoreComponents();
			injectionBinder.Unbind<ICommandBinder>();
			injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
		}

		protected override void mapBindings()
		{
			base.mapBindings();

			// command binder /////////////////////////////////
			// we bind a command to StartSignal since it is invoked by SignalContext (the parent class) during on Launch()
			// commandBinder.Bind<StartSignal>().To<StartCommand>().Once();
			commandBinder.Bind<RequestStartNewGameSignal>().To<RequestStartNewGameCommand>();
			commandBinder.Bind<StartGameSignal>().To<StartGameCommand>();
			commandBinder.Bind<ValidateMoveSignal>().To<ValidateMoveCommand>();
			commandBinder.Bind<RequestMoveMapSignal>().To<RequestMoveMapCommand>();
			commandBinder.Bind<RequestSelectionMapSignal>().To<RequestSelectionMapCommand>();
			commandBinder.Bind<PlaySoundSignal>().To<PlaySoundCommand>();
			commandBinder.Bind<RecordMoveSignal>().To<RecordMoveCommand>();
			commandBinder.Bind<StubGameSignal>().To<StubGameCommand>().Pooled(); // THIS IS THE NEW MAPPING!!!
			commandBinder.Bind<SetupGameSignal>().To<SetupGameCommand>();
			// commandBinder.Bind<CameraSequenceSignal>().To<CameraFlythroughCommand>().To<CameraAttachCommand>().InSequence();
			commandBinder.Bind<CameraSequenceSignal>().To<CameraFlythroughCommand>();
			commandBinder.Bind<CompleteMoveSignal>().To<CompleteMoveCommand>();
			commandBinder.Bind<CameraMoveCompleteSignal>().To<CameraMoveCompleteCommand>();
			commandBinder.Bind<CameraPlayerZoomCompleteSignal>().To<CameraPlayerZoomCompleteCommand>();
			commandBinder.Bind<EndGameSignal>().To<EndGameCommand>();

			// mediator binder ////////////////////////////////
			mediationBinder.Bind<GameView>().To<GameMediator>();
			mediationBinder.Bind<BoardView>().To<BoardMediator>();
			mediationBinder.Bind<PiecesView>().To<PiecesMediator>();
			mediationBinder.Bind<GuiView>().To<GuiMediator>();
			mediationBinder.Bind<CameraView>().To<CameraMediator>();
			mediationBinder.Bind<ScoreboardView>().To<ScoreboardMediator>();
			mediationBinder.Bind<ReclaimView>().To<ReclaimMediator>();

			// injection binder ///////////////////////////////
			// ... managers
			// (as concrete implementation)
			// injectionBinder.Bind<ISomeManager>().To<ManagerAsNormalClass>().ToSingleton();
			// (as mono-behaviour)
			// GameManager gameManager = GameObject.Find("manager_game").GetComponent<GameManager>();
			// injectionBinder.Bind<IGameManager>().ToValue(gameManager);
			// bind the manager implemented as a MonoBehaviour
			// NetworkManager networkManager = GameObject.Find("manager_network").GetComponent<NetworkManager>();
			// injectionBinder.Bind<IBasicManager>().ToValue(networkManager);
			// ... models
			injectionBinder.Bind<IGameModel>().To<GameModel>().ToSingleton();
			injectionBinder.Bind<ICamera>().To<CameraModel>().ToSingleton();
			// ... signals
			injectionBinder.Bind<CameraStateSignal>().ToSingleton();
			injectionBinder.Bind<FlythroughCompleteSignal>().ToSingleton();
			injectionBinder.Bind<StubMoveSignal>().ToSingleton().CrossContext();
			injectionBinder.Bind<MovePieceSignal>().ToSingleton().CrossContext();
			injectionBinder.Bind<DestroyPieceSignal>().ToSingleton().CrossContext();
			injectionBinder.Bind<UpdateSelectionMapSignal>().ToSingleton().CrossContext();
			injectionBinder.Bind<DisplayMoveMapSignal>().ToSingleton().CrossContext();
			// injectionBinder.Bind<EndGameSignal>().ToSingleton().CrossContext();
			injectionBinder.Bind<GameStateChangeSignal>().ToSingleton().CrossContext();
			injectionBinder.Bind<CameraFocusPlayerSignal>().ToSingleton().CrossContext();
			injectionBinder.Bind<ShowRescuePanelSignal>().ToSingleton();
			injectionBinder.Bind<PieceDeselectedSignal>().ToSingleton();
		}

		public override void Launch()
		{
			// base.Launch();

			Signal setupGameSignal = injectionBinder.GetInstance<SetupGameSignal>();
			setupGameSignal.Dispatch();
		}
	}
}