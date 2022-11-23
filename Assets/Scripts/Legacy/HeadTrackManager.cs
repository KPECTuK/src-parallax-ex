using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary> script for setting up ARKit for 3D head tracking purposes </summary>
public class HeadTrackManager : MonoBehaviour
{
	public enum OpenEye
	{
		Left,
		Right
	}

	public GameObject headCenter;
	public CameraManager camManager;
	public float leftEyeClosed;
	public float rightEyeClosed;
	// which eye is being tracked, auto or not
	public string eyeInfoText;
	// inter pupil distance (mm)
	public float IPD = 64f;
	// eye height from head anchor (mm)
	public float EyeHeight = 32f;
	public string ARError;

	[NonSerialized]
	public OpenEye openEye = OpenEye.Right;
	private bool _autoEye;

	public void Start()
	{
		ARError = string.Empty;

		var session = FindObjectOfType<ARSession>();
		var faceManager = FindObjectOfType<ARFaceManager>();

		faceManager.facesChanged += OnFaceChanged;

		//UnityARSessionNativeInterface.ARSessionFailedEvent += CatchARSessionFailed;
		ARSession.stateChanged += _ => { Debug.Log($"AR session state: {_.state}"); };

		Application.targetFrameRate = 60;
		//ARKitFaceTrackingConfiguration config = new ARKitFaceTrackingConfiguration();
		////config.alignment = UnityARAlignment.UnityARAlignmentGravity; // using gravity alignment enables orientation (3DOF) tracking of device camera. we don't need it
		//config.alignment = UnityARAlignment.UnityARAlignmentCamera;

		//config.enableLightEstimation = true;

		//if(config.IsSupported)
		//{
		//	m_session.RunWithConfig(config);

		//	UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdded;
		//	UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdated;
		//	UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent += FaceRemoved;
		//	// can't get the light direction estimate to work for some reason, it freezes the app
		//	//UnityARSessionNativeInterface.ARFrameUpdatedEvent += FrameUpdate; 
		//}
		//else
		//{
		//	Debug.Log("ARKitFaceTrackingConfiguration not supported");
		//}
	}

	private void CatchARSessionFailed(string error)
	{
		ARError = error;
	}

	private void UpdateLocationByFace(ARFace face, Transform target)
	{
		var pos = face.fixationPoint.position;
		var rot = face.fixationPoint.rotation;

		if(camManager.DeviceCamUsed)
		{
			// use device camera
			// in device cam viewing mode, don't invert on x because this view is mirrored
			target.SetPositionAndRotation(
				pos,
				rot);
		}
		else
		{
			// use device camera
			// invert on x because ARFaceAnchors are inverted on x (to mirror in display)
			target.SetPositionAndRotation(
				new Vector3(-pos.x, pos.y, pos.z),
				new Quaternion(-rot.x, rot.y, rot.z, -rot.w));
		}
	}

	private void OnFaceChanged(ARFacesChangedEventArgs args)
	{

	}

	private void FaceAdded(ARFace face)
	{
		UpdateLocationByFace(face, headCenter.transform);
		headCenter.SetActive(true);

		// _currentBlendShapes = anchorData.blendShapes;
	}

	private void FaceUpdate(ARFace face)
	{
		UpdateLocationByFace(face, headCenter.transform);
		// _currentBlendShapes = anchorData.blendShapes;

		var logBlink = string.Empty;

		if(_autoEye)
		{
			//?face.updated

			//if(_currentBlendShapes.ContainsKey("eyeBlink_L"))
			//{
			//	// mirrored geometry
			//	rightEyeClosed = _currentBlendShapes["eyeBlink_L"];
			//	logBlink = "blink: right eye";
			//}

			//if(_currentBlendShapes.ContainsKey("eyeBlink_R"))
			//{
			//	// mirrored geometry
			//	leftEyeClosed = _currentBlendShapes["eyeBlink_R"];
			//	logBlink = "blink: left eye";
			//}

			// these values seem to be in the 0.2 .. 0.7 range.. 
			// but sometimes, when viewing the phone low in the visual field, they get very high even while open (eyelids almost close)
			// we'll use a difference metric and if exceeded we select the most open eye

			if(Mathf.Abs(rightEyeClosed - leftEyeClosed) > 0.2f)
			{
				openEye = rightEyeClosed > leftEyeClosed
					? OpenEye.Left
					: OpenEye.Right;
			}

			// old method
			//if rightEyeClosed > 0.5 && leftEyeClosed < 0.5)
			//	openEye = OpenEye.Left;
			//if(rightEyeClosed < 0.5 && leftEyeClosed > 0.5)
			//	openEye = OpenEye.Right;
		}

		var logAuto = _autoEye ? "auto" : string.Empty;
		var logSide = openEye == OpenEye.Left ? "left" : "right";
		eyeInfoText = $"{logAuto}: {logSide} Eye ({logBlink})";
	}

	private void FaceRemove(ARFace face)
	{
		headCenter.SetActive(false);
		eyeInfoText = "eye was lost";
	}

	private void FrameUpdate()
	{
		//can't get the light direction estimate to work for some reason, it freezes the app
		//keylight.transform.rotation = Quaternion.FromToRotation(Vector3.back, cam.lightData.arDirectonalLightEstimate.primaryLightDirection); // <- probably incorrect way to do it
		//keylight.transform.rotation = Quaternion.LookRotation(cam.lightData.arDirectonalLightEstimate.primaryLightDirection); // <- probably correct way to do it
	}

	public void SetEyeHeight(float value)
	{
		EyeHeight = value;
	}

	public void SetIPD(float value)
	{
		IPD = value;
	}

	public void SetLeftEye()
	{
		_autoEye = false;
		openEye = OpenEye.Left;
	}

	public void SetRightEye()
	{
		_autoEye = false;
		openEye = OpenEye.Right;
	}

	public void SetAutoEye()
	{
		_autoEye = true;
	}
}
