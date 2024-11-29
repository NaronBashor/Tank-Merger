using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public bool playedTutorial;
    public int currentTankLevel;
    public int currentTankPurchaseIndex;
    public int playerCoinCount;
    public List<TankData> tanksOnField = new List<TankData>();
}

[System.Serializable]
public class TankData
{
    public int tankLevel;
    public int spawnIndex;
    public int cost;
}
