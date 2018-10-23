using System;

using UnityEngine;

using System.Collections.Generic;

using strange.extensions.context.api;
using strange.extensions.command.impl;
		
using cbc.cbcutils;

namespace cbc.cbcchess
{
	public class StubGameCommand:Command
	{
		[Inject]
		public StubMoveSignal response { get; set; }

		public override void Execute()
		{
			base.Execute();

			// get list
			// List<RequestMoveVO> requests = new List<RequestMoveVO>();
			RequestMoveVO request = new RequestMoveVO(1, 63, 0);
			// requests.Add(new RequestMoveVO(0, 0,63));

			response.Dispatch(request);
		}
	}
}