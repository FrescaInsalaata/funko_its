using UnityEngine;

public class InteractableDoorBehaviour : MonoBehaviour
{
    public Vector3 hingeOffset = new Vector3(-0.5f, 0, 0); // hotfix perchè le pivot son state fatte col culo di un macaco
    public float openAngle = 90f;
    private float currentAngle = 0f;
    public float speed = 2f;
    private bool isOpen = false;

    void Update()
    {
        float targetAngle = isOpen ? openAngle : 0f;
        float step = speed * Time.deltaTime;
        float angleToRotate = Mathf.MoveTowards(currentAngle, targetAngle, step) - currentAngle;

        Vector3 hingePoint = transform.position + hingeOffset;
        transform.RotateAround(hingePoint, Vector3.up, angleToRotate);

        currentAngle += angleToRotate;
    }

    public void ToggleOpen()
    {
        Debug.Log($"Toggling " + this.name + " door state");
        isOpen = !isOpen;
    }
}
