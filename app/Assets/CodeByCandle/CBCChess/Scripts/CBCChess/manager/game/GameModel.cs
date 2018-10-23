using UnityEngine;
using System.Collections.Generic;
using StrangeCamera.Game;

namespace cbc.cbcchess
{
	public class GameModel:IGameModel
	{
		// signals -----------------------------------------------------
		[Inject]
		public GameStateChangeSignal gameStateChangeSignal { get; set; }

		[Inject]
		public ShowRescuePanelSignal showRescuePanelSignal { get; set; }

		// getters / setters -------------------------------------------
		public bool initialized
		{
			get
			{
				return _initialized;
			}
		}

		// ... game type
		public GameType gameType
		{
			get
			{
				return _type;
			}
		}

		public void SetGameType(GameType value)
		{
			_type = value;
		}

		// ... state
		public GameState state
		{
			get
			{
				return _state;
			}
		}
	
		// ... game active
		public bool active
		{
			get
			{
				return _active;
			}
		}

		public void SetActive(bool value)
		{
			_active = value;
		}

		// ... active player
		public int player
		{
			get
			{
				return _player;
			}
		}

		// ... victor
		public int victor
		{
			get
			{
				return _victor;
			}
		}

		public void SetVictor(int value)
		{
			_victor = value;
		}

		// vars (private) ----------------------------------------------
		private static bool _initialized = false;
		private GameType _type;
		private GameState _state;
		private bool _active;
		private int _player;
		private int _victor;
		private List<PieceDataVO> pieces;
		private Dictionary<int, int> map;
		private List<MoveVO> moves;
		// TODO - refactor this to more efficient var
		private int[] terminatingIndices1;
		private int[] terminatingIndices2;

		public void Setup()
		{
			if(!_initialized)
			{
				// get terminating indices /////////////////////////
				int rowCount = GameConstants.ROW_CELL_COUNT;
				
				// ... side #1
				terminatingIndices1 = new int[rowCount];
				for(int i = 0; i < rowCount; i++)
				{
					terminatingIndices1[i] = i;
				}
				
				// ... side #2
				int maxCount = rowCount * rowCount;
				int startIndex = maxCount - rowCount;
				terminatingIndices2 = new int[rowCount];
				for(int j = 0; j < rowCount; j++)
				{
					terminatingIndices2[j] = startIndex + j;
				}
				////////////////////////////////////////////////////

				int count = GameConstants.ROW_CELL_COUNT * 4;

				Dictionary<int, int> map = new Dictionary<int, int>();
				List<PieceDataVO> pieces = new List<PieceDataVO>();
				List<string> prefabIDsP1 = GetPlayerPieceSetPrefabIDList(0);
				List<string> prefabIDsP2 = GetPlayerPieceSetPrefabIDList(1);
				int countHalfway = (int)(count / 2);

				for(int i = 0; i < count; i++)
				{
					int playerIndex = -1;
					int pieceIndex = -1;
					int typeIndex = -1;
					string prefabID = string.Empty;
					int locationIndex = -1;

					if(i < countHalfway)
					{
						playerIndex = 0;
						pieceIndex = i;
						typeIndex = GetTypeIndex(i);
						prefabID = prefabIDsP1[i];
						locationIndex = i;
					}
					else
					{
						playerIndex = 1;
						pieceIndex = i;

						int offSetIndex = i - countHalfway;
						typeIndex = GetTypeIndex(offSetIndex);
						prefabID = prefabIDsP2[offSetIndex];
						locationIndex = 63 - offSetIndex;
					}

					PieceDataVO piece = new PieceDataVO(playerIndex, 
						                    pieceIndex, 
						                    typeIndex, 
						                    prefabID, 
						                    locationIndex, 
						                    ((playerIndex == 0) ? terminatingIndices2 : terminatingIndices1));

					pieces.Add(piece);
					map[piece.locationIndex] = piece.pieceIndex;		
				}

				this.pieces = pieces;
				this.map = map;

				_initialized = true;

				/*
				Debug.Log ("----------******************--------> recoding moves.......");
				int dictCount = 0;
				foreach(KeyValuePair<int, int> entry in map)
				{
					Debug.Log ((++dictCount) + ". map key:" + entry.Key + " val:" + entry.Value);
					// do something with entry.Value or entry.Key
				}
				*/
			}
		}

