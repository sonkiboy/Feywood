using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : MonoBehaviour
{
    #region Object and Component Refrences

    Collider _collider;
    Vector3 colliderSize;
    [SerializeField] float pointOffset = .9f; 

    #endregion

    // stores the position of the 4 face point in LOCAL SPACE
    public Vector3[] facePoints;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<Collider>();

        colliderSize = _collider.bounds.size;

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateFacePoints()
    {
        // create a new array that will represent the 4 directions of the object 
        facePoints = new Vector3[4];

        // create the points for +x, -x, +z, -z
        facePoints[0] = new Vector3((colliderSize.x / 2) + pointOffset, 0, 0) / transform.localScale.x;
        facePoints[1] = new Vector3( -(colliderSize.x / 2) - pointOffset, 0, 0)/transform.localScale.x;
        facePoints[2] = new Vector3(0, 0,  (colliderSize.z / 2) + pointOffset) / transform.localScale.z;
        facePoints[3] = new Vector3(0, 0,  -(colliderSize.z / 2) - pointOffset) / transform.localScale.z;

        //Debug.Log($"Points before global: 0:{facePoints[0]}, 1:{facePoints[1]}, 2:{facePoints[2]}, 3:{facePoints[3]}");

        transform.TransformPoints(facePoints);

        //Debug.Log($"Points after global: 0:{facePoints[0]}, 1:{facePoints[1]}, 2:{facePoints[2]}, 3:{facePoints[3]}");
    }
}
