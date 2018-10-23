﻿using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.command.impl;
using cbc.cbcutils;

namespace cbc.cbcchess
{
	public class RequestStartNewGameCommand:CBCCommand
	{	
		[Inject]
		public CameraSequenceSignal cameraSequenceSignal { get; set; }
		
		[Inject]
		public IGameModel gameModel { get; set; }
		
		public override void Execute()
		{
			base.Execute();

			if(gameModel.active)
			{
				gameModel.ForfeitGame();
				
				gameModel.Setup();
			}
			else if(gameModel.victor > -1)
			{
				gameModel.Setup();
			}

			gameModel.BeginGame();
		}
	}
}