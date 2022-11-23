using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct Frustum
{
	public Vector3 VectorSource;
	public Vector3 VectorForward;

	public Vector3 VectorUR;
	public Vector3 VectorDR;
	public Vector3 VectorDL;
	public Vector3 VectorUL;

	public Plane PlaneScreen;
	public Plane PlaneLeft;
	public Plane PlaneRight;
	public Plane PlaneUp;
	public Plane PlaneDown;

	public Vector3 ToPointUR => (VectorUR - VectorSource).normalized;
	public Vector3 ToPointDR => (VectorDR - VectorSource).normalized;
	public Vector3 ToPointDL => (VectorDL - VectorSource).normalized;
	public Vector3 ToPointUL => (VectorUL - VectorSource).normalized;

	public float AngleUp => Mathf.Acos(Vector3.Dot(ToPointUR, ToPointUL)) * Mathf.Rad2Deg;
	public float AngleDown => Mathf.Acos(Vector3.Dot(ToPointDR, ToPointDL)) * Mathf.Rad2Deg;
	public float AngleLeft => Mathf.Acos(Vector3.Dot(ToPointUL, ToPointDL)) * Mathf.Rad2Deg;
	public float AngleRight => Mathf.Acos(Vector3.Dot(ToPointUR, ToPointDR)) * Mathf.Rad2Deg;

	public float SizeUp => (VectorUR - VectorUL).magnitude;
	public float SizeDown => (VectorDR - VectorDL).magnitude;
	public float SizeCenterH => ((VectorUR + VectorDR - VectorUL - VectorDL) * .5f).magnitude;

	public float SizeLeft => (VectorUL - VectorDL).magnitude;
	public float SizeRight => (VectorUR - VectorDR).magnitude;
	public float SizeCenterV => ((VectorUL + VectorUR - VectorDL - VectorDR) * .5f).magnitude;

	public Frustum Inflate(float offset)
	{
		var normal = PlaneScreen.normal;

		Vector3 Offset(Vector3 current, Vector3 previous, Vector3 next)
		{
			var planePrev = new Plane(previous, current, current + normal);
			var planeNext = new Plane(current, next, next + normal);
			var dirPrev = (current - previous).normalized;
			var dirNext = (current - next).normalized;
			var compNext = dirPrev * (offset / Vector3.Dot(planeNext.normal, dirPrev));
			var compPrev = dirNext * (offset / Vector3.Dot(planePrev.normal, dirNext));
			return current + compPrev + compNext;
		}

		var vur = Offset(VectorUR, VectorUL, VectorDR);
		var vdr = Offset(VectorDR, VectorUR, VectorDL);
		var vdl = Offset(VectorDL, VectorDR, VectorUL);
		var vul = Offset(VectorUL, VectorDL, VectorUR);

		return new Frustum
		{
			VectorSource = VectorSource,
			VectorForward = VectorForward,

			VectorUR = vur,
			VectorDR = vdr,
			VectorDL = vdl,
			VectorUL = vul,

			PlaneScreen = PlaneScreen,

			PlaneLeft = new Plane(vdl, vul, vul + VectorForward),
			PlaneRight = new Plane(vur, vdr, vur + VectorForward),
			PlaneUp = new Plane(vul, vur, vur + VectorForward),
			PlaneDown = new Plane(vdr, vdl, vdr + VectorForward),
		};
	}

	public static Frustum Create(Camera source, Plane on)
	{
		var ray1 = source.ViewportPointToRay(new Vector3(1f, 1f));
		if(!on.Raycast(ray1, out var dist1))
		{
			return new Frustum();
		}

		var ray2 = source.ViewportPointToRay(new Vector3(1f, 0f));
		if(!on.Raycast(ray2, out var dist2))
		{
			return new Frustum();
		}

		var ray3 = source.ViewportPointToRay(new Vector3(0f, 0f));
		if(!on.Raycast(ray3, out var dist3))
		{
			return new Frustum();
		}

		var ray4 = source.ViewportPointToRay(new Vector3(0f, 1f));
		if(!on.Raycast(ray4, out var dist4))
		{
			return new Frustum();
		}

		var vur = ray1.GetPoint(dist1);
		var vdr = ray2.GetPoint(dist2);
		var vdl = ray3.GetPoint(dist3);
		var vul = ray4.GetPoint(dist4);

		var camera = source.transform;
		var forward = camera.forward;

		return new Frustum
		{
			VectorSource = camera.position,
			VectorForward = forward,

			VectorUR = vur,
			VectorDR = vdr,
			VectorDL = vdl,
			VectorUL = vul,

			// TODO: assure towards camera
			PlaneScreen = on,

			PlaneLeft = new Plane(vdl, vul, vul + forward),
			PlaneRight = new Plane(vur, vdr, vur + forward),
			PlaneUp = new Plane(vul, vur, vur + forward),
			PlaneDown = new Plane(vdr, vdl, vdr + forward),
		};
	}
}
