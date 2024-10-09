using UnityEngine;

public class MiniMapUsing : MonoBehaviour
{
	public GameObject InGameUI;

	public GameObject MapUI;

	public void OnClick()
	{
		Debug.Log("Click");
		MapUI.SetActive(true);
		InGameUI.SetActive(false);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
