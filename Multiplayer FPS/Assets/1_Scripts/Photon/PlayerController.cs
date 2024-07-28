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

        if(!PauseManager.Instance.paused)
            Look();

        Move();
        Jump();

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
        //Get the items here 
        //itemHolder;

        ////if there is at least 1 item
        //if(items.Length > 0)
        //{
        //    //disable all items
        //    for (int i = 0; i < items.Length; i++)
        //    {
        //        items[i].itemGameobject.SetActive(false);
        //    }

        //    //equip the base item
        //    EquipItem(0);
        //}

        //disable my own username
        Destroy(playerRefrences.text_Username.transform.parent.gameObject);
    }

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
            
            //move the left hand to its grab point
            playerRefrences.handTransitioner.SetLeftHandTarget(gunC.grabPointLeft);

            //move the right hand to its grab point
            playerRefrences.handTransitioner.SetRightHandTarget(gunC.grabPointRight);
        }
    }

    #region Movement + Looking
    private void Move()
    {
        //walk/sprint
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }

    void Jump()
    {
        //jumping
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            playerRefrences.rb.AddForce(transform.up * jumpForce);
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
