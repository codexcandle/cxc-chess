using UnityEngine;
using System.Collections.Generic;
using StrangeCamera.Game;

public class CameraModel:ICamera
{	
	private CameraState _state;
	private List<CameraWaypoint> _waypoints;

	// HACK - below allows 2 1st camera-move completes, then start game... 
	private int _cameraMoveCompleteCount;
	
	public CameraState state
	{
		get
		{
			return _state;
		}
	}
	
	public List<CameraWaypoint> waypoints
	{
		get
		{
			return _waypoints;
		}
	}
	
	public CameraModel()
	{
		_waypoints = new List<CameraWaypoint>();
	}
	
	public void SetState(CameraState value)
	{
		_state = value;
	}
	
	public void AddWaypoint(CameraWaypoint value)
	{
		_waypoints.Add(value);
	}

	public int cameraMoveCompleteCount
	{
		get
		{
			return _cameraMoveCompleteCount;
		}
	}
	
	public void SetCameraMoveCompleteCount(int value)
	{
		_cameraMoveCompleteCount = value;
	}

	public void ClearWaypoints()
	{
		_waypoints.Clear();
	}
}