		public void BeginGame()
		{
			SetState(GameState.STARTING);

			_active = true;
		}

		public void LoadPlayer(int playerIndex)
		{
			_player = playerIndex;

			SetState(GameState.LOAD_PLAYER);
		}

		public void PlayTurn()
		{
			SetState(GameState.TURN_READY);
		}

		public List<PieceDataVO> GetPieceData()
		{
			// NOTE: passing back "copy" of data
			return (pieces != null) ? new List<PieceDataVO>(pieces) : null;
		}

		public List<int> RequestMoveStartCandidates(int playerIndex)
		{
			List<int> returnVal = new List<int>();
			
			int count = pieces.Count;
			for(int i = 0; i < count; i++)
			{
				PieceDataVO piece = pieces[i];
				if(piece.playerIndex == playerIndex)
				{
					List<int> list = GetMoveEndCandidates(playerIndex, piece.locationIndex);
					if(list != null)
						returnVal.Add(piece.locationIndex);
				}
			}
			
			return returnVal;
		}

		public List<int> RequestMoveEndCandidates(RequestMoveMapVO vo)
		{
			List<int> returnVal = GetMoveEndCandidates(vo.playerIndex, vo.startIndex);

			return returnVal;
		}

		public int GetTypeIndex(int pieceIndex)
		{
			int returnIndex = -1;

			// TODO - fix this hack?
			int teamPieceCount = GameConstants.ROW_CELL_COUNT * 2;
			if(pieceIndex >= teamPieceCount)
			{
				pieceIndex -= teamPieceCount;
			}

			switch(pieceIndex)
			{
				case 0:
				case 7:
					returnIndex = (int)PieceType.rook;
					break;
				case 1:
				case 6:
					returnIndex = (int)PieceType.knight;
					break;
				case 2:
				case 5:
					returnIndex = (int)PieceType.bishop;
					break;
				case 3:
					returnIndex = (int)PieceType.queen;
					break;
				case 4:
					returnIndex = (int)PieceType.king;
					break;
				case 8:
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
					returnIndex = (int)PieceType.pawn;
					break;
			}

			// Debug.Log(" ******________ pieceIndex: " + pieceIndex + " returnIndex:" + returnIndex);

			return returnIndex;
		}

		public MoveVO RequestMove(RequestMoveVO request)
		{
			MoveVO response = new MoveVO(-1, request.startIndex, -1, -1, new Vector2(0, 0), -1);
			
			if(map.ContainsKey(request.startIndex))
			{
				// get move owner index
				// Vector2 startPos = GetPositionFromIndex(request.startIndex);
				// int ownerIndex = (pieceNumberMap[(int)startPos.x, (int)startPos.y]) - 1;
				// response.pieceIndex = ownerIndex;
				
				// TODO - elim dupe code found in ValidateMove
				
				int destroyIndex = -1;
				if(map.ContainsKey(request.destinationIndex))
				{
					destroyIndex = map[request.destinationIndex];
				}
				
				// Vector2 destPos = GetPositionFromIndex(request.destinationIndex);
				// int destroyIndex = pieceNumberMap[(int)destPos.x, (int)destPos.y] - 1;
				
				// set vals
				response.playerIndex = request.playerIndex;
				response.pieceIndex = map[request.startIndex];
				response.destroyIndex = destroyIndex;
				response.distance = GetMoveVector(request.startIndex, request.destinationIndex);
				response.startIndex = request.startIndex;
				response.destinationIndex = request.destinationIndex;
			}
			
			//////   _requestedMoves.Add(request);
			
			// Debug.Log("__response: " + " playerIndex:" + response.playerIndex + " pieceIndex:" + response.pieceIndex + " destroy:" + response.destroyIndex + " vec_x:" + response.distance.x + " y:" + response.distance.y + " startIndex:" + request.startIndex);
			
			return response;
		}

