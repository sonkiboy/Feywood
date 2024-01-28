using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Object and Component Refrences

    GameObject model;
    Rigidbody rb;

    #endregion

    public FeywoodPlayerActions playerControls;

    private InputAction move;
    private InputAction jump;

    Vector2 moveDirection;

    // movement speed
    [SerializeField] float Speed;

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
    }

    

    // Start is called before the first frame update
    void Start()
    {
        #region Saving Object and Component Refrences

        // find the model child object
        model = transform.Find("Model").gameObject;

        // get the instance of the  object's rigidbody and save it to its variable for later use
        rb = GetComponent<Rigidbody>();

        #endregion


    }

    // Update is called once per frame
    void Update()
    {
        // update the direction the player is trying to move based on the move input action
        moveDirection = move.ReadValue<Vector2>();


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
}
