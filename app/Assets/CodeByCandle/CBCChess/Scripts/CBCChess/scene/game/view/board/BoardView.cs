using UnityEngine;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace cbc.cbcchess
{
	public class BoardView:View
	{
		#region VARS (public)
		public Rigidbody cameraBody;
		// ... prefabs
		public GameObject cellPrefab;
		public GameObject destroyFXPrefab;
		// public GameObject moveMarkerPrefab;
		// ... signals
		public Signal<int> selectionMapRequested 						= new Signal<int>();
		public Signal<int> moveMapRequested 							= new Signal<int>();
		public Signal<int, int> moveRequested 							= new Signal<int, int>();
		public Signal<MoveVO> moveStarted 								= new Signal<MoveVO>();
		public Signal<string> requestPlaySound 							= new Signal<string>();
		public Signal moveCompleted										= new Signal();
		#endregion

		#region CONSTANTS (private)
		private const String TAG_PLAYER1								= "player1Piece";
		private const String TAG_PLAYER2								= "player2Piece";
		private const float RAYCAST_DISTANCE_MAX						= 1000.0F;						// tweak this?
		private const float CELL_PIXEL_WIDTH 							= 20.0F;
		// ... move
		private const float MOVE_BUFFER_SECS 							= 10.0f;
		private const float MOVE_DELAY_SECS 							= 0;
		private const float MOVE_DURATION_SECS 							= 0.5F;
		private const float	MOVE_POST_PAUSE_SECS 						= 1.0F;
		#endregion

		#region VARS (private)
		// ... comps
		private List<GameObject> spaces;	
		private BoardCellView spaceSelectedBefore;
		private GameObject pieceHighlighted;
		// ... data
		private enum ViewState
		{
			initGame,
			selectStart,
			selectDestination,
			moveComplete
		}
		private ViewState state;
		private List<MoveVO> moves;
		private MoveVO previousMove;
		private List<int> moveEndCandidates;
		private Color COLOR_SPACE1 										= Color.white;
		private Color COLOR_SPACE2 										= new Color(0.4F, 0.4F, 0.4F);
		private int playerIndex											= -1;
		private int selectedPieceIndex 									= -1;
		#endregion

		#region FUNCTIONS (public)
		public void AddBoard()
		{
			// add gameboard
			spaces = AddGameboard(GameConstants.ROW_CELL_COUNT, 
			                      COLOR_SPACE1, 
			                      COLOR_SPACE2, 
			                      CELL_PIXEL_WIDTH);
		}

		public void DisplayMoveEndCandidates(List<int> moveEndCandidates)
		{
			if(spaces != null)
			{
				// board spaces - reset old "end candidates"
				ResetMoveEndCandidates();

				// board spaces - highlight new "end candidates"
				int count = moveEndCandidates.Count;
				for(int i = 0; i < count; i++)
				{
					GameObject obj = spaces[moveEndCandidates[i]];
					BoardCellView cell = obj.GetComponent<BoardCellView>();
					cell.state = BoardCellView.STATE.SelectableAsDestination;
				}
	
				this.moveEndCandidates = moveEndCandidates;
			}
			
			SetState(ViewState.selectDestination);
		}

		public void Reset()
		{
			ResetMoveEndCandidates();
		}
		#endregion

		#region FUNCTIONS (protected)
		protected override void Start()
		{
			base.Start ();

			// ...
		}
		#endregion

		#region FUNCTIONS (internal)
		internal void Update()
		{
			switch((int)state)
			{
				case (int)ViewState.selectDestination:
					HighlightDestination();
					break;
			}
		}
		#endregion

		#region FUNCTIONS (private)
		private void SetState(ViewState newState)
		{
			if(newState != state)
			{
				switch((int)newState)
				{
					case (int)ViewState.moveComplete:
						moveCompleted.Dispatch();

						break;
				}

				state = newState;
			}
		}

		// ... init
		private List<GameObject> AddGameboard(int rowCellCount, 
		                                      Color color1, 
		                                      Color color2, 
		                                      float cellPixelWidth)
		{
			float xPos = 0.0F;
			float zPos = 0.0F;
			int total = rowCellCount * rowCellCount;
			int curPieceCount = 0;
			int rowCount = 0;
			Color color = color1;
			
			List<GameObject> cells = new List<GameObject>();
			for(int i = 0; i < total; i++)
			{
				// get color
				bool allowColorFlip = true;
				if((curPieceCount > 0) && (curPieceCount % rowCellCount == 0))
					allowColorFlip = false;

				if(allowColorFlip)
					color = (color == color1) ? color2 : color1;
				
				// instantiate
				GameObject prefab = Instantiate(cellPrefab) as GameObject;
				
				// TODO - remove this temp hack to store which player-piece "owns" this board space
				int ownerPieceIndex = -1;
				if(i < 16)
					ownerPieceIndex = i;
				else if(i > 47 && i < 64)
					ownerPieceIndex = 16 + (63 - i);
				
				prefab.GetComponent<BoardCellView>().Init(i, ownerPieceIndex, color);
				
				// position
				prefab.transform.parent = gameObject.transform;
				if((curPieceCount > 0) && (curPieceCount % rowCellCount == 0))
				{
					xPos = 0;
					zPos += cellPixelWidth;
					rowCount++;
				}
				prefab.transform.position = new Vector3(xPos, 0, zPos);
				xPos += cellPixelWidth;
				
				cells.Add(prefab);
				
				// update count
				curPieceCount++;
			}
			
			return cells;
		}

		// ... update
		private void HighlightDestination()
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, RAYCAST_DISTANCE_MAX))
			{
				GameObject hitTarget = hit.collider.gameObject;

				// *** handle AS SPACE ////////////////////////////
				BoardCellView destCell = hitTarget.GetComponentInParent<BoardCellView>();
				if(destCell != null)
				{
					if(Input.GetMouseButtonDown(0))
					{
						if(moveEndCandidates == null || !moveEndCandidates.Contains(destCell.viewIndex))
							return;
							
						RequestMove(selectedPieceIndex, destCell.viewIndex);

						requestPlaySound.Dispatch(GameConstants.SOUND_DESTINATION_SELECTED);
					}
				}
				else
				{
					// *** handle AS PIECE ////////////////////////////
					if(Input.GetMouseButtonDown(0))
					 {
						PieceView destPiece = GetPieceViewFromGameObject(hitTarget);
						if(destPiece != null)
						{
							String friendTag = (playerIndex == 0) ? TAG_PLAYER1 : TAG_PLAYER2;
							if(friendTag == hitTarget.tag)
							{
							//	if(destPiece.viewIndex == selectedPieceIndex)
							//		ResetSelectedPiece();
							}
							else
							{
								if((moveEndCandidates != null) && !moveEndCandidates.Contains(destPiece.locationIndex))
									return;

								RequestMove(selectedPieceIndex, destPiece.locationIndex);

								requestPlaySound.Dispatch(GameConstants.SOUND_DESTINATION_SELECTED);
							}
						}
					}
				}
			}
			else
			{
				if(pieceHighlighted)
				{
					pieceHighlighted = null;
				}
			}
		}

		// ... move
		private void RequestMove(int pieceIndex, int moveEndIndex)
		{
		// TODO - fix this perhaps?
			PieceView piece = null;		// GetPieceViewFromGameObject(pieces[pieceIndex]);
			if(piece == null)
				return;
			
			moveRequested.Dispatch(piece.locationIndex, moveEndIndex);
		}

		private void RequestMoveOptions(int cellIndex)
		{
			moveMapRequested.Dispatch(cellIndex);
		}

		private void ResetMoveEndCandidates()
		{
			if(moveEndCandidates != null)
			{
				int oldCount = moveEndCandidates.Count;
				for(int a = 0; a < oldCount; a++)
				{
					GameObject oldObj = spaces[moveEndCandidates[a]];
					BoardCellView oldCell = oldObj.GetComponent<BoardCellView>();
					if(oldCell.state == BoardCellView.STATE.SelectableAsDestination)
						oldCell.state = BoardCellView.STATE.Init;
				}

				moveEndCandidates = null;
			}
		}
		
		private void ResetSpecialCells()
		{
			if(moveEndCandidates != null)
			{

// TODO - fix this!!!
/*
				int oldCount = moveEndCandidates.Count;
				for(int a = 0; a < oldCount; a++)
				{
					GameObject oldObj = spaces[moveMap[a]];
					BoardCellView oldCell = oldObj.GetComponent<BoardCellView>();
					oldCell.state = BoardCellView.STATE.Init;
				}
*/
			}
			
			if(selectedPieceIndex > -1)
			{
				BoardCellView boardCell = spaces[selectedPieceIndex].GetComponent<BoardCellView>();
				boardCell.state = BoardCellView.STATE.Init;
			}
		}

		private PieceView GetPieceViewFromGameObject(GameObject target)
		{
			// TODOO - swap out below dupe func for function call
			PieceView piece = target.GetComponent<PieceView>();
			
			// HACK: since 3d model for "knights" has special conditions...
			if(piece == null)
			{
				// try parent?
				piece = target.GetComponentInParent<PieceView>();
			}
			
			return piece;
		}
		#endregion
	}
}