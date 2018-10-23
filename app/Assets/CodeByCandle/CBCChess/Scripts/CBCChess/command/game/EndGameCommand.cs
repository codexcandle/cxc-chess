using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.command.impl;
using cbc.cbcutils;

namespace cbc.cbcchess
{
	public class EndGameCommand:CBCCommand
	{	
		[Inject]
		public IGameModel gameModel { get; set; }
		
		public override void Execute()
		{
			base.Execute();

			if(gameModel.active)
			{
				gameModel.ForfeitGame();

				gameModel.Setup();
				
				gameModel.BeginGame();
			}
		}
	}
}