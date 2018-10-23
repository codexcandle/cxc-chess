using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.command.impl;

namespace cbc.cbcutils
{
	public class CBCCommand:Command
	{	
		public override void Execute()
		{
			// print command call to debug console
			if(Debug.isDebugBuild)
			{
				string[] parts = this.ToString().Split('.');
				int count = parts.Length;
				string trimName = parts[count - 1];
				Debug.Log ("<b>Command:</b> --- " + trimName);
			}
		}
	}
}