using UnityEngine;

public class AnimConfirmation : StateMachineBehaviour
{
	public string[] onEnterDelegates;

	public string[] onExitDelegates;

	private SimpleDelegation delegation;

	private SimpleDelegation GetDelegation(Animator animator)
	{
		if (!delegation)
		{
			delegation = animator.gameObject.GetComponent<SimpleDelegation>();
		}
		return delegation;
	}

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		SimpleDelegation simpleDelegation = GetDelegation(animator);
		if ((bool)simpleDelegation)
		{
			for (int i = 0; i < onEnterDelegates.Length; i++)
			{
				simpleDelegation.CallDelegate(onEnterDelegates[i]);
			}
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		SimpleDelegation simpleDelegation = GetDelegation(animator);
		if ((bool)simpleDelegation)
		{
			for (int i = 0; i < onExitDelegates.Length; i++)
			{
				simpleDelegation.CallDelegate(onExitDelegates[i]);
			}
		}
	}
}
