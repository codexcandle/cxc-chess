using System;

using UnityEngine;

using System.Collections.Generic;

using strange.extensions.context.api;
using strange.extensions.command.impl;

using cbc.cbcutils;

namespace cbc.cbcchess
{
	public class RequestSelectionMapCommand:CBCCommand
	{
		[Inject]
		public IGameModel gameModel { get; set; }

		[Inject]
		public RequestSelectionMapVO request {get; set;}
		
		[Inject]
		public UpdateSelectionMapSignal response { get; set; }
		
		public override void Execute()
		{
			base.Execute();

			// gameModel.SetState(GameState.SelectPiece);

			response.Dispatch(new UpdateSelectionMapVO(gameModel.RequestMoveStartCandidates(request.playerIndex)));
		}
	}
}