using UnityEngine;
using System.Collections;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace StrangeCamera.Game
{	
	public class CameraView:View
	{
		public GameObject target;

		public Signal movementComplete 		= new Signal();

		private const float DISTANCE 		= 15f;
		private const float HEIGHT 			= 8f;
		private const float SPEED 			= 2.5f;

		private Transform _transform;
		private CameraState _state;
		private CameraWaypoint _waypoint;

		// demo controls
		private float cameraDistance 		= DISTANCE;
		private float cameraHeight 			= HEIGHT;
		private float cameraSpeed 			= SPEED;
		private bool cameraLookAt 			= false;

		// functions (public) --------------------------------
/*
		public void zoomToPlayer(int playerIndex)
		{
			float smoothTime = 0.3F;
			Vector3 velocity = Vector3.zero;
			
			// TODO - temp target below...
			// Transform target = pieces[31].transform;
			
			//	Vector3 targetPosition = target.TransformPoint(new Vector3(0, 5, -10));
			//	transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
		}
*/
		
		public void LoadPlayerPerspective(int playerIndex)
		{
			float moveX = 0.0F;
			float moveZ = 0.0F;
			float rotateY = 0.0F;
			if(playerIndex == 0)
			{
				moveX = 50.0F;
				moveZ = -10.0F;
				rotateY = 10.0F;
			}
			else
			{
				moveX = 70.0F;
				moveZ = 150.0F;
				rotateY = 175.0F;
			}

			// TODO - combine 2 bottom calls....
			LeanTween.move(gameObject, 
			               new Vector3(moveX, 150.0F, moveZ), 
			               2)
						.setEase(LeanTweenType.easeInOutSine);
			
			LeanTween.rotate(gameObject, new Vector3(60.0F, rotateY, 0.0F), 1.0F)
						.setUseEstimatedTime(true)
						.setOnComplete(onLoadPlayerPerspectiveComplete)
						.setDelay(1)
						.setEase(LeanTweenType.easeInOutSine);
		}

		// functions (internal) ------------------------------
		internal void init()
		{
			_transform = transform;
		}

		internal void LateUpdate()
		{
			/*
			if(_state == CameraState.CHARACTER)
				updateCharacterCamera();
			else*/ 
			
			if(_state == CameraState.CINEMATIC)
				updateCinematicCamera();
		}
		
		internal void stateChange(CameraState state)
		{
			_state = state;

			/*
			if(state == CameraState.CHARACTER)
				LoadPlayerPerspective(0);
			*/
		}
		
		internal void flyToWaypoint(CameraWaypoint waypoint)
		{
			_transform.position = waypoint.from.position;
			_transform.localRotation = waypoint.from.rotation;
			
			_waypoint = waypoint;
		}
		
		internal void beginFlythrough()
		{
			// demo - disable controls
			// target.GetComponent<ThirdPersonController>().isControllable = false;
		}

		internal void attachToCharacter() {
			// demo - enable controls
			// target.GetComponent<ThirdPersonController>().isControllable = true;
		}

		// ... demo controls
		internal void setCameraDistance(float distance) {
			cameraDistance = distance;
		}
		
		internal void setCameraHeight(float height) {
			cameraHeight = height;
		}
		
		internal void setCameraSpeed(float speed) {
			cameraSpeed = speed;
		}
		
		internal void setLookAtTarget(bool lookAt) {
			cameraLookAt = lookAt;
		}

		// functions (private) -------------------------------
		private void updateCinematicCamera()
		{
			float t = _waypoint.duration / 10f * Time.deltaTime;
			
			_transform.position = Vector3.Lerp(_transform.position, _waypoint.to.position, t);
			_transform.localRotation = Quaternion.Slerp(_transform.localRotation, _waypoint.to.rotation, t);
		}
		
		private void updateCharacterCamera()
		{
			float t = cameraSpeed * Time.deltaTime;
			
			_transform.position = Vector3.Lerp(_transform.position, 
			                                   target.transform.position + new Vector3(cameraDistance, cameraHeight, -cameraDistance), 
			                                   t);

			// hiding this now to allow camera-down-tilt for gameboard
			/*
			if(cameraLookAt)
			{
				_transform.rotation = Quaternion.Slerp (_transform.rotation,
				                                        Quaternion.LookRotation(target.transform.position - _transform.position), t);
			}
			else
			{
				_transform.rotation = Quaternion.Slerp(_transform.rotation,
				                                       Quaternion.Euler(new Vector3(20f, 0f, 0)), t);
			}
			*/
		}
		
		private void onLoadPlayerPerspectiveComplete()
		{
			// play sound
			// requestPlaySound.Dispatch(GameConstants.SOUND_PLAYER_READY);
			
			// SetState(STATE.SelectPiece);
			
			// DisplayPromptMoveText(playerIndex);
			
			movementComplete.Dispatch();
		}
	}
}