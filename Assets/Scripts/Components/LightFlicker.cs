using UnityEngine;

public class LightFlicker : MonoBehaviour
{
        [Header("Flicker Settings")]
        public float flickRate = 0.5f;
        public float originalIntensity = 1.0f;
        public float flickerAmount = 0.3f; // How much the light dims when flickering
        
        private Light lightComponent;
        private float flickerTimer;
        private bool isFlickering = false;
        
        void Start()
        {
            lightComponent = GetComponent<Light>();
            if (lightComponent == null)
            {
                Debug.LogWarning($"LightFlicker component on {gameObject.name} requires a Light component!");
                enabled = false;
                return;
            }
            
            originalIntensity = lightComponent.intensity;
        }
        
        void Update()
        {
            if (lightComponent == null) return;
            
            flickerTimer += Time.deltaTime;
            
            // Check if it's time to flicker based on flicker rate
            if (flickerTimer >= flickRate)
            {
                flickerTimer = 0f;
                isFlickering = !isFlickering; // Toggle flicker state
                
                if (isFlickering)
                {
                    // Dim the light
                    lightComponent.intensity = originalIntensity * (1f - flickerAmount);
                }
                else
                {
                    // Restore original intensity
                    lightComponent.intensity = originalIntensity;
                }
            }
        }
        
        // Allow runtime adjustment of flicker parameters
        public void SetFlickerRate(float newRate)
        {
            flickRate = newRate;
        }
        
        public void SetFlickerAmount(float amount)
        {
            flickerAmount = Mathf.Clamp01(amount);
        }
    }