		public bool RecordMove(MoveVO move)
		{
			bool continueGame = true;

			PieceDataVO foundPiece = null;

			int playerIndex = move.playerIndex;
			
			// TODO - elim dupe code found in ValidateMove
			// update start index
			if(playerIndex > -1)
			{
				int count = pieces.Count;
				for(int i = 0; i < count; i++)
				{
					PieceDataVO piece = pieces[i];
					if(piece.pieceIndex == move.pieceIndex)
					{
						// remove old map index
						map.Remove(piece.locationIndex);

						// update piece
						piece.locationIndex = move.destinationIndex;

						// add new map index
						map[piece.locationIndex] = piece.pieceIndex;

						// store ref
						foundPiece = piece;

						break;
					}
				}
				
				// store move
				if(moves == null)
					moves = new List<MoveVO>();
				
				moves.Add(move);
			}

			bool reachedOtherSide = false;
			if(move.destroyIndex > -1)
			{
				if(GetTypeIndex(move.destroyIndex) == (int)PieceType.king)
				{
					return false;
				}
			}

			// did piece reach "enemy side?"
			int[] terminatingRowIndices = foundPiece.terminatingRowIndices;
			if(terminatingRowIndices != null)
			{
				int iCount = terminatingRowIndices.Length; 
				for(int i = 0; i < iCount; i++)
				{
					int val = terminatingRowIndices[i];
					if(val == foundPiece.locationIndex)
						reachedOtherSide = true;
				}
			}

			// show "rescue piece" panel
			if(reachedOtherSide == true)
			{
				// TODO - switch terminatingRowIndices ....

				showRescuePanelSignal.Dispatch();
			}

			/*
			Debug.Log ("------------------> recoding moves.......");
			int dictCount = 0;
			foreach(KeyValuePair<int, int> entry in map)
			{
				Debug.Log ((++dictCount) + ". map key:" + entry.Key + " val:" + entry.Value);
				// do something with entry.Value or entry.Key
			}
			*/

			return continueGame;
		}

		public void EndGame(int victorIndex)
		{
			_active = false;

			_player = 0;
			
			_initialized = false;

			_victor = victorIndex;

			map.Clear();
			pieces.Clear();

			SetState(GameState.GAME_OVER);
		}

		public void ForfeitGame()
		{
			EndGame((player == 0) ? 1 : 0);
		}

		private void SetState(GameState value)
		{
			_state = value;
			
			gameStateChangeSignal.Dispatch(value);
		}

		// util (list) -------------------------------------------------
		private List<string> GetPlayerPieceSetPrefabIDList(int playerIndex)
		{
			List<string> list = new List<string>();
			
			// TODO - refactor way to define set lists
			if(playerIndex <= 0)
			{
				list.Add(PrefabNames.ROOK_LIGHT); 
				list.Add(PrefabNames.KNIGHT_LIGHT);
				list.Add(PrefabNames.BISHOP_LIGHT);
				list.Add(PrefabNames.QUEEN_LIGHT);
				list.Add(PrefabNames.KING_LIGHT);
				list.Add(PrefabNames.BISHOP_LIGHT);
				list.Add(PrefabNames.KNIGHT_LIGHT);
				list.Add(PrefabNames.ROOK_LIGHT);
				// row #2
				list.Add(PrefabNames.PAWN_LIGHT);
				list.Add(PrefabNames.PAWN_LIGHT);
				list.Add(PrefabNames.PAWN_LIGHT);
				list.Add(PrefabNames.PAWN_LIGHT);
				list.Add(PrefabNames.PAWN_LIGHT);
				list.Add(PrefabNames.PAWN_LIGHT);
				list.Add(PrefabNames.PAWN_LIGHT);
				list.Add(PrefabNames.PAWN_LIGHT);
			}
			else
			{
				list.Add(PrefabNames.ROOK_DARK); 
				list.Add(PrefabNames.KNIGHT_DARK);
				list.Add(PrefabNames.BISHOP_DARK);
				list.Add(PrefabNames.QUEEN_DARK);
				list.Add(PrefabNames.KING_DARK);
				list.Add(PrefabNames.BISHOP_DARK);
				list.Add(PrefabNames.KNIGHT_DARK);
				list.Add(PrefabNames.ROOK_DARK);
				// row #2
				list.Add(PrefabNames.PAWN_DARK);
				list.Add(PrefabNames.PAWN_DARK);
				list.Add(PrefabNames.PAWN_DARK);
				list.Add(PrefabNames.PAWN_DARK);
				list.Add(PrefabNames.PAWN_DARK);
				list.Add(PrefabNames.PAWN_DARK);
				list.Add(PrefabNames.PAWN_DARK);
				list.Add(PrefabNames.PAWN_DARK);
			}
			
			return list;
		}

