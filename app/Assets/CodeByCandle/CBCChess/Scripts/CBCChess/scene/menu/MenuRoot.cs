using System;

using UnityEngine;

using strange.extensions.context.impl;

namespace cbc.cbcchess
{
	public class MenuRoot : ContextView
	{	
		void Awake()
		{
			this.context = new MenuContext(this);
		}
	}
}