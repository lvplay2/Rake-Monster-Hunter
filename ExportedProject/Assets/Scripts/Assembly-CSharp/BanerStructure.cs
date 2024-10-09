using UnityEngine;

[CreateAssetMenu(menuName = "Baner/Baner Structure")]
public class BanerStructure : ScriptableObject
{
	public string Identifier;

	public RuntimePlatform platform = RuntimePlatform.Android;

	public Sprite[] sprites;
}
