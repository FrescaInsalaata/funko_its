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

    private InputAction lookAction;
    private Vector2 lookInput;


    [Header("Camera")]
    public Camera mainCamera;
    public LayerMask groundMask; // layer del terreno o piano su cui vuoi proiettare il mouse


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
        lookAction = playerInput.actions["Look"];
        attackAction = playerInput.actions["Attack"];
    }

    private void OnEnable()
    {
        moveAction.Enable();
        attackAction.Enable();
        lookAction.Enable();
        attackAction.performed += OnFire;
    }

    private void OnDisable()
    {
        moveAction.Disable();
        attackAction.Disable();
        lookAction.Disable();
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
        lookInput = lookAction.ReadValue<Vector2>();
    }

    private Vector3 lastLookDirection;

    private void FixedUpdate()
    {
        // Movimento con stick sinistro
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
        if (direction.magnitude > 1f) direction.Normalize();
        rb.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);

        // ---- Rotazione ----
        Vector3 lookDir = Vector3.zero;

        // 1. Stick destro (priorità)
        if (lookInput.sqrMagnitude > 0.01f)
        {
            lookDir = new Vector3(lookInput.x, 0f, lookInput.y).normalized;
            lastLookDirection = lookDir; // aggiorniamo l'ultima direzione joystick valida
        }
        else if (lastLookDirection.sqrMagnitude > 0.01f)
        {
            // Se lo stick non è mosso, mantieni l'ultima direzione del joystick
            lookDir = lastLookDirection;
        }
        else
        {
            // 2. Mouse come fallback solo se lo stick non ha mai avuto input
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
            {
                Vector3 lookPos = hit.point;
                lookPos.y = transform.position.y;
                lookDir = (lookPos - transform.position).normalized;
            }
        }

        // Applica la rotazione
        if (lookDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f);
        }
    }



    public void EquipWeapon(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;

        // Destroy old weapon
        if (weaponInstance != null)
            Destroy(weaponInstance);


        // Instantiate new weapon
        weaponInstance = Instantiate(currentWeapon.weaponPrefab, handMount);
        currentMuzzle = weaponInstance.transform.Find("Muzzle");
        weaponInstance.transform.SetParent(handMount, worldPositionStays: true);
        weaponInstance.transform.localScale = Vector3.one;

        if (currentMuzzle == null)
            Debug.LogWarning("Weapon prefab missing Muzzle transform!");

        Debug.Log("Equipped " + newWeapon.weaponName);
    }
}