		private List<int> GetMoveEndCandidates(int playerIndex, int startIndex)
		{
			List<int> result = null;

			if(!isTargetIndexOccupied(startIndex))
				return null;

			/*
			if(isTargetIndexAnEnemy(playerIndex, startIndex))
			   return null;
			*/

			int typeIndex = GetTypeIndexFromPositionIndex(startIndex);
			if(typeIndex > -1)
			{
				switch(typeIndex)
				{
					case (int)PieceType.king:
						result = GetKingMoves(playerIndex, startIndex);
						break;
					case (int)PieceType.queen:
						result = GetQueenMoves(playerIndex, startIndex);
						break;
					case (int)PieceType.rook:
						result = GetRookMoves(playerIndex, startIndex);
						break;
					case (int)PieceType.bishop:
						result = GetBishopMoves(playerIndex, startIndex);
						break;
					case (int)PieceType.knight:
						result = GetKnightMoves(playerIndex, startIndex);
						break;
					case (int)PieceType.pawn:
						result = GetPawnMoves(playerIndex, startIndex);
						break;
					default:
						Debug.Log("error @ getMoveTargetList @ GameManager (typeIndex = " + typeIndex + ")");
						break;
				}
			}
			
			return (result != null ? result : null);
		}

		private List<int> GetRookMoves(int playerIndex, int startIndex)
		{
			List<int> result = new List<int>();

			int typeIndex = GetTypeIndexFromPositionIndex(startIndex);
			if(typeIndex == (int)PieceType.rook)
			{
				List<int> list = null;
				
				// forward
				list = GetValidMoveTargetsByDirection(playerIndex, startIndex, MoveDirectionType.forward);
				if(list != null && (list.Count > 0))
				{
					int count = list.Count;
					for(int i = 0; i < count; i++)
						result.Add(list[i]);
				}
				
				// backward
				list = GetValidMoveTargetsByDirection(playerIndex, startIndex, MoveDirectionType.backward);
				if(list != null && (list.Count > 0))
				{
					int count = list.Count;
					for(int i = 0; i < count; i++)
						result.Add(list[i]);
				}
				
				// left
				list = GetValidMoveTargetsByDirection(playerIndex, startIndex, MoveDirectionType.left);
				if(list != null && (list.Count > 0))
				{
					int count = list.Count;
					for(int i = 0; i < count; i++)
						result.Add(list[i]);
				}
				
				// right
				list = GetValidMoveTargetsByDirection(playerIndex, startIndex, MoveDirectionType.right);
				if(list != null && (list.Count > 0))
				{
					int count = list.Count;
					for(int i = 0; i < count; i++)
						result.Add(list[i]);
				}
			}
			
			// sanitze list
			if(result.Count < 1)
			{
				result = null;
			}
			
			return result;
		}

		private List<int> GetKnightMoves(int playerIndex, int startIndex)
		{
			// get possible move indicies ///////////////////////////////
			int rowCellCount = GameConstants.ROW_CELL_COUNT;
			
			List<int> possibles = new List<int>();
			
			// TODO - refactor below for less repeated logic...
			
			// ... 2 forward, 1 left
			int possible = startIndex + (rowCellCount * 2) - 1;
			if(!wouldMoveBreakBorder(startIndex, possible, MoveDirectionType.left))
				possibles.Add(possible);
			
			// ... 2 forward, 1 right
			possible = startIndex + (rowCellCount * 2) + 1;
			if(!wouldMoveBreakBorder(startIndex, possible, MoveDirectionType.right))
				possibles.Add(possible);
			
			// ... 2 left, 1 forward
			possible = startIndex - 2 + rowCellCount;
			if(!wouldMoveBreakBorder(startIndex, possible, MoveDirectionType.left))
				possibles.Add(possible);
			
			// ... 2 left, 1 backward
			possible = startIndex - 2 - rowCellCount;
			if(!wouldMoveBreakBorder(startIndex, possible, MoveDirectionType.left))
				possibles.Add(possible);
			
			// ... 2 right, 1 forward
			possible = startIndex + 2 + rowCellCount;
			if(!wouldMoveBreakBorder(startIndex, possible, MoveDirectionType.right))
				possibles.Add(possible);
			
			// ... 2 right, 1 backward
			possible = startIndex + 2 - rowCellCount;
			if(!wouldMoveBreakBorder(startIndex, possible, MoveDirectionType.right))
				possibles.Add(possible);
			
			// ... 2 backward, 1 left
			possible = startIndex - (rowCellCount * 2) - 1;
			if(!wouldMoveBreakBorder(startIndex, possible, MoveDirectionType.left))
				possibles.Add(possible);
			
			// ... 2 backward, 1 right
			possible = startIndex - (rowCellCount * 2) + 1;
			if(!wouldMoveBreakBorder(startIndex, possible, MoveDirectionType.right))
				possibles.Add(possible);
			/////////////////////////////////////////////////////////////
			
			List<int> result = GetValidMoveTargetsByPossibleTargets(playerIndex, startIndex, possibles);
			
			return result;
		}

