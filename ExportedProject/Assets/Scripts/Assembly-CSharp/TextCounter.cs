using UnityEngine;
using UnityEngine.UI;

public class TextCounter : MonoBehaviour
{
	public Text text;

	private void Start()
	{
	}

	public void SetValue(string value)
	{
		text.text = value;
	}

	private void Update()
	{
	}
}
