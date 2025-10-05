using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviour : MonoBehaviour
{
    [Header("Sounds")]
    public AudioClip healthSound;
    public AudioClip vibeCheckSound;
    public AudioClip pickupSound;
    public AudioClip footstepSound;

    [Header("Camera")]
    private CinemachineTargetGroup targetGroup;

    [Header("Player Colors")]
    public Color[] playerColors; // e.g., [0] = blue, [1] = red

    [Header("Movement")]
    public float baseMoveSpeed = 5f;
    public float moveSpeedMultiplier = 2f;
    private float moveSpeed;
    private bool isRunning;

    [Header("PickupWeaponDatas")]
    public WeaponData coltPistolWeaponData;
    public WeaponData ak47RifleWeaponData;
    public WeaponData revolverWeaponData;

    [Header("Animation")]
    private Animator animator;
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int FireHash = Animator.StringToHash("Fire");
    private static readonly int ReloadHash = Animator.StringToHash("Reload");
    private static readonly int DeathHash = Animator.StringToHash("Death");

    [Header("Weapon")]
    public WeaponData currentWeapon;
    private WeaponInstance myWeaponInstance;
    public GameObject handMount;
    private GameObject weaponInstance;
    public GameObject firePoint;

    [Header("Item")]
    public ItemData currentItem;
    public GameObject throwMount;
    private bool isVibeCheckActive;

    [Header("Camera")]
    public Camera mainCamera;
    public LayerMask groundMask;

    public PlayerInput playerInput;
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
    private InputDevice device => playerInput.currentControlScheme == "Gamepad" ? (InputDevice)Gamepad.current : Keyboard.current;
    public float angleCorrection = 30f; // tweak to match your camera
    public float spawnRadius = 1f; // radius around closest player to spawn
    public Transform defaultSpawnPoint; // assign your "PlayerSpawnPoint" here

    public int playerID;
    int index;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
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

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if (animator == null)
            Debug.LogWarning("Animator not assigned and not found in children. Assign it in the Inspector.");
        if (animator != null)
            animator.applyRootMotion = false;
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

    private void Start()
    {
        SpawnNearFriend();
        EquipWeapon(currentWeapon);
        ApplyPlayerColor();
        //find PlayerManager gameobject and call SpawnPlayerUI
        PlayerManager pm = FindAnyObjectByType<PlayerManager>();
        if (pm != null)
        {
            pm.SpawnPlayerUI(this.gameObject);
        }
        else
        {
            Debug.LogError("No PlayerManager found in the scene. Please add one.");
        }
        if (targetGroup == null)
        {
            targetGroup = FindFirstObjectByType<CinemachineTargetGroup>();
            if (targetGroup == null)
            {
                Debug.LogError("No CinemachineTargetGroup found in the scene. Please add one.");
                return;
            }
            targetGroup.AddMember(this.transform, 1f, 1f);
        }
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Movement();
    }
    private void LateUpdate()
    {
        Look();
    }
    private void ApplyPlayerColor()
    {
        index = Mathf.Clamp(playerInput.playerIndex, 0, playerColors.Length - 1);
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            rend.material = new Material(rend.material); // Instance material
            rend.material.color = playerColors[index];
        }
    }
    private void SpawnNearFriend()
    {
        Transform closestPlayer = FindClosestPlayer(playerInput.transform);
        Vector3 spawnPosition;

        if (closestPlayer != null)
        {
            // Spawn near the closest player
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle.normalized * spawnRadius;
            spawnPosition = closestPlayer.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            // Optional collision check
            if (Physics.CheckSphere(spawnPosition, 0.5f))
            {
                spawnPosition += new Vector3(spawnRadius, 0, spawnRadius);
            }

            playerInput.transform.position = spawnPosition;
            playerInput.transform.rotation = Quaternion.identity;
        }       
    }
    private void Movement()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
        if (direction.magnitude > 1f)
        {
            direction.Normalize();
        }
        else if (direction.magnitude < 0.1f)
        {
            animator.SetBool(IsMovingHash, false);
            return;
        }
        rb.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);
        animator.SetBool(IsMovingHash, true);
        /*if (!GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().clip = footstepSound;
            GetComponent<AudioSource>().Play();
        }*/
    }

    private void Look()
    {
        float rotationSpeed = 10f;
        if (device is Gamepad && lookInput != Vector2.zero)
        {
            lookDir = new Vector3(lookInput.x, 0f, lookInput.y);
            lastLookDirection = lookDir;
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else if (device is Keyboard || device is Mouse)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, groundMask))
            {
                Vector3 direction = hit.point - transform.position;
                direction.y = 0;

                // Apply small rotation around Y to compensate
                direction = Quaternion.Euler(0, angleCorrection, 0) * direction;

                if (direction.magnitude > 0.1f)
                {
                    lastLookDirection = direction;
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pickup")
        {
            nearbyPickup = other.GetComponent<PickupBehaviour>();
            Debug.Log("Pickup found: " + other.name);
            if (nearbyPickup.itemName.ToLower() == "healthpickup")
            {
                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
                audioSource.clip = healthSound;
                audioSource.Play();
                Health playerHealth = GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.Heal(50);
                    Destroy(nearbyPickup.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Pickup" && nearbyPickup != null)
        {
            nearbyPickup = null;
        }
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (PauseMenu.GameIsPaused) return; // Blocca interazione in pausa

        if (nearbyPickup != null)
        {
            if (nearbyPickup.weaponData != null)
            {
                Debug.Log("Picking up weapon: " + nearbyPickup.weaponData.weaponName);
                EquipWeapon(nearbyPickup.weaponData);
                Destroy(nearbyPickup.gameObject);
                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
                audioSource.clip = pickupSound;
                audioSource.Play();
            }
            else if (nearbyPickup.itemData != null)
            {
                Debug.Log("Picking up item: " + nearbyPickup.itemData.itemName);
                EquipItem(nearbyPickup.itemData);
                Destroy(nearbyPickup.gameObject);
                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
                audioSource.clip = pickupSound;
                audioSource.Play();
            }
            return;
        }
    }

    private void OnUseItem(InputAction.CallbackContext ctx)
    {
        if (PauseMenu.GameIsPaused) return; // Blocca uso item in pausa

        if (currentItem != null)
        {
            ApplyVibeCheck(currentItem.multSpeedBoost, currentItem.duration);
        }
        else
        {
            Debug.LogWarning("Tried to use item, but no item assigned!");
        }
    }

    private void OnFire(InputAction.CallbackContext ctx)
    {
        if (PauseMenu.GameIsPaused) return; // Blocca lo sparo in pausa

        if (myWeaponInstance != null)
        {
            myWeaponInstance.Fire(firePoint, playerID, playerColors[index], isVibeCheckActive);
            if (animator != null)
            {
                animator.SetTrigger(FireHash);
            }
        }
    }

    private void OnReload(InputAction.CallbackContext ctx)
    {
        if (PauseMenu.GameIsPaused) return; // Blocca reload in pausa

        if (myWeaponInstance != null)
        {
            myWeaponInstance.Reload(this, playerID);
            if (animator != null)
            {
                animator.SetTrigger(ReloadHash);
            }
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
        weaponInstance.transform.localScale = currentWeapon.weaponPrefab.transform.localScale;

        myWeaponInstance = new WeaponInstance(currentWeapon);

        // Aggiorna la UI
        UIManager.Instance.updateAmmo(playerID, Mathf.RoundToInt(myWeaponInstance.currentAmmo));
        Debug.Log("Equipped weapon: " + currentWeapon.weaponName + "Current Ammo: " + myWeaponInstance.currentAmmo);

        // Trova il firePoint nella nuova arma
        firePoint = weaponInstance.transform.Find("FirePoint")?.gameObject;
        if (firePoint == null)
        {
            Debug.LogError("FirePoint non trovato nell'arma: " + weaponInstance.name);
        } else
        {
            Debug.Log("FirePoint trovato: " + firePoint.name);
        }
        ApplyPlayerColor();
    }

    public void EquipItem(ItemData newItem)
    {
        currentItem = newItem;
    }

    public void ApplyVibeCheck(float multSpeedBoost, float duration)
    {
        currentItem = null;
        moveSpeed *= multSpeedBoost;
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = vibeCheckSound;
        audioSource.Play();
        Debug.Log("VibeCheck applied! New moveSpeed: " + moveSpeed);
        isVibeCheckActive = true;
        // Start a coroutine to reset the speed after the duration
        StartCoroutine(ResetSpeedAfterDelay(duration, multSpeedBoost));
    }

    private IEnumerator ResetSpeedAfterDelay(float delay, float multSpeedBoost)
    {
        yield return new WaitForSeconds(delay);
        moveSpeed /= multSpeedBoost;
        isVibeCheckActive = false;
        Debug.Log("VibeCheck ended. MoveSpeed reset to: " + moveSpeed);
    }

    private Transform FindClosestPlayer(Transform target)
    {
        PlayerInput[] players = FindObjectsOfType<PlayerInput>();
        Transform closest = null;
        float minDistance = float.MaxValue;

        foreach (PlayerInput player in players)
        {
            if (player.transform == target) continue; // skip the new player
            float distance = Vector3.Distance(target.position, player.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = player.transform;
            }
        }

        return closest;
    }
}
