using UnityEngine;
using UnityEngine.UI;

public class CamView : MonoBehaviour
{
	public GameObject inGameUi;

	public Camera playerCam;

	public GameObject[] camButtons;

	public GameObject blackScreen;

	public Text blackScreenText;

	public FP_Lookpad lookPad;

	public float sens = 1f;

	private Transform camTransform;

	private Transform nextTransform;

	private Camera currCam;

	private Camera nextCam;

	private bool isInited;

	private GameObject thisGameObject;

	private GameObject lastUsedCam;

	public Color bthToggled = Color.white;

	public Color btnUnToggled = new Color(1f, 1f, 1f, 0.5f);

	public void RefreshButtons(int curr)
	{
		for (int i = 0; i < camButtons.Length; i++)
		{
			GameObject gameObject = camButtons[i];
			if (i < CamInstance.allCams.Count)
			{
				gameObject.SetActive(true);
				Image component = gameObject.GetComponent<Image>();
				Button component2 = gameObject.GetComponent<Button>();
				CamDetectIndicator component3 = gameObject.GetComponent<CamDetectIndicator>();
				if (curr == i)
				{
					component.color = bthToggled;
					component2.interactable = false;
					if ((bool)component3)
					{
						JackUtils.ControllEnabled(component3, false);
					}
					continue;
				}
				component.color = btnUnToggled;
				component2.interactable = true;
				if ((bool)component3)
				{
					component3.checkNum = i;
					JackUtils.ControllEnabled(component3, true);
				}
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
	}

	public void SwitchCamToNum(int num)
	{
		RefreshButtons(num);
		CancelInvoke();
		if (num >= 0 && num < CamInstance.allCams.Count)
		{
			CamInstance camInstance = (CamInstance)CamInstance.allCams[num];
			if ((bool)camInstance && (bool)camInstance.cam)
			{
				SwitchCamTo(camInstance.cam, camInstance.camTransform);
				return;
			}
			blackScreen.SetActive(true);
			blackScreenText.text = "NO SIGNAL";
		}
		else
		{
			blackScreen.SetActive(true);
			blackScreenText.text = "NO SIGNAL";
		}
	}

	private void SwitchCamTo(Camera nextCam_, Transform camTransform_)
	{
		currCam.enabled = false;
		nextCam = nextCam_;
		nextTransform = camTransform_;
		blackScreen.SetActive(true);
		blackScreenText.text = "OPENING";
		camTransform = null;
		CancelInvoke();
		Invoke("EnableNextCam", 0.5f);
	}

	private void EnableNextCam()
	{
		blackScreen.SetActive(false);
		nextCam.enabled = true;
		currCam = nextCam;
		camTransform = nextTransform;
	}

	private void EnableNextCamAndExit()
	{
		EnableNextCam();
		thisGameObject.SetActive(false);
		inGameUi.SetActive(true);
	}

	public void OnExitClick()
	{
		Debug.Log("OnExitClick");
		currCam.enabled = false;
		nextCam = playerCam;
		camTransform = null;
		blackScreen.SetActive(true);
		blackScreenText.text = "LOG OUT";
		CancelInvoke();
		Invoke("EnableNextCamAndExit", 0.5f);
	}

	public int GetTriggredCamNum()
	{
		for (int i = 0; i < camButtons.Length; i++)
		{
			GameObject gameObject = camButtons[i];
			if (gameObject.GetComponent<CamDetectIndicator>().isHighlighted)
			{
				return i;
			}
		}
		return 0;
	}

	public void OnEnterClick()
	{
		Debug.Log("OnEnterClick");
		Init();
		thisGameObject.SetActive(true);
		inGameUi.SetActive(false);
		camTransform = null;
		int triggredCamNum = GetTriggredCamNum();
		SwitchCamToNum(triggredCamNum);
	}

	protected virtual void Init()
	{
		if (!isInited)
		{
			currCam = playerCam;
			thisGameObject = base.gameObject;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if ((bool)lookPad && (bool)camTransform)
		{
			float x = lookPad.LookInput().x;
			x *= currCam.fieldOfView * 2f / 800f * sens;
			camTransform.Rotate(Vector3.forward, x);
		}
	}
}
