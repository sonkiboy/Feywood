using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Object and Component Refrences

    GameObject model;
    Rigidbody rb;
    Transform groundCheck;
    Vector3 groundCheckDimentions = new Vector3(.2f, .2f, .2f);

    Transform heldItemPos;
    Hitbox itemHitbox;

    #endregion

    public FeywoodPlayerActions playerControls;

    private InputAction move;
    private InputAction jump;
    private InputAction interact;

    Vector2 moveDirection;

    // movement speed
    [SerializeField] float Speed = 3;

    [SerializeField] float JumpForce = 3;
    [SerializeField] bool isGrounded = false;

    GameObject _heldObj;
    GameObject heldObject
    {
        get { return _heldObj; }
        set
        {
            if (value != null)
            {
                _heldObj = value;

                _heldObj.transform.parent = model.transform;
                _heldObj.transform.position = heldItemPos.position;
                _heldObj.GetComponent<Rigidbody>().isKinematic = true;
            }
            else
            {
                _heldObj.GetComponent<Rigidbody>().isKinematic = false;
                _heldObj.transform.parent = null;
                _heldObj = null;

            }

            
        }
    }

    private void Awake()
    {
        // initialize the player actions by creating a new instance
        playerControls = new FeywoodPlayerActions();
    }
    private void OnEnable()
    {
        // enable move input system
        move = playerControls.Player.Move;
        move.Enable();

        jump = playerControls.Player.Jump;
        jump.Enable();
        jump.performed += Jump;

        interact = playerControls.Player.Interact;
        interact.Enable();
        interact.performed += Interact;
    }

    

    // Start is called before the first frame update
    void Start()
    {
        #region Saving Object and Component Refrences

        // find the model child object
        model = transform.Find("Model").gameObject;

        // get the instance of the  object's rigidbody and save it to its variable for later use
        rb = GetComponent<Rigidbody>();

        // ground check is a point in the object where a collision box will be drawn to see if the player is on the ground
        groundCheck = transform.Find("GroundCheck");

        itemHitbox = model.transform.Find("ItemHitBox").GetComponent<Hitbox>();

        heldItemPos = model.transform.Find("HeldItemPos");
        #endregion


    }

    // Update is called once per frame
    void Update()
    {
        // update the direction the player is trying to move based on the move input action
        moveDirection = move.ReadValue<Vector2>();

        // if the move direction isn't 0,0 (meaning there is input) then update the direction of the model
        if(moveDirection != Vector2.zero)
        {
            model.transform.rotation = Quaternion.LookRotation(new Vector3(-moveDirection.x, 0, -moveDirection.y));
        }
        

        //Debug.Log($"Move direction = {moveDirection}");
    }

    private void FixedUpdate()
    {
        
        // calculate the new position based on the move direction, speed, and delta time
        Vector3 newPos = new Vector3(moveDirection.y * -Speed * Time.deltaTime, 0f, moveDirection.x * Speed * Time.deltaTime);

        //Debug.Log(newPos);

        // rb.MovePosition used to move the object while accounting for any collisions made
        rb.MovePosition(rb.position + newPos);
    }

    void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump hit");
        if (isGrounded)
        {
            isGrounded = false;
            rb.AddForce(Vector3.up * JumpForce);
        }
        
    }

    void Interact(InputAction.CallbackContext context)
    {
        Debug.Log("Interact hit");

        // if there is an object being held, drop it
        if(heldObject != null)
        {
            Debug.Log($"Dropping {heldObject.name}");

            heldObject = null;
            
           
        }
        // if there is no current object being held, check if there is one in the item hitbox
        else
        {
            if (itemHitbox.TargetObjects.Count != 0)
            {
                heldObject = itemHitbox.TargetObjects[0];
            }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        // check if to see if the collision is with ground underneath it
        Collider[] checkCollisions = Physics.OverlapBox(groundCheck.position, groundCheckDimentions);
        if(checkCollisions == null)
        {
            isGrounded = false;
        }
        else
        {
            isGrounded = true;
        }
    }


}
