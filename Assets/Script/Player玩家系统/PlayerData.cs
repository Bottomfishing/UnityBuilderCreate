using System;

[Serializable]
public class PlayerData
{
    public int coins = 0;
    public int diamonds = 0;
    public int energy = 0;
    public int maxEnergy = 100;
    
    public PlayerData()
    {
        coins = 0;
        diamonds = 0;
        energy = 100;
        maxEnergy = 100;
    }
}
