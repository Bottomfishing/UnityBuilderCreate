using UnityEngine;

public enum TowerSource
{
    Buildable,
    UpgradeOnly
}

[System.Serializable]
public class TowerDataForBook
{
    public GameObject towerPrefab;
    public Sprite towerIcon;
    public string towerName = "炮塔";
    public string description = "基础炮塔";
    public TowerSource source = TowerSource.Buildable;
    public int buildCost = 50;
    public string upgradeFromTower = "";
    public float attackRange = 3f;
    public float attackDamage = 10f;
    public float attackSpeed = 1f;
}
