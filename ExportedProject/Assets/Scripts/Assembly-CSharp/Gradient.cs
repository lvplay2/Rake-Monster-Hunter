using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Gradient")]
public class Gradient : BaseMeshEffect
{
	public enum Type
	{
		Vertical = 0,
		Horizontal = 1
	}

	[SerializeField]
	public Type GradientType;

	[SerializeField]
	[Range(-1.5f, 1.5f)]
	public float Offset;

	[SerializeField]
	private Color32 StartColor = Color.white;

	[SerializeField]
	private Color32 EndColor = Color.black;

	public override void ModifyMesh(VertexHelper helper)
	{
		if (!IsActive() || helper.currentVertCount == 0)
		{
			return;
		}
		List<UIVertex> list = new List<UIVertex>();
		helper.GetUIVertexStream(list);
		int count = list.Count;
		switch (GradientType)
		{
		case Type.Vertical:
		{
			float num6 = list[0].position.y;
			float num7 = list[0].position.y;
			float num8 = 0f;
			for (int num9 = count - 1; num9 >= 1; num9--)
			{
				num8 = list[num9].position.y;
				if (num8 > num7)
				{
					num7 = num8;
				}
				else if (num8 < num6)
				{
					num6 = num8;
				}
			}
			float num10 = 1f / (num7 - num6);
			UIVertex vertex2 = default(UIVertex);
			for (int j = 0; j < helper.currentVertCount; j++)
			{
				helper.PopulateUIVertex(ref vertex2, j);
				vertex2.color = Color32.Lerp(EndColor, StartColor, (vertex2.position.y - num6) * num10 - Offset);
				helper.SetUIVertex(vertex2, j);
			}
			break;
		}
		case Type.Horizontal:
		{
			float num = list[0].position.x;
			float num2 = list[0].position.x;
			float num3 = 0f;
			for (int num4 = count - 1; num4 >= 1; num4--)
			{
				num3 = list[num4].position.x;
				if (num3 > num2)
				{
					num2 = num3;
				}
				else if (num3 < num)
				{
					num = num3;
				}
			}
			float num5 = 1f / (num2 - num);
			UIVertex vertex = default(UIVertex);
			for (int i = 0; i < helper.currentVertCount; i++)
			{
				helper.PopulateUIVertex(ref vertex, i);
				vertex.color = Color32.Lerp(EndColor, StartColor, (vertex.position.x - num) * num5 - Offset);
				helper.SetUIVertex(vertex, i);
			}
			break;
		}
		}
	}
}
