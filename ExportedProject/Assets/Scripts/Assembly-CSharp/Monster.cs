using UnityEngine;

public abstract class Monster : Creature
{
	public Transform[] spawnPoints;

	public abstract void StrikeSucces();

	public virtual void SelectSpawnPoint()
	{
		if (spawnPoints != null && spawnPoints.Length > 0)
		{
			Transform transform = base.transform;
			int num = Random.Range(0, spawnPoints.Length);
			Transform transform2 = spawnPoints[num];
			if ((bool)transform2)
			{
				transform.position = transform2.position;
			}
		}
	}
}