		private List<int> GetBishopMoves(int playerIndex, int startIndex)
		{
			List<int> result = new List<int>();
			
			int typeIndex = GetTypeIndexFromPositionIndex(startIndex);
			if(typeIndex == (int)PieceType.bishop)
			{
				List<int> list = new List<int>();
				
				// diagonal forward-left
				list = GetValidMoveTargetsByDirection(playerIndex, startIndex, MoveDirectionType.diagonalForwardLeft);
				if(list != null && (list.Count > 0))
				{
					int count = list.Count;
					for(int i = 0; i < count; i++)
						result.Add(list[i]);
				}
				
				// diagonal forward-right
				list = GetValidMoveTargetsByDirection(playerIndex, startIndex, MoveDirectionType.diagonalForwardRight);
				if(list != null && (list.Count > 0))
				{
					int count = list.Count;
					for(int i = 0; i < count; i++)
						result.Add(list[i]);
				}
				
				// diagonal backward-left
				list = GetValidMoveTargetsByDirection(playerIndex, startIndex, MoveDirectionType.diagonalBackwardLeft);
				if(list != null && (list.Count > 0))
				{
					int count = list.Count;
					for(int i = 0; i < count; i++)
						result.Add(list[i]);
				}
				
				// diagonal backward-right
				list = GetValidMoveTargetsByDirection(playerIndex, startIndex, MoveDirectionType.diagonalBackwardRight);
				if(list != null && (list.Count > 0))
				{
					int count = list.Count;
					for(int i = 0; i < count; i++)
						result.Add(list[i]);
				}
			}
			
			// sanitze list
			if(result.Count < 1)
			{
				result = null;
			}
			
			return result;
		}

		private List<int> GetKingMoves(int playerIndex, int startIndex)
		{
			// get possible move indicies ///////////////////////////////
			int rowCellCount = GameConstants.ROW_CELL_COUNT;
			
			List<int> possibles = new List<int>();
			
			// ... forward
			possibles.Add(startIndex + rowCellCount);
			// ... backward
			possibles.Add(startIndex - rowCellCount);
			// ... left
			possibles.Add(startIndex - 1);
			// ... right
			possibles.Add(startIndex + 1);
			// ... diagonal forward-left
			possibles.Add(startIndex + rowCellCount - 1);
			// ... diagonal forward-right
			possibles.Add(startIndex + rowCellCount + 1);
			// ... diagonal backward-left
			possibles.Add(startIndex - rowCellCount - 1);
			// ... diagonal backward-right
			possibles.Add(startIndex - rowCellCount + 1);
			/////////////////////////////////////////////////////////////
			
			return GetValidMoveTargetsByPossibleTargets(playerIndex, startIndex, possibles);
		}

