using UnityEngine;

public class TempCameraMove : MonoBehaviour
{
	public float speed = 200f;

	public float rotateSpeed = 40f;

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKey("w"))
		{
			base.transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
		}
		if (Input.GetKey("s"))
		{
			base.transform.Translate(Vector3.back * speed * Time.deltaTime, Space.Self);
		}
		if (Input.GetKey("a"))
		{
			base.transform.Translate(Vector3.left * speed * Time.deltaTime, Space.Self);
		}
		if (Input.GetKey("d"))
		{
			base.transform.Translate(Vector3.right * speed * Time.deltaTime, Space.Self);
		}
		if (Input.GetKey(KeyCode.UpArrow))
		{
			base.transform.Rotate(Vector3.left * rotateSpeed * Time.deltaTime, Space.Self);
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			base.transform.Rotate(Vector3.right * rotateSpeed * Time.deltaTime, Space.Self);
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			base.transform.Rotate(Vector3.down * rotateSpeed * Time.deltaTime, Space.World);
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			base.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
		}
	}
}
