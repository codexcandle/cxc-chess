using System;
using UnityEngine;
using strange.extensions.mediation.impl;

namespace cbc.cbcchess
{
	public class MenuMediator:Mediator
	{
		[Inject]
		public MenuView view { get; set; }
		
		[Inject]
		public LoadGameSignal loadGameSignal {get; set;}
		
		public override void OnRegister()
		{
			base.OnRegister();

			// add listeners
			view.playComputerClick.AddOnce(onClickPlayComputer);
			view.playNetworkClick.AddOnce(onClickPlayNetwork);
		}

		private void onClickPlayComputer()
		{
			loadGameSignal.Dispatch(GameType.VsComputer);
		}

		private void onClickPlayNetwork()
		{
			loadGameSignal.Dispatch(GameType.VsNetwork);
		}
	}
}