using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public RectTransform fillBar;
    public float maxWidth = 300f;
    public float height = 30f;
    public CompleteHeadBob headBob;
    public float smoothSpeed = 10f;
    public float fadeSpeed = 2f;
    
    private CanvasGroup canvasGroup;
    private float targetAlpha = 1f;

    private void Start()
    {
        if (fillBar != null)
        {
            fillBar.sizeDelta = new Vector2(maxWidth, height);
        }

        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; // Start faded out
    }

    private void Update()
    {
        if (fillBar != null && headBob != null)
        {
            float staminaPercent = headBob.GetStaminaPercentage();
            float targetWidth = maxWidth * staminaPercent;
            
            // Update bar width
            Vector2 currentSize = fillBar.sizeDelta;
            currentSize.x = Mathf.Lerp(currentSize.x, targetWidth, Time.deltaTime * smoothSpeed);
            fillBar.sizeDelta = currentSize;

            // Handle visibility
            bool isFullStamina = staminaPercent >= 0.99f;
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);

            // Determine if bar should be visible
            if (isFullStamina && !isSprinting)
            {
                targetAlpha = 0f; // Fade out
            }
            else
            {
                targetAlpha = 1f; // Fade in
            }

            // Apply fade
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        }
    }
}