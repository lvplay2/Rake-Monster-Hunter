using UnityEngine;

public class FP_Input : MonoBehaviour
{
	public bool UseMobileInput = true;

	public Inputs mobileInputs;

	public Vector3 MoveInput()
	{
		return mobileInputs.moveJoystick.MoveInput();
	}

	public Vector2 LookInput()
	{
		return (!(mobileInputs.lookPad != null)) ? Vector2.zero : mobileInputs.lookPad.LookInput();
	}

	public Vector2 ShotInput()
	{
		return (!(mobileInputs.shotButton != null)) ? Vector2.zero : mobileInputs.shotButton.MoveInput();
	}

	public bool Shoot()
	{
		return mobileInputs.shotButton != null && mobileInputs.shotButton.IsPressed();
	}

	public bool Reload()
	{
		return mobileInputs.reloadButton != null && mobileInputs.reloadButton.OnRelease();
	}

	public bool Run()
	{
		return mobileInputs.runButton != null && mobileInputs.runButton.IsPressed();
	}

	public bool Jump()
	{
		return mobileInputs.jumpButton != null && mobileInputs.jumpButton.IsPressed();
	}

	public bool Crouch()
	{
		return mobileInputs.crouchButton != null && mobileInputs.crouchButton.Toggle();
	}
}
