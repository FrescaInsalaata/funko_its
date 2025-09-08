using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    public InputAction move;
    public InputAction jump;

    private Rigidbody rb;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        move = InputSystem.actions.FindAction("Move");
        jump = InputSystem.actions.FindAction("Jump");
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        Vector2 movement = move.ReadValue<Vector2>();
        Vector3 direction = new Vector3(movement.x, 0, movement.y);

        rb.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);
    }
}
