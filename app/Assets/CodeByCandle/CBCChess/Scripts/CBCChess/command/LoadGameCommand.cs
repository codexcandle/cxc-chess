using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.command.impl;
using cbc.cbcutils;

namespace cbc.cbcchess
{
	public class LoadGameCommand:CBCCommand
	{	
		// TODO - fix & uncomment below to allow for typed game-types
		/*
		 * [Inject]
		public IGameModel gameModel { get; set; }

		[Inject]
		public GameType gameType { get; set;}
		*/

		public override void Execute()
		{
			base.Execute();

			// gameModel.SetGameType(gameType);

			Application.LoadLevel("Game");
		}
	}
}