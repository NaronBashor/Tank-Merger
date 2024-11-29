using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TankSpawner tankSpawner;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private GameObject gameOverPanel;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private float maxHP;
    [SerializeField] public float currentHP;
    [SerializeField] private Image fillImage;

    [SerializeField] private GameObject buyTankButton;
    [SerializeField] private GameObject buyShieldButton;
    [SerializeField] private GameObject shopButton;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject shopPanelBackground;
    [SerializeField] private GameObject soundManager;

    [SerializeField] private List<Sprite> tankImages;

    [SerializeField] private Image tankIcon;
    [SerializeField] private Image badgeLevelIcon;
    [SerializeField] private TextMeshProUGUI tankLevelText;

    [SerializeField] private Sprite soundOffSprite;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Button soundButton;

    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI cointTotalText;
    [SerializeField] private GameObject pausePanel;

    private string sceneName;


    public Color highColor = Color.white;
    public Color midColor = new Color(1f, 0.5f, 0f);
    public Color lowColor = Color.red;
    public Color shieldColor = Color.blue;

    public bool soundOn;
    public bool shieldActive = false;

    private void Start()
    {
        //gameOverPanel.SetActive(false);

        sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Game") {
            GameObject.Find("Sounds").GetComponent<SoundPlayer>().mainMenuMusic.Stop();
        }

        if (soundManager == null) {
            soundManager = GameObject.Find("SoundManager");
        }

        shopPanel.SetActive(false);
        shopPanelBackground.SetActive(false);
        pausePanel.SetActive(false);

        soundButton.image.sprite = soundOnSprite;
        soundOn = true;

        tankIcon.sprite = tankImages[tankSpawner.currentTankLevel];
        tankLevelText.text = (1 + tankSpawner.currentTankLevel).ToString();
        currentHP = maxHP;

        hpSlider.onValueChanged.AddListener(UpdateSliderColor);
        UpdateSliderColor((currentHP / 100));
    }

    private void UpdateSliderColor(float value)
    {
        hpSlider.value = value;
        if (shieldActive) {
            return;
        }
        if (value > 0.5f) {
            fillImage.color = highColor;
        } else if (value > 0.25f) {
            fillImage.color = midColor;
        } else {
            fillImage.color = lowColor;
        }
    }

    private void Update()
    {
        UpdateSliderColor((currentHP / 100));
        if (currentHP <= 0) {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0;
        }
        waveText.text = "Wave: " + (enemyManager.GetComponent<EnemyManager>().currentWaveIndex + 1).ToString();
        cointTotalText.text = LevelManager.coinCount.ToString();

        //if (Input.GetKeyDown(KeyCode.Space)) {
        //    LevelManager.AddCoins(1000000);
        //}

        if (Input.GetKeyDown(KeyCode.Escape)) {
            SwitchTimeScale();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (shopPanel.activeInHierarchy && shopPanelBackground.activeInHierarchy) {
                OnShopButtonExitButtonPressed();
            }
        }
    }

    void SwitchTimeScale()
    {
        if (Time.timeScale == 1) {
            Time.timeScale = 0;
            pausePanel.SetActive(true);
        } else if (Time.timeScale == 0) {
            Time.timeScale = 1;
            pausePanel.SetActive(false);
        }
    }

    public void ApplyDamage(float amount)
    {
        currentHP -= amount;
    }

    public void ToggleSound()
    {
        if (soundManager == null) {
            soundManager = GameObject.Find("SoundManager");
        }
        soundOn = !soundOn;
        soundManager.SetActive(soundOn);
        if (soundOn) {
            soundButton.image.sprite = soundOnSprite;
            if (sceneName == "MainMenu") {
                SoundManager.Instance.TriggerSoundEvent("mainMenu");
            } else if (sceneName == "Game") {
                SoundManager.Instance.TriggerSoundEvent("game");
                GameObject.Find("Sounds").GetComponent<SoundPlayer>().mainMenuMusic.Stop();
            }
        } else {
            soundButton.image.sprite = soundOffSprite;
        }
    }

    public bool CheckShieldPurchase()
    {
        if (currentHP < 80) {
            return true;
        }
        return false;
    }

    public void OnBuyShieldButtonPressed()
    {
        int level = GameObject.Find("ShopPanelManager").GetComponent<ShopPanelManager>().shieldLevelIndex;
        float healingPercentage = CalculateHealingPercentage(level);

        float missingHp = maxHP - currentHP;
        float amountToRegain = missingHp * healingPercentage;
        StartCoroutine(ShieldCooldown(amountToRegain, 2f));
    }

    private float CalculateHealingPercentage(int level)
    {
        float initialHealPercent = 0.20f;
        float finalHealPercent = 0.75f;
        int maxLevel = 10;

        float incrementPerLevel = (finalHealPercent - initialHealPercent) / (maxLevel - 1);

        return initialHealPercent + incrementPerLevel * (level - 1);
    }

    public void OnShopButtonPressed()
    {
        SoundManager.Instance.TriggerSoundEvent("buttonClick");
        Time.timeScale = 0f;
        shopPanel.SetActive(true);
        shopPanelBackground.SetActive(true);
    }

    public void OnShopButtonExitButtonPressed()
    {
        SoundManager.Instance.TriggerSoundEvent("buttonClick");
        Time.timeScale = 1f;
        shopPanel.SetActive(false);
        shopPanelBackground.SetActive(false);
    }

    IEnumerator ShieldCooldown(float amount, float duration)
    {
        shieldActive = true;
        fillImage.color = Color.blue;

        float targetHP = currentHP + amount;
        float startHP = currentHP;
        float elapsed = 0f;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            currentHP = Mathf.Lerp(startHP, targetHP, elapsed / duration);
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);

            yield return null;
        }

        currentHP = Mathf.Clamp(targetHP, 0, maxHP);
        shieldActive = false;
    }
}