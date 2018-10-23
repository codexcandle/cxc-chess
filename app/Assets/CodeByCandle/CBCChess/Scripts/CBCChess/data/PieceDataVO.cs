using UnityEngine;
using System.Collections;

namespace cbc.cbcchess
{
	public class PieceDataVO
	{
		public int playerIndex;
		public int pieceIndex;
		public int typeIndex;
		public int locationIndex;
		public string prefabID;

		/* this will be used for comparison to 
		 * determine when a piece has reached
		 * a targetIndex that is on its 
		 * "opposite side" or "enemy's side" - 
		 * potentially rewarding player with 
		 * chance to reclaim taken piece. 
		 * (e.g. player 1 reaches index 63; 
		 * other side!)*/
		public int[] terminatingRowIndices;
		
		public PieceDataVO(int playerIndex,
		                   int pieceIndex,
		                   int typeIndex,	
		                   string prefabID,
		                   int locationIndex,
		                   int[] terminatingRowIndices)
		{
			this.playerIndex = playerIndex;
			this.pieceIndex = pieceIndex;
			this.typeIndex = typeIndex;
			this.prefabID = prefabID;
			this.locationIndex = locationIndex;
			this.terminatingRowIndices = terminatingRowIndices;
		}
	}
}