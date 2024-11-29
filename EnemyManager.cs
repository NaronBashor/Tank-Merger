using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPositions = new List<Transform>();
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();

    public Transform enemyParent;
    public GameObject countDownTimer;
    public Text countDownText;

    public float spawnTimer;
    public float spawnTime;
    public float reductionRate = 0.05f;

    private int lastIndex = -1;
    private int secondLastIndex = -1;

    public Wave[] waves;

    public int currentWaveIndex = 0;
    private int enemiesRemainingToSpawn;
    private int enemiesRemainingAlive;

    public float timeBetweenWaves = 5f;
    private float nextTickTime;
    private float waveCountdown;
    private bool waveInProgress = false;

    private bool isCountingDown = false;

    private void OnEnable() // Add this to reset the game when the scene is reloaded
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() // Cleanup to avoid memory leaks when the object is disabled
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ensure the game state is reset each time the scene loads
        StartNewGame();
        SoundManager.Instance.TriggerSoundEvent("game");
    }

    private void Start()
    {
        // Commented out StartNewGame because it is now handled in OnSceneLoaded
        // StartNewGame(); // Initialize the game when the scene starts
        SoundManager.Instance.TriggerSoundEvent("game");
    }

    private void Update()
    {
        if (waveCountdown <= 0f && !waveInProgress) {
            StartCoroutine(SpawnWave());
            countDownTimer.SetActive(false);
            isCountingDown = false;
        } else {
            waveCountdown -= Time.deltaTime;
            if (waveCountdown <= nextTickTime && waveCountdown > 0f) {
                SoundManager.Instance.TriggerSoundEvent("timerSound");

                nextTickTime -= 1f;
            }
        }

        if (isCountingDown && waveCountdown > 0) {
            DisplayCountDown();
        }
    }

    // Method to start a new game and reset necessary variables
    public void StartNewGame()
    {
        if (Time.timeScale != 1) {
            Time.timeScale = 1;
        }

        // Reset wave index and state variables
        currentWaveIndex = 0;
        enemiesRemainingToSpawn = 0;
        enemiesRemainingAlive = 0;
        waveInProgress = false;

        // Reset timers and countdown
        isCountingDown = true;
        nextTickTime = 5f;
        timeBetweenWaves = 5f;
        waveCountdown = timeBetweenWaves;
        spawnTime = spawnTimer;

        // Clear any enemies left from the previous game
        foreach (Transform child in enemyParent) {
            Destroy(child.gameObject);
        }

        // Reinitialize the waves
        CreateNewWave();
        DisplayCountDown();
    }

    private void CreateNewWave()
    {
        Wave[] updatedWaveArray = new Wave[waves.Length + 1];
        for (int i = 0; i < waves.Length; i++) {
            updatedWaveArray[i] = waves[i];
        }

        Wave newWave = new Wave();
        float timeAtLevel = 6 * Mathf.Pow(1 - reductionRate, currentWaveIndex - 1);
        newWave.numberOfEnemies = ((currentWaveIndex + 1) * 5);
        newWave.spawnInterval = timeAtLevel;

        updatedWaveArray[updatedWaveArray.Length - 1] = newWave;

        waves = updatedWaveArray;
    }

    private void DisplayCountDown()
    {
        countDownTimer.SetActive(true);
        countDownText.text = waveCountdown.ToString("F2") + "s";
        countDownTimer.GetComponent<Image>().fillAmount = waveCountdown / 5f;
    }

    IEnumerator SpawnWave()
    {
        waveInProgress = true;

        Wave wave = waves[currentWaveIndex];

        if ((currentWaveIndex + 1) % 3 == 0) {
            GameObject.Find("TankManager").GetComponent<TankSpawner>().CheckPossibleSpawnLocations("", 0);
        }

        enemiesRemainingToSpawn = wave.numberOfEnemies;
        enemiesRemainingAlive = wave.numberOfEnemies;

        for (int i = 0; i < wave.numberOfEnemies; i++) {
            SpawnEnemy();
            enemiesRemainingToSpawn--;

            yield return new WaitForSeconds(wave.spawnInterval);
        }

        while (enemiesRemainingAlive > 0) {
            yield return null;
        }

        waveInProgress = false;
    }

    private void SpawnEnemy()
    {
        int spawnIndex;

        do {
            spawnIndex = Random.Range(0, spawnPositions.Count);
        } while (spawnIndex == lastIndex && spawnIndex == secondLastIndex);

        secondLastIndex = lastIndex;
        lastIndex = spawnIndex;

        GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, currentWaveIndex + 1)], spawnPositions[spawnIndex].position, Quaternion.identity, enemyParent);
        enemy.transform.SetAsFirstSibling();
    }

    public void OnEnemyDeath()
    {
        enemiesRemainingAlive--;

        if (enemiesRemainingAlive <= 0 && enemiesRemainingToSpawn <= 0) {
            currentWaveIndex++;
            CreateNewWave();
            waveCountdown = timeBetweenWaves;
            nextTickTime = 5f;
            isCountingDown = true;
        }
    }
}