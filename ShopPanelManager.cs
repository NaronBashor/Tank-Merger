using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanelManager : MonoBehaviour
{
    [SerializeField] public int tankRefundLevelIndex;
    [SerializeField] public int coinFactorLevelIndex;
    [SerializeField] public int shieldLevelIndex;
    [SerializeField] public int tankReduceLevelIndex;

    [SerializeField] private TextMeshProUGUI trashCostText;
    [SerializeField] private TextMeshProUGUI coinCostText;
    [SerializeField] private TextMeshProUGUI shieldCostText;
    [SerializeField] private TextMeshProUGUI tankCostText;

    [SerializeField] private Sprite emptyStar;
    [SerializeField] private Sprite fullStar;

    [SerializeField] private List<Image> coinStars = new List<Image>();
    [SerializeField] private List<Image> trashStars = new List<Image>();
    [SerializeField] private List<Image> shieldStars = new List<Image>();
    [SerializeField] private List<Image> tankStars = new List<Image>();

    private int trashBaseCost;
    private int coinBaseCost;
    private int shieldBaseCost;
    private int tankBaseCost;

    private void Start()
    {
        tankRefundLevelIndex = 0;
        coinFactorLevelIndex = 0;
        shieldLevelIndex = 0;
        tankReduceLevelIndex = 0;

        trashBaseCost = 125;
        coinBaseCost = 150;
        shieldBaseCost = 140;
        tankBaseCost = 175;

        FillStarsForUpgrades("trash");
        FillStarsForUpgrades("coin");
        FillStarsForUpgrades("shield");
        FillStarsForUpgrades("tank");
    }

    private void Update()
    {
        trashCostText.text = (trashBaseCost * (Mathf.Pow(1.5f, (tankRefundLevelIndex)))).ToString();
        coinCostText.text = (coinBaseCost * (Mathf.Pow(1.5f, (coinFactorLevelIndex)))).ToString();
        shieldCostText.text = (shieldBaseCost * (Mathf.Pow(1.5f, (shieldLevelIndex)))).ToString();
        tankCostText.text = (tankBaseCost * (Mathf.Pow(1.5f, (tankReduceLevelIndex)))).ToString();
    }

    private void FillStarsForUpgrades(string name)
    {
        int levelIndex = 0;
        List<Image> stars = null;

        switch (name) {
            case "trash":
                levelIndex = tankRefundLevelIndex;
                stars = trashStars;
                break;
            case "coin":
                levelIndex = coinFactorLevelIndex;
                stars = coinStars;
                break;
            case "shield":
                levelIndex = shieldLevelIndex;
                stars = shieldStars;
                break;
            case "tank":
                levelIndex = tankReduceLevelIndex;
                stars = tankStars;
                break;
        }

        if (stars != null) {
            FillStars(stars, levelIndex);
        }
    }

    private void FillStars(List<Image> stars, int levelIndex)
    {
        int fullStarCount = (levelIndex) / 2;

        for (int i = 0; i < stars.Count; i++) {
            stars[i].sprite = i < fullStarCount ? fullStar : emptyStar;
        }
    }


    public void OnTankRefundLevelIncrease()
    {
        float cost = trashBaseCost * (Mathf.Pow(2, (tankRefundLevelIndex)));
        if (tankRefundLevelIndex < 10) {
            if (cost <= LevelManager.coinCount) {
                LevelManager.RemoveCoins(Mathf.RoundToInt(cost));
                tankRefundLevelIndex++;
                FillStarsForUpgrades("trash");
            } else {
                Debug.Log("Not enough coins.");
            }
        } else {
            Debug.Log("Max level reached.");
        }
    }

    public void OnCoinFactorLevelIncrease()
    {
        float cost = coinBaseCost * (Mathf.Pow(2, (coinFactorLevelIndex)));
        if (coinFactorLevelIndex < 10) {
            if (cost <= LevelManager.coinCount) {
                LevelManager.RemoveCoins(Mathf.RoundToInt(cost));
                coinFactorLevelIndex++;
                FillStarsForUpgrades("coin");
            } else {
                Debug.Log("Not enough coins.");
            }
        } else {
            Debug.Log("Max level reached.");
        }
    }

    public void OnShieldLevelIncrease()
    {
        float cost = shieldBaseCost * (Mathf.Pow(2, (shieldLevelIndex)));
        if (shieldLevelIndex < 10) {
            if (cost <= LevelManager.coinCount) {
                LevelManager.RemoveCoins(Mathf.RoundToInt(cost));
                shieldLevelIndex++;
                FillStarsForUpgrades("shield");
            } else {
                Debug.Log("Not enough coins.");
            }
        } else {
            Debug.Log("Max level reached.");
        }
    }

    public void OnTankCostLevelIncrease()
    {
        float cost = tankBaseCost * (Mathf.Pow(2, (tankReduceLevelIndex)));
        if (tankReduceLevelIndex < 10) {
            if (cost <= LevelManager.coinCount) {
                LevelManager.RemoveCoins(Mathf.RoundToInt(cost));
                tankReduceLevelIndex++;
                FillStarsForUpgrades("tank");
            } else {
                Debug.Log("Not enough coins.");
            }
        } else {
            Debug.Log("Max level reached.");
        }
    }
}