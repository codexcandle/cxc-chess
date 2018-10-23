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
	public class MenuContext:SignalContext
	{
		public MenuContext(MonoBehaviour contextView):base(contextView)
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

			injectionBinder.Bind<IAppModel>().To<AppModel>().ToSingleton().CrossContext();

			// commandBinder.Bind<DoManagementSignal>().To<DoManagementCommand>().Pooled(); // THIS IS THE NEW MAPPING!!!
			// we bind a command to StartSignal since it is invoked by SignalContext (the parent class) during on Launch()
			// commandBinder.Bind<StartSignal>().To<StartCommand>().Once();
			commandBinder.Bind<LoadGameSignal>().To<LoadGameCommand>();

			// mediator binder ////////////////////////////////
			mediationBinder.Bind<MenuView>().To<MenuMediator>();



			// injection binder ///////////////////////////////
			// ... signals
			// injectionBinder.Bind<Model>().ToSingleton().CrossContext();
// injectionBinder.Bind<MovePieceSignal>().ToSingleton().CrossContext();
			// ... managers
			// (as mono-behaviour)
// GameManager manager = GameObject.Find("manager").GetComponent<GameManager>();
// injectionBinder.Bind<ISomeManager>().ToValue(manager);
			// (as concrete implementation)
			// injectionBinder.Bind<ISomeManager>().To<ManagerAsNormalClass>().ToSingleton();
		}
		
		public override void Launch()
		{
			// base.Launch();
			
			// Debug.Log ("launch 2");
		}
	}
}