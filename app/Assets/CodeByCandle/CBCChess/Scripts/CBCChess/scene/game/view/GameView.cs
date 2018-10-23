using UnityEngine;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace cbc.cbcchess
{
	public class GameView:View
	{
		public GameView():base(){}

		protected override void Start()
		{
			base.Start ();
			
			initGame();
		}

		private void initGame()
		{	
			// gameActive = true;
		}
	}
}