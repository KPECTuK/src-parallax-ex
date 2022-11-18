using UnityEngine;

/// <summary> this script is for setting up the non-symmetric camera frustum and compute the off-axis projection matrix used for the eye camera </summary>
// [ExecuteInEditMode]
public class OffAxisProjection : MonoBehaviour
{
	public Camera deviceCamera;
	public Camera eyeCamera;
	public LineRenderer lineRenderer;
	public float left;
	public float right;
	public float bottom;
	public float top;
	public float near;
	public float far;
	public CameraManager camManager;

	public float nearDist;

	private void LateUpdate()
	{
		// look opposite direction of device cam
		var opposite = deviceCamera.transform.rotation * Quaternion.Euler(Vector3.up * 180);
		eyeCamera.transform.rotation = opposite;
		// find device camera in rendering camera's view space
		var deviceCamPos = eyeCamera.transform.worldToLocalMatrix.MultiplyPoint(deviceCamera.transform.position);
		// normal of plane defined by device camera
		var fwd = eyeCamera.transform.worldToLocalMatrix.MultiplyVector(deviceCamera.transform.forward);
		var devicePlane = new Plane(fwd, deviceCamPos);

		var close = devicePlane.ClosestPointOnPlane(Vector3.zero);
		near = close.magnitude;

		// couldn't get device orientation to work properly in all cases,
		// so just landscape for now (it's just the UI that is locked to landscape,
		// everything else works just fine)

		// portrait iphone X
		//if(Screen.orientation == ScreenOrientation.Portrait)
		//{ 
		//	left = trackedCamPos.x - 0.040f;
		//	right = trackedCamPos.x + 0.022f;
		//	top = trackedCamPos.y + 0.000f;
		//	bottom = trackedCamPos.y - 0.135f;
		//}
		//else
		//{

		// landscape iPhone X, measures in meters
		left = deviceCamPos.x - 0.000f;
		right = deviceCamPos.x + 0.135f;
		top = deviceCamPos.y + 0.022f;
		bottom = deviceCamPos.y - 0.040f;

		// may need bigger for bigger scenes, max 10 meters for now
		far = 10f;

		//var topLeft = new Vector3(left, top, near);
		//var topRight = new Vector3(right, top, near);
		//var bottomLeft = new Vector3(left, bottom, near);
		//var bottomRight = new Vector3(right, bottom, near);

		//if(lineRenderer != null && camManager != null)
		//{
		//	if(camManager.EyeCamUsed)
		//	{
		//		lineRenderer.enabled = false;
		//	}
		//	else
		//	{
		//		// visualize frustum. or more exactly, the 4 sided pyramid in front of the actual frustum

		//		lineRenderer.enabled = true;

		//		var w_topLeft = eyeCamera.transform.localToWorldMatrix.MultiplyPoint(topLeft);
		//		var w_topRight = eyeCamera.transform.localToWorldMatrix.MultiplyPoint(topRight);
		//		var w_bottomLeft = eyeCamera.transform.localToWorldMatrix.MultiplyPoint(bottomLeft);
		//		var w_bottomRight = eyeCamera.transform.localToWorldMatrix.MultiplyPoint(bottomRight);

		//		if(camManager.DeviceCamUsed)
		//		{
		//			// flip on x. still a bit unsure what is correct here (in device cam mirrored view) but I like this visualisation.
		//			w_topLeft = eyeCamera.transform.localToWorldMatrix.MultiplyPoint(new Vector3(topLeft.x, topLeft.y, topLeft.z));
		//			w_topRight = eyeCamera.transform.localToWorldMatrix.MultiplyPoint(new Vector3(topRight.x, topRight.y, topRight.z));
		//			w_bottomLeft = eyeCamera.transform.localToWorldMatrix.MultiplyPoint(new Vector3(bottomLeft.x, bottomLeft.y, bottomLeft.z));
		//			w_bottomRight = eyeCamera.transform.localToWorldMatrix.MultiplyPoint(new Vector3(bottomRight.x, bottomRight.y, bottomRight.z));
		//		}

		//		lineRenderer.SetPosition(0, eyeCamera.transform.position);
		//		lineRenderer.SetPosition(1, w_topLeft);
		//		lineRenderer.SetPosition(2, w_topRight);
		//		lineRenderer.SetPosition(3, eyeCamera.transform.position);
		//		lineRenderer.SetPosition(4, w_bottomLeft);
		//		lineRenderer.SetPosition(5, w_bottomRight);
		//		lineRenderer.SetPosition(6, eyeCamera.transform.position);
		//	}
		//}

		nearDist = near;

		// move near to 0.01 (1 cm from eye)
		// [scale using import settings]

		var scaleFactor = 0.01f / near;
		near *= scaleFactor;
		left *= scaleFactor;
		right *= scaleFactor;
		top *= scaleFactor;
		bottom *= scaleFactor;

		// [use Matrix4x4.Projection()]
		eyeCamera.projectionMatrix = PerspectiveOffCenter(left, right, bottom, top, near, far);
	}

	/// <summary> to perspective off center projection matrix </summary>
	private static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
	{
		var x = 2.0F * near / (right - left);
		var y = 2.0F * near / (top - bottom);
		var a = (right + left) / (right - left);
		var b = (top + bottom) / (top - bottom);
		var c = -(far + near) / (far - near);
		var d = -(2.0F * far * near) / (far - near);
		var e = -1.0F;
		// @formatter:off
		var m = new Matrix4x4
		{
			[0, 0] = x, [0, 1] = 0, [0, 2] = a, [0, 3] = 0,
			[1, 0] = 0, [1, 1] = y, [1, 2] = b, [1, 3] = 0,
			[2, 0] = 0, [2, 1] = 0, [2, 2] = c, [2, 3] = d,
			[3, 0] = 0, [3, 1] = 0, [3, 2] = e, [3, 3] = 0
		};
		// @formatter:on
		return m;
	}
}
