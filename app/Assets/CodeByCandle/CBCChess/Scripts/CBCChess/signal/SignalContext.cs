using System;

using UnityEngine;

using strange.extensions.context.impl;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace cbc.cbcchess
{
	public class SignalContext:MVCSContext
	{
		public SignalContext(MonoBehaviour contextView) : base(contextView)
		{
		}

		protected override void addCoreComponents()
		{
			base.addCoreComponents();
			
			// bind signal command binder
			injectionBinder.Unbind<ICommandBinder>();
			injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
		}

		public override void Launch()
		{
			base.Launch();

			// fire start signal!
			(injectionBinder.GetInstance<StartSignal>()).Dispatch();
		}
	}
}