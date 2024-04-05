using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;

public class PlayerController : MonoBehaviour, IDataPersistance
{
    #region Object and Component Refrences

    GameObject model;
    Rigidbody rb;
    Transform groundCheck;
    Vector3 groundCheckDimentions = new Vector3(.2f, .5f, .2f);

    Transform heldItemPos;
    BoxCollider ghostItemCollider;

    Hitbox itemHitbox;

    SneakUI sneakIcon;

    #endregion

    public FeywoodPlayerActions playerControls;

    private InputActionMap actions;

    private InputAction move;
    private InputAction jump;
    private InputAction interact;

    Vector2 moveDirection;

    public Vector3 RespawnPos;

    public string CurrentRoom;

    public enum PlayerStates
    {
        Idle,
        Walking,
        Running,
        Climbing,
        Jumping, //Jumping is a state that can overlap with holding currently
        Holding,
        Pushing
    }


    public enum MovementRestrictions
    {
        None,
        xOnly,
        zOnly,
        RestrictedDirection,
        Climbing,
        noMovement
    }

    [SerializeField] public MovementRestrictions currentRestriction = MovementRestrictions.None;

    PlayerStates playerState = PlayerStates.Idle;

    Vector2 restrictedDirection;

    // movement speed
    [SerializeField] float Speed = 3;

    [SerializeField] float JumpForce = 3;
    bool isGrounded = false;

    GameObject _heldObj;
    public GameObject heldObject
    {
        get { return _heldObj; }
        set
        {
            if (value != null)
            {
                _heldObj = value;

                Collider collider = _heldObj.GetComponent<Collider>();

                // set the rotation back to 0
                _heldObj.transform.rotation = heldItemPos.transform.rotation;

                // set the model as the parent of the object so it moves with it
                _heldObj.transform.parent = model.transform;

                //Debug.Log($"obj collider: {collider}");

                // move the object into position
                _heldObj.transform.position = heldItemPos.position + heldItemPos.transform.TransformDirection(new Vector3(0, collider.bounds.size.y/2, collider.bounds.size.z / 2));

                Rigidbody itemRB = _heldObj.GetComponent<Rigidbody>();

                // set to kinematic so it wont be moved by the physics system
                itemRB.isKinematic = true;
                itemRB.detectCollisions = false;

                ghostItemCollider.enabled = true;

                Vector3 newSize = Vector3.zero;

                if(collider is SphereCollider)
                {
                    SphereCollider sphereCollider = (SphereCollider)collider;

                    newSize = Vector3.one * sphereCollider.radius;
                    ghostItemCollider.center = new Vector3(0, sphereCollider.radius/2, sphereCollider.radius/2 );

                }
                else if (collider is CapsuleCollider)
                {
                    CapsuleCollider capsuleCollider = (CapsuleCollider)collider;

                    newSize = new Vector3(capsuleCollider.radius, capsuleCollider.radius, capsuleCollider.height);
                    ghostItemCollider.center = new Vector3(0, capsuleCollider.height/2, capsuleCollider.radius );
                }
                else if (collider is BoxCollider)
                {
                    BoxCollider boxCollider = (BoxCollider)collider;

                    newSize = boxCollider.size;
                    ghostItemCollider.center = new Vector3(0, boxCollider.size.y / 2 * _heldObj.transform.localScale.y, boxCollider.size.z / 2 * _heldObj.transform.localScale.z);

                    newSize = new Vector3(newSize.x * _heldObj.transform.localScale.x, newSize.y * _heldObj.transform.localScale.y, newSize.z * _heldObj.transform.localScale.z);
                    _heldObj.transform.position = heldItemPos.position + heldItemPos.transform.TransformDirection(new Vector3(0, boxCollider.size.y / 2 * _heldObj.transform.localScale.y, boxCollider.size.z / 2 * _heldObj.transform.localScale.z));

                }

                else
                {
                    newSize = collider.bounds.size;
                    ghostItemCollider.center = new Vector3(0, collider.bounds.size.y/2, collider.bounds.size.z / 2);
                }

                //ghostItemCollider.transform.localScale = _heldObj.transform.localScale;
                //ghostItemCollider.size = new Vector3(newSize.x * _heldObj.transform.localScale.x, newSize.y * _heldObj.transform.localScale.y, newSize.z * _heldObj.transform.localScale.z);


                ghostItemCollider.size = newSize;

                // disable the collider so there aren't any weird physics interactions
                //collider.enabled = false;
            }
            else
            {

                _heldObj.transform.parent = null;

                // set the object back to normal values (inverse of set functions)
                Rigidbody itemRB = _heldObj.GetComponent<Rigidbody>();

                ghostItemCollider.enabled = false;

                // set to kinematic so it wont be moved by the physics system
                itemRB.isKinematic = false;
                itemRB.detectCollisions = true;

                _heldObj = null;

            }

            
        }
    }


    GameObject pushingObj;

    GameObject climbingObj;

    [SerializeField] LayerMask climbingMask;

    [SerializeField] float climbingAngleThreshold = 40;

