using UnityEngine;

public class RandomSwingLight : MonoBehaviour
{
    [Header("References")]
    public Light discoLight;

    [Header("Settings")]
    public float maxSwingAngle = 30f;      // Maximum swing from center
    public float swingSpeed = 2f;          // How fast the light moves to the target angle
    public float intensityMin = 100f;
    public float intensityMax = 120f;
    public float pulseSpeed = 5f;

    [Header("Optional")]
    public float colorChangeSpeed = 5f;    // Color cycling speed

    private Quaternion baseRotation;
    private float targetAngle;
    private float currentAngle;

    private void Start()
    {
        baseRotation = transform.localRotation;
        currentAngle = 0f;
        targetAngle = Random.Range(-maxSwingAngle, maxSwingAngle);
    }

    private void Update()
    {
        if (discoLight == null) return;

        // --- Color cycling ---
        float hue = Mathf.PingPong(Time.time * colorChangeSpeed, 1f);
        discoLight.color = Color.HSVToRGB(hue, 1f, 1f);

        // --- Random swing ---
        // Smoothly move towards targetAngle
        currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * swingSpeed);
        transform.localRotation = baseRotation * Quaternion.Euler(currentAngle, 0f, 0f);

        // If close enough to target, pick a new random target
        if (Mathf.Abs(currentAngle - targetAngle) < 0.5f)
        {
            targetAngle = Random.Range(-maxSwingAngle, maxSwingAngle);
        }

        // --- Pulse intensity ---
        discoLight.intensity = Mathf.Lerp(intensityMin, intensityMax,
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
    }
}