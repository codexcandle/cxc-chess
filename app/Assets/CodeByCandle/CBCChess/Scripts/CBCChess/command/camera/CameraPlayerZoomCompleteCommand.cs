using UnityEngine;
using System.Collections;

using System;
using strange.extensions.command.impl;
using cbc.cbcchess;
using cbc.cbcutils;

namespace StrangeCamera.Game
{	
	public class CameraPlayerZoomCompleteCommand:CBCCommand
	{	
		[Inject]
		public IGameModel gameModel { get; set; }
		
		[Inject]
		public ICamera cameraModel { get; set; }
		
		public override void Execute()
		{
			base.Execute();

			gameModel.PlayTurn();
		}
	}
}