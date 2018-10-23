using System;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;

using cbc.cbcutils;

namespace cbc.cbcchess
{
	public class PiecesMediator:CBCMediator
	{
		[Inject]
		public PiecesView view { get; set; }
		
		[Inject]
		public IGameModel gameModel { get; set; }
		
		// signals
		[Inject]
		public GameStateChangeSignal gameStateChangeSignal {get; set;}
		
		[Inject]
		public PlaySoundSignal playSoundSignal {get; set;}
		
		// ... startpos-map
		[Inject]
		public UpdateSelectionMapSignal updateSelectionMapSignal {get; set;}
		
		// ... endpos map
		[Inject]
		public RequestMoveMapSignal requestMoveMapSignal {get; set;}
		[Inject]
		public DisplayMoveMapSignal displayMoveMapSignal {get; set;}
		
		// move
		[Inject]
		public ValidateMoveSignal validateMove { get; set; }
		[Inject]
		public RecordMoveSignal recordMoveSignal {get; set;}
		[Inject]
		public MovePieceSignal moveSignal  { get; set; }
		[Inject]
		public CompleteMoveSignal completeMoveSignal {get; set;}
		[Inject]
		public PieceDeselectedSignal pieceDeselectedSignal {get; set;}

		private bool initialized;
		
		// functions (public) ----------------------------
		public override void OnRegister()
		{
			base.OnRegister();
			
			enableListeners(true);
			
			handleGameState(gameModel.state);
		}
		
		public override void OnRemove()
		{
			enableListeners(false);
		}
		
		// functions (private) ---------------------------
		private void enableListeners(bool enable)
		{
			if(enable)
			{
				// ... app
				gameStateChangeSignal.AddListener(handleGameState);
				updateSelectionMapSignal.AddListener(onRequestMoveStartCandidatesResponse);
				moveSignal.AddListener(PerformMoves);
				// ... view
				view.moveMapRequested.AddListener(onMoveMapRequested);
				view.moveRequested.AddListener(NotifyMoveRequested);
				view.moveStarted.AddListener(NotifyMoveStarted);
				view.moveCompleted.AddListener(onMoveComplete);
				view.requestPlaySound.AddListener(requestPlaySound);
				view.pieceDeselected.AddListener(onPieceDeselected);
			}
			else
			{
				// ... app
				gameStateChangeSignal.RemoveListener(handleGameState);
				updateSelectionMapSignal.RemoveListener(onRequestMoveStartCandidatesResponse);
				moveSignal.RemoveListener(PerformMoves);
				// ... view
				view.moveMapRequested.RemoveListener(onMoveMapRequested);
				view.moveRequested.RemoveListener(NotifyMoveRequested);
				view.moveStarted.RemoveListener(NotifyMoveStarted);
				view.moveCompleted.RemoveListener(onMoveComplete);
				view.requestPlaySound.RemoveListener(requestPlaySound);
				view.pieceDeselected.RemoveListener(onPieceDeselected);
			}
		}
		
		private void handleGameState(GameState state)
		{
			switch(state)
			{
			case GameState.SETUP:
				view.AddPieces(gameModel.GetPieceData());
				
				break;
			case GameState.STARTING:
				if(initialized)
				{
					view.Reset();
					
					view.AddPieces(gameModel.GetPieceData());
				}
				
				initialized = true;
				
				break;
			}
		}
		
		private void requestPlaySound(string soundID)
		{
			playSoundSignal.Dispatch(soundID);
		}
		
		// ... startPos-map
		private void onRequestMoveStartCandidatesResponse(UpdateSelectionMapVO vo)
		{
			view.DisplayMoveStartCandidates(gameModel.player, vo.data);
		}
		
		// ... endPos-map
		private void onMoveMapRequested(int startIndex)
		{
			requestMoveMapSignal.Dispatch(new RequestMoveMapVO(gameModel.player, startIndex));
		}
		
		// ... move
		private void NotifyMoveRequested(int startIndex, int destIndex)
		{
			RequestMoveVO vo = new RequestMoveVO(gameModel.player, startIndex, destIndex);
			
			validateMove.Dispatch(vo);
		}
		
		private void NotifyMoveStarted(MoveVO move)
		{
			move.playerIndex = gameModel.player;
			
			recordMoveSignal.Dispatch(move);
		}
		
		private void PerformMoves(List<MoveVO> moves)
		{
			view.MovePieces(moves);
		}
		
		private void onMoveComplete()
		{
			completeMoveSignal.Dispatch();
		}

		private void onPieceDeselected()
		{
			pieceDeselectedSignal.Dispatch();
		}
	}
}