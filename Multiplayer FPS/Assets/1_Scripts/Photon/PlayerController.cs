using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PhotonView pv;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject cameraHolder;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float smoothTime;
    [SerializeField] private float maxLookUpDownAngle = 90f;

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

    void Start()
    {
        //online
        if (PhotonNetwork.IsConnected) 
        {
            //online do for me
            if (pv.IsMine)
            {
                InitializeMe();
            }
            //do for everyone but me
            else
            {
                //remove camera
                Destroy(GetComponentInChildren<Camera>().gameObject);
                //remove rigidbody
                Destroy(rb);
            }
        }
        //offline
        else
        {
            InitializeMe();
        }
    }

    void Update()
    {
        //if you are online and you dont own this player do nothing!
        if (PhotonNetwork.IsConnected && !pv.IsMine) { return; }

        Look();
        Move();
        Jump();
        SwapItemInput();
    }

    private void FixedUpdate()
    {
        //if you are online and you dont own this player do nothing!
        if (PhotonNetwork.IsConnected && !pv.IsMine) { return; }

        //move the player
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount * Time.fixedDeltaTime));
    }

    void InitializeMe()
    {
        Debug.Log($"Initialize me");
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
    #endregion
}
