using UnityEngine;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace cbc.cbcchess
{
	public class ReclaimView:View
	{
		// TODO - remove below dupe code; also found in PiecesView
		private const String TAG_PLAYER1								= "player1Piece";
		private const String TAG_PLAYER2								= "player2Piece";

		#region vars (public)
		public GameObject pieceBox;
		public GameObject piecePositionTarget;
		public Signal<string> requestPlaySound 				= new Signal<string>();
		public bool makeUsable;
		#endregion

		private const String TAG_RECLAIM_PIECE				= "reclaimPiece";
		private const float RAYCAST_DISTANCE_MAX			= 1000.0F;		
		
		#region VARS (private)
		// ... data
		private List<GameObject> pieces;
		// ... comps
		private GameObject pieceHighlighted;
		#endregion

		#region funcs (public)
		public void Init(List<string> prefabIDs)
		{
			pieces = GetPieces(prefabIDs);
		}

		public void Enable(bool enable)
		{
			GetComponent<Transform>().gameObject.SetActive(enable);
		}
		#endregion

		#region FUNCTIONS (internal)
		internal void Update()
		{
			/*
			switch((int)state)
			{
			case (int)ViewState.selectDestination:
				HighlightDestination();
				break;
			}
			*/
	
			HighlightStartPiece();
		}
		#endregion

		#region funcs (private)
		private List<GameObject> GetPieces(List<String> prefabIDs)
		{
			int padding = 10;
			float xPad = 45F;
			float totalPieceWidth = 0;
			float targetY = 70F;
			
			pieces = new List<GameObject>();
			
			int count = prefabIDs.Count;
			for(int i = 0; i < count; i++)
			{
				// instantiate piece
				int teamIndex = 0;
				GameObject go = PieceManager.current.GetPooledPiece(teamIndex, prefabIDs[i]);
			
				// set tag
				go.tag = TAG_RECLAIM_PIECE;

				// set parent (piece box)
				go.transform.parent = pieceBox.transform;
				
				// scale
				go.transform.localScale = piecePositionTarget.transform.localScale;
				
				// rotate (from target)
				go.transform.RotateAround(go.transform.position, go.transform.up, 180);
				
				// make active
				go.SetActive(true);
				
				// save piece
				pieces.Add(go);
			
				////////////////////////////////////////////////////
				// position (from target)
				Vector3 pos = piecePositionTarget.transform.position;
				// pos.x = xPad + (i * padding);
				pos.y = targetY;
				
				// ... set 1st time
				go.transform.position = pos;
				
				Vector3 size = Vector3.zero;

				Renderer body = go.GetComponent<Renderer>();

				// HACK - below accounts for odd "knight prefab" behavior
				if(body == null)
				{
					Renderer[] rend = GetComponentsInChildren<Renderer>();

					try
					{	
						body = rend[1];

						size = body.bounds.size;
						totalPieceWidth += size.x;
						
						pos.x += xPad + (padding * i) + totalPieceWidth;
						pos.y += (size.y / 2.0F);
						go.transform.position = pos;
					}
					catch(Exception e)
					{
						Debug.Log ("test @ ReclaimView");
					}
					
					// TODO - fix hack below! since above math not finding exaxt halves?
					string prefabName = prefabIDs[i];
					switch(prefabName)
					{
						case PrefabNames.PAWN_LIGHT:
							// ...
							break;
						case PrefabNames.KNIGHT_LIGHT:
							pos.y += 4.55F;
							go.transform.position = pos;
							break;
						case PrefabNames.BISHOP_LIGHT:
							pos.y += 0.3F;
							go.transform.position = pos;
							break;
						case PrefabNames.ROOK_LIGHT: 
							pos.y += 1.35F;
							go.transform.position = pos;
							break;
						case PrefabNames.QUEEN_LIGHT:
							pos.y += 5.6F;
							go.transform.position = pos;
							break;
						case PrefabNames.KING_LIGHT:
							pos.y += 2;
							go.transform.position = pos;
							break;
						default:
							pos.y += 40;
							go.transform.position = pos;
							break;
					}
				}
				else
				{
					size = body.bounds.size;
					totalPieceWidth += size.x;
					
					pos.x += xPad + (padding * i) + totalPieceWidth;
					pos.y += (size.y / 2.0F);
					go.transform.position = pos;
				}

				// Debug.Log((i + 1) + ".______test_______DIFF: " + heightDiff + " y-pos: " + pos.y + " z-pos: " + pos.z + " sizeX: " + size.x + " sizeY: " + size.y + " sizeZ: " + size.z);

				pieces.Add(go);
			}

			return pieces;
		}

		// ... update
		private void HighlightStartPiece()
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, RAYCAST_DISTANCE_MAX))	
			{
				// validate (via tag)
				GameObject hitTarget = hit.collider.gameObject;
				bool hasValidTag = false;
				if(hitTarget.tag == TAG_RECLAIM_PIECE)
				{
					hasValidTag = true;
				}
				else if(hitTarget.tag == TAG_PLAYER1 || hitTarget.tag == TAG_PLAYER2)
				{
					if(hitTarget.transform.parent.gameObject.tag == TAG_RECLAIM_PIECE)
					{
						hasValidTag = true;
					}
				}
				else
				{
					Debug.Log ("____ ************ _____________ no but tag is: " + hitTarget.tag);
				}
				if(!hasValidTag) return;

				// get ref
				PieceView piece = GetPieceViewFromGameObject(hitTarget);
				if(piece == null)
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
						// RequestMoveOptions(pieceView.locationIndex);
						
						// store index
						// selectedPieceIndex = pieceView.viewIndex;
						
						// SetState(ViewState.selectDestination);
						
						// fun test!
						// hit.collider.gameObject.GetComponentInParent<Rigidbody>().useGravity = true;
						// pieces[selectedPieceIndex].GetComponentInParent<Rigidbody>().useGravity = true;
					}
				}
			}
		}

		private Renderer GetRendererFromGameObject(GameObject go)
		{
			Renderer rend = go.GetComponent<Renderer>();
			
			// HACK - below accounts for odd "knight prefab" behavior
			if(rend == null)
			{
				Renderer[] rends = GetComponentsInChildren<Renderer>();
				rend = rends[1];
			}

			return rend;
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

			int offset = 4;
			trans.position = new Vector3(pos.x, pos.y + (raise ? offset : -offset), pos.z);
		}
		#endregion
	}
}