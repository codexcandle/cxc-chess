
using UnityEngine;

using System.Collections.Generic;

using strange.extensions.context.api;
using strange.extensions.command.impl;

using cbc.cbcutils;

namespace cbc.cbcchess
{
	public class CompleteMoveCommand:CBCCommand
	{
		[Inject]
		public CameraFocusPlayerSignal cameraFocusPlayerSignal {get; set;}

		[Inject]
		public IGameModel gameModel { get; set; }
		
		public override void Execute()
		{
			base.Execute();

			if(gameModel.active)
			{
//				gameModel.SetState(GameState.MoveComplete);

				int newPlayerIndex = (gameModel.player == 0) ? 1 : 0;

				gameModel.LoadPlayer(newPlayerIndex);

//				cameraFocusPlayerSignal.Dispatch(newPlayerIndex);
			}
			else
			{
//				gameModel.SetState(GameState.GameOver);
			}
		}
	}
}