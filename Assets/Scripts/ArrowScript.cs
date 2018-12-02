using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArrowScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    #region Public Variables
    public Sprite notSelected, selected;
    public bool VRMode;
    #endregion

    #region Private Variables
    MapScript mapScript;
    QuestionPanelScript questionPanelScript;
    PanelScript panelScript, canvasPanelScript;
    TextMesh textDisplay;
    AudioSource openPanelSound;
    private string text;
    private int locationIndex;
    #endregion

    #region MonoBehaviour Methods
    // Use this for initialization
    void Start()
    {
        var canvas = GameObject.Find("Canvas");
        var questionPanel = GameObject.FindGameObjectWithTag("QuestionPanel");
        var audio = GameObject.FindGameObjectWithTag("AudioOpen");
        openPanelSound = (AudioSource)audio.GetComponent(typeof(AudioSource));
        mapScript = (MapScript)canvas.GetComponent(typeof(MapScript));
        canvasPanelScript = (PanelScript)canvas.GetComponent(typeof(PanelScript));
        questionPanelScript = (QuestionPanelScript)questionPanel.GetComponent(typeof(QuestionPanelScript));
        panelScript = (PanelScript)questionPanel.GetComponent(typeof(PanelScript));
        textDisplay = gameObject.GetComponentInParent(typeof(TextMesh)) as TextMesh;
    }
    #endregion

    #region MouseEvent Methods
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        openPanelSound.Play();
        exitArrow();
        mapScript.toggleElements();
        if(VRMode)
            canvasPanelScript.toggleCanvasLocation(this.transform.position.x/2, this.transform.position.y + 2, this.transform.position.z/2);
        panelScript.turnOn();
        questionPanelScript.randomizeQuestion(mapScript.getCurrLocation());
        questionPanelScript.setSelectedArrow(this.gameObject);  
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (VRMode)
            this.GetComponent<Image>().sprite = selected;
        else
            this.GetComponent<SpriteRenderer>().sprite = selected;
        textDisplay.text = "\n" + text;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        exitArrow();
    }
    #endregion

    #region Script Specific Methods
    public void exitArrow()
    {
        if (VRMode)
            this.GetComponent<Image>().sprite = notSelected;
        else
            this.GetComponent<SpriteRenderer>().sprite = notSelected;
        textDisplay.text = "";
    }

    public void moveLocation()
    {
        panelScript.turnOff();
        if(VRMode)
            canvasPanelScript.hideCanvasLocation();
        mapScript.destroyArrows();
        mapScript.updateLocationIndex(locationIndex);
        mapScript.toggleInfoIcon();
        mapScript.setLocation();
        mapScript.decreaseQuestionCount();
    }
    #endregion

    #region Set/Get Methods
    public void setText(string newText)
    {
        text = newText;      
    }

    public void setLocationIndex(int newIndex)
    {
        locationIndex = newIndex;
    }
    #endregion
}
