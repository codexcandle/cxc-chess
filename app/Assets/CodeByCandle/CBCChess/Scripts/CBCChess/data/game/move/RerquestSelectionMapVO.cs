using UnityEngine;
using System.Collections;

namespace cbc.cbcchess
{
	public class RequestSelectionMapVO
	{
		public int playerIndex;
		
		public RequestSelectionMapVO(int playerIndex)
		{
			this.playerIndex = playerIndex;
		}
	}
}