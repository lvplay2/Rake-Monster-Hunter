using UnityEngine;
using UnityEngine.UI;

public class FlashLightUsing : MonoBehaviour
{
	public GameObject lightObj;

	public Image btnImg;

	public Color colOn = new Color(1f, 1f, 1f, 1f);

	public Color colOff = new Color(1f, 1f, 1f, 0.5f);

	private void SetStatusToButton()
	{
		if (lightObj.activeSelf)
		{
			btnImg.color = colOn;
		}
		else
		{
			btnImg.color = colOff;
		}
	}

	private void Start()
	{
		SetStatusToButton();
	}

	public void OnClick()
	{
		if ((bool)lightObj)
		{
			lightObj.SetActive(!lightObj.activeSelf);
			SetStatusToButton();
		}
	}
}
