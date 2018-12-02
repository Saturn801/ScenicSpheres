using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelScript : MonoBehaviour {

    #region Private Variables;
    CanvasGroup canvas;
    private float currX, currY, currZ;
    #endregion

    #region MonoBehaviour Methods
    // Use this for initialization
    void Start()
    {
        canvas = (CanvasGroup)GetComponent(typeof(CanvasGroup));
    }
    #endregion

    #region Script Specific Methods
    public void turnOff()
    {
        canvas.alpha = 0;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }

    public void turnOn()
    {
        canvas.alpha = 1;
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
    }
    #endregion

    #region Set/Get Methods
    public void hideCanvasLocation()
    {
        this.gameObject.transform.position = new Vector3(1000, 1000, 1000);
    }

    public void resetMainCanvasLocation()
    {
        this.gameObject.transform.position = new Vector3(-0.334f, 1.676f, 6);
        this.gameObject.transform.rotation = Quaternion.identity;
    }

    public void toggleCanvasLocation(float x, float y, float z)
    {
        this.gameObject.transform.position = new Vector3(x, y, z);
        this.gameObject.transform.LookAt(Camera.main.transform);
        this.gameObject.transform.rotation = this.gameObject.transform.rotation * Quaternion.AngleAxis(180, Vector3.up);
    }

    public void updateCanvasLocation()
    {
        toggleCanvasLocation(currX, currY, currZ);
    }

    public void setCanvasCoordinates(float x, float y, float z)
    {
        currX = x;
        currY = y;
        currZ = z;
    }
    #endregion
}