    [SerializeField] bool isHidden = false;

    public bool IsHidden
    {
        get { return isHidden; }

        set
        {
            isHidden = value;

            if (sneakIcon != null)
            {
                if (isHidden)
                {
                    sneakIcon.CurrentState = SneakUI.SneakStates.Hidden;
                }
                else
                {
                    sneakIcon.CurrentState = SneakUI.SneakStates.Exposed;
                }
            }

        }
    }

    private void Awake()
    {
        // initialize the player actions by creating a new instance
        playerControls = new FeywoodPlayerActions();

        // find the model child object
        model = transform.Find("Model").gameObject;

        // get the instance of the  object's rigidbody and save it to its variable for later use
        rb = GetComponent<Rigidbody>();

        //Disable UI
        GetComponentInChildren<CanvasGroup>().alpha = 0;
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

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        interact.Disable();
    }

    public void LoadData(GameData data)
    {
        //Debug.Log("Player load hit");

        playerState = PlayerStates.Idle;

        GameObject spawn = GameObject.Find(data.SpawnPointName);

        //Debug.Log($"Carrying item: {data.heldObj}");

        // check if the spawn object was found and double check to make sure it is a check point
        if (spawn != null)
        {
            if (spawn.tag == "CheckPoint")
            {
                //Debug.Log($"Spawn found, sending player to {spawn.transform.GetChild(0).position}");
                this.transform.position = spawn.transform.GetChild(0).position;

            }
        }
        else
        {
            //Debug.Log("Spawn not found, starting at normal scene position");
        }
        if (data.heldObj != null)
        {
            Debug.Log($"Spawning in {data.heldObj.name}");

            StartCoroutine(StartUp(data.heldObj));


        }
    }

    IEnumerator StartUp(GameObject passedObj)
    {
        yield return null;
        Debug.Log("Player Startup initiated");
        //heldObject = Instantiate(passedObj, this.transform.position, passedObj.transform.rotation);
        heldObject = DataManager.instance.Data.heldObj;
    }

    public void SaveData(ref GameData data)
    {
        data.heldObj = heldObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        #region Saving Object and Component Refrences

        

        // ground check is a point in the object where a collision box will be drawn to see if the player is on the ground
        groundCheck = transform.Find("GroundCheck");

        itemHitbox = model.transform.Find("ItemHitBox").GetComponent<Hitbox>();

        heldItemPos = model.transform.Find("HeldItemPos");

        ghostItemCollider = heldItemPos.gameObject.GetComponent<BoxCollider>();

        if(GameObject.Find("SneakIcon") != null)
        {
            //Debug.Log("Sneak icon found");
            sneakIcon = GameObject.Find("SneakIcon").GetComponent<SneakUI>();
        }

        #endregion
        
        //gets the player's position at the start of the level. 

    }

    private void CheckForSpawnData()
    {
        
    }
    public void RespawnPlayer()
    {
        transform.position = RespawnPos;
        Debug.Log("Player was caught and will respawn");
        //changes the player's position to their position at the start of the level. Made this public void so it's accessible in the Dad behavior script. 

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(this.transform.position, model.transform.TransformDirection(Vector3.forward),Color.blue);

        switch (currentRestriction)
        {
            // default is used in the case of both no restriction and climbing (climbing is checked later on)
            default:
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
        
        if(currentRestriction == MovementRestrictions.Climbing)
        {
            //Debug.Log($"Current z rotation {model.transform.rotation.eulerAngles.z} vs threshold {climbingAngleThreshold}");
            // if the player is climbing and they are climbing too low of an angle, then get them out of climbing mode
            if (model.transform.rotation.eulerAngles.z >= 180 && model.transform.rotation.eulerAngles.z <= 360-climbingAngleThreshold)
            {
                Debug.Log("Angle too low, exiting climb mode");
                //currentRestriction = MovementRestrictions.None;

            }
            
        }
        
        
        
        

        // if the move direction isn't 0,0 (meaning there is input) then update the direction of the model
        if(moveDirection != Vector2.zero && currentRestriction == MovementRestrictions.None)
        {
            model.transform.rotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.y));
        }


        //Debug.Log($"Move direction = {moveDirection}");

