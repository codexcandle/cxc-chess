using System;

using UnityEngine;

using System.Collections.Generic;

using strange.extensions.context.api;
using strange.extensions.command.impl;

using cbc.cbcutils;

namespace cbc.cbcchess
{
	public class ValidateMoveCommand:CBCCommand
	{
		[Inject]
		public IGameModel gameModel { get; set;}

		[Inject]
		public RequestMoveVO request { get; set; }
	
		[Inject]
		public MovePieceSignal response { get; set; }

		public override void Execute()
		{
			base.Execute();

			List<MoveVO> moves = new List<MoveVO>();
			MoveVO move = gameModel.RequestMove(request);
			moves.Add(move);

			response.Dispatch(moves);
		}
	}
}