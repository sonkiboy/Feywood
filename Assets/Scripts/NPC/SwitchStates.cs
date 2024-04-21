using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchStates : MonoBehaviour
{
    [SerializeField] GameObject PatrolShell;
    [SerializeField] GameObject MoveShell;
    public SisterPatrol sisterPatrol= new SisterPatrol();
    // Start is called before the first frame update
    void Start()
    {
        MoveShell.SetActive(true);
        PatrolShell.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        //if(gameObject.)
    }
}
