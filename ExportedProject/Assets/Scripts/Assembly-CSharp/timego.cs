using UnityEngine;
using UnityEngine.UI;

public class timego : MonoBehaviour
{
	public Text text;

	private int i;

	private void Start()
	{
		InvokeRepeating("Tick", 0f, 1f);
	}

	private void Tick()
	{
		text.text = "time: " + i;
		i++;
	}
}
