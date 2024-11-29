using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tank : MonoBehaviour
{
    Collider2D coll;

    [SerializeField] public int currentTankLevel;
    [SerializeField] public TextMeshProUGUI badgeText;

    public int refundLevelIndex;

    public int tankCost;

    public bool isActive = false;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
    }

    private void Update()
    {
        Collider2D[] overlappingColliders = Physics2D.OverlapBoxAll(coll.bounds.center, coll.bounds.size, 0);
        foreach (Collider2D overlappingCollider in overlappingColliders) {
            if (overlappingCollider.CompareTag("SpawnPoint")) {
                if (overlappingCollider.GetComponent<SpawnPointChecker>().isActivePosition) {
                    isActive = true;
                    if (!GetComponent<TankMovement>().isDragging) {
                        GetComponent<TankMovement>().canShoot = true;
                    } else {
                        GetComponent<TankMovement>().canShoot = false;
                    }
                } else {
                    isActive = false;
                }
            }
        }
    }

    public int GetCurrentLevel()
    {
        return currentTankLevel;
    }

    public void UpdateTankLevel(int tankLevel)
    {
        currentTankLevel = tankLevel;
        badgeText.text = (currentTankLevel + 1).ToString();
    }

    public int CalculateRefund()
    {
        float[] refundPercentages = { 0.10f, 0.15f, 0.20f, 0.25f, 0.30f, 0.35f, 0.40f, 0.45f, 0.50f, 0.55f };
        refundLevelIndex = GameObject.Find("ShopPanelManager").GetComponent<ShopPanelManager>().tankRefundLevelIndex;
        float refundPercentage = refundPercentages[refundLevelIndex];
        if (tankCost <= 0) {
            tankCost = 125;
        }
        return Mathf.RoundToInt(tankCost * refundPercentage);
    }
}