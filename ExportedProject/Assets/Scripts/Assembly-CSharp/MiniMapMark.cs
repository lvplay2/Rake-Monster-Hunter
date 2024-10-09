using UnityEngine;

public class MiniMapMark : MonoBehaviour
{
	private void Start()
	{
		Renderer component = GetComponent<Renderer>();
		if ((bool)component && !component.enabled)
		{
			component.enabled = true;
		}
		else
		{
			Debug.Log("no renderer finded");
		}
	}
}
