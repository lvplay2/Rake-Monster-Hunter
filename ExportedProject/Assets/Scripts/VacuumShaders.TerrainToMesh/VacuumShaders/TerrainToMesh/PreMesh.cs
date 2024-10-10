using UnityEngine;

namespace VacuumShaders.TerrainToMesh
{
	internal class PreMesh
	{
		public string name = string.Empty;

		public Vector3[] vertices;

		public int[] triangles;

		public Vector2[] uv;

		public Vector2[] uv2;

		public Vector3[] normals;

		public Vector4[] tangents;

		public void Clear()
		{
			vertices = null;
			triangles = null;
			uv = null;
			uv2 = null;
			normals = null;
			tangents = null;
		}
	}
}