		private List<int> GetQueenMoves(int playerIndex, int startIndex)
		{
			List<int> result = new List<int>();
			
			int typeIndex = GetTypeIndexFromPositionIndex(startIndex);
			if(typeIndex == (int)PieceType.queen)
			{
				List<int> list = new List<int>();
				
				// forward
				list = GetValidMoveTargetsByDirection(playerIndex, startIndex, MoveDirectionType.forward);
				if(list != null && (list.Count > 0))
				{
					int count = list.Count;
					for(int i = 0; i < count; i++)
						result.Add(list[i]);
				}

				// backward
				list = GetValidMoveTargetsByDirection(playerIndex, startIndex, MoveDirectionType.backward);
				if(list != null && (list.Count > 0))
				{
					int count = list.Count;
					for(int i = 0; i < count; i++)
						result.Add(list[i]);
				}

				// left
				list = GetValidMoveTargetsByDirection(playerIndex, startIndex, MoveDirectionType.left);
				if(list != null && (list.Count > 0))
				{
					int count = list.Count;
					for(int i = 0; i < count; i++)
						result.Add(list[i]);
				}

				// right
				list = GetValidMoveTargetsByDirection(playerIndex, startIndex, MoveDirectionType.right);
				if(list != null && (list.Count > 0))
				{
					int count = list.Count;
					for(int i = 0; i < count; i++)
						result.Add(list[i]);
				}

				// diagonal forward-left
				list = GetValidMoveTargetsByDirection(playerIndex, startIndex, MoveDirectionType.diagonalForwardLeft);
				if(list != null && (list.Count > 0))
				{
					int count = list.Count;
					for(int i = 0; i < count; i++)
						result.Add(list[i]);
				}

				// diagonal forward-right
				list = GetValidMoveTargetsByDirection(playerIndex, startIndex, MoveDirectionType.diagonalForwardRight);
				if(list != null && (list.Count > 0))
				{
					int count = list.Count;
					for(int i = 0; i < count; i++)
						result.Add(list[i]);
				}

				// diagonal backward-left
				list = GetValidMoveTargetsByDirection(playerIndex, startIndex, MoveDirectionType.diagonalBackwardLeft);
				if(list != null && (list.Count > 0))
				{
					int count = list.Count;
					for(int i = 0; i < count; i++)
						result.Add(list[i]);
				}

				// diagonal backward-right
				list = GetValidMoveTargetsByDirection(playerIndex, startIndex, MoveDirectionType.diagonalBackwardRight);
				if(list != null && (list.Count > 0))
				{
					int count = list.Count;
					for(int i = 0; i < count; i++)
						result.Add(list[i]);
				}
			}
			
			// sanitze list
			if(result.Count < 1)
			{
				result = null;
			}
			
			return result;
		}

		private List<int> GetPawnMoves(int playerIndex, int startIndex)
		{
			int rowCount = GameConstants.ROW_CELL_COUNT;

			int directionNullifier = (playerIndex == 0) ? 1 : -1;

			int typeIndex = GetTypeIndexFromPositionIndex(startIndex);
			if(typeIndex == (int)PieceType.pawn)
			{
				List<int> possibles = new List<int>();
				
				// forward
				int targetIndex = startIndex + (rowCount * directionNullifier);
				if(!isTargetIndexOccupied(targetIndex))
					possibles.Add(targetIndex);
				
				// forward x 2 (if 1st move)
				bool isFirstMove = true;
				if(map.ContainsKey(startIndex))
				{
					int pieceIndex = map[startIndex];
					if(hasPieceMovedBefore(pieceIndex))
						isFirstMove = false;
				}
				if(isFirstMove && !isTargetIndexOccupied(targetIndex))
				{
					targetIndex = startIndex + ((rowCount * 2) * directionNullifier);
					if(!isTargetIndexOccupied(targetIndex))
						possibles.Add(targetIndex);
				}
				
				// diagonal forward-left
				targetIndex = startIndex + ((rowCount - 1) * directionNullifier);
				if(isTargetIndexAnEnemy(playerIndex, targetIndex))
					possibles.Add(targetIndex);
				
				// diagonal forward-right
				targetIndex = startIndex + ((rowCount + 1) * directionNullifier);
				if(isTargetIndexAnEnemy(playerIndex, targetIndex))
					possibles.Add(targetIndex);
				
				return GetValidMoveTargetsByPossibleTargets(playerIndex, startIndex, possibles);
			}
			
			return null;
		}

