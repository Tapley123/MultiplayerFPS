using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    PlayerManager playerManager;
    public PlayerRefrences playerRefrences;
    [SerializeField] private PhotonView pv;
    [SerializeField] private Transform headBone;

    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float smoothTime;
    [SerializeField] private float maxLookUpDownAngle = 90f;

    //Killbox
    [SerializeField][NaughtyAttributes.Tag] private string killboxTag;

    //moving locals
    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    //weapons
    [ReadOnly][SerializeField] private int currentWeaponIndex = -1;
    [SerializeField] private List<GameObject> weapons = new List<GameObject>();

    //health
    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    //crouching
    [ReadOnly] public bool crouching = false;
    private bool moveToNewCrouchPos = false;
    private Transform currentBodyTarget;
    public float crouchSpeed = 5f; // The speed at which to move

    //Animation
    public Animator playerAnimator;
    [AnimatorParam("playerAnimator")] public string anim_Direction;
    [AnimatorParam("playerAnimator")] public string anim_Speed;
    [AnimatorParam("playerAnimator")] public string anim_Moving;
    [AnimatorParam("playerAnimator")] public string anim_MovingBackwards;
    [AnimatorParam("playerAnimator")] public string anim_Grounded;
    [ReadOnly] [SerializeField] private Vector3 moveDirection;
    [ReadOnly] [SerializeField] private float currentSpeed;
    [ReadOnly] [SerializeField] private bool moving;
    [ReadOnly] [SerializeField] private bool movingBackwards;


    private void OnEnable()
    {
        if (PhotonNetwork.IsConnected && !pv.IsMine) { return; }

        PlayerInput.toggleCrouchInput += ToggleCrouch;
    }

    private void OnDisable()
    {
        if (PhotonNetwork.IsConnected && !pv.IsMine) { return; }

        PlayerInput.toggleCrouchInput -= ToggleCrouch;
    }

    void Awake()
    {
        //online
        if (PhotonNetwork.IsConnected)
        {
            //get the player manager
            playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();

            //only do this if the player belongs to me
            if(pv.IsMine)
            {
                //scale the head down making it invisible for the local player
                headBone.localScale = Vector3.zero;
            }
        }
        //offline
        else
        {
            //get the player manager
            playerManager = GameObject.FindObjectOfType<PlayerManager>();
            //scale the head down making it invisible for the local player
            headBone.localScale = Vector3.zero;
        }
    }

    void Start()
    {
        PlayerInput.swapWeaponInput += NextWeapon;
        NextWeapon();

        //online
        if (PhotonNetwork.IsConnected) 
        {
            Debug.Log($"ONLINE");

            //set the username text to its owners username
            playerRefrences.text_Username.text = pv.Owner.NickName;

            //online do for me
            if (pv.IsMine)
            {
                Debug.Log($"ME");
                Initialize();
            }
            //do for everyone but me
            else
            {
                Debug.Log($"NOT ME");

                //remove camera
                Destroy(GetComponentInChildren<Camera>().gameObject);
                //remove rigidbody
                Destroy(playerRefrences.rb);
                //remove the players UI
                Destroy(playerRefrences.playerUI);
            }
        }
        //offline
        else
        {
            Debug.Log($"OFFLINE");
            Initialize();
        }
    }

    void Update()
    {
        //if you are online and you dont own this player do nothing!
        if (PhotonNetwork.IsConnected && !pv.IsMine) { return; }

        Jump();
        Animate();

        if (moveToNewCrouchPos)
            MoveCrouch();

        //dont do anything below this if you are paused
        if (PauseManager.Instance.paused) { return; }

        Look();
        Move();
        

        //SwapItemInput();
        //UseItemInput();
    }

    private void FixedUpdate()
    {
        //if you are online and you dont own this player do nothing!
        if (PhotonNetwork.IsConnected && !pv.IsMine) { return; }

        //move the player
        playerRefrences.rb.MovePosition(playerRefrences.rb.position + transform.TransformDirection(moveAmount * Time.fixedDeltaTime));
    }

    void Initialize()
    {
        Debug.Log($"Initialize");
        //disable my own username
        Destroy(playerRefrences.text_Username.transform.parent.gameObject);
    }

    #region Weapons
    public void NextWeapon()
    {
        //can go up
        if(currentWeaponIndex < weapons.Count -1)
            currentWeaponIndex++;
        //go back to start
        else
            currentWeaponIndex = 0;

        EquipWeapon(currentWeaponIndex);
    }

    public void PreviousWeapon()
    {
        //can go up
        if (currentWeaponIndex > 0)
            currentWeaponIndex--;
        //go back to start
        else
            currentWeaponIndex = weapons.Count-1;

        EquipWeapon(currentWeaponIndex);
    }

    void EquipWeapon(int index)
    {
        //disable all the weapons
        foreach (GameObject go in weapons)
        {
            go.SetActive(false);
        }

        //turn on specific weapon
        weapons[index].SetActive(true);

        //Online and mine
        if (PhotonNetwork.IsConnected && pv.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", index);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        //Weapoin has a gun controller
        if(weapons[index].GetComponent<GunController>())
        {
            GunController gunC = weapons[index].GetComponent<GunController>();
            
            if(playerRefrences.handTransitioner != null)
            {
                //move the left hand to its grab point
                playerRefrences.handTransitioner.SetLeftHandTarget(gunC.grabPointLeft);

                //move the right hand to its grab point
                playerRefrences.handTransitioner.SetRightHandTarget(gunC.grabPointRight);
            }
        }
    }
    #endregion

    #region Movement + Looking
    private void Move()
    {
        //walk/sprint
        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDirection * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);

        // Determine speed
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = isRunning ? sprintSpeed : walkSpeed;

        //moving
        if (moveDirection != Vector3.zero)
        {
            moving = true;

            //moving backwards
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                movingBackwards = true;
            }
            //moving forwards
            else
            {
                movingBackwards = false;
            }
        }
        //not moving
        else
        {
            moving = false;
            movingBackwards = false;
        }
    }

    void Jump()
    {
        //jumping
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            playerRefrences.rb.AddForce(transform.up * jumpForce);
            grounded = false;
            playerAnimator.SetBool(anim_Grounded, grounded);
        }
    }

    void ToggleCrouch()
    {
        //Uncrouch
        if (crouching)
        {
            Debug.Log("Un-Crouch");
            //set the ik target to be standing
            currentBodyTarget = playerRefrences.standingIkRef;
        }
        //Crouch
        else
        {
            Debug.Log("Crouch");
            //set the ik target to be crouching
            currentBodyTarget = playerRefrences.crouchingIkRef;
        }

        // start lerping to the new crouch position
        moveToNewCrouchPos = true;

        //swap the current crouching state
        crouching = !crouching;
    }

    void MoveCrouch()
    {
        // Calculate the distance to the target
        float distance = Vector3.Distance(playerRefrences.bodyIkTarget.position, currentBodyTarget.position);

        // Check if the transform is near the target
        if (distance > 0.1f)
        {
            // Move towards the target
            playerRefrences.bodyIkTarget.position = Vector3.Lerp(playerRefrences.bodyIkTarget.position, currentBodyTarget.position, crouchSpeed * Time.deltaTime);
        }
        else
        {
            // Stop lerping
            moveToNewCrouchPos = false;
        }
    }

    void Look()
    {
        //look left and right
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        //look up and down
        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -maxLookUpDownAngle, maxLookUpDownAngle);
        playerRefrences.cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }
    #endregion

    #region Animation
    void Animate()
    {
        // Set animation parameters
        playerAnimator.SetFloat(anim_Speed, moveDirection.magnitude * currentSpeed);
        playerAnimator.SetFloat(anim_Direction, Vector3.SignedAngle(transform.forward, moveDirection, Vector3.up));
        playerAnimator.SetBool(anim_Moving, moving);
        playerAnimator.SetBool(anim_MovingBackwards, movingBackwards);
        playerAnimator.SetBool(anim_Grounded, grounded);
    }
    #endregion

    #region Take Damage
    public void TakeDamage(float damage)
    {
        //Online
        if (PhotonNetwork.IsConnected)
        {
            pv.RPC(nameof(RPC_TakeDamage), pv.Owner, damage);
        }
        //Offline
        else
        {
            RPC_TakeDamage(damage);
        }
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info = new PhotonMessageInfo())
    {
        Debug.Log($"Took {damage} Damage");

        //subtract the health
        currentHealth -= damage;

        //calculate the health between 0-1
        float healthNormalized = Mathf.Clamp01(currentHealth / maxHealth);

        playerRefrences.image_HealthBar.fillAmount = healthNormalized;

        //dead
        if (currentHealth <= 0)
        {
            Die();

            //if you are online
            if (PhotonNetwork.IsConnected)
            {
                //find the player manager that killed you and send them that killed you
                PlayerManager.Find(info.Sender).GetKill();
            }
        }
    }

    void Die()
    {
        Debug.Log($"Die");

        playerManager.Die();
    }
    #endregion

    #region PhotonCallbacks
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        //NOT MINE, this is the script belonging to the player who made a change
        if(changedProps.ContainsKey("itemIndex") && !pv.IsMine && targetPlayer == pv.Owner)
        {
            //Show this player equip the correct item accross the network no RPC needed
            EquipWeapon((int)changedProps["itemIndex"]);
        }
    }
    #endregion

    #region Triggers
    private void OnTriggerEnter(Collider other)
    {
        //if online and not mine
        if (PhotonNetwork.IsConnected && !pv.IsMine) { return; }

        if (other.CompareTag(killboxTag))
        {
            Die();
        }
    }
    #endregion
}
