using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
[RequireComponent(typeof(CanvasGroup))]
public class FP_Lookpad : MonoBehaviour
{
	private Vector2 touchInput;

	private Vector2 prevDelta;

	private Vector2 dragInput;

	private bool isPressed;

	private EventTrigger eventTrigger;

	private CanvasGroup canvasGroup;

	private void Start()
	{
		SetupListeners();
		canvasGroup = GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0f;
	}

	private void Update()
	{
		touchInput = (dragInput - prevDelta) / Time.deltaTime;
		prevDelta = dragInput;
	}

	private void SetupListeners()
	{
		eventTrigger = base.gameObject.GetComponent<EventTrigger>();
		EventTrigger.TriggerEvent triggerEvent = new EventTrigger.TriggerEvent();
		triggerEvent.AddListener(delegate(BaseEventData data)
		{
			PointerEventData pointerEventData2 = (PointerEventData)data;
			data.Use();
			isPressed = true;
			prevDelta = (dragInput = pointerEventData2.position);
		});
		eventTrigger.triggers.Add(new EventTrigger.Entry
		{
			callback = triggerEvent,
			eventID = EventTriggerType.PointerDown
		});
		EventTrigger.TriggerEvent triggerEvent2 = new EventTrigger.TriggerEvent();
		triggerEvent2.AddListener(delegate(BaseEventData data)
		{
			PointerEventData pointerEventData = (PointerEventData)data;
			data.Use();
			dragInput = pointerEventData.position;
		});
		eventTrigger.triggers.Add(new EventTrigger.Entry
		{
			callback = triggerEvent2,
			eventID = EventTriggerType.Drag
		});
		EventTrigger.TriggerEvent triggerEvent3 = new EventTrigger.TriggerEvent();
		triggerEvent3.AddListener(delegate
		{
			touchInput = Vector2.zero;
			isPressed = false;
		});
		eventTrigger.triggers.Add(new EventTrigger.Entry
		{
			callback = triggerEvent3,
			eventID = EventTriggerType.EndDrag
		});
	}

	public void Reset()
	{
		if (isPressed)
		{
			touchInput = Vector2.zero;
			isPressed = false;
		}
	}

	public Vector2 LookInput()
	{
		return touchInput * Time.deltaTime;
	}

	public bool IsPressed()
	{
		return isPressed;
	}
}
