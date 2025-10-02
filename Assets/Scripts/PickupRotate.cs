using UnityEngine;
using UnityEngine.UIElements;

public class PickupRotate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material.color = Color.yellow;
        }
        else
        {
            MeshRenderer childMeshRenderer = GetComponentInChildren<MeshRenderer>();
            if (childMeshRenderer != null)
            {
                childMeshRenderer.material.color = Color.yellow;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Rotation();
    }
    private void Rotation()
    {
        transform.Rotate(new Vector3(0, 50, 0) * Time.deltaTime);
    }
}
