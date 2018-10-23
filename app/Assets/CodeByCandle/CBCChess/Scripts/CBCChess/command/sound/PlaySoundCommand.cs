using System;

using UnityEngine;

using System.Collections.Generic;

using strange.extensions.context.api;
using strange.extensions.command.impl;

using cbc.cbcutils;

namespace cbc.cbcchess
{
	public class PlaySoundCommand:CBCCommand
	{
		[Inject]
		public string soundID { get; set; }

		public override void Execute()
		{
			base.Execute();

			SfxrSynth synth = new SfxrSynth();
			synth.parameters.SetSettingsString(soundID);
			synth.CacheSound(() => synth.Play());

			// response.Dispatch(moves);
		}
	}
}