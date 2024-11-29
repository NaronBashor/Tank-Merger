using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    Animator anim;

    private float maxHealth;
    public float currentHealth;

    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject coinAmountPopUp;
    [SerializeField] private GameObject coinPopupParent;

    public int enemyLevel;
    public int baseCoinValue;
    public int multiplier;

    public float scalingCoinFactor;

    public bool isAlive;
    private bool playOnce;

    private void Start()
    {
        anim = GetComponent<Animator>();
        maxHealth = 28 + (enemyLevel * 10);
        currentHealth = maxHealth;
        isAlive = true;
        baseCoinValue = 1;
        multiplier = 6;
        healthBar.SetActive(false);
        scalingCoinFactor = 2.25f;

        coinPopupParent = GameObject.Find("CoinPopUpParent");
    }

    private void Update()
    {
        if (healthBar.activeSelf) {
            Transform child = healthBar.transform.GetChild(0);
            child.GetComponent<Image>().fillAmount = (currentHealth / maxHealth);
        }
        if (currentHealth <= 0) {
            isAlive = false;
            if (!playOnce) {
                playOnce = true;
                SoundManager.Instance.TriggerSoundEvent("death");
            }
            anim.SetBool("isAlive", false);
        }
    }

    public void DealDamage(float damage)
    {
        if (!healthBar.activeSelf) {
            healthBar.SetActive(true);
        }
        if (currentHealth > 0) {
            currentHealth -= damage;
        }
    }

    void CoinPopUpThingy(int amount)
    {
        GameObject popUp = Instantiate(coinAmountPopUp, new Vector2(this.transform.position.x, this.transform.position.y + 50), Quaternion.identity);
        popUp.transform.SetParent(coinPopupParent.transform);
        popUp.GetComponent<TextMeshProUGUI>().text = "+ " + Mathf.RoundToInt(amount).ToString();
    }

    private void AwardCoinsForKill(int level, int multiplier)
    {
        // Basic coin amount calculation based on level and multiplier
        int amount = baseCoinValue + ((level + 1) * multiplier);

        // Get coin factor upgrade level from the shop manager
        GameObject shopPanelObj = GameObject.Find("ShopPanelManager");
        if (shopPanelObj == null) {
            Debug.LogError("ShopPanelManager not found!");
            return;
        }
        ShopPanelManager shopPanelManager = shopPanelObj.GetComponent<ShopPanelManager>();
        int upgradeLevel = shopPanelManager.coinFactorLevelIndex;

        // Apply logarithmic scaling for diminishing returns
        // Mathf.Log scales slowly as the value increases, providing a diminishing increase
        float logScalingFactor = Mathf.Max(Mathf.Log(level + 2, 10), 1.0f); // Ensures it's never less than 1
        float initialBurstScaling = Mathf.Pow(scalingCoinFactor, upgradeLevel); // Keep initial burst for early progression
        Debug.Log($"LogScalingFactor: {logScalingFactor}, InitialBurstScaling: {initialBurstScaling}");

        // Combine the logarithmic scaling with an initial burst effect
        float newAmount = amount * initialBurstScaling * logScalingFactor;
        Debug.Log($"Raw coin amount: {newAmount}");

        // Round the final coin value to an integer
        int finalCoins = Mathf.RoundToInt(newAmount);

        // Award the coins and display the coin popup
        LevelManager.AddCoins(finalCoins);
        CoinPopUpThingy(finalCoins);
    }


    public void DestroyAfterDeath()
    {
        AwardCoinsForKill(enemyLevel, multiplier);
        LevelManager.AddKill();
        GameObject.Find("EnemyManager").GetComponent<EnemyManager>().OnEnemyDeath();
        Destroy(this.gameObject);
    }
}