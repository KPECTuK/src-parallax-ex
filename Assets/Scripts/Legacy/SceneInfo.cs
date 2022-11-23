using UnityEngine;

/// <summary> scene manager descriptor, must at the root of each scene </summary>
public class SceneInfo : MonoBehaviour
{
	public string sceneName;
	public bool use;
	public Color ambientLight;
	public Color bgColor;

	/// <summary> a light that moves with user's head, good for specular </summary>
	public bool headLight;
}
