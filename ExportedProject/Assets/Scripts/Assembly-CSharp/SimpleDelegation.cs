using System.Collections.Generic;
using UnityEngine;

public class SimpleDelegation : MonoBehaviour
{
	public delegate void SimpleFunc();

	private SortedDictionary<string, SimpleFunc> delegates = new SortedDictionary<string, SimpleFunc>();

	public void Add(string name, SimpleFunc func)
	{
		delegates.Add(name, func);
	}

	public void CallDelegate(string name)
	{
		if (delegates.ContainsKey(name))
		{
			delegates[name]();
		}
		else
		{
			Debug.Log("Cant find delegate with Key " + name);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
