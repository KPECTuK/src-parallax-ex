using UnityEngine;

/// <summary>
/// this script doesn't actually select which eye to track (that happens in HeadTrackManager); 
/// this script merely moves the eye camera to the correct place (taking IPD into account); 
/// and also move the red dots that represent the eyes to the correct places  
/// </summary>
public class ControllerConstraintEye : MonoBehaviour
{
	public HeadTrackManager headTrackManager;
	public CameraManager camManager;
	public Transform leftEye;
	public Transform rightEye;
	public Transform thirdEye;

	private void Update()
	{
		var pos = transform.localPosition;
		var ipd = headTrackManager.IPD;
		var eyeHeight = headTrackManager.EyeHeight;

		var dist = ipd * 0.001f * 0.5f; // in meters and half
		var height = eyeHeight * 0.001f;

		if(headTrackManager.openEye == HeadTrackManager.OpenEye.Right)
		{
			// move camera to open eye
			if(!camManager.DeviceCamUsed)
			{
				pos.x = -dist;
			}
			else
			{
				pos.x = dist; // mirror in device cam
			}
		}
		else
		{
			if(!camManager.DeviceCamUsed)
			{
				pos.x = dist;
			}
			else
			{
				pos.x = -dist;
			}
		}

		transform.localPosition = pos;

		pos = thirdEye.transform.localPosition;
		pos.y = height;
		thirdEye.transform.localPosition = pos;

		// update eye positions, only for visualization purpose

		rightEye.localPosition = new Vector3(dist, 0, 0);
		leftEye.localPosition = new Vector3(-dist, 0, 0);
	}
}
