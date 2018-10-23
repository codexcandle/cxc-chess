using System;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;

using cbc.cbcutils;

namespace cbc.cbcchess
{
	public class BoardMediator:CBCMediator
	{
		[Inject]
		public BoardView view { get; set; }

		[Inject]
		public IGameModel gameModel { get; set; }

		// signals
		[Inject]
		public GameStateChangeSignal gameStateChangeSignal {get; set;}

		[Inject]
		public PlaySoundSignal playSoundSignal {get; set;}

		// ... startpos-map
		[Inject]
		public RequestSelectionMapSignal requestSelectionMapSignal {get; set;}
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
				displayMoveMapSignal.AddListener(onRequestMoveEndCandidatesResponse);
				pieceDeselectedSignal.AddListener(onPieceDeselected);
				moveSignal.AddListener(onMove);
				// ... view
				view.selectionMapRequested.AddListener(requestSelectionMap);
				view.moveMapRequested.AddListener(onMoveMapRequested);
				view.moveRequested.AddListener(NotifyMoveRequested);
				view.moveStarted.AddListener(NotifyMoveStarted);
				view.requestPlaySound.AddListener(requestPlaySound);
			}
			else
			{
				// ... app
				gameStateChangeSignal.RemoveListener(handleGameState);
				displayMoveMapSignal.RemoveListener(onRequestMoveEndCandidatesResponse);
				pieceDeselectedSignal.RemoveListener(onPieceDeselected);
				moveSignal.RemoveListener(onMove);
				// ... view
				view.selectionMapRequested.RemoveListener(requestSelectionMap);
				view.moveMapRequested.RemoveListener(onMoveMapRequested);
				view.moveRequested.RemoveListener(NotifyMoveRequested);
				view.moveStarted.RemoveListener(NotifyMoveStarted);
				view.requestPlaySound.RemoveListener(requestPlaySound);
			}
		}

		private void handleGameState(GameState state)
		{
			// Debug.Log("__________________state @ BoardMediator: " + state);

			switch(state)
			{
				case GameState.SETUP:
					view.AddBoard();
					break;
				case GameState.STARTING:
					if(initialized)
						view.Reset();

					initialized = true;

					break;
				case GameState.TURN_READY:
					requestSelectionMap(gameModel.player);
					break;
			}
		}

		private void requestPlaySound(string soundID)
		{
			playSoundSignal.Dispatch(soundID);
		}

		// ... startPos-map
		private void requestSelectionMap(int playerIndex)
		{
			requestSelectionMapSignal.Dispatch(new RequestSelectionMapVO(playerIndex));
		}

		// ... endPos-map
		private void onMoveMapRequested(int startIndex)
		{
			requestMoveMapSignal.Dispatch(new RequestMoveMapVO(gameModel.player, startIndex));
		}
		
		private void onRequestMoveEndCandidatesResponse(DisplayMoveMapVO vo)
		{
			view.DisplayMoveEndCandidates(vo.data);
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

		private void onMove(List<MoveVO> moves)
		{
			view.Reset();
		}

		// ...
		private void onPieceDeselected()
		{
			view.Reset();
		}
	}
}