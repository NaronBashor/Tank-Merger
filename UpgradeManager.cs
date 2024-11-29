using System.Collections;
using TMPro;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private TankSpawner tankSpawner;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject uiParent;

    [SerializeField] private TextMeshProUGUI currentTankPurchaseCostText;
    [SerializeField] private TextMeshProUGUI currentShieldPurchaseCostText;

    [SerializeField] private GameObject notEnoughCoinsWarning;
    [SerializeField] private Transform tankCoinWarningPos;
    [SerializeField] private Transform shieldCoinWarningPos;

    [SerializeField] private int currentRepairIndex;
    [SerializeField] public int currentTankPurchaseIndex;

    private int baseCost;

    private void Start()
    {
        baseCost = 150;

        currentRepairIndex = 0;
        currentTankPurchaseIndex = 0;

        tankSpawner = tankSpawner.GetComponent<TankSpawner>();
    }

    private void Update()
    {
        currentTankPurchaseCostText.text = GetPurchaseAmount("tank").ToString();
        currentShieldPurchaseCostText.text = GetPurchaseAmount("shield").ToString();
    }

    public void PurchaseNewTank(string name)
    {
        SoundManager.Instance.TriggerSoundEvent("buttonClick");
        if (tankSpawner != null) {
            if (CheckCoinTotal(GetPurchaseAmount(name), name)) {

            } else {
                StartCoroutine(Warning("tank"));
            }
        }
    }

    public void PurchaseShieldUpgrade(string name)
    {
        SoundManager.Instance.TriggerSoundEvent("buttonClick");
        if (uiManager != null) {
            if (uiManager.CheckShieldPurchase()) {
                if (CheckCoinTotal(GetPurchaseAmount(name), name)) {
                    uiManager.OnBuyShieldButtonPressed();
                    currentRepairIndex += 1;
                }
            } else {
                StartCoroutine(Warning("healthnotbelow80"));
            }
        }
    }

    private int GetPurchaseAmount(string name)
    {
        if (name == "tank") {
            float[] refundPercentages = { 0.20f, 0.25f, 0.30f, 0.35f, 0.40f, 0.45f, 0.50f, 0.55f, 0.60f, 0.65f };
            int tankReductionLevelIndex = GameObject.Find("ShopPanelManager").GetComponent<ShopPanelManager>().tankReduceLevelIndex;
            float reduceCost = refundPercentages[tankReductionLevelIndex];
            float cost = baseCost * (Mathf.Pow(2, (currentTankPurchaseIndex)));
            return Mathf.RoundToInt(cost - (cost * reduceCost));
        }
        if (name == "shield") {
            float cost = baseCost * (Mathf.Pow(2, (currentRepairIndex)));
            return (int)cost;
        }
        return 999999999;
    }

    private bool CheckCoinTotal(int cost, string name)
    {
        int coins = LevelManager.coinCount;
        if (coins >= cost) {
            if (name == "tank") {
                tankSpawner.CheckPossibleSpawnLocations("tank", cost);
            }
            return true;
        }
        StartCoroutine(Warning("tank"));
        return false;
    }

    public void StartTheCoroutine(string name)
    {
        StartCoroutine(Warning(name));
    }

    IEnumerator Warning(string name)
    {
        if (name == "tank") {
            GameObject warningPopUp = Instantiate(notEnoughCoinsWarning, tankCoinWarningPos.position, Quaternion.identity, uiParent.transform);
            yield return new WaitForSeconds(1f);
            Destroy(warningPopUp);
        }
        if (name == "shield") {
            GameObject warningPopUp = Instantiate(notEnoughCoinsWarning, shieldCoinWarningPos.position, Quaternion.identity, uiParent.transform);
            yield return new WaitForSeconds(1f);
            Destroy(warningPopUp);
        }
        if (name == "healthnotbelow80") {
            GameObject warningPopUp = Instantiate(notEnoughCoinsWarning, shieldCoinWarningPos.position, Quaternion.identity, uiParent.transform);
            warningPopUp.GetComponentInChildren<TextMeshProUGUI>().text = "Must be below 80% HP";
            yield return new WaitForSeconds(1f);
            Destroy(warningPopUp);
        }
        if (name == "tankcantspawn") {
            GameObject warningPopUp = Instantiate(notEnoughCoinsWarning, tankCoinWarningPos.position, Quaternion.identity, uiParent.transform);
            warningPopUp.GetComponentInChildren<TextMeshProUGUI>().text = "No spawn position available";
            yield return new WaitForSeconds(1f);
            Destroy(warningPopUp);
        }
    }
}