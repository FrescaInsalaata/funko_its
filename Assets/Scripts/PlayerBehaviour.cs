using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviour : MonoBehaviour
{
    [Header("Movement")]
    public float baseMoveSpeed = 5f;
    public float moveSpeedMultiplier = 2f;
    private float moveSpeed;
    private bool isRunning;

    [Header("Weapon")]
    public WeaponData currentWeapon;
    private WeaponInstance myWeaponInstance;
    public GameObject handMount;
    private GameObject weaponInstance;
    public GameObject firePoint;

    [Header("Item")]
    public ItemData currentItem;
    public GameObject throwMount;

    [Header("Camera")]
    public Camera mainCamera;
    public LayerMask groundMask;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction attackAction;
    private InputAction interactAction;
    private InputAction useItemAction;
    private InputAction reloadAction;
    private InputAction runAction;

    private Rigidbody rb;
    private Vector2 moveInput;
    private InputAction lookAction;
    private Vector2 lookInput;
    Vector3 lookDir;
    private Vector3 lastLookDirection;
    private PickupBehaviour nearbyPickup;


    public int playerID;


    private void Start()
    {
        if (currentWeapon != null)
            EquipWeapon(currentWeapon);
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();
    }
    private void FixedUpdate()
    {
        Movement();
        Look();
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        attackAction = playerInput.actions["Attack"];
        interactAction = playerInput.actions["Interact"];
        useItemAction = playerInput.actions["UseItem"];
        reloadAction = playerInput.actions["Reload"];
        runAction = playerInput.actions["Run"];

        moveSpeed = baseMoveSpeed;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        attackAction.Enable();
        lookAction.Enable();
        interactAction.Enable();
        useItemAction.Enable();
        reloadAction.Enable();
        runAction.Enable();
        attackAction.performed += OnFire;
        interactAction.performed += OnInteract;
        useItemAction.performed += OnUseItem;
        reloadAction.performed += OnReload;
        runAction.performed += OnRun;
    }

    private void OnDisable()
    {
        moveAction.Disable();
        attackAction.Disable();
        lookAction.Disable();
        interactAction.Disable();
        useItemAction.Disable();
        reloadAction.Disable();
        attackAction.performed -= OnFire;
        interactAction.performed -= OnInteract;
        useItemAction.performed -= OnUseItem;
        reloadAction.performed -= OnReload;
        runAction.performed -= OnRun;
    }

    private void Movement()
    {
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
        if (direction.magnitude > 1f) direction.Normalize();

        float speed = isRunning ? baseMoveSpeed * moveSpeedMultiplier : baseMoveSpeed;
        rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);
    }
    private void Look()
    {
        lookDir = Vector3.zero;

        // Mouse first (if valid raycast)
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
        {
            Vector3 lookPos = hit.point;
            lookPos.y = transform.position.y;
            lookDir = (lookPos - transform.position).normalized;
        }
        // Otherwise fallback to stick
        else if (lookInput.sqrMagnitude > 0.01f)
        {
            lookDir = new Vector3(lookInput.x, 0f, lookInput.y).normalized;
        }

        if (lookDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        var pickup = other.GetComponent<PickupBehaviour>();
        if (pickup != null)
            nearbyPickup = pickup;
        if (pickup.itemName.ToLower() == "healthpickup")
        {
            Debug.Log("using health pickup...");
            Health playerHealth = GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Heal(50);
                Destroy(pickup.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var pickup = other.GetComponent<PickupBehaviour>();
        if (pickup != null && pickup == nearbyPickup)
            nearbyPickup = null;
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (nearbyPickup == null) return;

        if (nearbyPickup.weaponData != null)
        {
            Debug.Log("Picking up weapon: " + nearbyPickup.weaponData.weaponName);
            EquipWeapon(nearbyPickup.weaponData);
            Destroy(nearbyPickup.gameObject);
        }
        else if (nearbyPickup.itemData != null)
        {
            Debug.Log("Picking up item: " + nearbyPickup.itemData.itemName);
            EquipItem(nearbyPickup.itemData);
            Destroy(nearbyPickup.gameObject);
        }
    }
    private void OnUseItem(InputAction.CallbackContext ctx)
    {
        if (currentItem != null && currentItem.uses >= 0 && throwMount != null)
        {
            currentItem.UseItem(throwMount.transform);
        }
        else
        {
            Debug.LogWarning("Tried to use item, but no item or throw mount assigned!");
        }
    }
    private void OnFire(InputAction.CallbackContext ctx)
    {
        if (myWeaponInstance != null)
        {
            myWeaponInstance.Fire(firePoint, playerID);
        }
    }

    private void OnReload(InputAction.CallbackContext ctx)
    {
        if (myWeaponInstance != null)
        {
            myWeaponInstance.Reload(this, playerID);
        }
    }
    private void OnRun(InputAction.CallbackContext ctx)
    {
        isRunning = !isRunning;
    }
    public void EquipWeapon(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;

        if (weaponInstance != null)
            Destroy(weaponInstance);

        weaponInstance = Instantiate(currentWeapon.weaponPrefab, handMount.transform);
        weaponInstance.transform.localScale = Vector3.one;

        myWeaponInstance = new WeaponInstance(currentWeapon);

        // Aggiorna la UI
        UIManager.Instance.updateAmmo(playerID, Mathf.RoundToInt(myWeaponInstance.currentAmmo));

        // Trova il firePoint nella nuova arma
        firePoint = weaponInstance.transform.Find("FirePoint")?.gameObject;
        if (firePoint == null)
        {
            Debug.LogError("FirePoint non trovato nell'arma: " + weaponInstance.name);
        }
    }

    public void EquipItem(ItemData newItem)
    {
        currentItem = newItem;
    }
}