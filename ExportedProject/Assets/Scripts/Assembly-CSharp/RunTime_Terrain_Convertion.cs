using UnityEngine;
using VacuumShaders.TerrainToMesh;

[AddComponentMenu("VacuumShaders/Terrain To Mesh/Example/Runtime Converter")]
public class RunTime_Terrain_Convertion : MonoBehaviour
{
	public Terrain sourceTerrain;

	public TerrainConvertInfo convertInfo;

	public bool generateBasemap;

	public bool attachMeshCollider;

	private void Start()
	{
		if (!(sourceTerrain != null))
		{
			return;
		}
		Mesh[] array = TerrainToMeshConverter.Convert(sourceTerrain, convertInfo, false);
		if (array == null)
		{
			return;
		}
		Material material = null;
		material = ((!generateBasemap) ? GenerateMaterial_Splatmap() : GenerateMaterial_Basemap());
		if (array.Length == 1)
		{
			MeshFilter meshFilter = base.gameObject.GetComponent<MeshFilter>();
			if (meshFilter == null)
			{
				meshFilter = base.gameObject.AddComponent<MeshFilter>();
			}
			meshFilter.sharedMesh = array[0];
			MeshRenderer meshRenderer = base.gameObject.GetComponent<MeshRenderer>();
			if (meshRenderer == null)
			{
				meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
			}
			meshRenderer.sharedMaterial = material;
			if (attachMeshCollider)
			{
				base.gameObject.AddComponent<MeshCollider>().sharedMesh = meshFilter.sharedMesh;
			}
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = new GameObject(array[i].name);
			gameObject.transform.parent = base.gameObject.transform;
			gameObject.transform.localPosition = Vector3.zero;
			MeshFilter meshFilter2 = gameObject.AddComponent<MeshFilter>();
			meshFilter2.sharedMesh = array[i];
			MeshRenderer meshRenderer2 = gameObject.AddComponent<MeshRenderer>();
			meshRenderer2.sharedMaterial = material;
			if (attachMeshCollider)
			{
				gameObject.AddComponent<MeshCollider>().sharedMesh = meshFilter2.sharedMesh;
			}
		}
	}

	private Material GenerateMaterial_Basemap()
	{
		Texture2D _diffuseMap = null;
		Texture2D _normalMap = null;
		bool sRGB = QualitySettings.activeColorSpace == ColorSpace.Linear;
		TerrainToMeshConverter.ExtractBasemap(sourceTerrain, out _diffuseMap, out _normalMap, 1024, 1024, sRGB);
		Material material = new Material(Shader.Find((!(_normalMap != null)) ? "Legacy Shaders/Diffuse" : "Legacy Shaders/Bumped Diffuse"));
		material.mainTexture = _diffuseMap;
		if (_normalMap != null)
		{
			material.SetTexture("_BumpMap", _normalMap);
		}
		return material;
	}

	private Material GenerateMaterial_Splatmap()
	{
		Material material = null;
		Texture2D[] array = TerrainToMeshConverter.ExtractSplatmaps(sourceTerrain);
		if (array == null || array.Length == 0)
		{
			return material;
		}
		Texture2D[] _diffuseTextures;
		Texture2D[] _normalTextures;
		Vector2[] _uvScale;
		Vector2[] _uvOffset;
		float[] _metalic;
		float[] _smoothness;
		int num = TerrainToMeshConverter.ExtractTexturesInfo(sourceTerrain, out _diffuseTextures, out _normalTextures, out _uvScale, out _uvOffset, out _metalic, out _smoothness);
		if (num == 0 || _diffuseTextures == null)
		{
			Debug.LogWarning("usedTexturesCount == 0");
			return material;
		}
		if (num == 1)
		{
			Shader shader = Shader.Find("Legacy Shaders/Diffuse");
			if (shader != null)
			{
				material = new Material(shader);
				material.mainTexture = _diffuseTextures[0];
				material.mainTextureScale = _uvScale[0];
				material.mainTextureOffset = _uvOffset[0];
			}
			return material;
		}
		num = Mathf.Clamp(num, 2, 8);
		bool flag = false;
		if (_normalTextures != null && num < 5)
		{
			flag = true;
		}
		Shader shader2 = Shader.Find(string.Format("VacuumShaders/Terrain To Mesh/Standard/" + ((!flag) ? "Diffuse" : "Bumped") + "/{0} Textures", num));
		if (shader2 == null)
		{
			Debug.LogWarning("Shader not found: " + string.Format("VacuumShaders/Terrain To Mesh/Standard/" + ((!flag) ? "Diffuse" : "Bumped") + "/{0} Textures", num));
			return material;
		}
		material = new Material(shader2);
		if (array.Length == 1)
		{
			material.SetTexture("_V_T2M_Control", array[0]);
		}
		else
		{
			if (array.Length > 2)
			{
				Debug.Log("TerrainToMesh shaders support max 2 control textures. Current terrain uses " + array.Length);
			}
			material.SetTexture("_V_T2M_Control", array[0]);
			material.SetTexture("_V_T2M_Control2", array[1]);
		}
		for (int i = 0; i < num; i++)
		{
			material.SetTexture(string.Format("_V_T2M_Splat{0}", i + 1), _diffuseTextures[i]);
			material.SetFloat(string.Format("_V_T2M_Splat{0}_uvScale", i + 1), _uvScale[i].x);
			material.SetFloat(string.Format("_V_T2M_Splat{0}_Metallic", i + 1), _metalic[i]);
			material.SetFloat(string.Format("_V_T2M_Splat{0}_Glossiness", i + 1), _smoothness[i]);
			if (flag)
			{
				material.SetTexture(string.Format("_V_T2M_Splat{0}_bumpMap", i + 1), _normalTextures[i]);
			}
		}
		return material;
	}
}
