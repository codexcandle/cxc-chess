using System;
using UnityEngine;
using strange.extensions.context.impl;

namespace cbc.cbcchess
{
	public class MainRoot:ContextView
	{
		void Awake()
		{
			context = new MainContext(this);
		}
	}
}