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

    enum MovementRestrictions
    {
        None,
        xOnly,
        zOnly,
        RestrictedDirection,
        noMovement
    }

    [SerializeField] MovementRestrictions currentRestriction = MovementRestrictions.None;

    [SerializeField] Vector2 restrictedDirection;

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

                Collider collider = _heldObj.GetComponent<Collider>();

                // set the model as the parent of the object so it moves with it
                _heldObj.transform.parent = model.transform;

                // set the rotation back to 0
                _heldObj.transform.rotation = Quaternion.Euler(Vector3.zero);

                // move the object into position
                _heldObj.transform.position = heldItemPos.position + heldItemPos.transform.TransformDirection(new Vector3(collider.bounds.size.x/2, 0, 0));

                // set to kinematic so it wont be moved by the physics system
                _heldObj.GetComponent<Rigidbody>().isKinematic = true;

                // disable the collider so there aren't any weird physics interactions
                //collider.enabled = false;
            }
            else
            {
                // set the object back to normal values (inverse of set functions)
                _heldObj.GetComponent<Rigidbody>().isKinematic = false;
                //_heldObj.GetComponent<Collider>().enabled = true;
                _heldObj.transform.parent = null;
                _heldObj = null;

            }

            
        }
    }

    GameObject pushingObj;

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

        switch (currentRestriction)
        {
            case MovementRestrictions.None:
                moveDirection = move.ReadValue<Vector2>();
                break;

            case MovementRestrictions.xOnly:
                moveDirection = new Vector2(0, move.ReadValue<Vector2>().y);
                
                break;

            case MovementRestrictions.zOnly:
                moveDirection = new Vector2(move.ReadValue<Vector2>().x, 0);
                break;

            case MovementRestrictions.RestrictedDirection:
                //Debug.Log($"Dot: {Vector2.Dot(restrictedDirection, move.ReadValue<Vector2>())}");
                moveDirection = restrictedDirection * Vector2.Dot(restrictedDirection, move.ReadValue<Vector2>());
                
                break;

            case MovementRestrictions.noMovement:
                moveDirection = Vector2.zero;
                break;

        }
        
        

        
        
        

        // if the move direction isn't 0,0 (meaning there is input) then update the direction of the model
        if(moveDirection != Vector2.zero && currentRestriction == MovementRestrictions.None)
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
        //Debug.Log("Jump hit");
        if (isGrounded)
        {
            isGrounded = false;
            rb.AddForce(Vector3.up * JumpForce);
        }
        
    }

    void Interact(InputAction.CallbackContext context)
    {
        //Debug.Log("Interact hit");

        // if there is an object being held, drop it
        if(heldObject != null)
        {
            Debug.Log($"Dropping {heldObject.name}");

            heldObject = null;
            
           
        }
        else if (pushingObj != null)
        {
            pushingObj.transform.parent = null;

            pushingObj = null;

            currentRestriction = MovementRestrictions.None;

        }


        // if there is no current object being held and no object being pushed, check if there is an object in the item hitbox
        else
        {
            if (itemHitbox.TargetObjects.Count != 0)
            {
                // save the instance of the target object for easy use
                GameObject targetObj = itemHitbox.TargetObjects[0];

                // if the target object is a Pickup, then set that to the heldObject
                if(targetObj.tag == "Pickup")
                {
                    heldObject = itemHitbox.TargetObjects[0];
                }

                else if(targetObj.tag == "PushPull")
                {
                    StartCoroutine(PushPull(targetObj));
                }
            }
        }
        
    }

    IEnumerator PushPull(GameObject obj)
    {
        
        // save an instance of the pushableObject component
        PushableObject poBehavior = obj.GetComponent<PushableObject>();

        // refreshes the positions of the face points
        poBehavior.CreateFacePoints();

        // declare the closent point the player will latch onto
        Vector3 closestPoint = poBehavior.facePoints[0];
        
        // go through all the points on the objects faces and determine which is closest to the player
        for (int i = 0; i < poBehavior.facePoints.Length - 1; i++)
        {
            if(Vector3.Distance(transform.position,closestPoint) > Vector3.Distance(transform.position,poBehavior.facePoints[i + 1]))
            {
                closestPoint = poBehavior.facePoints[i + 1];
                
            }
        }

        // wait a frame
        yield return null;

        //Debug.Log($"Moving to{closestPoint}");

        // create a variable to store the new position for easy use
        Vector3 newPos = new Vector3(closestPoint.x, transform.position.y, closestPoint.z);

        // calculate the direction from the point the player is moving to, to the center of the object they are pushing
        // this will be the axis the player can move along
        restrictedDirection = new Vector2(-(closestPoint.z - obj.transform.position.z), (closestPoint.x - obj.transform.position.x)).normalized;

        // move the player to the new position
        transform.position = newPos;

        // make the player character look at the object

        model.transform.rotation = Quaternion.LookRotation(new Vector3(-restrictedDirection.x, 0, -restrictedDirection.y));

        // wait a frame
        yield return null;

        // set the player as the parent of the object being pushed and save it to the pushingObj variable
        obj.transform.parent = this.transform;

        pushingObj = obj;

        // set the movement restriction to only follow the saved restricted direction
        currentRestriction = MovementRestrictions.RestrictedDirection;

        
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
