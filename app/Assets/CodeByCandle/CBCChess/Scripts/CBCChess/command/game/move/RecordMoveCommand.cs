using System;

using UnityEngine;

using System.Collections.Generic;

using strange.extensions.context.api;
using strange.extensions.command.impl;

using cbc.cbcutils;

namespace cbc.cbcchess
{
	public class RecordMoveCommand:CBCCommand
	{
		[Inject]
		public IGameModel gameModel { get; set; }

		[Inject]
		public MoveVO move {get; set;}
		
		public override void Execute()
		{
			base.Execute();

			bool continueGame = gameModel.RecordMove(move);
			if(!continueGame)
				gameModel.EndGame(move.playerIndex);
		}
	}
}