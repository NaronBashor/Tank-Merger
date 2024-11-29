using UnityEngine;

public class TankDragAndSnap : MonoBehaviour
{
    public Transform[] snapPoints; // Array of snap points (center of the squares)
    public float snapThreshold = 50f; // Distance within which the snap should happen
    public LayerMask tankLayerMask; // Layer mask for detecting other tanks

    private bool isDragging = false;
    private Vector3 offset;
    private Transform closestSnapPoint;
    private Vector3 originalPosition;
    public Transform originalSnapPoint;

    void Start()
    {
        if (snapPoints.Length <= 0) {
            GameObject[] findAllPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

            snapPoints = new Transform[findAllPoints.Length];

            for (int i = 0; i < findAllPoints.Length; i++) {
                snapPoints[i] = findAllPoints[i].transform;
            }
        }

        // Initially find the snap point this tank is occupying (if any)
        originalSnapPoint = FindClosestSnapPoint(transform.position);
        if (originalSnapPoint != null) {
            MarkSnapPointAsOccupied(originalSnapPoint, true);
        }
    }

    void Update()
    {
        // Dragging logic
        if (isDragging) {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x, mousePosition.y, 0) + offset;
        }

        // Snapping logic
        if (Input.GetMouseButtonUp(0) && isDragging) {
            isDragging = false;
            SnapToClosestPoint();
        }
    }

    void OnMouseDown()
    {
        // Start dragging
        isDragging = true;
        originalPosition = transform.position; // Save the original position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mousePosition.x, mousePosition.y, 0);

        // Ensure the original snap point remains marked as occupied during dragging
        if (originalSnapPoint != null) {
            MarkSnapPointAsOccupied(originalSnapPoint, true);
        }
    }

    void SnapToClosestPoint()
    {
        float closestDistance = Mathf.Infinity;
        closestSnapPoint = null;

        foreach (Transform snapPoint in snapPoints) {
            float distance = Vector3.Distance(transform.position, snapPoint.position);

            if (distance < closestDistance) {
                SpawnPointChecker checker = snapPoint.GetComponent<SpawnPointChecker>();
                if (checker != null && !checker.IsOccupied && snapPoint != originalSnapPoint) {
                    closestDistance = distance;
                    closestSnapPoint = snapPoint;
                }
            }
        }

        if (closestSnapPoint != null && closestDistance <= snapThreshold) {
            // Mark the original snap point as unoccupied only after a new snap point is found
            MarkSnapPointAsOccupied(originalSnapPoint, false);

            // Snap to the new closest snap point
            transform.position = closestSnapPoint.position;
            originalSnapPoint = closestSnapPoint; // Update the original snap point

            // Mark the new snap point as occupied
            MarkSnapPointAsOccupied(originalSnapPoint, true);
        } else {
            // If no valid snap point is found, snap back to the original position
            transform.position = originalPosition;

            // Ensure the original snap point remains marked as occupied
            MarkSnapPointAsOccupied(originalSnapPoint, true);
        }
    }

    private Transform FindClosestSnapPoint(Vector3 position)
    {
        float closestDistance = Mathf.Infinity;
        Transform closestPoint = null;

        foreach (Transform snapPoint in snapPoints) {
            float distance = Vector3.Distance(position, snapPoint.position);

            if (distance < closestDistance) {
                closestDistance = distance;
                closestPoint = snapPoint;
            }
        }

        return closestPoint;
    }

    public void MarkSnapPointAsOccupied(Transform snapPoint, bool occupied)
    {
        SpawnPointChecker checker = snapPoint.GetComponent<SpawnPointChecker>();
        if (checker != null) {
            checker.SetOccupied(occupied); // Correctly set the snap point as occupied or unoccupied
        }
    }
}