		private List<int> GetValidMoveTargetsByPossibleTargets(int playerIndex, int startIndex, List<int> possibles)
		{
			List<int> result = new List<int>();
			
			int count = possibles.Count;
			for(int i = 0; i < count; i++)
			{
				int possible = possibles[i];
				if(isValidBoardPosition(possible))
				{
					// check if breaks border edge with move direction...
					// move must happen w/o breaking edge...
					
					if(!isTargetIndexOccupied(possible) || isTargetIndexAnEnemy(playerIndex, possible))
					{
						result.Add(possible);
					}
				}
			}
			
			// sanitze list
			if(result.Count < 1)
			{
				result = null;
			}
			
			return result;
		}

		private List<int> GetValidMoveTargetsByDirection(int playerIndex, int startIndex, MoveDirectionType direction)
		{
			List<int> result = new List<int>();
			
			// ensure player can use "this" startIndex
			int startPlayerIndex = GetPlayerIndexFromLocationIndex(startIndex);
			if(startPlayerIndex == playerIndex)
			{
				int rowCount = GameConstants.ROW_CELL_COUNT;
				int playerIndexNormalizer = (playerIndex == 0) ? 1 : -1;
				
				bool breaksBorder = false;
				for(int i = 1; i <= rowCount; i++)
				{
					// forward
					int targetIndex = -1;
					
					switch(direction)
					{
						case MoveDirectionType.forward:
							targetIndex = startIndex + (rowCount * i * playerIndexNormalizer);
							break;
						case MoveDirectionType.backward:
							targetIndex = startIndex - (rowCount * i * playerIndexNormalizer);
							break;
						case MoveDirectionType.left:
							targetIndex = startIndex - (i * playerIndexNormalizer);
							break;
						case MoveDirectionType.right:
							targetIndex = startIndex + (i * playerIndexNormalizer);
							break;
						case MoveDirectionType.diagonalForwardLeft:
							targetIndex = startIndex + (rowCount * i * playerIndexNormalizer) - i;
							break;
						case MoveDirectionType.diagonalForwardRight:
							targetIndex = startIndex + (rowCount * i * playerIndexNormalizer) + i;
							break;
						case MoveDirectionType.diagonalBackwardLeft:
							targetIndex = startIndex - (rowCount * i * playerIndexNormalizer) - i;
							break;
						case MoveDirectionType.diagonalBackwardRight:
							targetIndex = startIndex - (rowCount * i * playerIndexNormalizer) + i;
							break;
					}
					
					breaksBorder = wouldMoveBreakBorder(startIndex, targetIndex, direction);
					if(breaksBorder)
						break;
					
					if(!breaksBorder && isValidBoardPosition(targetIndex))
					{
						int targetPlayerIndex = GetPlayerIndexFromLocationIndex(targetIndex);
						if(targetPlayerIndex == -1)
						{
							result.Add(targetIndex);
						}
						else
						{
							if(startPlayerIndex == targetPlayerIndex)
							{
								break;
							}
							else
							{
								result.Add(targetIndex);
								
								break;
							}
						}
					}
				}
			}
			else
			{
				return null;
			}
			
			return result;
		}

		// util (vector2) ----------------------------------------------
		private Vector2 GetPositionFromIndex(int index)
		{
			// TODO - articulate why below is needed (pieceNumMap can only have non-zero values to allow collision detection), therefore pieceNum #1 = pieceIndex (0)
			// index += 1;
			
			int rowCellCount = GameConstants.ROW_CELL_COUNT;
			
			int numDiv = index / rowCellCount;
			int xVal = index - (numDiv * rowCellCount);
			
			return new Vector2(xVal, numDiv);
		}

		private Vector2 GetMoveVector(int startIndex, int destIndex)
		{
			int rowCellCount = GameConstants.ROW_CELL_COUNT;
			
			int numDiv = startIndex / rowCellCount;
			int startX = startIndex - (numDiv * rowCellCount);
			
			Vector2 start = new Vector2(startX, numDiv);
			
			int destNumDiv = destIndex / rowCellCount;
			int destX = destIndex - (destNumDiv * rowCellCount);
			Vector2 dest = new Vector2(destX, destNumDiv);
			
			Vector2 distance = new Vector2(dest.x - start.x, dest.y - start.y);
			
			// Debug.Log("distance VECTOR is: x_" + distance.x + " y:" + distance.y + " destX:" + destX + " startX:" + startX);
			
			return distance;
		}

