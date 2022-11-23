using UnityEngine;

public static class Extensions
{
	public static void Log(this object source)
	{
		Debug.Log(ReferenceEquals(null, source) 
			? "<color=red>[null]</color>" 
			: source.ToString());
	}

	public static Frustum ToFrustum(this Camera source, Plane on)
	{
		return Frustum.Create(source, on);
	}

	public static void Draw(this Vector3 position, Quaternion rotation, Color color, float size = 1f, float duration = 0f)
	{
		const float DIM = 1.0f;
		const float AXIS_GAP = 0.7f;

		Debug.DrawLine(position + rotation * Vector3.up * size * AXIS_GAP, position + rotation * Vector3.up * size, Color.green * DIM, duration);
		Debug.DrawLine(position, position + rotation * Vector3.up * size * AXIS_GAP, color * DIM, duration);
		Debug.DrawLine(position, position - rotation * Vector3.up * size, color * DIM, duration);

		Debug.DrawLine(position + rotation * Vector3.right * size * AXIS_GAP, position + rotation * Vector3.right * size, Color.red * DIM, duration);
		Debug.DrawLine(position, position + rotation * Vector3.right * size * AXIS_GAP, color * DIM, duration);
		Debug.DrawLine(position, position - rotation * Vector3.right * size, color * DIM, duration);

		Debug.DrawLine(position + rotation * Vector3.forward * size * AXIS_GAP, position + rotation * Vector3.forward * size, Color.blue * DIM, duration);
		Debug.DrawLine(position, position + rotation * Vector3.forward * size * AXIS_GAP, color * DIM, duration);
		Debug.DrawLine(position, position - rotation * Vector3.forward * size, color * DIM, duration);
	}

	public static void Draw(this Plane source, Color color, float size = 1f)
	{
		var origin = source.normal * -source.distance;
		const float STEP = Mathf.PI / 12f;
		var unit = Quaternion.FromToRotation(Vector3.up, source.normal) * Vector3.left * size;
		for(var delta = 0f; delta < 2f * Mathf.PI; delta += Mathf.PI / 12f)
		{
			Debug.DrawLine(
				origin + Quaternion.AngleAxis(delta * Mathf.Rad2Deg, source.normal) * unit,
				origin + Quaternion.AngleAxis((delta + STEP) * Mathf.Rad2Deg, source.normal) * unit,
				color);
		}

		Debug.DrawLine(origin, origin + source.normal * size * 1.2f, Color.cyan);
		//origin.Draw(Quaternion.LookRotation(source.normal), color);
	}

	public static void Draw(this Ray source, Color color)
	{
		Debug.DrawLine(source.origin, source.origin + source.direction, color);
	}

	public static void Draw(this Frustum source, Color color, bool extended = true, float size = 1f)
	{
		Debug.DrawLine(source.VectorDL, source.VectorDR, color);
		Debug.DrawLine(source.VectorDR, source.VectorUR, color);
		Debug.DrawLine(source.VectorUR, source.VectorUL, color);
		Debug.DrawLine(source.VectorUL, source.VectorDL, color);

		if(extended)
		{
			source.PlaneDown.Draw(Color.green * .5f, size);
			source.PlaneUp.Draw(Color.green, size);
			source.PlaneLeft.Draw(Color.red * .5f, size);
			source.PlaneRight.Draw(Color.red, size);

			source.PlaneScreen.Draw(Color.grey, size);
		}
	}

	public static void Draw(this Vector3[] source, Color color)
	{
		for(var index = 0; index < source.Length; index++)
		{
			var next = (index + 1) % source.Length;
			Debug.DrawLine(source[index], source[next], color);
		}
	}
}
