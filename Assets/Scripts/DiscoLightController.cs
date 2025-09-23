using UnityEngine;

public class DiscoLightController : MonoBehaviour
{
    public Light discoLight;            
    public float switchInterval = 2f;   
    private float timer = 0f;
    private int colorIndex = 0;

    // Colori primari da ciclare
    private Color[] colors = { Color.red, Color.green, Color.blue };

    void Start()
    {
        if (discoLight == null)
        {
            discoLight = GetComponent<Light>(); 
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= switchInterval)
        {
            timer = 0f;
            colorIndex = (colorIndex + 1) % colors.Length; 
            discoLight.color = colors[colorIndex];         
        }
    }
}

