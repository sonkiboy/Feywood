using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DadBehavior : MonoBehaviour
{
    #region Object and Component Refrences

    // player object
    GameObject player;

    // represents where the dad can see
    BoxCollider visionCollider;

    // dad's rigidbody
    Rigidbody rb;

    #endregion

    [Range(0f, 2f)]
    [SerializeField] float turnTime = 1;

    [SerializeField] float waitTime = 3;


    [SerializeField] float walkDistance = 5;

    [SerializeField] float walkSpeed = 1;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody>();
        visionCollider = transform.Find("SightBounds").GetComponent<BoxCollider>();

        StartCoroutine(Monitor());
    }
   

    // Update is called once per frame
    void Update()
    {
        CheckSight();


    }

    IEnumerator Monitor()
    {
        yield return null;

        while (true)
        {
            // Walk
            Vector3 targetPos = this.transform.TransformPoint((Vector3.forward * walkDistance)/2);

            //Debug.Log($"Target Pos: {targetPos} ({Vector3.forward * walkDistance})");

            while(Vector3.Distance(this.transform.position, targetPos) > .1)
            {
                Vector3 dir = -(this.transform.position - targetPos).normalized;

                //Debug.Log($"Driection Pos: {dir}");


                rb.MovePosition(this.transform.position + (dir * walkSpeed * Time.deltaTime));

                yield return new WaitForFixedUpdate();
            }

            rb.MovePosition(targetPos);

            // Stop
            yield return new WaitForSeconds(waitTime);

            //Turn

            transform.RotateAround(transform.position, transform.up, 180f);
           // player.GetComponent<PlayerController>().RespawnPlayer(); will respawn player to starting point when caught. 
           //Debug.Log("Player was caught!");
        }
        
    }

    void CheckSight()
    {
        Collider[] foundColliders = Physics.OverlapBox(visionCollider.bounds.center, visionCollider.bounds.size,visionCollider.gameObject.transform.rotation,visionCollider.includeLayers);

        //Debug.Log($"Found: {foundColliders.Length}");
        if(foundColliders.Length > 0)
        {
            if (foundColliders[0].gameObject.name == "Player")
            {
                Debug.Log($"Player in Dad's sight");
                PlayerController controller = foundColliders[0].gameObject.GetComponent<PlayerController>();

                if(controller.heldObject != null)
                {
                    if (controller.heldObject.name == "Key" && controller.IsHidden == false)
                    {
                        CatchPlayer();
                    }
                }
                
            }
        }
        
    }

    // called when player is caught
    private void CatchPlayer()
    {
        Debug.Log("Dad caught Player!");

        player.GetComponent<PlayerController>().RespawnPlayer();
    }
}
