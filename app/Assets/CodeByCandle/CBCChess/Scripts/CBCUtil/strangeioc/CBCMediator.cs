using System;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;

namespace cbc.cbcutils
{
	public class CBCMediator:Mediator
	{	
		// functions (public) -----------------------------------
		public override void OnRegister()
		{
			base.OnRegister();

			// print command call to debug console
			if(Debug.isDebugBuild)
			{
				string[] parts = this.ToString().Split('.');
				int count = parts.Length;
				string trimName = parts[count - 1];

				// not sure why, but must remove extra ending char (")") for mediator class
				trimName = trimName.Substring(0, trimName.Length - 1);

				Debug.Log ("<b>Mediator:</b> " + trimName);
			}
		}
	}
}