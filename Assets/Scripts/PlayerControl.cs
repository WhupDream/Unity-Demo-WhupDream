using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerControl : MonoBehaviour
{
    public float jumpForce;
    public float probeLen;
    public bool grounded; // Only public for debugging purposes
    public LayerMask whatIsGround;
    public float walkSpeed;
    public float maxWalk;
    public float turnSpeed;
    private Vector2 moveInput;
    private Vector2 rotateInput;
    private Rigidbody rigi;
    private IA_PlayerControls ctrl;
    private int keyCount;
    private int coinCount;
    public int minKeys;
    public TextMeshProUGUI displayKeys;
    public TextMeshProUGUI displayCoins;
    public TextMeshProUGUI displayTime;
    public TextMeshProUGUI displayAmmo;
    public float timer;
    public bool isAlive;
    public GameObject restartButton;
    public GameObject projectile;
    public float spawnDistance;
    public int ammo;
    public int refillAmount;

    void jump(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (grounded)
        {
            rigi.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void fire(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (ammo > 0 && isAlive)
        {
            Instantiate(projectile, 
                this.transform.position + this.transform.forward * spawnDistance,
                this.transform.rotation);
            ammo--;
            UpdateAmmo();
        }
    }

    void UpdateKeys()
    {
        displayKeys.text = keyCount.ToString("00") + " / " 
                                 + minKeys.ToString("00") + " Keys";
    }

    void UpdateCoins()
    {
        displayCoins.text = coinCount.ToString("00") + " Coins";
    }

    void UpdateTimer()
    {
        displayTime.text = timer.ToString("000.00");
    }

    void UpdateAmmo()
    {
        displayAmmo.text = ammo.ToString("00") + " Ammo";
    }
    
    /* Awake is called when the object instantiates in a scene
     (When the game object wakes up in the scene)*/
    void Awake()
    {
        rigi = GetComponent<Rigidbody>();
        ctrl = new IA_PlayerControls();
        ctrl.Player.Jump.started += jump;
        ctrl.Player.Attack.started += fire;
        ctrl.Enable();
        keyCount = 0;
        UpdateKeys();
        UpdateCoins();
        UpdateAmmo();
        timer = 0f;
        UpdateTimer();
        isAlive = true;
        restartButton.SetActive(false);
    }

    private void OnDisable()
    {
        ctrl.Disable();
    }
    
    // FixedUpdate is called at a regular interval (50 per second)
    private void FixedUpdate()
    {
        if (isAlive)
        {
            grounded = Physics.Raycast(this.transform.position, Vector3.down,
                probeLen, whatIsGround);

            // read input from user
            moveInput = ctrl.Player.Move.ReadValue<Vector2>();
            rotateInput = ctrl.Player.Rotate.ReadValue<Vector2>();

            timer += Time.deltaTime;
            UpdateTimer();

            if (rotateInput.magnitude > 0.1f)
            {
                Vector3 angleVelocity = new Vector3(0f, rotateInput.x * turnSpeed, 0f);
                Quaternion deltaRot = Quaternion.Euler(angleVelocity * Time.deltaTime);
                rigi.MoveRotation(rigi.rotation * deltaRot);
            }

            // if movement input, put together vector and move player
            if (moveInput.magnitude > 0.1f)
            {
                Vector3 moveForward = moveInput.y * this.transform.forward;
                Vector3 moveRight = moveInput.x * this.transform.right;
                Vector3 moveVector = moveForward + moveRight;
                rigi.AddForce(moveVector * walkSpeed * Time.deltaTime);

                rigi.linearVelocity = Vector3.ClampMagnitude(rigi.linearVelocity, maxWalk);
            }
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.transform.tag == "Key")
        {
            keyCount++;
            //Debug.Log(keyCount);
            UpdateKeys();
            Destroy(other.gameObject);
        }
        else if (other.transform.tag == "Coin")
        {
            coinCount++;
            //Debug.Log(coinCount);
            UpdateCoins();
            Destroy(other.gameObject);
        }
        else if (other.transform.tag == "Refill")
        {
            ammo += refillAmount;
            Destroy(other.gameObject);
            UpdateAmmo();
        }
        else if (other.transform.tag == "Finish")
        {
            if (keyCount < minKeys)
            {
                Debug.Log("Collect more keys to exit");
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Enemy")
        {
            isAlive = false;
            restartButton.SetActive(true);
        }
    }
}   
