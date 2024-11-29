using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class TankMovement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    TankSpawner tankSpawner;
    Rigidbody2D rb;

    public RectTransform[] allTanks;
    public RectTransform[] snapPoints;
    public float snapThreshold = 50f;
    public Canvas canvas;
    public int currentSnapPointPosition;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private RectTransform closestSnapPoint;
    public RectTransform originalSnapPoint;

    public float tiltAmount;   // Amount of tilt (in degrees)
    public float rotationResetSpeed;  // Speed at which rotation resets
    public float movementThreshold; // Threshold to detect when the object stops moving

    public bool isDragging = false;
    public bool canShoot = false;
    public int siblingIndex;

    private Vector3 previousMousePosition;
    private Vector3 currentMousePosition;
    private Vector2 lastPosition;
    private bool isResettingRotation = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        previousMousePosition = Input.mousePosition;

        transform.rotation = Quaternion.identity;

        tankSpawner = GameObject.Find("TankManager").GetComponent<TankSpawner>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null) {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (snapPoints.Length <= 0) {
            GameObject[] findAllPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

            snapPoints = new RectTransform[findAllPoints.Length];

            for (int i = 0; i < findAllPoints.Length; i++) {
                snapPoints[i] = findAllPoints[i].GetComponent<RectTransform>();
            }
        }

        originalSnapPoint = FindClosestSnapPoint(rectTransform.anchoredPosition);
        if (originalSnapPoint != null) {
            MarkSnapPointAsOccupied(originalSnapPoint, true);
        }

        isDragging = false;
    }

    private void Update()
    {
        if (isDragging) {
            this.rectTransform.SetAsLastSibling();
        }

        Vector2 currentPosition = rb.position;
        Vector2 velocity = (currentPosition - lastPosition) / Time.deltaTime;

        // Check the velocity to determine if the tank has stopped moving
        if (velocity.magnitude < movementThreshold) {
            if (!isResettingRotation) {
                StartCoroutine(SmoothResetRotation());
            }
        } else {
            StopAllCoroutines();  // Stop the reset coroutine if the tank is moving
            isResettingRotation = false;

            // Calculate the tilt based on horizontal velocity
            if (Time.timeScale == 1) {
                float targetTilt = Mathf.Clamp(-velocity.x * tiltAmount, -tiltAmount, tiltAmount);

                // Calculate the target rotation quaternion using AngleAxis
                Quaternion targetRotation = Quaternion.AngleAxis(targetTilt, Vector3.forward);

                // Lerp towards the target rotation with Time.deltaTime to smooth the transition
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationResetSpeed * Time.deltaTime);

            }
        }

        lastPosition = currentPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        canShoot = false;
        siblingIndex = this.rectTransform.GetSiblingIndex();

        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;

        rectTransform.SetAsLastSibling();

        previousMousePosition = Input.mousePosition; // Initialize previous mouse position

        if (originalSnapPoint != null) {
            MarkSnapPointAsOccupied(originalSnapPoint, true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPointerPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out localPointerPosition);
        rectTransform.anchoredPosition = localPointerPosition;

        RectTransform otherTank = CheckIfHoveringAnotherTank();
        if (otherTank != null) {
            TankMovement otherTankScript = otherTank.GetComponent<TankMovement>();

            if (otherTankScript != null && otherTankScript.GetComponent<Tank>().currentTankLevel == GetComponent<Tank>().currentTankLevel && !otherTankScript.GetComponent<Tank>().isActive && !GetComponent<Tank>().isActive) {
                MarkSnapPointAsOccupied(originalSnapPoint, false);
                CombineTanks(otherTankScript);
            }
        }

        currentMousePosition = Input.mousePosition;

        rb.MovePosition(rectTransform.anchoredPosition);

        // Calculate the drag direction on the x-axis
        float dragDirection = currentMousePosition.x - previousMousePosition.x;

        // Calculate the target tilt based on the drag direction, opposite the movement direction
        float targetTilt = Mathf.Clamp(-dragDirection * tiltAmount, -tiltAmount, tiltAmount);

        // Smoothly rotate to the target tilt angle
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, targetTilt), Time.deltaTime * rotationResetSpeed);

        // Update previous mouse position
        previousMousePosition = currentMousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        SnapToClosestPoint();
        canvasGroup.blocksRaycasts = true;

        // Ensure the tank is upright when dragging ends
        StartCoroutine(SmoothResetRotation());
    }

    private IEnumerator SmoothResetRotation()
    {
        isResettingRotation = true;

        Quaternion initialRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.identity;

        float elapsedTime = 0f;
        float duration = 0.125f; // Increase the duration to make the reset slower
        while (elapsedTime < duration) {
            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final rotation is set to exactly upright
        transform.rotation = targetRotation;
        isResettingRotation = false;
    }

    private RectTransform CheckIfHoveringAnotherTank()
    {
        allTanks = FindObjectsOfType<RectTransform>();

        foreach (RectTransform otherTank in allTanks) {
            if (otherTank != rectTransform && otherTank.CompareTag("Tank")) {
                if (RectOverlaps(rectTransform, otherTank)) {
                    return otherTank;
                }
            }
        }
        return null;
    }

    private bool RectOverlaps(RectTransform rect1, RectTransform rect2, float scale = 0.25f)
    {
        Vector3[] corners1 = new Vector3[4];
        rect1.GetWorldCorners(corners1);
        Rect worldRect1 = new Rect(corners1[0], (corners1[2] - corners1[0]) * scale);

        Vector3[] corners2 = new Vector3[4];
        rect2.GetWorldCorners(corners2);
        Rect worldRect2 = new Rect(corners2[0], (corners2[2] - corners2[0]) * scale);

        return worldRect1.Overlaps(worldRect2);
    }

    private void CombineTanks(TankMovement otherTankScript)
    {
        SoundManager.Instance.TriggerSoundEvent("merge");

        int nextLevel = GetComponent<Tank>().currentTankLevel + 1;

        GameObject newTank = Instantiate(tankSpawner.tankPrefabList[nextLevel], otherTankScript.rectTransform.position, Quaternion.identity, canvas.transform);

        TankMovement newTankScript = newTank.GetComponent<TankMovement>();
        newTankScript.GetComponent<Tank>().UpdateTankLevel(nextLevel);

        if (newTankScript.siblingIndex < 6) {
            newTankScript.transform.SetAsFirstSibling();
        } else {
            newTankScript.transform.SetAsLastSibling();
        }

        newTank.GetComponent<RectTransform>().anchoredPosition = otherTankScript.rectTransform.anchoredPosition;
        newTankScript.originalSnapPoint = otherTankScript.originalSnapPoint;
        newTankScript.siblingIndex = otherTankScript.siblingIndex;
        newTank.transform.SetSiblingIndex(siblingIndex);

        MarkSnapPointAsOccupied(newTankScript.originalSnapPoint, true);

        if (nextLevel > tankSpawner.currentTankLevel) {
            tankSpawner.currentTankLevel = nextLevel;
        }

        Destroy(otherTankScript.gameObject);
        Destroy(this.gameObject);
    }



    void SnapToClosestPoint()
    {
        float closestDistance = Mathf.Infinity;
        closestSnapPoint = null;

        foreach (RectTransform snapPoint in snapPoints) {
            float distance = Vector2.Distance(rectTransform.anchoredPosition, snapPoint.anchoredPosition);

            if (distance < closestDistance) {
                SpawnPointChecker checker = snapPoint.GetComponent<SpawnPointChecker>();
                if (checker != null && !checker.IsOccupied && snapPoint != originalSnapPoint) {
                    closestDistance = distance;
                    closestSnapPoint = snapPoint;
                    if (checker.isActivePosition) {
                        canShoot = true;
                    }
                }
            }
        }

        if (closestSnapPoint != null && closestDistance <= snapThreshold) {
            MarkSnapPointAsOccupied(originalSnapPoint, false);

            rectTransform.anchoredPosition = closestSnapPoint.anchoredPosition;
            originalSnapPoint = closestSnapPoint;
            siblingIndex = closestSnapPoint.GetComponent<SpawnPointChecker>().currentPos;
            this.rectTransform.SetSiblingIndex(siblingIndex);

            MarkSnapPointAsOccupied(originalSnapPoint, true);
        } else {
            rectTransform.anchoredPosition = originalPosition;
            this.rectTransform.SetSiblingIndex(siblingIndex);

            MarkSnapPointAsOccupied(originalSnapPoint, true);
        }
    }

    private RectTransform FindClosestSnapPoint(Vector2 position)
    {
        float closestDistance = Mathf.Infinity;
        RectTransform closestPoint = null;

        foreach (RectTransform snapPoint in snapPoints) {
            float distance = Vector2.Distance(position, snapPoint.anchoredPosition);

            if (distance < closestDistance) {
                closestDistance = distance;
                closestPoint = snapPoint;
            }
        }

        return closestPoint;
    }

    public void MarkSnapPointAsOccupied(RectTransform snapPoint, bool occupied)
    {
        SpawnPointChecker checker = snapPoint.GetComponent<SpawnPointChecker>();
        if (checker != null) {
            checker.SetOccupied(occupied);
        }
    }
}