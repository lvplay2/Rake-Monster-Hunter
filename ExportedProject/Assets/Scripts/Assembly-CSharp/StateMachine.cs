using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
	public delegate bool CheckLinkFunc();

	public delegate void OnEvent();

	public class Link
	{
		public string id;

		public CheckLinkFunc verifyDelegate;

		public Link(string id_, CheckLinkFunc f)
		{
			id = id_;
			verifyDelegate = f;
		}
	}

	public class State
	{
		public string id = string.Empty;

		private ArrayList links = new ArrayList();

		public OnEvent OnEnter;

		public OnEvent Update;

		public OnEvent OnExit;

		public State(string id_)
		{
			id = id_;
		}

		public virtual bool VerifyLinksConditions(out string id)
		{
			id = string.Empty;
			foreach (Link link in links)
			{
				if (link.verifyDelegate != null && link.verifyDelegate())
				{
					id = link.id;
					return true;
				}
			}
			return false;
		}

		public virtual void AddLink(Link link)
		{
			links.Add(link);
		}

		public virtual void AddLink(string id, CheckLinkFunc func)
		{
			AddLink(new Link(id, func));
		}
	}

	public string logName = string.Empty;

	public bool isLoggingEnabled;

	private State current;

	private SortedDictionary<string, State> states = new SortedDictionary<string, State>();

	public void AddState(State state)
	{
		states.Add(state.id, state);
	}

	public void SwitchStateTo(State next)
	{
		string text = ((current == null) ? "null" : current.id);
		string text2 = ((next == null) ? "null" : next.id);
		if (isLoggingEnabled)
		{
			Debug.Log(logName + " switch state '" + text + "' -- '" + text2 + "'  time:" + Time.time);
		}
		if (current != null && current.OnExit != null)
		{
			current.OnExit();
		}
		if (next != null && next.OnEnter != null)
		{
			next.OnEnter();
		}
		current = next;
	}

	public State GetStateById(string id)
	{
		if (states.ContainsKey(id))
		{
			return states[id];
		}
		Debug.Log(logName + " SateMahine cant find state with id '" + id + "'");
		return null;
	}

	public State GetCurrState()
	{
		return current;
	}

	public void Update()
	{
		if (current == null)
		{
			return;
		}
		if (current.Update != null)
		{
			current.Update();
		}
		string id;
		if (current.VerifyLinksConditions(out id))
		{
			State stateById = GetStateById(id);
			if (stateById != null)
			{
				SwitchStateTo(stateById);
			}
		}
	}
}
