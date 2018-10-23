using UnityEngine;
using System.Collections;

using strange.extensions.context.api;
using strange.extensions.command.impl;
using StrangeCamera.Game;
using cbc.cbcchess;
using cbc.cbcutils;

public class CameraFlythroughCommand:CBCCommand
{	
	[Inject]
	public IGameModel gameModel { get; set; }

	[Inject]
	public ICamera model { get; set; }
	
	[Inject]
	public CameraStateSignal cameraStateSignal { get; set; }

	[Inject]
	public FlythroughCompleteSignal flythroughCompleteSignal { get; set; }

	[Inject]
	public GameStateChangeSignal gameStateChangeSignal {get; set; }


	public override void Execute()
	{
		base.Execute();

		// stop sequencer until release is called
		Retain();

		flythroughCompleteSignal.AddListener(onFlythroughComplete);
		
		startFlythrough();
	}
	
	private void startFlythrough()
	{
		// waypoint:
		// - from position, to position
		// - from rotation, to rotation
		// - duration, delay

		/*
		model.AddWaypoint(new CameraWaypoint(
			new Vector3(0.5f, 227f, 32f), new Vector3(6f, 200f, 24f),
			new Vector3(24f, 104f, 0), new Vector3(57f, 7f, 0),
			5f, 0.1f
			));
*/
		model.AddWaypoint(new CameraWaypoint(
							new Vector3(6f, 200f, 24f), new Vector3(20.0f,40, 0),
							new Vector3(60.0f, 10.0f, 0), new Vector3(20f, 10f, 0),
							5.0f, 1.0f));

/*
		model.AddWaypoint(new CameraWaypoint(
			new Vector3(0f, 0f, -21f), new Vector3(70.0f, 150f, -10.0f),
			new Vector3(35f, 260f, 0), new Vector3(60.0f, 0, 0),
			4f, 5.0f
			));
*/
		cameraStateSignal.Dispatch(CameraState.CINEMATIC);
	}
	
	private void onFlythroughComplete()
	{
		model.ClearWaypoints();
		
		flythroughCompleteSignal.RemoveListener(onFlythroughComplete);
			
		cameraStateSignal.Dispatch(CameraState.CHARACTER);

		// continue sequencer
		Release();
	}
}