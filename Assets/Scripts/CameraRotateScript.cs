using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateScript : MonoBehaviour {

    #region Public Variables
    public float speed;
    #endregion 

    #region Private Variables
    Vector3 rotationVector;
    float min = -90.0f;
    float max = 90.0f;
    #endregion

    #region MonoBehaviour Methods
    // Use this for initialization
    void Start()
    {
        rotationVector = Vector3.zero;
        transform.eulerAngles = rotationVector;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            rotationVector.x += Input.GetAxis("Mouse Y") * speed;
            rotationVector.y -= Input.GetAxis("Mouse X") * speed;
            rotationVector.x = Mathf.Clamp(rotationVector.x, min, max);
            transform.eulerAngles = rotationVector;
        }
    }
    #endregion

    #region Script Specific Methods
    public void resetCamera()
    {
        rotationVector.x = 0;
        rotationVector.y = 0;
        rotationVector.x = 0;
        transform.eulerAngles = rotationVector;
    }
    #endregion
}
