using UnityEngine;

namespace taecg.tools.thirdPersonController
{
	[RequireComponent(typeof(ThirdPersonCharacter))]
	public class ThirdPersonUserControl : MonoBehaviour
	{
		private ThirdPersonCharacter m_Character;

		private Transform m_CamTrans;

		private Vector3 m_CamForward;

		private Vector3 moveVec3;

		private void Start()
		{
			if (Camera.main != null)
			{
				m_CamTrans = Camera.main.transform;
			}
			else
			{
				Debug.LogError("请设置主角的摄像机Tag为MainCamera");
			}
			m_Character = GetComponent<ThirdPersonCharacter>();
		}

		private void FixedUpdate()
		{
			float axis = Input.GetAxis("Horizontal");
			float axis2 = Input.GetAxis("Vertical");
			if (m_CamTrans != null)
			{
				m_CamForward = Vector3.Scale(m_CamTrans.forward, new Vector3(1f, 0f, 1f)).normalized;
				moveVec3 = axis2 * m_CamForward + axis * m_CamTrans.right;
			}
			m_Character.Move(moveVec3);
		}
	}
}
