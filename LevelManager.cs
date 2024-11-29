using UnityEngine;

public static class LevelManager
{
    public static int enemyKillCount { get; private set; }
    public static int coinCount { get; private set; }

    static LevelManager()
    {
        enemyKillCount = 0;
        coinCount = 0;
    }

    public static void AddCoins(int amount) => coinCount += amount;

    public static void RemoveCoins(int amount) => coinCount = Mathf.Max(0, coinCount - amount);

    public static void AddKill() => enemyKillCount++;

    public static void ResetKillCount() => enemyKillCount = 0;

    public static void ResetCoinCount() => coinCount = 0;

    // New methods for loading saved data
    public static void SetCoinCount(int amount) => coinCount = amount;
    public static int GetCoinCount() => coinCount;
    public static void SetKillCount(int count) => enemyKillCount = count;
}
