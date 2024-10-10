using System;

namespace VacuumShaders.TerrainToMesh
{
	[Serializable]
	public class TerrainConvertInfo
	{
		public const int maxVertexCount = 65000;

		public int chunkCountHorizontal;

		public int chunkCountVertical;

		public int vertexCountHorizontal;

		public int vertexCountVertical;

		public bool generateSkirt;

		public float skirtGroundLevel;

		public TerrainConvertInfo()
		{
			Reset();
		}

		public TerrainConvertInfo(TerrainConvertInfo _right)
		{
			chunkCountHorizontal = _right.chunkCountHorizontal;
			chunkCountVertical = _right.chunkCountVertical;
			vertexCountHorizontal = _right.vertexCountHorizontal;
			vertexCountVertical = _right.vertexCountVertical;
			generateSkirt = _right.generateSkirt;
			skirtGroundLevel = _right.skirtGroundLevel;
		}

		public void Reset()
		{
			chunkCountHorizontal = 1;
			chunkCountVertical = 1;
			vertexCountHorizontal = 25;
			vertexCountVertical = 25;
			generateSkirt = false;
			skirtGroundLevel = 0f;
		}

		public int GetChunkCount()
		{
			if (chunkCountHorizontal < 1)
			{
				chunkCountHorizontal = 1;
			}
			if (chunkCountVertical < 1)
			{
				chunkCountVertical = 1;
			}
			return chunkCountHorizontal * chunkCountVertical;
		}

		public int GetVertexCountPerChunk()
		{
			int num = 0;
			if (generateSkirt)
			{
				num = 2 * (vertexCountHorizontal + vertexCountVertical);
			}
			return vertexCountHorizontal * vertexCountVertical + num;
		}

		public int GetVertexCountTotal()
		{
			return GetChunkCount() * GetVertexCountPerChunk();
		}

		public int GetTriangleCountPerChunk()
		{
			int num = 0;
			if (generateSkirt)
			{
				num = 2 * (vertexCountHorizontal + vertexCountVertical) * 2;
			}
			return (vertexCountHorizontal - 1) * (vertexCountVertical - 1) * 2 + num;
		}

		public int GetTriangleCountTotal()
		{
			return GetChunkCount() * GetTriangleCountPerChunk();
		}
	}
}
