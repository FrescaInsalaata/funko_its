using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviour : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private Vector2 moveInput;

    [Header("Weapon")]
    public WeaponData currentWeapon;
    public Transform handMount;
    private Transform currentMuzzle;
    private GameObject weaponInstance;

    private InputAction moveAction;
    private InputAction fireAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (moveAction == null)
            moveAction = InputSystem.actions.FindAction("Move");
        if (fireAction == null)
            fireAction = InputSystem.actions.FindAction("Fire1");
    }

    private void OnEnable()
    {
        moveAction.Enable();
        fireAction.Enable();
        fireAction.performed += OnFire;
    }

    private void OnDisable()
    {
        moveAction.Disable();
        fireAction.Disable();
        fireAction.performed -= OnFire;
    }

    private void OnFire(InputAction.CallbackContext ctx)
    {
        if (currentWeapon != null)
            currentWeapon.Fire();
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
        if (direction.magnitude > 1f)
            direction.Normalize();

        rb.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);

        // Optional: rotate player toward movement direction
        if (direction.sqrMagnitude > 0.01f)
            transform.forward = direction;
    }

    public void EquipWeapon(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;

        // Destroy old weapon
        if (weaponInstance != null)
            Destroy(weaponInstance);

        // Instantiate new weapon
        weaponInstance = Instantiate(currentWeapon.weaponPrefab, handMount.position, handMount.rotation, handMount);
        currentMuzzle = weaponInstance.transform.Find("Muzzle");

        if (currentMuzzle == null)
            Debug.LogWarning("Weapon prefab missing Muzzle transform!");

        Debug.Log("Equipped " + newWeapon.weaponName);
    }
}