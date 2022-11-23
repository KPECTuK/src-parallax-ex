using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARSessionOrigin))]
[RequireComponent(typeof(ARFaceManager))]
public class CompController : MonoBehaviour
{
	private enum TrackPoint
	{
		EyeRight,
		FaceMiddlePoint,
		EyeLeft,
	}

	// can get from TargetEye on Camera component
	[SerializeField]
	private TrackPoint _trackPoint;
	[SerializeField]
	private Camera _cameraEye;

	private Camera _cameraDevice;

	private bool _eyeOpenLeft;
	private bool _eyeOpenRight;
	private bool _eyeAuto;

	private ARSession _session;
	private ARSessionOrigin _origin;
	private ARInputManager _managerInput;
	private ARFaceManager _managerFace;
	private ARCameraManager _managerCamera;
	private ARPoseDriver _driver;
	private ARCameraBackground _background;

	private void Awake()
	{
		_session = FindObjectOfType<ARSession>();
		_origin = GetComponent<ARSessionOrigin>();
		_cameraDevice = _origin.camera;
		_managerInput = _session.GetComponent<ARInputManager>();
		_managerCamera = _cameraDevice.GetComponent<ARCameraManager>();
		_managerFace = GetComponent<ARFaceManager>();
		_driver = _cameraDevice.GetComponent<ARPoseDriver>();
		_background = _cameraDevice.GetComponent<ARCameraBackground>();

		Application.targetFrameRate = 60;

		ARSession.stateChanged += OnSessionStateChange;
		_managerFace.facesChanged += OnFaceChange;
		_managerCamera.frameReceived += OnFrameReceive;
		_origin.trackablesParentTransformChanged += OnTrackablesChanged;

		$"[AR] session state: {ARSession.state}".Log();
	}

	private void OnDestroy()
	{
		// TODO: clear all event handlers injected
	}

	private void SetStatusMessage(string message) { }

	private void OnFrameReceive(ARCameraFrameEventArgs args)
	{
		"[AR] frame".Log();
	}

	private void OnSessionStateChange(ARSessionStateChangedEventArgs args)
	{
		$"[AR] status: {args.state}".Log();
	}

	private void OnTrackablesChanged(ARTrackablesParentTransformChangedEventArgs args)
	{
		$"[AR] trackables changed: {args.trackablesParent.name}".Log();
	}

	private void OnFaceChange(ARFacesChangedEventArgs args)
	{
		$"[AR] faces change: [ upd: {args.updated.Count}; add: {args.added.Count}; rem: {args.removed.Count} ]".Log();

		foreach(var face in args.removed)
		{
			FaceAdded(face);
		}

		foreach(var face in args.added)
		{
			FaceRemove(face);
		}

		foreach(var face in args.updated)
		{
			FaceUpdate(face);
		}
	}

	private void SetLocationParking(Transform target) { }

	private void SetLocationByFace(Transform target, ARFace face)
	{
		var pos = face.fixationPoint.position;
		var rot = face.fixationPoint.rotation;

		// use device camera
		// in device cam viewing mode, don't invert on x because this view is mirrored
		target.SetPositionAndRotation(
			pos,
			rot);

		//if(camManager.DeviceCamUsed)
		//{
		//	// use device camera
		//	// in device cam viewing mode, don't invert on x because this view is mirrored
		//	target.SetPositionAndRotation(
		//		pos,
		//		rot);
		//}
		//else
		//{
		//	// use device camera
		//	// invert on x because ARFaceAnchors are inverted on x (to mirror in display)
		//	target.SetPositionAndRotation(
		//		new Vector3(-pos.x, pos.y, pos.z),
		//		new Quaternion(-rot.x, rot.y, rot.z, -rot.w));
		//}
	}

	private void FaceAdded(ARFace face)
	{
		//! storing this
		// face.updated += OnFaceUpdate;

		SetLocationByFace(_cameraEye.transform, face);
		_cameraEye.enabled = true;

		"[AR] start track eye".Log();
	}

	private void FaceRemove(ARFace face)
	{
		_cameraEye.enabled = false;
		SetLocationParking(_cameraEye.transform);

		"[AR] stop track eye".Log();
	}

	private void FaceUpdate(ARFace face)
	{
		SetLocationByFace(_cameraEye.transform, face);

		if(_eyeAuto)
		{
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

			//if(Mathf.Abs(rightEyeClosed - leftEyeClosed) > 0.2f)
			//{
			//	openEye = rightEyeClosed > leftEyeClosed
			//		? OpenEye.Left
			//		: OpenEye.Right;
			//}
		}

		var logAuto = _eyeAuto ? "auto" : string.Empty;
		var logEyeLeft = _eyeOpenLeft ? string.Empty : "right";
		var logEyeRight = _eyeOpenRight ? string.Empty : "right";
		var message = $"[AR] eye {logAuto}: racking {_trackPoint} (closed: {logEyeLeft} / {logEyeRight})";
		SetStatusMessage(message);
	}
}
