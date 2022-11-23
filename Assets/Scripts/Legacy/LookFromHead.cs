using UnityEngine;

public class LookFromHead : MonoBehaviour
{
	public Transform head;

	private void Update()
	{
		gameObject.transform.rotation = Quaternion.LookRotation(-head.position);
	}
}
