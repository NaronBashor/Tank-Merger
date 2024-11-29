using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuTween : MonoBehaviour
{
    [SerializeField] private RectTransform uiPanel;

    private float animationTime = 1f;
    private float bounceScale = 1.25f;
    private float finalScale = 1f;

    private void Start()
    {
        // Ensure that the uiPanel scale is reset properly when the scene starts
        if (!uiPanel.gameObject.activeInHierarchy) {
            return;
        }

        // Reset uiPanel scale to a known state (Vector3.one) before starting the animation
        uiPanel.localScale = Vector3.one;
        TweenMenu();
    }

    public void TweenMenu()
    {
        // Start the tweening animation
        LeanTween.scale(uiPanel, new Vector3(bounceScale, bounceScale, 1f), animationTime)
            .setEaseOutBack()
            .setOnComplete(() =>
            {
                LeanTween.scale(uiPanel, new Vector3(finalScale, finalScale, 1f), 0.2f)
                    .setEaseOutBounce()
                    .setOnComplete(() =>
                    {
                        // Ensure the uiPanel stays at the final scale after the animation completes
                        uiPanel.localScale = new Vector3(finalScale, finalScale, 1f);
                    });
            });

        StartCoroutine(DelayUICheck());
    }

    IEnumerator DelayUICheck()
    {
        yield return new WaitForSeconds(1f);

        // Additional safety check to make sure the scale is correctly set after the tween
        if (uiPanel.localScale.x == 0 || uiPanel.localScale.y == 0) {
            uiPanel.localScale = Vector3.one;
        }
    }
}
