using UnityEngine;

namespace VacuumShaders.TerrainToMesh
{
	public class CompressedMeshLoader : MonoBehaviour
	{
		private enum SIDE
		{
			Left = 0,
			Right = 1,
			Top = 2,
			Bottom = 3
		}

		[HideInInspector]
		public int chunkCountHorizontal;

		[HideInInspector]
		public int chunkCountVertical;

		[HideInInspector]
		public int vertexCountHorizontal;

		[HideInInspector]
		public int vertexCountVertical;

		[HideInInspector]
		public bool usingPerChunkBasemap;

		[HideInInspector]
		public bool usingSkirt;

		private void Start()
		{
			MeshFilter[] componentsInChildren = base.transform.GetComponentsInChildren<MeshFilter>();
			int num = 0;
			if (componentsInChildren == null || componentsInChildren.Length < 1)
			{
				Debug.LogError("CompressedMeshLoader: '" + base.transform.name + "' has no compressed mesh data to load\n", this);
				base.enabled = false;
				return;
			}
			if (componentsInChildren[0] == null || componentsInChildren[0].sharedMesh == null || componentsInChildren[0].sharedMesh.vertexCount < 4)
			{
				Debug.LogError("CompressedMeshLoader: '" + base.transform.name + "' failed to load some of compressed meshes\n", this);
				base.enabled = false;
				return;
			}
			num = componentsInChildren[0].sharedMesh.vertexCount;
			for (int i = 1; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i] == null || componentsInChildren[i].sharedMesh == null || componentsInChildren[i].sharedMesh.vertexCount != num)
				{
					Debug.LogError("CompressedMeshLoader: '" + base.transform.name + "' failed to load some of compressed meshes\n", this);
					base.enabled = false;
					return;
				}
			}
			if (chunkCountHorizontal < 1)
			{
				chunkCountHorizontal = 1;
			}
			if (chunkCountVertical < 1)
			{
				chunkCountVertical = 1;
			}
			if (base.transform.childCount != chunkCountHorizontal * chunkCountVertical)
			{
				Debug.LogError("CompressedMeshLoader: Total chunk count is not correct or gameobject is marked as Static\n", this);
				base.enabled = false;
				return;
			}
			if (vertexCountHorizontal < 2)
			{
				vertexCountHorizontal = 2;
			}
			if (vertexCountVertical < 2)
			{
				vertexCountVertical = 2;
			}
			if (usingSkirt)
			{
				num -= 2 * (vertexCountHorizontal + vertexCountVertical);
			}
			if (num != vertexCountHorizontal * vertexCountVertical)
			{
				Debug.LogError("CompressedMeshLoader: Total vertex count is not correct or gameobject is marked as Static\n", this);
				base.enabled = false;
			}
			else
			{
				Fix(componentsInChildren);
			}
		}

		private void Fix(MeshFilter[] _childMeshFilter)
		{
			int num = 0;
			for (int i = 0; i < chunkCountHorizontal; i++)
			{
				for (int j = 0; j < chunkCountVertical - 1; j++)
				{
					Combine(_childMeshFilter[num + j].sharedMesh, _childMeshFilter[num + j + 1].sharedMesh, true);
				}
				num += chunkCountVertical;
			}
			num = 0;
			for (int k = 0; k < chunkCountHorizontal - 1; k++)
			{
				for (int l = 0; l < chunkCountVertical; l++)
				{
					Combine(_childMeshFilter[num + l].sharedMesh, _childMeshFilter[num + l + chunkCountVertical].sharedMesh, false);
				}
				num += chunkCountVertical;
			}
		}

		private void GetMeshData(Mesh _mesh, SIDE _side, out Vector3[] vertices, out Vector2[] uv, out Vector2[] uv2, out Vector3[] normal, out Vector4[] tangent)
		{
			int[] indexies = GetIndexies(_side);
			Vector3[] vertices2 = _mesh.vertices;
			Vector2[] uv3 = _mesh.uv;
			Vector2[] uv4 = _mesh.uv2;
			Vector3[] normals = _mesh.normals;
			Vector4[] tangents = _mesh.tangents;
			bool flag = ((_mesh.uv != null && _mesh.uv.Length == _mesh.vertices.Length) ? true : false);
			bool flag2 = ((_mesh.uv2 != null && _mesh.uv2.Length == _mesh.vertices.Length) ? true : false);
			bool flag3 = ((_mesh.normals != null && _mesh.normals.Length == _mesh.vertices.Length) ? true : false);
			bool flag4 = ((_mesh.tangents != null && _mesh.tangents.Length == _mesh.vertices.Length) ? true : false);
			vertices = new Vector3[indexies.Length];
			uv = (flag ? new Vector2[indexies.Length] : null);
			uv2 = (flag2 ? new Vector2[indexies.Length] : null);
			normal = (flag3 ? new Vector3[indexies.Length] : null);
			tangent = (flag4 ? new Vector4[indexies.Length] : null);
			for (int i = 0; i < indexies.Length; i++)
			{
				int num = indexies[i];
				vertices[i] = vertices2[num];
				if (flag)
				{
					uv[i] = uv3[num];
				}
				if (flag2)
				{
					uv2[i] = uv4[num];
				}
				if (flag3)
				{
					normal[i] = normals[num];
				}
				if (flag4)
				{
					tangent[i] = tangents[num];
				}
			}
		}

		private int[] GetIndexies(SIDE _side)
		{
			int[] array = new int[(_side == SIDE.Left || _side == SIDE.Right) ? vertexCountVertical : vertexCountHorizontal];
			switch (_side)
			{
			case SIDE.Bottom:
			{
				for (int j = 0; j < vertexCountHorizontal; j++)
				{
					array[j] = j;
				}
				break;
			}
			case SIDE.Top:
			{
				int num2 = vertexCountHorizontal * (vertexCountVertical - 1);
				for (int l = 0; l < vertexCountHorizontal; l++)
				{
					array[l] = num2 + l;
				}
				break;
			}
			case SIDE.Left:
			{
				int num3 = 0;
				for (int m = 0; m < vertexCountVertical; m++)
				{
					array[m] = num3;
					num3 += vertexCountHorizontal;
				}
				break;
			}
			case SIDE.Right:
			{
				int num = vertexCountHorizontal - 1;
				for (int k = 0; k < vertexCountVertical; k++)
				{
					array[k] = num;
					num += vertexCountHorizontal;
				}
				break;
			}
			default:
			{
				for (int i = 0; i < vertexCountHorizontal; i++)
				{
					array[i] = 0;
				}
				break;
			}
			}
			return array;
		}

		private void Combine(Mesh _meshBottom, Mesh _meshTop, bool bottomTop)
		{
			Vector3[] vertices;
			Vector2[] uv;
			Vector2[] uv2;
			Vector3[] normal;
			Vector4[] tangent;
			GetMeshData(_meshBottom, (!bottomTop) ? SIDE.Right : SIDE.Top, out vertices, out uv, out uv2, out normal, out tangent);
			int[] indexies = GetIndexies(bottomTop ? SIDE.Bottom : SIDE.Left);
			Vector3[] vertices2 = _meshTop.vertices;
			Vector2[] uv3 = _meshTop.uv;
			Vector2[] uv4 = _meshTop.uv2;
			Vector3[] normals = _meshTop.normals;
			Vector4[] tangents = _meshTop.tangents;
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices2[indexies[i]] = vertices[i];
				if (uv != null)
				{
					uv3[indexies[i]] = uv[i];
				}
				if (uv2 != null)
				{
					uv4[indexies[i]] = uv2[i];
				}
				if (normal != null)
				{
					normals[indexies[i]] = normal[i];
				}
				if (tangent != null)
				{
					tangents[indexies[i]] = tangent[i];
				}
			}
			_meshTop.vertices = vertices2;
			if (uv != null && !usingPerChunkBasemap)
			{
				_meshTop.uv = uv3;
			}
			if (uv2 != null)
			{
				_meshTop.uv2 = uv4;
			}
			if (normal != null)
			{
				_meshTop.normals = normals;
			}
			if (tangent != null)
			{
				_meshTop.tangents = tangents;
			}
		}
	}
}
