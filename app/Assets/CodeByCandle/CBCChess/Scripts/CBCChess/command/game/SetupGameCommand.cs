using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.command.impl;
using cbc.cbcutils;
using StrangeCamera.Game;

namespace cbc.cbcchess
{
	public class SetupGameCommand:CBCCommand
	{	
		[Inject]
		public IGameModel gameModel { get; set; }

		public override void Execute()
		{
			base.Execute();

			gameModel.Setup();

//gameModel.BeginGame();
		}
	}
}