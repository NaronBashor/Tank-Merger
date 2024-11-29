using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TankSpawner : MonoBehaviour
{
    [SerializeField] public int currentTankLevel;

    [SerializeField] private Transform tankParent;

    [SerializeField] public List<GameObject> tankPrefabList = new List<GameObject>();

    [SerializeField] private List<Transform> tanksSpawnPositions = new List<Transform>();

    [SerializeField] private float maxSpawnTime;

    public List<int> possibleSpawnPositions = new List<int>();

    private float currentspawnTime;

    public bool canSpawn;
    public bool playerdTutorial = false;

    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Application.persistentDataPath + "/gamedata.json";
    }

    private void Start()
    {
        LoadGame();

        canSpawn = true;
        // Only spawn a tank if there is no saved game data
        if (!File.Exists(saveFilePath)) {
            CheckPossibleSpawnLocations("", 0);
        }
    }

    public void CheckPossibleSpawnLocations(string type, int cost)
    {
        possibleSpawnPositions.Clear();
        int tankLevel = PickWeightedRandom(currentTankLevel);
        for (int i = 0; i < tanksSpawnPositions.Count - 6; i++) {
            if (tanksSpawnPositions[i].GetComponent<SpawnPointChecker>().CanSpawn()) {
                possibleSpawnPositions.Add(i);
            }
        }
        if (possibleSpawnPositions.Count > 0) {
            int randomSpawnPos = possibleSpawnPositions[Random.Range(0, possibleSpawnPositions.Count)];
            if (canSpawn) {
                if (type == "tank") {
                    LevelManager.RemoveCoins(cost);
                    //Debug.Log("Spent: " + cost + " on a new tank.");
                    GameObject.Find("LevelManager").GetComponent<UpgradeManager>().currentTankPurchaseIndex += 1;
                }
                SpawnTank(tankLevel, randomSpawnPos, cost);
            }
        } else if (possibleSpawnPositions.Count <= 0) {
            GameObject.Find("LevelManager").GetComponent<UpgradeManager>().StartTheCoroutine("tankcantspawn");
        }
    }

    public void SpawnTank(int tankLevel, int spawnIndex, int cost)
    {
        // Use the specified tankLevel from saved data instead of randomly picking a level
        GameObject tank = Instantiate(tankPrefabList[tankLevel], tanksSpawnPositions[spawnIndex].position, Quaternion.identity, tankParent);
        tanksSpawnPositions[spawnIndex].GetComponent<SpawnPointChecker>().OccupyingTank = tank;

        // Set properties of the tank to match saved data
        tank.GetComponent<Tank>().tankCost = cost;
        tank.GetComponent<Tank>().UpdateTankLevel(tankLevel);  // Ensure this method sets the correct level
        tank.GetComponent<TankMovement>().currentSnapPointPosition = spawnIndex;

        if (spawnIndex < 6) {
            tank.transform.SetAsFirstSibling();
        } else {
            tank.transform.SetAsLastSibling();
        }
    }


    public int PickWeightedRandom(int maxLevel)
    {
        int[] weights = new int[maxLevel + 1];

        for (int i = 0; i <= maxLevel; i++) {
            weights[i] = Mathf.RoundToInt(Mathf.Pow(2, maxLevel - i));
        }

        int totalWeight = 0;
        foreach (int weight in weights) {
            totalWeight += weight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        for (int i = 0; i <= maxLevel; i++) {
            cumulativeWeight += weights[i];
            if (randomValue < cumulativeWeight) {
                return i;
            }
        }

        return maxLevel;
    }

    private void OnApplicationQuit()
    {
        SaveGame();
        //LevelManager.ResetCoinCount();
    }

    public void SaveGame()
    {
        GameData saveData = new GameData {
            playedTutorial = playerdTutorial,
            currentTankLevel = currentTankLevel,
            currentTankPurchaseIndex = GameObject.Find("LevelManager").GetComponent<UpgradeManager>().currentTankPurchaseIndex,
            playerCoinCount = LevelManager.GetCoinCount()
        };

        foreach (var pos in tanksSpawnPositions) {
            if (pos.GetComponent<SpawnPointChecker>().OccupyingTank != null) {
                Tank tank = pos.GetComponent<SpawnPointChecker>().OccupyingTank.GetComponent<Tank>();
                saveData.tanksOnField.Add(new TankData {
                    tankLevel = tank.GetCurrentLevel(),  // Ensure this method gets the tank's actual level
                    spawnIndex = tanksSpawnPositions.IndexOf(pos),
                    cost = tank.tankCost
                });
            }
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game saved successfully at " + saveFilePath);
    }


    public void LoadGame()
    {
        if (File.Exists(saveFilePath)) {
            string json = File.ReadAllText(saveFilePath);
            GameData loadedData = JsonUtility.FromJson<GameData>(json);

            currentTankLevel = loadedData.currentTankLevel;
            playerdTutorial = loadedData.playedTutorial;
            GameObject.Find("LevelManager").GetComponent<UpgradeManager>().currentTankPurchaseIndex = loadedData.currentTankPurchaseIndex;
            LevelManager.SetCoinCount(loadedData.playerCoinCount);

            foreach (var tankData in loadedData.tanksOnField) {
                // Spawn each tank at its saved level and position
                SpawnTank(tankData.tankLevel, tankData.spawnIndex, tankData.cost);
            }

            Debug.Log("Game loaded successfully from " + saveFilePath);
        } else {
            Debug.LogWarning("Save file not found at " + saveFilePath);
        }
    }

}