using UnityEngine;
using System.Collections;

namespace cbc.cbcchess
{
	public class MoveVO
	{
		public int playerIndex;
	    public int pieceIndex;
		public int startIndex;
		public int destinationIndex;
		public int destroyIndex;
	    public Vector2 distance;
	    
		public MoveVO(int playerIndex, 
		              int pieceIndex, 
		              int startIndex,
		              int destinationIndex,
		              Vector2 distance,
		              int destroyIndex = -1)
	    {
			this.playerIndex = playerIndex;
	        this.pieceIndex = pieceIndex;
			this.startIndex = startIndex;
			this.destinationIndex = destinationIndex;
			this.distance = distance;
			this.destroyIndex = destroyIndex;
		}
	}
}