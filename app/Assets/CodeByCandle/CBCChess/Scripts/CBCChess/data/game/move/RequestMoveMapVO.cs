using UnityEngine;
using System.Collections;

namespace cbc.cbcchess
{
	public class RequestMoveMapVO
	{
		public int playerIndex;
		public int startIndex;
		
		public RequestMoveMapVO(int playerIndex,
		                        int startIndex)
		{
			this.playerIndex = playerIndex;
			this.startIndex = startIndex;
		}
	}
}