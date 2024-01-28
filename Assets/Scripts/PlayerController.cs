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
        playerControls = new FeywoodPlayerActions();
    }
    private void OnEnable()
    {
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

        Debug.Log($"Move direction = {moveDirection}");
    }

    private void FixedUpdate()
    {
        

        Vector3 newPos = new Vector3(moveDirection.y * -Speed * Time.deltaTime, 0f, moveDirection.x * Speed * Time.deltaTime);

        //Debug.Log(newPos);
        rb.MovePosition(rb.position + newPos);
    }
}
