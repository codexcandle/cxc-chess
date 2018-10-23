using UnityEngine;
using System.Collections;

namespace cbc.cbcchess
{
	public class RequestMoveVO
	{
		public int playerIndex;
		public int startIndex;
		public int destinationIndex;
		
		public RequestMoveVO(int playerIndex, 
		                     int startIndex,
		                     int destinationIndex)
		{
			this.playerIndex = playerIndex;
			this.startIndex = startIndex;
			this.destinationIndex = destinationIndex;
		}
	}
}