		// util (int) --------------------------------------------------
		private int GetTypeIndexFromPositionIndex(int locationIndex)
		{
			if(map == null)
				return -1;

			if(!map.ContainsKey(locationIndex))
				return -1;
			
			int pieceIndex = map[locationIndex];
			int typeIndex = GetTypeIndex(pieceIndex);

			return typeIndex;
		}

		private int GetPlayerIndexFromLocationIndex(int locationIndex)
		{
			int playerIndex = -1;
			if(map.ContainsKey(locationIndex))
				playerIndex = map[locationIndex];
			
			return GetPlayerIndexFromOwnerIndex(playerIndex);
		}

		private int GetPlayerIndexFromOwnerIndex(int ownerIndex)
		{
			int teamPieceCount = GameConstants.ROW_CELL_COUNT * 2;
			int totalPieceCount = teamPieceCount * 2;
			
			int result = -1;
			if(ownerIndex > -1)
			{
				if(ownerIndex < teamPieceCount)
				{
					result = 0;
				}
				else if((ownerIndex >= teamPieceCount) && (ownerIndex < totalPieceCount))
				{
					result = 1;
				}
			}
			
			return result;
		}

		private int GetRelativeRowIndex(int startIndex)
		{
			int relativeRowIndex = -1;
			if(startIndex < GameConstants.ROW_CELL_COUNT)
				relativeRowIndex = startIndex;
			else
				relativeRowIndex = startIndex % GameConstants.ROW_CELL_COUNT;
			
			return relativeRowIndex;
		}

		// util (bool) -------------------------------------------------
		private bool isTargetIndexOccupied(int targetIndex)
		{
			int targetPlayerIndex = GetPlayerIndexFromLocationIndex(targetIndex);
			if(targetPlayerIndex > -1)
			{
				return true;
			}
			
			return false;
		}

		private bool isTargetIndexAnEnemy(int playerIndex, int locationIndex)
		{
			int maxIndex = (int)(Mathf.Pow(GameConstants.ROW_CELL_COUNT, 2)) - 1;
			if(locationIndex >= maxIndex)
				return false;
			
			int targetPlayerIndex = GetPlayerIndexFromLocationIndex(locationIndex);
			if((targetPlayerIndex > -1) && (targetPlayerIndex != playerIndex))
			{
				return true;
			}
			
			return false;
		}

		private bool wouldMoveBreakBorder(int startIndex, int destIndex, MoveDirectionType direction)
		{
			// sanitize range (NOTE: also, this handles FORWARD or BACKWARD calls)
			int maxIndex = (int)(Mathf.Pow(GameConstants.ROW_CELL_COUNT, 2)) - 1;
			if(startIndex > maxIndex || startIndex < 0
			   || destIndex > maxIndex || destIndex < 0)
				return true;
			
			// handle LEFT / RIGHT & DIAGONAL
			int startRelativeRowIndex = GetRelativeRowIndex(startIndex);
			int destRelativeRowIndex = GetRelativeRowIndex(destIndex);
			switch(direction)
			{
				case MoveDirectionType.left:
				case MoveDirectionType.diagonalForwardLeft:
					if(startRelativeRowIndex < destRelativeRowIndex)
						return true;
					break;
				case MoveDirectionType.right:
				case MoveDirectionType.diagonalForwardRight:
					if(startRelativeRowIndex > destRelativeRowIndex)
						return true;
					break;
			}
			
			return false;
		}

		private bool isValidBoardPosition(int index)
		{
			bool valid = false;
			
			int boardCellCount = GameConstants.ROW_CELL_COUNT * GameConstants.ROW_CELL_COUNT;
			if((index > -1) && (index < boardCellCount))
			{
				valid = true;
			}
			
			return valid;
		}

		private bool hasPieceMovedBefore(int pieceIndex)
		{
			bool result = false;
			
			if(moves != null)
			{
				int count = moves.Count;
				for(int i = 0; i < count; i++)
				{
					MoveVO move = moves[i];
					if(move.pieceIndex == pieceIndex)
					{
						return true;
					}
				}
			}
			
			return result;
		}
	}
}