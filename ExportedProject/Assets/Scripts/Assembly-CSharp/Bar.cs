using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
	public RectTransform rtFull;

	public RectTransform rtFilled;

	public bool invisibleWhenFull;

	protected Image imgFull;

	protected Image imgFilled;

	private void Start()
	{
	}

	private void SetImgData()
	{
		if (!imgFull || !imgFilled)
		{
			imgFull = rtFull.GetComponent<Image>();
			imgFilled = rtFilled.GetComponent<Image>();
		}
	}

	public void SetValue(float value)
	{
		if (value == 1f && invisibleWhenFull)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			base.gameObject.SetActive(true);
		}
		value *= rtFull.rect.width;
		rtFilled.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
	}

	private void SetBarAlpha(float a)
	{
		SetImgData();
		imgFull.color = new Color(imgFull.color.r, imgFull.color.g, imgFull.color.b, a);
		imgFilled.color = new Color(imgFilled.color.r, imgFilled.color.g, imgFilled.color.b, a);
	}

	private void Update()
	{
	}
}
