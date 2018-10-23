using UnityEngine;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

using System;
using System.Collections;
using System.Collections.Generic;

namespace cbc.cbcchess
{
	public class MenuView:View
	{
		public GameObject playButton;

		public Signal playComputerClick = new Signal();
		public Signal playNetworkClick = new Signal();
		
		// vars (private) ------------------------------------
		private bool initialized;
		
		#region PUBLIC
		// TODO - am now using mapped handler in Hierarchy window? is this most OOP?
		public void clickPlayComputer()
		{
			playComputerClick.Dispatch();
		}

		// TODO - am now using mapped handler in Hierarchy window? is this most OOP?
		public void clickPlayNetwork()
		{
			playNetworkClick.Dispatch();
		}

		public void Initialize()
		{
			if(initialized)
			{
				return;
			}
			
			initialized = true;

			/// playButton.add
		}
		#endregion
	}
}