using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    public InputAction move;
    public WeaponData currentWeapon;
    public Transform firePoint;
    private Rigidbody rb;
    public float moveSpeed = 5f;

    void Start()
    {
        move = InputSystem.actions.FindAction("Move");
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (Input.GetButton("Fire1") && currentWeapon != null)
        {
            currentWeapon.Fire();
        }
    }
    void FixedUpdate()
    {
        Vector2 movement = move.ReadValue<Vector2>();
        Vector3 direction = new Vector3(movement.x, 0, movement.y);
        rb.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);
    }
    public void EquipWeapon(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;
        Debug.Log("Equipped " + newWeapon.weaponName);
    }
}
