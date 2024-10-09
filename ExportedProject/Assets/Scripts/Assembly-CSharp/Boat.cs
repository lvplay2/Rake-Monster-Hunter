using UnityEngine;

public class Boat : MonoBehaviour
{
	private float orgY;

	private float waveSpeed = 1f;

	private float moveDis = 0.2f;

	private void Start()
	{
		orgY = base.transform.position.y;
	}

	private void Update()
	{
		base.transform.position = new Vector3(base.transform.position.x, orgY + moveDis * Mathf.Sin(Time.time * waveSpeed), base.transform.position.z);
		base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, 3f * Mathf.Sin(Time.time * waveSpeed + 1.5f));
	}
}
