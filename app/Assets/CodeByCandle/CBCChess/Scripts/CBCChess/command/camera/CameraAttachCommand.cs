using UnityEngine;
using System.Collections;
using System;
using strange.extensions.command.impl;
using cbc.cbcchess;
using cbc.cbcutils;

namespace StrangeCamera.Game
{	
	public class CameraAttachCommand:CBCCommand
	{
		[Inject]
		public IGameModel gameModel { get; set; }

		[Inject]
		public CameraStateSignal cameraStateSignal { get; set; }

		public override void Execute()
		{
			base.Execute();

			cameraStateSignal.Dispatch(CameraState.CHARACTER);
		}
	}
}