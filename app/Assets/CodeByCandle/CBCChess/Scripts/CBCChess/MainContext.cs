using UnityEngine;
using System.Collections;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.command.api;
using strange.extensions.command.impl;

namespace cbc.cbcchess
{
	public class MainContext:SignalContext
	{
		public MainContext(MonoBehaviour view) : base(view)
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

			// injectionBinder.Bind<IAppModel>().To<AppModel>().ToSingleton().CrossContext();
			// injectionBinder.Bind<IGameModel>().To<GameModel>().ToSingleton();

			// bind command to startSignal (since invoked by signalContext parent class during onLaunch)
			commandBinder.Bind<StartSignal>().To<LoadMenuCommand>().Once();

			commandBinder.Bind<LoadGameSignal>().To<LoadGameCommand>().Once();
		}
	}
}