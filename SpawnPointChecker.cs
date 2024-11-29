using UnityEngine;

public class SpawnPointChecker : MonoBehaviour
{
    private Collider2D coll;
    [SerializeField] public int currentPos;

    public bool IsOccupied { get; private set; }

    public bool isActivePosition;

    public GameObject OccupyingTank;

    private void Awake()
    {
        coll = GetComponent<Collider2D>();
        IsOccupied = false;
    }

    public bool CanSpawn()
    {
        if (IsOccupied) return false;

        Collider2D[] overlappingColliders = Physics2D.OverlapBoxAll(coll.bounds.center, coll.bounds.size, 0);
        foreach (Collider2D overlappingCollider in overlappingColliders) {
            if (overlappingCollider.CompareTag("Tank")) {
                overlappingCollider.GetComponent<TankMovement>().currentSnapPointPosition = currentPos;
            }
        }
        return true;
    }

    public void SetOccupied(bool occupied)
    {
        IsOccupied = occupied;
    }
}