        // If Interactble is in array display UI as well as turning off if currently interacting
        if (itemHitbox.TargetObjects.Count != 0 && playerState == PlayerStates.Idle)
        {
            // Enable UI
            GetComponentInChildren<CanvasGroup>().alpha = 1;
            // save the instance of the target object for easy use
            GameObject targetObj = itemHitbox.TargetObjects[0];

            // if the target object is a Pickup, then change text to appropiate context
            if (targetObj.tag == "Pickup")
            {
                GetComponentInChildren<TextMeshProUGUI>().text = "Pickup";
            }

            else if (targetObj.tag == "PushPull")
            {
                GetComponentInChildren<TextMeshProUGUI>().text = "Push/Pull";
            }

            else if (targetObj.tag == "Climbable")
            {
                GetComponentInChildren<TextMeshProUGUI>().text = "Climb";
            }
        }
        else
        {
            GetComponentInChildren<CanvasGroup>().alpha = 0;
        }
    }

    private void FixedUpdate()
    {
        Vector3 newPos = Vector3.zero;

        if (currentRestriction == MovementRestrictions.Climbing)
        {
            // calculate the new position based on the direction, speed, and deltatime
            newPos = new Vector3(moveDirection.x * Speed * Time.deltaTime, moveDirection.y * Speed * Time.deltaTime, 0f);
            newPos = model.transform.TransformDirection(newPos);


        }

        else
        {
            // calculate the new position based on the move direction, speed, and delta time
            newPos = new Vector3(moveDirection.x * Speed * Time.deltaTime, rb.velocity.y, moveDirection.y * Speed * Time.deltaTime);
        }


        //Debug.Log(newPos);

        // rb.MovePosition used to move the object while accounting for any collisions made
        //rb.MovePosition(rb.position + newPos);

        //Debug.Log("newPos = " + newPos);
        rb.velocity = newPos;
    }

    void Jump(InputAction.CallbackContext context)
    {
        if(currentRestriction != MovementRestrictions.noMovement)
        {
            //playerState = PlayerStates.Jumping;
            //Debug.Log("Jump hit");
            if (currentRestriction == MovementRestrictions.Climbing)
            {
                playerState = PlayerStates.Idle; //Replace this. Flawed logic
                currentRestriction = MovementRestrictions.None;
                isGrounded = false;
                rb.AddForce(Vector3.up * JumpForce);
            }
            else if (isGrounded)
            {
                isGrounded = false;
                rb.AddForce(Vector3.up * JumpForce);
            }
        }
        

    }

    void Interact(InputAction.CallbackContext context)
    {
        //Debug.Log("Interact hit");

        // if there is an object being held, drop it
        if(heldObject != null)
        {
            Debug.Log($"Dropping {heldObject.name}");

            playerState = PlayerStates.Idle;

            heldObject = null;
            
           
        }
        else if (pushingObj != null)
        {
            playerState = PlayerStates.Idle;

            pushingObj.transform.parent = null;

            pushingObj = null;

            currentRestriction = MovementRestrictions.None;

        }
        
        else if(currentRestriction == MovementRestrictions.Climbing)
        {
            playerState = PlayerStates.Idle;

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
                if (targetObj.tag == "Pickup")
                {
                    playerState = PlayerStates.Holding;
                    heldObject = itemHitbox.TargetObjects[0];
                }

                else if (targetObj.tag == "PushPull" && isGrounded)
                {
                    playerState = PlayerStates.Pushing;
                    StartCoroutine(PushPull(targetObj));
                }

                else if(targetObj.tag == "Climbable")
                {
                    playerState = PlayerStates.Climbing;
                    currentRestriction = MovementRestrictions.Climbing;
                    StartCoroutine(Climbing(targetObj));
                }
            }
        }
        
    }

    IEnumerator PushPull(GameObject obj)
    {
        //Debug.Log("Hit");
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
        restrictedDirection = new Vector2( (closestPoint.x - obj.transform.position.x), -(closestPoint.z - obj.transform.position.z)).normalized;

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

    IEnumerator Climbing(GameObject surface)
    {
        climbingObj = surface;
        RaycastHit hit;

        rb.useGravity = false;

        //Vector3 lookDir = new Vector3();

        while (currentRestriction == MovementRestrictions.Climbing)
        {
            //Vector3 dir = climbingObj.transform.position - transform.position;
            //dir = dir.normalized;

            Vector3 dir = model.transform.TransformDirection(Vector3.forward);

            Debug.DrawRay(model.transform.position, dir, Color.red, 4);

            if (Physics.Raycast(model.transform.position,dir,out hit,1f,climbingMask))
            {
                //Debug.Log($"Hit: {hit.collider.gameObject} Normal: {hit.normal}");

                Vector3 lookDir = (this.transform.position - hit.point).normalized;

                
                model.transform.rotation = Quaternion.LookRotation(-hit.normal);

                

                
            }
            else
            {
                playerState = PlayerStates.Idle;
                currentRestriction = MovementRestrictions.None;
                break;
            }

            yield return null;
        }

        rb.useGravity = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log($"Player collision detected: {collision.gameObject.name}");
        // check if to see if the collision is with ground underneath it
        Collider[] checkCollisions = Physics.OverlapBox(groundCheck.position, groundCheckDimentions);
        if(checkCollisions == null)
        {
            isGrounded = false;
        }
        else
        {
            //playerState = PlayerStates.Idle;
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // if the collision is a hidden zone, then the player is hidden
        if (other.gameObject.tag == "Hidden")
        {
            //Debug.Log("Hidden");

            IsHidden = true;
        }
        else if(other.gameObject.tag == "Room")
        {
            CurrentRoom = other.gameObject.name;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // if the player is leaving a hidden collision, then they are no longer hidden
        if (other.gameObject.tag == "Hidden")
        {
            //Debug.Log("Not hidden");
            IsHidden = false;
        }
    }
}
