using System;
using UnityEngine;

[RequireComponent(typeof(FP_Controller))]
[RequireComponent(typeof(FP_FootSteps))]
[RequireComponent(typeof(FP_Input))]
public class FP_CameraLook : MonoBehaviour
{
	public Transform PlayerHead;

	public float LookSensitivity = 2f;

	public float ShootSensitivity = 1f;

	[Range(-35f, -90f)]
	public float minimumY = -60f;

	[Range(35f, 90f)]
	public float maximumY = 60f;

	public float Smooth = 25f;

	private Vector2 lookAt;

	private float sensitivity;

	[HideInInspector]
	public float rotationY;

	public HeadBob headBob;

	private Vector3 originalLocalPos;

	private Quaternion originalLocalRot;

	private float nextStepTime = 0.5f;

	private float headBobCycle;

	private float headBobFade;

	private float springPos;

	private float springVelocity;

	private float springElastic = 1.1f;

	private float springDampen = 0.8f;

	private float springVelocityThreshold = 0.05f;

	private float springPositionThreshold = 0.05f;

	private Vector3 prevPosition;

	private Vector3 prevVelocity = Vector3.zero;

	private Vector3 velocity;

	private Vector3 velocityChange;

	private bool prevGrounded = true;

	private float flatVelocity;

	private float strideLengthen;

	private float bobFactor;

	private float bobSwayFactor;

	private float speedHeightFactor;

	private float xPos;

	private float yPos;

	private float xTilt;

	private float zTilt;

	private float stepVolume;

	private float InputX;

	private float InputY;

	private AudioSource audioSource;

	private FP_Input playerInput;

	private FP_Controller playerController;

	private FP_FootSteps footSteps;

	private void Awake()
	{
		headBob.MainCamera.tag = "MainCamera";
	}

	private void Start()
	{
		playerController = GetComponent<FP_Controller>();
		footSteps = GetComponent<FP_FootSteps>();
		playerInput = GetComponent<FP_Input>();
		originalLocalPos = headBob.MainCamera.localPosition;
		originalLocalRot = headBob.MainCamera.localRotation;
		if (GetComponent<AudioSource>() == null)
		{
			base.gameObject.AddComponent<AudioSource>();
		}
		prevPosition = base.transform.position;
		audioSource = GetComponent<AudioSource>();
	}

	private void Update()
	{
		if (playerController.canControl)
		{
			switch (playerInput.UseMobileInput)
			{
			case true:
				InputX = playerInput.LookInput().x + playerInput.ShotInput().x;
				InputY = playerInput.LookInput().y + playerInput.ShotInput().y;
				break;
			case false:
				InputX = Input.GetAxis("Mouse X") * 10f;
				InputY = Input.GetAxis("Mouse Y") * 10f;
				break;
			}
			sensitivity = ((!playerInput.Shoot()) ? LookSensitivity : ShootSensitivity);
			PlayerHead.localPosition = Vector3.Lerp(PlayerHead.localPosition, new Vector3(PlayerHead.localPosition.x, playerController.controller.center.y + playerController.controller.height / 2f - 0.25f, PlayerHead.localPosition.z), 0.45000002f);
			lookAt.x = Mathf.Lerp(lookAt.x, InputX, Smooth * Time.deltaTime);
			lookAt.y = Mathf.Lerp(lookAt.y, InputY, Smooth * Time.deltaTime);
			base.transform.Rotate(0f, lookAt.x * (sensitivity / 10f), 0f);
			rotationY += lookAt.y * (sensitivity / 10f);
			rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
			PlayerHead.localEulerAngles = new Vector3(0f - rotationY, PlayerHead.localEulerAngles.y, 0f);
		}
	}

	private void FixedUpdate()
	{
		velocity = (base.transform.position - prevPosition) / Time.deltaTime;
		velocityChange = velocity - prevVelocity;
		prevPosition = base.transform.position;
		prevVelocity = velocity;
		springVelocity -= velocityChange.y;
		springVelocity -= springPos * springElastic;
		springVelocity *= springDampen;
		springPos += springVelocity * Time.deltaTime;
		springPos = Mathf.Clamp(springPos, -0.3f, 0.3f);
		if (Mathf.Abs(springVelocity) < springVelocityThreshold && Mathf.Abs(springPos) < springPositionThreshold)
		{
			springVelocity = 0f;
			springPos = 0f;
		}
		flatVelocity = new Vector3(velocity.x, 0f, velocity.z).magnitude;
		strideLengthen = 1f + flatVelocity * headBob.strideSpeedLengthen;
		headBobCycle += flatVelocity / strideLengthen * (Time.deltaTime / headBob.BobFrequency);
		bobFactor = Mathf.Sin(headBobCycle * (float)Math.PI * 2f);
		bobSwayFactor = Mathf.Sin(headBobCycle * (float)Math.PI * 2f + (float)Math.PI / 2f);
		bobFactor = 1f - (bobFactor * 0.5f + 1f);
		bobFactor *= bobFactor;
		if (new Vector3(velocity.x, 0f, velocity.z).magnitude < 0.1f)
		{
			headBobFade = Mathf.Lerp(headBobFade, 0f, Time.deltaTime);
		}
		else
		{
			headBobFade = Mathf.Lerp(headBobFade, 1f, Time.deltaTime);
		}
		speedHeightFactor = 1f + flatVelocity * headBob.heightSpeedMultiplier;
		xPos = (0f - headBob.BobSideMovement) * bobSwayFactor;
		yPos = springPos * headBob.jumpLandMove + bobFactor * headBob.BobHeight * headBobFade * speedHeightFactor;
		xTilt = (0f - springPos) * headBob.jumpLandTilt;
		zTilt = bobSwayFactor * headBob.BobSwayAngle * headBobFade;
		headBob.MainCamera.localPosition = originalLocalPos + new Vector3(xPos, yPos, 0f);
		headBob.MainCamera.localRotation = originalLocalRot * Quaternion.Euler(xTilt, 0f, zTilt);
		if (playerController.IsGrounded())
		{
			headBob.MainCamera.localRotation = originalLocalRot * Quaternion.Euler(xTilt, 0f, zTilt);
			if (!prevGrounded)
			{
				if (headBobCycle > nextStepTime)
				{
					nextStepTime = headBobCycle + 0.5f;
					footSteps.ResetFootstepSounds(audioSource);
				}
			}
			else if (headBobCycle > nextStepTime)
			{
				nextStepTime = headBobCycle + 0.5f;
				footSteps.PlayFootstepSounds(audioSource);
			}
			prevGrounded = true;
		}
		else
		{
			prevGrounded = false;
		}
	}
}
