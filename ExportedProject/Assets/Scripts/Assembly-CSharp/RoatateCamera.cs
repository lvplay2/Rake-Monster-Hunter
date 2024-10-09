using UnityEngine;

public class RoatateCamera : MonoBehaviour
{
	public float senstivity = 5f;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.eulerAngles += new Vector3((0f - Input.GetAxis("Mouse Y")) * senstivity, Input.GetAxis("Mouse X") * senstivity, 0f);
	}
}
