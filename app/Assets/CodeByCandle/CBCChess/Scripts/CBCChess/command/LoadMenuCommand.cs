using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.command.impl;
using cbc.cbcutils;

namespace cbc.cbcchess
{
	public class LoadMenuCommand:CBCCommand
	{	
		// [Inject]
		//	public AppModel ModelInstance { get; set; }c

		public override void Execute()
		{
			base.Execute();

			Application.LoadLevel("Menu");
		}
	}
}