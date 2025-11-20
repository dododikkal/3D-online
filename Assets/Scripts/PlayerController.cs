using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    [Header("Hareket Ayarları")]
    public float walkSpeed = 5f;      // Normal yürüyüş hızı
    public float runSpeed = 9f;       // Shift'e basınca koşma hızı
    public float mouseSensitivity = 2f;
    
    [Header("Fizik & Ölüm")]
    public float gravity = 30f;
    public float fallThreshold = -15f;

    private CharacterController controller;
    private Camera playerCamera;
    private Animator animator; // Animasyoncu
    
    private float verticalRotation = 0f;
    private float verticalVelocity; 

    // UI & Sistem
    private GameObject deathScreenPanel;
    private Button restartButton;
    private Button quitButton;
    private bool isDead = false;
    private float spawnProtectionTimer = 1.0f;
    
    // Animasyon Senkronizasyonu
    private Vector3 lastPosition;
    private float currentAnimationSpeed; // Animator'a gönderilecek değer

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        // Animator'ı çocuk objelerde (Modelin içinde) ara
        animator = GetComponentInChildren<Animator>();

        if (IsOwner)
        {
            InitializeDeathScreen();
            MoveToSpawnPoint();
            LockCursor();
            spawnProtectionTimer = 1.0f;
        }
    }

    void InitializeDeathScreen()
    {
        GameObject canvas = GameObject.Find("Canvas"); 
        if (canvas != null)
        {
            Transform dsTransform = canvas.transform.Find("DeathScreen"); 
            if (dsTransform != null)
            {
                deathScreenPanel = dsTransform.gameObject;
                Transform restartBtnTr = deathScreenPanel.transform.Find("RestartButton");
                Transform quitBtnTr = deathScreenPanel.transform.Find("QuitButton");

                if (restartBtnTr != null) restartButton = restartBtnTr.GetComponent<Button>();
                if (quitBtnTr != null) quitButton = quitBtnTr.GetComponent<Button>();

                if (restartButton) restartButton.onClick.AddListener(Respawn);
                if (quitButton) quitButton.onClick.AddListener(QuitGame);
                deathScreenPanel.SetActive(false);
            }
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (deathScreenPanel != null) deathScreenPanel.SetActive(true);
        
        // Ölünce animasyonu anında kes (Idle'a dön)
        if(animator) animator.SetFloat("Speed", 0);
    }

    void Respawn()
    {
        isDead = false;
        if (deathScreenPanel != null) deathScreenPanel.SetActive(false);
        MoveToSpawnPoint();
        spawnProtectionTimer = 1.0f; 
        LockCursor();
    }

    void QuitGame()
    {
        Application.Quit();
    }

    private void MoveToSpawnPoint()
    {
        CharacterController cc = GetComponent<CharacterController>();
        cc.enabled = false; 
        GameObject spawnPoint = IsHost ? GameObject.Find("SpawnPos_Host") : GameObject.Find("SpawnPos_Client");
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;
            verticalVelocity = 0f; 
        }
        cc.enabled = true; 
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        animator = GetComponentInChildren<Animator>();
        lastPosition = transform.position;
    }

    void Update()
    {
        // 1. ANİMASYONU GÜNCELLE (Herkes Görsün)
        UpdateAnimation();

        // 2. KORUMA VE ÖLÜM KONTROLÜ
        if (!IsOwner) return;

        if (spawnProtectionTimer > 0)
        {
            spawnProtectionTimer -= Time.deltaTime;
            return; 
        }
        
        if (isDead) return;

        if (Input.GetMouseButtonDown(0)) LockCursor();

        // 3. KAMERA
        float horizontalRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, horizontalRotation, 0);
        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        if (playerCamera != null) playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        // 4. HAREKET VE HIZ (Shift ile Koşma)
        if (controller.isGrounded && verticalVelocity < 0) verticalVelocity = -2f;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        
        // Shift'e basılıysa RunSpeed, değilse WalkSpeed kullan
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        
        // Karakteri yürüt
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Yerçekimi
        verticalVelocity -= gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);

        if (transform.position.y < fallThreshold) Die();
    }

    // --- BURASI HIZLANINCA ANİMASYONU ÇALIŞTIRAN YER ---
    void UpdateAnimation()
    {
        if (animator == null) return;

        float targetValue = 0f;

        if (IsOwner)
        {
            // Bizim ekranımızda: Tuşlara basıyorsak (WASD) animasyon değeri 1 olsun
            Vector3 inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            
            // Eğer hareket tuşuna basıyorsa 1 (Koş), basmıyorsa 0 (Dur)
            if (inputDir.magnitude > 0.1f) 
            {
                targetValue = 1f; 
            }
            else 
            {
                targetValue = 0f;
            }
        }
        else
        {
            // Arkadaşımızın ekranında: Gerçekten yer değiştiriyorsa animasyon oynasın
            float dist = Vector3.Distance(transform.position, lastPosition);
            float velocity = dist / Time.deltaTime;
            
            // Eğer hızı 0.1'den büyükse koşuyordur
            targetValue = velocity > 0.1f ? 1f : 0f;
            
            lastPosition = transform.position;
        }

        // Değeri yumuşakça değiştir (Anında küt diye geçmesin)
        currentAnimationSpeed = Mathf.Lerp(currentAnimationSpeed, targetValue, Time.deltaTime * 10f);
        
        // Animator'daki "Speed" parametresine bu değeri gönder
        animator.SetFloat("Speed", currentAnimationSpeed);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}