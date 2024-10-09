using UnityEngine;

public class EnvirometSettings : MonoBehaviour
{
	[HideInInspector]
	public static EnvirometSettings sets;

	public bool animateFog = true;

	public float fogStartDelta = 10f;

	public float fogEndtDelta = 10f;

	public float fogNormalEnd = 45f;

	public float fogNormalStart = 45f;

	public float fogFarEnd = 80f;

	public float fogFarStart = 70f;

	public float fogDestStart;

	public float fogDestEnd;

	public int rockets;

	private void Start()
	{
		if (sets == null)
		{
			sets = this;
		}
	}

	public void SetFogIsGone()
	{
		animateFog = true;
		fogDestStart = fogFarStart;
		fogDestEnd = fogFarEnd;
	}

	public void SetFogIsCome()
	{
		animateFog = true;
		fogDestStart = fogNormalStart;
		fogDestEnd = fogNormalEnd;
	}

	public void ConfirmRocketFire(bool isOn)
	{
		if (isOn)
		{
			rockets++;
		}
		else
		{
			rockets--;
		}
		if (rockets <= 0)
		{
			SetFogIsCome();
		}
		else
		{
			SetFogIsGone();
		}
	}

	private void Update()
	{
		if (animateFog)
		{
			RenderSettings.fogStartDistance = Mathf.MoveTowards(RenderSettings.fogStartDistance, fogDestStart, fogStartDelta * Time.deltaTime);
			RenderSettings.fogEndDistance = Mathf.MoveTowards(RenderSettings.fogEndDistance, fogDestEnd, fogEndtDelta * Time.deltaTime);
			if (RenderSettings.fogStartDistance == fogDestStart && RenderSettings.fogEndDistance == fogDestEnd)
			{
				animateFog = false;
			}
		}
	}
}
