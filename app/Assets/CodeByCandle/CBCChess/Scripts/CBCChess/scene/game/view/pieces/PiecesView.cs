using UnityEngine;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace cbc.cbcchess
{
	public class PiecesView:View
	{
		#region VARS (public)
		public Rigidbody cameraBody;
		// ... prefabs
/*
		public GameObject[] team1Prefabs;
		public GameObject[] team2Prefabs;
*/
		public GameObject destroyFXPrefab;
		// ... signals
		public Signal<int> selectionMapRequested 						= new Signal<int>();
		public Signal<int> moveMapRequested 							= new Signal<int>();
		public Signal<int, int> moveRequested 							= new Signal<int, int>();
		public Signal pieceDeselected									= new Signal();
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
		private List<GameObject> pieces;
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
		private List<int> moveStartCandidates;
		private Color COLOR_DEFAULT_PLAYER1								= new Color(0.643F, 0.565F, 0.431F, 0.44F);
		private Color COLOR_DEFAULT_PLAYER2								= new Color(0.2F, 0.2F, 0.1F, 0.2F);
		private int playerIndex											= -1;
		private int selectedPieceIndex 									= -1;
		#endregion

		#region FUNCTIONS (public)
		public void AddPieces(List<PieceDataVO> pieceData)
		{
			pieces = InitPieces(pieceData, pieces);
		}
		
		public void DisplayMoveStartCandidates(int playerIndex, List<int> moveStartCandidates)
		{
			this.moveStartCandidates = moveStartCandidates;
			
			this.playerIndex = playerIndex;
			
			SetState(ViewState.selectStart);
		}
		
		public void MovePieces(List<MoveVO> moves)
		{
			this.moves = moves;
			
			// reset selected piece
			if(selectedPieceIndex > -1)
			{
				PieceView piece = pieces[selectedPieceIndex].GetComponent<PieceView>();
				piece.state = PieceView.STATE.Init;
				
				selectedPieceIndex = -1;
			}
			
			StartCoroutine(PerformNextMove());
		}
		
		public void Reset()
		{
			// ResetPieces();
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
			case (int)ViewState.selectStart:
				HighlightStartPiece();
				break;
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
					default:
						// ...
						
						break;
				}
				
				state = newState;
			}
		}
		
		// ... init
		private List<GameObject> InitPieces(List<PieceDataVO> pieceData, List<GameObject> existingPieceObjects)
		{
			List<GameObject> returnVal = new List<GameObject>();
			
			int piecesPerTeam = GameConstants.ROW_CELL_COUNT * 2;
			
			int rowCellCount = GameConstants.ROW_CELL_COUNT;
			int teamPieceCount = rowCellCount * 2;
			
			int boardHeight = 10;
			float xPos = 0.0F;

			int count = pieceData.Count;
			for(int i = 0; i < count; i++)
			{
				// instantiate
				GameObject go = null;
				if(existingPieceObjects == null)
				{
					go = PieceManager.current.GetPooledPiece(0, pieceData[i].prefabID);
					// go = Instantiate(GetPiecePrefabFromID(pieceData[i].prefabID)) as GameObject;
				}
				else
				{
					go = existingPieceObjects[i];
				}

				//////////
				Transform trans = go.transform;
				Renderer rend = trans.GetComponent<Renderer>();
				if(rend)
					rend.enabled = true;
				
				float yPos = trans.position.y;
				if(existingPieceObjects == null)
					yPos += boardHeight;
				
				if(i < teamPieceCount)
				{
					float zPos = (i > 7) ? 20 : 0;
					float zRotation = -1;
					
					trans.parent = gameObject.transform;
			 		trans.position = new Vector3(xPos, yPos, zPos);
					trans.RotateAround(trans.position, trans.up, zRotation);
					
					// update data
					xPos += CELL_PIXEL_WIDTH;
					if((i > 0) && ((i + 1) % rowCellCount == 0))
					{
						xPos = 0;
						zPos += CELL_PIXEL_WIDTH;
					}
				}
				else
				{
					int offset_i = i - 16;
					
					// transform
					float xOffset = (offset_i < 8) ? 140 : 120;
					xPos = xOffset - (offset_i * CELL_PIXEL_WIDTH);
					if(offset_i > 7)
						xPos += 180;
					
					float zPos = (offset_i > 7) ? 120 : 140;
					float zRotation = 180;
					
					trans.parent = gameObject.transform;
					trans.position = new Vector3(xPos, yPos, zPos);
					trans.RotateAround(trans.position, trans.up, zRotation);
					
					// update data
					if((offset_i > 0) && ((offset_i + 1) % rowCellCount == 0))
					{
						xPos = 0;
						zPos += CELL_PIXEL_WIDTH;
					}
				}

				PieceView piece = GetPieceViewFromGameObject(go);
				// PieceView piece = go.GetComponent<PieceView>();
				if(piece == null)
					return null;

				Color clr = (i < piecesPerTeam) ? COLOR_DEFAULT_PLAYER1 : COLOR_DEFAULT_PLAYER2;
				piece.Init(i, pieceData[i].locationIndex, clr);

				///////////
				// enable
	 			go.SetActive(true);
				// save 
				returnVal.Add(go);
			}

			return returnVal;
		}

		// ... update
		private void HighlightStartPiece()
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, RAYCAST_DISTANCE_MAX))	
			{
				GameObject hitTarget = hit.collider.gameObject;
				string desiredTag = (playerIndex == 0) ? TAG_PLAYER1 : TAG_PLAYER2;
				
				if(hitTarget.tag == desiredTag)
				{
					// get ref
					PieceView piece = GetPieceViewFromGameObject(hitTarget);
					if(piece == null)
						return;
					if(!moveStartCandidates.Contains(piece.locationIndex))
						return;
					
					// highlight
					if(pieceHighlighted == null)
						HighlightPieceObject(hitTarget, true, PieceView.STATE.Highlighted, GameConstants.SOUND_POSSIBLE_START_HIGHLIGHTED);
					else if(hitTarget != pieceHighlighted)
						HighlightPieceObject(pieceHighlighted, false, PieceView.STATE.Init, null);
					
					// handle click
					if(Input.GetMouseButtonDown(0))
					{
						if(pieceHighlighted != null)
						{
							// get ref
							PieceView pieceView = GetPieceViewFromGameObject(pieceHighlighted);
							if(pieceView == null)
								return;
							
							// highlight
							HighlightPieceObject(pieceHighlighted, false, PieceView.STATE.SelectedAsStart, GameConstants.SOUND_START_SELECTED);
							
							// request move-candidate map
							RequestMoveOptions(pieceView.locationIndex);
							
							// store index
							selectedPieceIndex = pieceView.viewIndex;

							SetState(ViewState.selectDestination);

							// fun test!
							// hit.collider.gameObject.GetComponentInParent<Rigidbody>().useGravity = true;
							// pieces[selectedPieceIndex].GetComponentInParent<Rigidbody>().useGravity = true;
						}
					}
				}
			}
		}
		
		private void HighlightPieceObject(GameObject target, bool highlight, PieceView.STATE pieceState, String soundID)
		{
			// get ref
			PieceView piece = GetPieceViewFromGameObject(target);
			if(piece == null)
				return;
			
			// upstate state
			piece.state = pieceState;
			
			// position
			RaiseObject(target.transform, highlight);
			
			// play sound
			if(soundID != null)
				requestPlaySound.Dispatch(soundID);
			
			// update ref
			pieceHighlighted = highlight ? target : null;
		}
		
		private void RaiseObject(Transform trans, bool raise)
		{
			Vector3 pos = trans.position;
			
			trans.position = new Vector3(pos.x, pos.y + (raise ? 20 : -20), pos.z);
		}
		
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
						/* TODO - do I need below?
						if(!moveEndCandidates.Contains(destCell.viewIndex))
							return;
						*/

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
								if(destPiece.viewIndex == selectedPieceIndex)
									ResetSelectedPiece();
							}
							else
							{
								/* TODO - do I need below?
								if((moveEndCandidates != null) && !moveEndCandidates.Contains(destPiece.locationIndex))
									return;
								*/

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
		private void RequestMoveOptions(int cellIndex)
		{
			moveMapRequested.Dispatch(cellIndex);
		}
		
		private void RequestMove(int pieceIndex, int moveEndIndex)
		{
			PieceView piece = GetPieceViewFromGameObject(pieces[pieceIndex]);
			if(piece == null)
				return;
			
			moveRequested.Dispatch(piece.locationIndex, moveEndIndex);
		}
		
		private void ResetSelectedPiece()
		{
			// get ref
			PieceView pieceView = GetPieceViewFromGameObject(pieces[selectedPieceIndex]);
			if(pieceView == null)
				return;
			
			// reset index
			selectedPieceIndex = -1;
			
			// reset state
			pieceView.state = PieceView.STATE.Init;
			SetState(ViewState.selectStart);

			// dispatch "deselect" signal
			pieceDeselected.Dispatch();
		}
		
		IEnumerator PerformNextMove()
		{
			if(moves != null)
			{
				int count = moves.Count;
				if(count > 0)
				{
					MoveVO move = moves[0];
					
					MovePiece(move);
					
					// Debug.Log("move #" + (moveCount + 1) + " :" + " playerIndex_" + move.playerIndex + " pieceIndex_ " + move.pieceIndex + " destroyIndex_" + move.destroyIndex + " destIndex:" + move.destinationIndex + " dist (X:" + move.distance.x + " Y:" + move.distance.y + ")");
					
					// TODO - should I remove sooner? to allow to fix "short move time" bug?
// moves.RemoveAt(0);
					
					// save previous move
					// TODO - confirm is can just copy vs creating new instance below?
					previousMove = new MoveVO(move.playerIndex, move.pieceIndex, move.startIndex, move.destinationIndex, move.distance, move.destroyIndex);
					
					yield return new WaitForSeconds(MOVE_BUFFER_SECS);
					
					StartCoroutine(PerformNextMove());
				}
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
		
		private void MovePiece(MoveVO move)
		{
			moveStarted.Dispatch(move);
			
			int pieceIndex = move.pieceIndex;
			Vector2 distance = move.distance;
			GameObject piece = pieces[pieceIndex];
			Vector3 pos = piece.transform.position;
			
			float zPos = pos.z + (distance.y * CELL_PIXEL_WIDTH);
			Vector3 dest = new Vector3(pos.x + (distance.x * CELL_PIXEL_WIDTH), pos.y, zPos);
			
			// play sound
			// requestPlaySound.Dispatch(GameConstants.SOUND_POSSIBLE_START_HIGHLIGHTED);
			
			LeanTween.move(piece, dest, MOVE_DURATION_SECS).setUseEstimatedTime(true).
				setOnComplete(OnMoveComplete).
					setDelay(MOVE_DELAY_SECS);

			return;
			/* TODO - fix below!!!!
			 * 
			 * 
			 * 
			// update "owner index" for start + end cells
			// ... reset start cell owner index
			BoardCellView space = spaces[move.startIndex].GetComponent<BoardCellView>();
			space.ownerIndex = -1;
			// ... update dest cell to reflect new piece index
			space = spaces[move.destinationIndex].GetComponent<BoardCellView>();
			space.ownerIndex = pieceIndex;
			*/
		}
		
		private void OnMoveComplete()
		{
			moves.RemoveAt(0);

			// destroy piece?
			int destroyIndex = previousMove.destroyIndex;
			if(destroyIndex > -1)
				KillPiece(destroyIndex);
			
			// update "moved" piece's new location
			PieceView piece = GetPieceViewFromGameObject(pieces[previousMove.pieceIndex]);
			if(piece != null)
				piece.locationIndex = previousMove.destinationIndex;
			
			ResetSpecialCells();
			
			// moveCount++;
			
			// if(gameActive)
			// {
			StartCoroutine(PauseAfterMove());
			// }
		}
		
		private void ResetSpecialCells()
		{
			
			// TODO - fix this!!!
			/*

			if(moveEndCandidates != null)
			{
				

				int oldCount = moveEndCandidates.Count;
				for(int a = 0; a < oldCount; a++)
				{
					GameObject oldObj = spaces[moveMap[a]];
					BoardCellView oldCell = oldObj.GetComponent<BoardCellView>();
					oldCell.state = BoardCellView.STATE.Init;
				}

			}
			
			if(selectedPieceIndex > -1)
			{
				BoardCellView boardCell = spaces[selectedPieceIndex].GetComponent<BoardCellView>();
				boardCell.state = BoardCellView.STATE.Init;
			}
			*/
		}
		
		private void KillPiece(int index)
		{
			GameObject piece = pieces[index];
			AddTakeEffect(piece.transform.position);
			
			// TODO - fix below! - adjusted indices?
			// TODO - fix issue with deleting knight renderers?
			// Destroy(piece);
			if(piece.transform.GetComponent<Renderer>() != null)
			{
				piece.transform.GetComponent<Renderer>().enabled = false;
			}
			else
			{
				piece.transform.localScale = new Vector3(0, 0, 0);
			}
			
			// play sound
			requestPlaySound.Dispatch(GameConstants.SOUND_PIECE_DESTROY);
			
			// int punishTeamIndex = (move.playerIndex == 0) ? 0 : 1;
			// pieceMoved.Dispatch(move.pieceIndex);
			
			// Transform trans = vo.view.transform; 
			// trans.localScale = new Vector3(trans.localScale.x, trans.localScale.y, trans.localScale.z);
			
			// Destroy(vo.view);
			
			/// ----------- > pieceToDestroy = null;
			
			///////  --------------- > GameObject de = Instantiate(destroyEffect, trans.position, trans.rotation) as GameObject;
			///////  --------------- > de.GetComponent<DestroyEffect>().InitPos(trans.position);
			/// 
		}
		
		private void AddTakeEffect(Vector3 pos)
		{
			GameObject eff = Instantiate(destroyFXPrefab) as GameObject;
			eff.transform.parent = gameObject.transform;
			eff.transform.position = pos;
		}
		
		IEnumerator PauseAfterMove()
		{
			yield return new WaitForSeconds(MOVE_POST_PAUSE_SECS);
			
			SetState(ViewState.moveComplete);
		}
		#endregion
	}
}