using System;

using UnityEngine;

using System.Collections.Generic;

using strange.extensions.context.api;
using strange.extensions.command.impl;

using cbc.cbcutils;

namespace cbc.cbcchess
{
	public class RequestMoveMapCommand:CBCCommand
	{
		[Inject]
		public IGameModel gameModel { get; set; }

		[Inject]
		public RequestMoveMapVO request {get; set;}

		[Inject]
		public DisplayMoveMapSignal response { get; set; }

		public override void Execute()
		{
			base.Execute();

// gameModel.SetState(GameState.SelectDestination);

			List<int> moveEndCandidates = gameModel.RequestMoveEndCandidates(request);
			response.Dispatch(new DisplayMoveMapVO(moveEndCandidates));
		}
	}
}