using UnityEngine;
using UnityEngine.UI;

public class ShowFPS : MonoBehaviour
{
	public Text text;

	private int frames;

	private float lastSecBegin;

	private void Start()
	{
		lastSecBegin = Time.time;
	}

	private void Show()
	{
		text.text = "fps: " + frames;
	}

	private void Update()
	{
		if (lastSecBegin + 1f <= Time.time)
		{
			Show();
			frames = 0;
			lastSecBegin = Time.time;
		}
		else
		{
			frames++;
		}
	}
}
