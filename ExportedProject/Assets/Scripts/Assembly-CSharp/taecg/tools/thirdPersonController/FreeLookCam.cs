using UnityEngine;

namespace taecg.tools.thirdPersonController
{
	public class FreeLookCam : MonoBehaviour
	{
		[Header("跟随的目标")]
		public Transform Target;

		[Header("摄像机跟随目标的速度")]
		public float FollowSpeed = 5f;

		[Range(0f, 10f)]
		[SerializeField]
		private float m_TurnSpeed = 1.5f;

		[SerializeField]
		private float m_TurnSmoothing;

		[Header("最大俯视角度")]
		public float TiltMax = 80f;

		[Header("最大仰视角度")]
		public float TiltMin = 45f;

		private Transform m_Cam;

		private Transform m_Pivot;

		private float m_LookAngle;

		private float m_TiltAngle;

		private const float k_LookDistance = 100f;

		private Vector3 m_PivotEulers;

		private Quaternion m_PivotTargetRot;

		private Quaternion m_TransformTargetRot;

		private void Awake()
		{
			m_Cam = GetComponentInChildren<Camera>().transform;
			m_Pivot = m_Cam.parent;
			m_PivotEulers = m_Pivot.rotation.eulerAngles;
			m_PivotTargetRot = m_Pivot.transform.localRotation;
			m_TransformTargetRot = base.transform.localRotation;
		}

		protected void Update()
		{
			HandleRotationMovement();
			FollowTarget(Time.deltaTime);
		}

		private void FollowTarget(float deltaTime)
		{
			if (Target == null)
			{
				Debug.LogWarning("跟随目标为空！");
			}
			base.transform.position = Vector3.Lerp(base.transform.position, Target.position, deltaTime * FollowSpeed);
		}

		private void HandleRotationMovement()
		{
			if (!(Time.timeScale < float.Epsilon))
			{
				float axis = Input.GetAxis("Mouse X");
				float axis2 = Input.GetAxis("Mouse Y");
				m_LookAngle += axis * m_TurnSpeed;
				m_TransformTargetRot = Quaternion.Euler(0f, m_LookAngle, 0f);
				m_TiltAngle -= axis2 * m_TurnSpeed;
				m_TiltAngle = Mathf.Clamp(m_TiltAngle, 0f - TiltMin, TiltMax);
				m_PivotTargetRot = Quaternion.Euler(m_TiltAngle, m_PivotEulers.y, m_PivotEulers.z);
				if (m_TurnSmoothing > 0f)
				{
					m_Pivot.localRotation = Quaternion.Slerp(m_Pivot.localRotation, m_PivotTargetRot, m_TurnSmoothing * Time.deltaTime);
					base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, m_TransformTargetRot, m_TurnSmoothing * Time.deltaTime);
				}
				else
				{
					m_Pivot.localRotation = m_PivotTargetRot;
					base.transform.localRotation = m_TransformTargetRot;
				}
			}
		}
	}
}
