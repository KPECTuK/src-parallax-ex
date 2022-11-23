using UnityEngine;

//using UnityEngine.XR.iOS;

/// <summary>
/// this script is for tracking the device camera;  
/// it doesn't track much at the moment since the AR session is using UnityARAlignment.UnityARAlignmentCamera;  
/// it may be used in the future;  
/// </summary>
public class CameraTracker : MonoBehaviour
{
	public Camera trackedCamera;

	// private bool _sessionStarted;

	//! do it as soon as AR service is available

	//private void Start()
	//{
	//	UnityARSessionNativeInterface.ARFrameUpdatedEvent += FirstFrameUpdate;
	//}

	//private void FirstFrameUpdate(UnityARCamera cam)
	//{
	//	_sessionStarted = true;
	//	UnityARSessionNativeInterface.ARFrameUpdatedEvent -= FirstFrameUpdate;
	//}

	private void Update()
	{
		//! from AR interface get device Camera position, rotation, and projection matrix and set it to model device camera

		//if(trackedCamera != null && _sessionStarted)
		//{
		//	Matrix4x4 cameraPose = UnityARSessionNativeInterface.GetARSessionNativeInterface().GetCameraPose();
		//	trackedCamera.transform.localPosition = UnityARMatrixOps.GetPosition(cameraPose);
		//	trackedCamera.transform.localRotation = UnityARMatrixOps.GetRotation(cameraPose);
		//	trackedCamera.projectionMatrix = UnityARSessionNativeInterface.GetARSessionNativeInterface().GetCameraProjection();
		//}
	}
}
