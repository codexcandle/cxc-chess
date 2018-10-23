using System;
using System.Collections.Generic;

using strange.extensions.signal.impl;
using StrangeCamera.Game;

namespace cbc.cbcchess
{	
	public class StartSignal:Signal{}

	public class PlaySoundSignal:Signal<string>{}

	// camera
	public class CameraStateSignal:Signal<CameraState> {}
	public class CameraSequenceSignal:Signal {}
	public class FlythroughCompleteSignal:Signal {}
}