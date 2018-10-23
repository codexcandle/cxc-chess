using UnityEngine;
using System.Collections;

using System;
using strange.extensions.command.impl;
using cbc.cbcchess;
using cbc.cbcutils;

namespace StrangeCamera.Game
{	
	public class CameraMoveCompleteCommand :CBCCommand
	{	
		[Inject]
		public IGameModel gameModel { get; set; }

		[Inject]
		public ICamera cameraModel { get; set; }

		[Inject]
		public CameraMoveCompleteSignal cameraMoveCompleteSignal 	{get; set;}
		
		public override void Execute()
		{
			base.Execute();

			// gameModel.StartGame();

			/*
			// HACK - below allows 2 1st camera-move completes, then start game... + playTurn after each cam reset...fix...
			cameraModel.SetCameraMoveCompleteCount(cameraModel.cameraMoveCompleteCount + 1);

			if(cameraModel.cameraMoveCompleteCount > 1)
				gameModel.SetState(GameState.PlayTurn);
			*/
		}
	}
}