using System;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;

using cbc.cbcutils;

namespace cbc.cbcchess
{
	public class GameMediator:CBCMediator
	{
		[Inject]
		public GameView view { get; set; }

		[Inject]
		public GameStateChangeSignal gameStateChangeSignal {get; set;}
	
		[Inject]
		public RequestStartNewGameSignal requestStartNewGameSignal { get; set; }

		// functions (public) -----------------------------------
		public override void OnRegister()
		{
			base.OnRegister();

			enableListeners(true);

			requestStartNewGameSignal.Dispatch();
		}
		
		public override void OnRemove()
		{
			enableListeners(false);
		}

		// functions (private) ----------------------------------
		private void enableListeners(bool enable)
		{
			if(enable)
			{
				// ... app
				gameStateChangeSignal.AddListener(onGameStateChange);
				// ... view
			}
			else
			{
				// ... app
				gameStateChangeSignal.RemoveListener(onGameStateChange);
				// ... view
			}
		}

		private void onGameStateChange(GameState state)
		{
			Debug.Log("state @ gameMediator: " + state);

			// if(state == GameState.StartGame)
			// {
				// view.beginTurn();
			// }
		}
	}
}