using UnityEngine;

[System.Serializable]
public class MonsterData
{
    public GameObject monsterPrefab;
    public Sprite monsterIcon;
    public string monsterName = "僵尸";
    public string description = "普通僵尸";
    public int maxHealth = 50;
    public float moveSpeed = 0.8f;
}
