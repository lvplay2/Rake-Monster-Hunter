using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AgentAnimDriver : MonoBehaviour
{
	[Serializable]
	public class SpeedInfo
	{
		public float realSpd;

		public float movingBlend;
	}

	public class SpeedInfoComparer : IComparer
	{
		int IComparer.Compare(object x, object y)
		{
			SpeedInfo speedInfo = (SpeedInfo)x;
			SpeedInfo speedInfo2 = (SpeedInfo)y;
			int result = 0;
			if (speedInfo.realSpd < speedInfo2.realSpd)
			{
				result = -1;
			}
			if (speedInfo.realSpd == speedInfo2.realSpd)
			{
				result = 0;
			}
			if (speedInfo.realSpd > speedInfo2.realSpd)
			{
				result = 1;
			}
			return result;
		}
	}

	public string movingBlendParam = "AgentSpd";

	public Animator anim;

	public SpeedInfo walk;

	public SpeedInfo run;

	private NavMeshAgent agent;

	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		ApplyChanges();
	}

	public void ApplyChanges()
	{
	}

	public float ConverToMovingBlend(float agentRealSpd)
	{
		if (agentRealSpd < 0f)
		{
			return 0f;
		}
		if (agentRealSpd >= 0f && agentRealSpd <= walk.realSpd)
		{
			return Mathf.Lerp(0f, walk.movingBlend, agentRealSpd / walk.realSpd);
		}
		if (agentRealSpd > walk.realSpd && agentRealSpd <= run.realSpd)
		{
			return Mathf.Lerp(walk.movingBlend, run.movingBlend, (agentRealSpd - walk.realSpd) / (run.realSpd - walk.realSpd));
		}
		return 1f;
	}

	private void Update()
	{
		if ((bool)agent && (bool)anim)
		{
			float agentRealSpd = ((!agent.enabled) ? 0f : agent.velocity.magnitude);
			anim.SetFloat(movingBlendParam, ConverToMovingBlend(agentRealSpd));
		}
	}
}
