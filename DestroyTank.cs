using UnityEngine;
using UnityEngine.EventSystems;

public class DestroyTank : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform uiElement;
    private RectTransform hoveringTank;

    private void Start()
    {
        uiElement = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && hoveringTank != null) {
            TankMovement tankScript = hoveringTank.GetComponent<TankMovement>();
            if (tankScript != null) {
                LevelManager.AddCoins(hoveringTank.GetComponent<Tank>().CalculateRefund());
                tankScript.MarkSnapPointAsOccupied(tankScript.originalSnapPoint, false);
                Destroy(hoveringTank.gameObject);
                hoveringTank = null;
            }
        }

        if (hoveringTank != null) {
            this.uiElement.localScale = Vector3.one * 1.25f;
        } else {
            this.uiElement.localScale = Vector3.one;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag.CompareTag("Tank")) {
            hoveringTank = eventData.pointerDrag.GetComponent<RectTransform>();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag.CompareTag("Tank")) {
            hoveringTank = null;
        }
    }
}