using System;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;

using cbc.cbcutils;

namespace cbc.cbcchess
{
	public class GuiMediator:CBCMediator
	{
		[Inject]
		public GuiView view { get; set; }

		[Inject]
		public IGameModel gameModel { get; set; }

		[Inject]
		public GameStateChangeSignal gameStateChangeSignal { get; set; }

		[Inject]
		public MovePieceSignal movePieceSignal { get; set; }

		[Inject]
		public RequestStartNewGameSignal requestStartNewGameSignal { get; set; }

		[Inject]
		public RecordMoveSignal recordMoveSignal { get; set; }

		[Inject]
		public EndGameSignal endGameSignal { get; set; }
		
		#region functions (public)
		public override void OnRegister()
		{
			base.OnRegister();
		
			enableListeners(true);

			// show panels
			// view.ShowNetworkPanel(gameModel.gameType == GameType.VsNetwork);
			view.ShowGameEndPanel(false);
			view.ShowScoreboard(true);

			// hide scoreboard components
			view.Init();

			// init w/ empty string
			updateViewStatus("");
		}

		public override void OnRemove()
		{
			enableListeners(false);
		}
		#endregion

		#region functions (private)
		// ... handlers
		private void enableListeners(bool enable)
		{
			if(enable)
			{
				// ... app
				gameStateChangeSignal.AddListener(handleGameState);
				movePieceSignal.AddListener(onMovePiece);
				recordMoveSignal.AddListener(onRecordMove);

				// ... view
				view.newGameClick.AddListener(onClickNewGame);
				// view.quitClick.AddListener(onClickQuit);
				// view.debugNewGameClick.AddListener(onClickDebugNewGame);
			}
			else
			{
				// ... app
				gameStateChangeSignal.RemoveListener(handleGameState);
				movePieceSignal.RemoveListener(onMovePiece);
				recordMoveSignal.AddListener(onRecordMove);

				// ... view
				view.newGameClick.RemoveListener(onClickNewGame);
				// view.quitClick.RemoveListener(onClickQuit);
				// view.debugNewGameClick.RemoveListener(onClickDebugNewGame);
			}
		}

		private void handleGameState(GameState state)
		{
			string txt = state.ToString();

			bool doUpdateStatusText = true;

			switch((int)state)
			{
				case (int)GameState.STARTING:
					txt = "let's begin.";
					
					view.ShowGameEndPanel(false, "");

					ResetScoreboard();

					break;
				case (int)GameState.LOAD_PLAYER:
					txt = "";
					
					break;
				case (int)GameState.TURN_READY:
					txt = "Player " + (gameModel.player + 1) + ", please select a piece to move.";
					
					break;
				case (int)GameState.SelectDestination:
					txt = "Player " + (gameModel.player + 1) + ", please select your move destination.";
					
					break;
				case (int)GameState.MoveComplete:
					txt = "";
					
					break;
				case (int)GameState.GAME_OVER:
					txt = "Player " + (gameModel.victor + 1) + " wins!";
					
					view.ShowGameEndPanel(true, txt);

					doUpdateStatusText = false;

					break;
			}

			if(doUpdateStatusText)
			{
				TextAnchor align = (gameModel.player == 0) ? TextAnchor.LowerRight : TextAnchor.LowerCenter;
				view.AlignStatusText(align);

				updateViewStatus(txt);
			}
		}

		private void onMovePiece(List<MoveVO> moves)
		{
			// TODO - confirm only care about first move in query payload here?
			MoveVO move = moves[0];
			
			updateViewStatus("(Player " + (gameModel.player + 1) + " moves piece #" + (move.pieceIndex + 1) + " to space #" + (move.destinationIndex + 1) + ".)");
		}

		private void onClickNewGame()
		{
			requestStartNewGameSignal.Dispatch();
		}

		private void onRecordMove(MoveVO move)
		{
			// Debug.Log ("LAAAA__________________***onRecordMOVE playerIndex: " + move.playerIndex + " destroy index:  " + move.destroyIndex + " pieceIndex:" + move.pieceIndex + " tyIndex:" + (gameModel.GetTypeIndex(move.destroyIndex) - 1));

// gameModel.GetTypeIndex(move.pieceIndex)

// view.ShowScoreboardImage(0, 3, false);

			view.UpdateScoreboard(move.playerIndex, gameModel.GetTypeIndex(move.destroyIndex) - 1, true);
		}

		// ... util
		private void updateViewStatus(string txt)
		{
			view.UpdateStatus(txt);
		}

		private void ResetScoreboard()
		{
			view.ResetScoreboard();
		}
		#endregion
	}
}