using System;
using UnityEngine;

[Serializable]
public class HeadBob
{
	public Transform MainCamera;

	[Range(0f, 5f)]
	public float BobFrequency = 1.5f;

	[Range(0f, 1f)]
	public float BobHeight = 0.3f;

	[Range(0f, 3f)]
	public float BobSwayAngle = 0.5f;

	[Range(0f, 1f)]
	public float BobSideMovement = 0.05f;

	[Range(0f, 1f)]
	public float heightSpeedMultiplier = 0.35f;

	[Range(0f, 1f)]
	public float strideSpeedLengthen = 0.35f;

	[Range(0f, 5f)]
	public float jumpLandMove = 1f;

	[Range(0f, 20f)]
	public float jumpLandTilt = 10f;
}
