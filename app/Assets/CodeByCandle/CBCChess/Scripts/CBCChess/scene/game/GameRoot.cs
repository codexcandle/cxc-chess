using System;

using UnityEngine;

using strange.extensions.context.impl;

namespace cbc.cbcchess
{
	public class GameRoot:ContextView
	{	
		void Awake()
		{
			this.context = new GameContext(this);
		}
	}
}