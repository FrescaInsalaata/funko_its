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

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction attackAction;

    private void Start()
    {
        if (currentWeapon != null)
            EquipWeapon(currentWeapon);
        else
            Debug.LogWarning("No starting weapon assigned to player!");
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        attackAction = playerInput.actions["Attack"];
    }

    private void OnEnable()
    {
        moveAction.Enable();
        attackAction.Enable();
        attackAction.performed += OnFire;
    }

    private void OnDisable()
    {
        moveAction.Disable();
        attackAction.Disable();
        attackAction.performed -= OnFire;
    }
    private void OnFire(InputAction.CallbackContext ctx)
    {
        Debug.Log("Attack button pressed!");

        if (currentWeapon != null && currentMuzzle != null)
        {
            Debug.Log($"Firing {currentWeapon.weaponName} from {currentMuzzle.name}");
            currentWeapon.Fire(currentMuzzle);
        }
        else
        {
            Debug.LogWarning("Tried to attack, but no weapon or muzzle assigned!");
        }
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

        weaponInstance = Instantiate(currentWeapon.weaponPrefab, handMount);
        weaponInstance.transform.localPosition = Vector3.zero;
        weaponInstance.transform.localRotation = Quaternion.identity;
        weaponInstance.transform.localScale = Vector3.one;

        if (currentMuzzle == null)
            Debug.LogWarning("Weapon prefab missing Muzzle transform!");

        Debug.Log("Equipped " + newWeapon.weaponName);
    }
}