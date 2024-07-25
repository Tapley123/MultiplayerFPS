using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using System.Linq;
using NaughtyAttributes;
using TMPro;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    PlayerManager playerManager;
    [SerializeField] private PhotonView pv;
    [SerializeField] private CursorController cursorController;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject cameraHolder;
    public Camera cam;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float smoothTime;
    [SerializeField] private float maxLookUpDownAngle = 90f;

    //UI
    [SerializeField] private GameObject playerUI;
    [SerializeField] private Image image_HealthBar;
    [SerializeField] private TMP_Text text_Username;

    //Killbox
    [SerializeField] [NaughtyAttributes.Tag] private string killboxTag;

    //moving locals
    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    //items
    [SerializeField] private Item[] items;
    [SerializeField] private Transform itemHolder;
    int itemIndex;
    int previousItemIndex = -1;

    //health
    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    void Awake()
    {
        //online
        if (PhotonNetwork.IsConnected)
        {
            playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();
        }
        //offline
        else
        {
            playerManager = GameObject.FindObjectOfType<PlayerManager>();
        }
    }

    void Start()
    {
        //online
        if (PhotonNetwork.IsConnected) 
        {
            Debug.Log($"ONLINE");

            //set the username text to its owners username
            text_Username.text = pv.Owner.NickName;

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
                Destroy(rb);
                //remove the players UI
                Destroy(playerUI);
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

        if(cursorController.locked)
            Look();

        Move();
        Jump();
        SwapItemInput();
        UseItemInput();
    }

    private void FixedUpdate()
    {
        //if you are online and you dont own this player do nothing!
        if (PhotonNetwork.IsConnected && !pv.IsMine) { return; }

        //move the player
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount * Time.fixedDeltaTime));
    }

    void Initialize()
    {
        Debug.Log($"Initialize");
        //Get the items here 
        //itemHolder;

        //if there is at least 1 item
        if(items.Length > 0)
        {
            //disable all items
            for (int i = 0; i < items.Length; i++)
            {
                items[i].itemGameobject.SetActive(false);
            }

            //equip the base item
            EquipItem(0);
        }

        //disable my own username
        Destroy(text_Username.transform.parent.gameObject);
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
            rb.AddForce(transform.up * jumpForce);
        }
    }

    void Look()
    {
        //look left and right
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        //look up and down
        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -maxLookUpDownAngle, maxLookUpDownAngle);
        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }
    #endregion

    #region Items
    void EquipItem(int _index)
    {
        //stop you from equiping what you have already equiped
        if (_index == previousItemIndex) { return; }

        //set the item index
        itemIndex = _index;

        //turn on this current items game object
        items[itemIndex].itemGameobject.SetActive(true);

        //has held an item before this one
        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameobject.SetActive(false);
        }

        //store this as the previous items index so when it is called again this can be rereferenced
        previousItemIndex = itemIndex;

        //Online and mine
        if(PhotonNetwork.IsConnected && pv.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    void SwapItemInput()
    {
        //if there is at least 1 item
        if (items.Length > 0)
        {
            //loop through all of the items
            for (int i = 0; i < items.Length; i++)
            {
                //if I press a number key that corosponds to a speific item (item1 = numkey1, item2 = numkey2 etc.)
                if (Input.GetKeyDown((i + 1).ToString()))
                {
                    //equip the item of the number you pressed
                    EquipItem(i);
                    break;
                }
            }

            //scroll up
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                //on the last item
                if (itemIndex >= items.Length - 1)
                {
                    //loop back to first item
                    EquipItem(0);
                }
                //not on last item
                else
                {
                    //go forward 1 item
                    EquipItem(itemIndex + 1);
                }
            }
            //scroll down
            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                //on the first item
                if (itemIndex <= 0)
                {
                    //Equip the last item on the list
                    EquipItem(items.Length - 1);
                }
                //not on the first item
                else
                {
                    //Go back 1 item
                    EquipItem(itemIndex - 1);
                }
            }
        }
    }

    void UseItemInput()
    {
        //if you click the mouse
        if(Input.GetMouseButtonDown(0))
        {
            //use the current equipped item
            items[itemIndex].Use();
        }
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

        image_HealthBar.fillAmount = healthNormalized;

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
            EquipItem((int)changedProps["itemIndex"]);
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
