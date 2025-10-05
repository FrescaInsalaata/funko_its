using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyCore : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 3f;
    public float detectionRange = 15f;
    private float timerChase = 0f;
    [HideInInspector] public bool isFighting = false;
    [HideInInspector] public Transform targetPlayer;
    [HideInInspector] public NavMeshAgent agent;

    void Awake()
    {
        // Set up NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        Debug.Log("GameObject name: " + gameObject.name);

        if (gameObject.name.Contains("BossEnemyFBX"))
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
            Debug.Log("Renderers found: " + renderers.Length);

            foreach (Renderer r in renderers)
            {
                foreach (Material mat in r.materials)
                {
                    // Only change the HAIR material
                    if (mat.name.Contains("Material.001"))
                    {
                        if (mat.HasProperty("_BaseColor"))
                        {
                            mat.SetColor("_BaseColor", Color.red);      // URP/Lit property
                        }
                        else if (mat.HasProperty("_Color"))
                        {
                            mat.color = Color.red;                      // Fallback for legacy shaders
                        }

                        Debug.Log($"Hair material changed: {mat.name}");
                    }
                }
            }
        }
    }



    void Update()
    {
        timerChase += Time.deltaTime;
        if(timerChase >= 0.35f)
        {
            timerChase = 0f;
            FindClosestPlayer();
        }
    }

    void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float minDistance = Mathf.Infinity;
        targetPlayer = null;

        foreach (GameObject player in players)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                targetPlayer = player.transform;
            }
        }
        if (minDistance > detectionRange)
        {
            isFighting = false;
        }
        else
        {
            isFighting = true;
        }
    }
}
