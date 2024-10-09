using UnityEngine;
using UnityEngine.UI;

public class CamDetectIndicator : MonoBehaviour
{
	public bool checkAll;

	public int checkNum;

	private float lastCheck;

	public float checkDelay = 1f;

	public bool isHighlighted;

	public GameObject highlightObj;

	public AudioSource beep;

	public Image highlightedImage;

	public Sprite spriteOn;

	public Sprite spriteOff;

	public bool IsCameraLookSomething(int camNum)
	{
		if (CamInstance.allCams != null && CamInstance.allCams[camNum] != null)
		{
			CamInstance camInstance = (CamInstance)CamInstance.allCams[camNum];
			return camInstance.IsCamLookSomebody();
		}
		return false;
	}

	public bool IsAnyCameraLookSomething()
	{
		foreach (CamInstance allCam in CamInstance.allCams)
		{
			if (allCam != null && allCam.IsCamLookSomebody())
			{
				return true;
			}
		}
		return false;
	}

	private void Update()
	{
		if (Time.time >= lastCheck + checkDelay)
		{
			if (checkAll)
			{
				isHighlighted = IsAnyCameraLookSomething();
			}
			else
			{
				isHighlighted = IsCameraLookSomething(checkNum);
			}
		}
		ApplyHighlight();
	}

	public void ApplyHighlight()
	{
		if ((bool)highlightObj)
		{
			JackUtils.ControllActive(highlightObj, isHighlighted);
		}
		if ((bool)beep)
		{
			JackUtils.ControllEnabled(beep, isHighlighted);
		}
		if ((bool)highlightedImage)
		{
			if (isHighlighted && highlightedImage.sprite != spriteOn)
			{
				highlightedImage.sprite = spriteOn;
			}
			else if (!isHighlighted && highlightedImage.sprite != spriteOff)
			{
				highlightedImage.sprite = spriteOff;
			}
		}
	}

	private void OnDisable()
	{
		isHighlighted = false;
		ApplyHighlight();
	}
}
