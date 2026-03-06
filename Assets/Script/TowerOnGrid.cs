using UnityEngine;

public class TowerOnGrid : MonoBehaviour
{
    [HideInInspector]
    public TowerData towerData;
    
    public void SetTowerData(TowerData data)
    {
        towerData = data;
    }
}
