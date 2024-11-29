using UnityEngine;
using UnityEngine.UI;

public class UIPulsate : MonoBehaviour
{
    [SerializeField] private Image targetImage; // Assign the UI Image to pulsate
    [SerializeField] private float minScale = 0.8f; // Minimum scale for pulsation
    [SerializeField] private float maxScale = 1.2f; // Maximum scale for pulsation
    [SerializeField] private float pulseSpeed = 1.0f; // Speed of the pulsation

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = targetImage.GetComponent<RectTransform>();
    }

    private void Update()
    {
        // Calculate a pulsing scale value using PingPong
        float scale = Mathf.Lerp(minScale, maxScale, Mathf.PingPong(Time.time * pulseSpeed, 1));
        rectTransform.localScale = new Vector3(scale, scale, scale);
    }
}
