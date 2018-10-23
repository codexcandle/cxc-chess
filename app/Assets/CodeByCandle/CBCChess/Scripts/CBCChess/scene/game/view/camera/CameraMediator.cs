using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;
using strange.extensions.mediation.impl;

using cbc.cbcchess;
using cbc.cbcutils;

namespace StrangeCamera.Game
{	
	public class CameraMediator:CBCMediator
	{
		[Inject]
		public CameraView view 										{get; set;}

		[Inject]
		public IGameModel gameModel 								{get; set;}
		[Inject]
		public ICamera model 										{get; set;}

		[Inject]
		public GameStateChangeSignal gameStateChangeSignal {get; set;}
		[Inject]
		public CameraSequenceSignal cameraSequenceSignal	 		{get; set;}
		[Inject]
		public CameraStateSignal cameraStateSignal					{get; set;}
		[Inject]
		public CameraMoveCompleteSignal cameraMoveCompleteSignal 	{get; set;}
		[Inject]
		public FlythroughCompleteSignal flythroughCompleteSignal 	{get; set;}
		[Inject]
		public CameraFocusPlayerSignal cameraFocusPlayerSignal 		{get; set;}
		// [Inject]
		// public ReplaySignal replaySignal { get; set; }// 
		[Inject]
		public CameraPlayerZoomCompleteSignal cameraPlayerZoomCompleteSignal	{get; set;}

		private bool initialZoomComplete;

		#region FUNCTIONS (public)
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
		#endregion
		
		#region FUNCTIONS (private)
		private void enableListeners(bool enable)
		{
			if(enable)
			{
				// ... app
				gameStateChangeSignal.AddListener(handleGameState);
				cameraStateSignal.AddListener(onCameraStateChanged);
				cameraFocusPlayerSignal.AddListener(onCameraFocusPlayer);

				// replaySignal.AddListener(onReplay);
				// ... view
				view.movementComplete.AddListener(onMovementComplete);
			}
			else
			{
				// ... app
				gameStateChangeSignal.AddListener(handleGameState);
				cameraStateSignal.RemoveListener(onCameraStateChanged);
				cameraFocusPlayerSignal.RemoveListener(onCameraFocusPlayer);
				// replaySignal.RemoveListener(onReplay);
				// ... view
				view.movementComplete.RemoveListener(onMovementComplete);
			}
		}

		private void handleGameState(GameState state)
		{
			switch(state)
			{
				case GameState.SETUP:
					view.init();
					break;
				case GameState.STARTING:
					// kick-off with intro-game zoom
					// cameraSequenceSignal.Dispatch();
					
					// -- OR --
					
					// quick-zoom kick-off
					zoomToPlayer(gameModel.player);
					
					break;
				case GameState.LOAD_PLAYER:
					zoomToPlayer(gameModel.player);
					break;
			}
		}

		private void onCameraStateChanged(CameraState state)
		{
			model.SetState(state);
			
			view.stateChange(state);
			
			switch((int)state)
			{
			case (int)CameraState.CINEMATIC:
				StartCoroutine(flyToWaypoints());
				
				// demo
				// view.beginFlythrough();
				// cinematicStart = true;	
				break;
			case (int)CameraState.CHARACTER:
				zoomToPlayer(gameModel.player);
				
				//  view.attachToCharacter();
				// demo
				// characterAttach = true;
				break;
			}
		}

		private void onCameraFocusPlayer(int playerIndex)
		{
			zoomToPlayer(playerIndex);
		}
		#endregion










		private void onMovementComplete()
		{
			cameraPlayerZoomCompleteSignal.Dispatch();

			if(!initialZoomComplete)
				initialZoomComplete = true;
		}
		
		private void onReplay()
		{
			OnRemove();
			OnRegister();
			
			// demo
			model.ClearWaypoints();
		}

		// ... move
		private void zoomToPlayer(int playerIndex)
		{
			view.LoadPlayerPerspective(playerIndex);
		}
		
		private IEnumerator flyToWaypoints()
		{
			CameraWaypoint waypoint;

			int len = model.waypoints.Count;
			for(int i = 0; i < len; i++)
			{
				waypoint = model.waypoints[i];
				view.flyToWaypoint(waypoint);

				yield return new WaitForSeconds(waypoint.duration + waypoint.delay);
				
				// demo
				// currentWaypoint++;
			}
			
			flythroughCompleteSignal.Dispatch();
			
			// demo
			// cinematicEnd = true;
			// yield return new WaitForSeconds(2f);
			// initialSequence = false;
			// currentWaypoint = -1;
			
			yield return null;
		}
	}
}