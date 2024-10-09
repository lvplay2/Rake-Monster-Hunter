using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class FP_Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IEventSystemHandler
{
	public RectTransform stick;

	public float returnRate = 15f;

	public float dragRadius = 65f;

	public AlphaControll colorAlpha;

	private bool _returnHandle;

	private bool pressed;

	private bool isEnabled = true;

	private RectTransform _canvas;

	private Vector3 globalStickPos;

	private Vector2 stickOffset;

	private CanvasGroup canvasGroup;

	private GameObject thisGameObject;

	private Vector2 Coordinates
	{
		get
		{
			if (stick.anchoredPosition.magnitude < dragRadius)
			{
				return stick.anchoredPosition / dragRadius;
			}
			return stick.anchoredPosition.normalized;
		}
	}

	public event Action<FP_Joystick, Vector2> OnStartJoystickMovement;

	public event Action<FP_Joystick, Vector2> OnJoystickMovement;

	public event Action<FP_Joystick> OnEndJoystickMovement;

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
	{
		pressed = true;
		_returnHandle = false;
		stickOffset = GetJoystickOffset(eventData);
		stick.anchoredPosition = stickOffset;
		if (this.OnStartJoystickMovement != null)
		{
			this.OnStartJoystickMovement(this, Coordinates);
		}
	}

	void IDragHandler.OnDrag(PointerEventData eventData)
	{
		stickOffset = GetJoystickOffset(eventData);
		stick.anchoredPosition = stickOffset;
		if (this.OnJoystickMovement != null)
		{
			this.OnJoystickMovement(this, Coordinates);
		}
	}

	void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
	{
		pressed = false;
		_returnHandle = true;
		if (this.OnEndJoystickMovement != null)
		{
			this.OnEndJoystickMovement(this);
		}
	}

	public void Reset()
	{
		if (pressed)
		{
			pressed = false;
			_returnHandle = true;
			if (this.OnEndJoystickMovement != null)
			{
				this.OnEndJoystickMovement(this);
			}
		}
	}

	private Vector2 GetJoystickOffset(PointerEventData eventData)
	{
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_canvas, eventData.position, eventData.pressEventCamera, out globalStickPos))
		{
			stick.position = globalStickPos;
		}
		Vector2 vector = stick.anchoredPosition;
		if (vector.magnitude > dragRadius)
		{
			vector = vector.normalized * dragRadius;
			stick.anchoredPosition = vector;
		}
		return vector;
	}

	private void Start()
	{
		thisGameObject = base.gameObject;
		canvasGroup = GetComponent("CanvasGroup") as CanvasGroup;
		_returnHandle = true;
		RectTransform rectTransform = GetComponent("RectTransform") as RectTransform;
		rectTransform.pivot = Vector2.one * 0.5f;
		stick.transform.SetParent(base.transform);
		Transform parent = base.transform;
		do
		{
			if (parent.GetComponent<Canvas>() != null)
			{
				_canvas = parent.GetComponent("RectTransform") as RectTransform;
				break;
			}
			parent = parent.parent;
		}
		while (parent != null);
	}

	private void FixedUpdate()
	{
		if (_returnHandle)
		{
			if (stick.anchoredPosition.magnitude > Mathf.Epsilon)
			{
				stick.anchoredPosition -= new Vector2(stick.anchoredPosition.x * returnRate, stick.anchoredPosition.y * returnRate) * Time.deltaTime;
			}
			else
			{
				_returnHandle = false;
			}
		}
		switch (isEnabled)
		{
		case true:
		{
			canvasGroup.alpha = ((!pressed) ? colorAlpha.idleAlpha : colorAlpha.pressedAlpha);
			CanvasGroup obj2 = canvasGroup;
			bool flag = true;
			canvasGroup.blocksRaycasts = flag;
			obj2.interactable = flag;
			break;
		}
		case false:
		{
			canvasGroup.alpha = 0f;
			CanvasGroup obj = canvasGroup;
			bool flag = false;
			canvasGroup.blocksRaycasts = flag;
			obj.interactable = flag;
			break;
		}
		}
	}

	public Vector3 MoveInput()
	{
		if (base.gameObject.activeInHierarchy)
		{
			return new Vector3(Coordinates.x, 0f, Coordinates.y);
		}
		return new Vector3(0f, 0f, 0f);
	}

	public void Rotate(Transform transformToRotate, float speed)
	{
		if (Coordinates != Vector2.zero)
		{
			transformToRotate.rotation = Quaternion.Slerp(transformToRotate.rotation, Quaternion.LookRotation(new Vector3(Coordinates.x, 0f, Coordinates.y)), speed * Time.deltaTime);
		}
	}

	public bool IsPressed()
	{
		return pressed;
	}

	public void Enable(bool enable)
	{
		isEnabled = enable;
	}
}
