using UnityEngine;

[System.Serializable]
public class WaveZombie
{
    public GameObject zombiePrefab;
    public int count = 5;
}

[System.Serializable]
public class WaveData
{
    public string waveName = "第1波";
    public WaveZombie[] zombies;
    public float spawnInterval = 2f;
    [Range(0f, 1f)]
    public float mixChance = 0f;
}
