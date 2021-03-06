﻿// (c) Copyright HutongGames, LLC 2010-2014. All rights reserved.

using UnityEngine;
using InControl;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("InControl")]
	[Tooltip("Gets a world direction Vector from 2 Incontrol control Axis for a given device. Typically used for a third person controller with Relative To set to the camera.")]
	public class GetInControlDeviceInputAxisVector : FsmStateAction
	{
		public enum AxisPlane
		{
			XZ,
			XY,
			YZ
		}

		[Tooltip("The index of the device. -1 to use the active device")]
		public FsmInt deviceIndex;
		
		public InputControlType horizontalAxis;

		public InputControlType verticalAxis;
		
		[Tooltip("Input axis are reported in the range -1 to 1, this multiplier lets you set a new range.")]
		public FsmFloat multiplier;
		
		[RequiredField]
		[Tooltip("The world plane to map the 2d input onto.")]
		public AxisPlane mapToPlane;
		
		[Tooltip("Make the result relative to a GameObject, typically the main camera.")]
		public FsmGameObject relativeTo;
		
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the direction vector.")]
		public FsmVector3 storeVector;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the length of the direction vector.")]
		public FsmFloat storeMagnitude;

		InputDevice _inputDevice;
		InputControl _inputControl;

		public override void Reset()
		{
			deviceIndex = 0;
			horizontalAxis = InputControlType.LeftStickRight;
			verticalAxis = InputControlType.LeftStickRight;
			multiplier = 1.0f;
			mapToPlane = AxisPlane.XZ;
			storeVector = null;
			storeMagnitude = null;
		}
		
		public override void OnUpdate()
		{
			var forward = new Vector3();
			var right = new Vector3();
			
			if (relativeTo.Value == null)
			{
				switch (mapToPlane) 
				{
				case AxisPlane.XZ:
					forward = Vector3.forward;
					right = Vector3.right;
					break;
					
				case AxisPlane.XY:
					forward = Vector3.up;
					right = Vector3.right;
					break;
					
				case AxisPlane.YZ:
					forward = Vector3.up;
					right = Vector3.forward;
					break;
				}
			}
			else
			{
				var transform = relativeTo.Value.transform;
				
				switch (mapToPlane) 
				{
				case AxisPlane.XZ:
					forward = transform.TransformDirection(Vector3.forward);
					forward.y = 0;
					forward = forward.normalized;
					right = new Vector3(forward.z, 0, -forward.x);
					break;
					
				case AxisPlane.XY:
				case AxisPlane.YZ:
					// NOTE: in relative mode XY ans YZ are the same!
					forward = Vector3.up;
					forward.z = 0;
					forward = forward.normalized;
					right = transform.TransformDirection(Vector3.right);
					break;
				}
				
				// Right vector relative to the object
				// Always orthogonal to the forward vector
				
			}
			
			// get individual axis
			// leaving an axis blank or set to None sets it to 0

			if (deviceIndex.Value ==-1)
			{
				_inputDevice = InputManager.ActiveDevice;
			}else{
				_inputDevice= InputManager.Devices[deviceIndex.Value];
			}

			var h = _inputDevice.GetControl(horizontalAxis).Value;
			var v = _inputDevice.GetControl(verticalAxis).Value;
			
			// calculate resulting direction vector
			
			var direction = h * right + v * forward;
			direction *= multiplier.Value;
			
			storeVector.Value = direction;
			
			if (!storeMagnitude.IsNone)
			{
				storeMagnitude.Value = direction.magnitude;
			}
		}
	}
}

