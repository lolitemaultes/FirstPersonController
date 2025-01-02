using UnityEngine;
using UnityEngine.UI;
using StarterAssets; // Add this line to reference FirstPersonController

public class CompleteHeadBob : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private AudioSource breathingAudioSource;
    [SerializeField] private Image staminaBarFill;
    [SerializeField] private FirstPersonController playerMovement;  // Changed to FirstPersonController

    [Header("Movement Bob")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkVerticalBobAmount = 0.05f;
    [SerializeField] private float walkHorizontalBobAmount = 0.025f;
    [SerializeField] private float walkTiltAmount = 0.5f;
    [SerializeField] private float walkForwardBobAmount = 0.015f;
    
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintVerticalBobAmount = 0.1f;
    [SerializeField] private float sprintHorizontalBobAmount = 0.08f;
    [SerializeField] private float sprintTiltAmount = 1f;
    [SerializeField] private float sprintForwardBobAmount = 0.03f;

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 20f;
    [SerializeField] private float staminaRegenRate = 10f;
    [SerializeField] private float staminaRegenDelay = 1f;
    [SerializeField] private float exhaustionThreshold = 10f;
    [SerializeField] private float minStaminaToSprint = 5f;

    [Header("Breathing")]
    [SerializeField] private float idleBreathingSpeed = 2f;
    [SerializeField] private float idleBreathingAmount = 0.02f;
    [SerializeField] private float exhaustedBreathingSpeed = 4f;
    [SerializeField] private float exhaustedBreathingAmount = 0.04f;
    [SerializeField] private float breathingSmoothness = 2f;

    [Header("Movement Smoothing")]
    [SerializeField] private float positionSmoothness = 12f;
    [SerializeField] private float rotationSmoothness = 12f;
    [SerializeField] private float recoverySpeed = 2f;

    [Header("Footsteps")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip[] walkFootstepSounds;    // Array for walking sounds
    [SerializeField] private AudioClip[] sprintFootstepSounds;  // Array for sprinting sounds
    private int lastFootstepIndex = -1;
    private bool canPlayFootstep = true;

    private Vector3 defaultPosition;
    private Quaternion defaultRotation;
    private float movementTimer;
    private float breathingTimer;
    private float currentStamina;
    private float lastSprintTime;
    private bool isExhausted;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private float currentSpeed;
    private float speedSmoothVelocity;


void Start()
{
    defaultPosition = transform.localPosition;
    defaultRotation = transform.localRotation;
    currentStamina = maxStamina;
    targetPosition = defaultPosition;
    targetRotation = defaultRotation;

    // Initialize sprint speed to normal walking speed
    if (playerMovement != null)
    {
        playerMovement.SprintSpeed = 2f;  // Set default sprint speed
    }

    if (breathingAudioSource != null)
    {
        breathingAudioSource.loop = true;
        breathingAudioSource.volume = 0;
        breathingAudioSource.Play();
    }
}

    void Update()
    {
        HandleStamina();
        ApplyMovementBob();
        ApplyBreathing();
        UpdateTransform();
        UpdateUI();
        UpdateBreathingSound();
    }

    void ApplyMovementBob()
    {
        bool canSprint = !isExhausted && currentStamina > minStaminaToSprint && Input.GetKey(KeyCode.LeftShift);
        bool isMoving = controller.velocity.magnitude > 0.1f;
    
        if (isMoving)
        {
            float bobSpeed = canSprint ? sprintBobSpeed : walkBobSpeed;
            float verticalAmount = canSprint ? sprintVerticalBobAmount : walkVerticalBobAmount;
            float horizontalAmount = canSprint ? sprintHorizontalBobAmount : walkHorizontalBobAmount;
            float tiltAmount = canSprint ? sprintTiltAmount : walkTiltAmount;
            float forwardAmount = canSprint ? sprintForwardBobAmount : walkForwardBobAmount;
            
            movementTimer += Time.deltaTime * bobSpeed;
    
            // Calculate target position with figure-8 motion
            targetPosition = defaultPosition;
            targetPosition.y += Mathf.Sin(movementTimer) * verticalAmount;
            targetPosition.x += Mathf.Cos(movementTimer / 2) * horizontalAmount;
            targetPosition.z += Mathf.Sin(movementTimer * 2) * forwardAmount;
    
            // Adjusted footstep timing
            if (canPlayFootstep && Mathf.Sin(movementTimer) < -0.5f)
            {
            PlayRandomFootstep(canSprint && Input.GetKey(KeyCode.LeftShift));
            canPlayFootstep = false;
            }
            else if (Mathf.Sin(movementTimer) > 0)
            {
                canPlayFootstep = true;
            }
    
            // Calculate tilt with diagonal motion
            float tiltZ = Mathf.Cos(movementTimer / 2) * tiltAmount;
            float tiltX = Mathf.Sin(movementTimer / 2) * (tiltAmount * 0.5f);
            float tiltY = Mathf.Sin(movementTimer) * (tiltAmount * 0.25f);
            
            targetRotation = defaultRotation * Quaternion.Euler(tiltX, tiltY, tiltZ);
    
            // Add movement direction influence
            Vector3 moveDirection = controller.velocity.normalized;
            targetPosition += moveDirection * (forwardAmount * 0.1f) * Mathf.Sin(movementTimer * 2);
        }
        else
        {
            movementTimer = 0;
            canPlayFootstep = true;
            targetPosition = Vector3.Lerp(targetPosition, defaultPosition, Time.deltaTime * recoverySpeed);
            targetRotation = Quaternion.Lerp(targetRotation, defaultRotation, Time.deltaTime * recoverySpeed);
        }
    }

    void PlayRandomFootstep(bool isSprinting)
    {
        AudioClip[] currentSoundSet = isSprinting ? sprintFootstepSounds : walkFootstepSounds;
    
        if (footstepSource != null && currentSoundSet != null && currentSoundSet.Length > 0)
        {
            int newIndex;
            do
            {
                newIndex = Random.Range(0, currentSoundSet.Length);
            } while (newIndex == lastFootstepIndex && currentSoundSet.Length > 1);
    
            lastFootstepIndex = newIndex;
            footstepSource.PlayOneShot(currentSoundSet[newIndex]);
        }
    }

    void HandleStamina()
    {
        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift);
        bool isMoving = controller.velocity.magnitude > 0.1f;
        float staminaPercentage = currentStamina / maxStamina;
        
        if (wantsToSprint && isMoving)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            lastSprintTime = Time.time;
    
            if (playerMovement != null)
            {
                // Smoothly transition sprint speed between 2 and 5 based on stamina percentage
                if (staminaPercentage < 0.3f) // 30% stamina threshold
                {
                    float speedMultiplier = Mathf.Lerp(2f, 5f, staminaPercentage / 0.3f);
                    playerMovement.SprintSpeed = speedMultiplier;
                }
                else
                {
                    playerMovement.SprintSpeed = 5f; // Full sprint speed
                }
            }
        }
        else
        {
            if (Time.time - lastSprintTime >= staminaRegenDelay)
            {
                float regenMultiplier = controller.velocity.magnitude < 0.1f ? 1.5f : 1f;
                currentStamina += staminaRegenRate * regenMultiplier * Time.deltaTime;
            }
        }
    
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        isExhausted = currentStamina < exhaustionThreshold;
    }

    void ApplyBreathing()
    {
        float staminaPercent = currentStamina / maxStamina;
        float breathingIntensity = 0f;
    
        // Calculate breathing intensity based on stamina
        if (staminaPercent < 0.6f) // 60% stamina threshold
        {
            // Smoothly increase breathing intensity as stamina drops
            breathingIntensity = Mathf.Lerp(0f, 1f, (0.6f - staminaPercent) / 0.6f);
        }
    
        // Use breathingIntensity to affect breathing parameters
        float breathingSpeed = Mathf.Lerp(idleBreathingSpeed, exhaustedBreathingSpeed, breathingIntensity);
        float breathingAmount = Mathf.Lerp(0, exhaustedBreathingAmount, breathingIntensity);
    
        breathingTimer += Time.deltaTime * breathingSpeed;
    
        if (controller.velocity.magnitude < 0.1f || breathingIntensity > 0)
        {
            Vector3 breathingOffset = Vector3.zero;
            breathingOffset.y = Mathf.Sin(breathingTimer) * breathingAmount;
            breathingOffset.z = Mathf.Sin(breathingTimer) * (breathingAmount * 0.5f);
    
            targetPosition += breathingOffset;
        }
    
        // Handle breathing audio
        if (breathingAudioSource != null)
        {
            float targetVolume = breathingIntensity;
            breathingAudioSource.volume = Mathf.Lerp(breathingAudioSource.volume, targetVolume, Time.deltaTime * breathingSmoothness);
        }
    }

    void UpdateTransform()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * positionSmoothness);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * rotationSmoothness);
    }

    void UpdateUI()
    {
        if (staminaBarFill != null)
        {
            staminaBarFill.fillAmount = Mathf.Lerp(staminaBarFill.fillAmount, currentStamina / maxStamina, Time.deltaTime * 8f);
        }
    }

    void UpdateBreathingSound()
    {
        if (breathingAudioSource != null)
        {
            float targetVolume = isExhausted ? 
                Mathf.Lerp(0.3f, 1f, 1f - (currentStamina / exhaustionThreshold)) : 0;
            
            breathingAudioSource.volume = Mathf.Lerp(breathingAudioSource.volume, targetVolume, 
                Time.deltaTime * breathingSmoothness);
        }
    }

    public bool CanSprint()
    {
        return currentStamina > 0;
    }

    public float GetStaminaPercentage()
    {
        return currentStamina / maxStamina;
    }
}
