using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SimpleJSON;

public class InfoScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

    #region Public Variables
    public PanelScript infoPanelScript, canvasPanelScript;
    public MapScript mapScript;
    public Text infoText;
    public AudioSource openPanelSound;
    public Sprite notSelected, selected;
    public TextAsset infoJson1, infoJson2, infoJson3;
    public Image infoImage;
    public bool VRMode;
    #endregion

    #region Private Variables
    TextAsset asset;
    private int infoIndex, infoCount, location;
    #endregion

    #region MouseEvent Methods
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        infoPanelScript.turnOn();
        mapScript.toggleElements();
        if(VRMode)
            canvasPanelScript.toggleCanvasLocation(this.transform.position.x, this.transform.position.y + 2, this.transform.position.z);
        location = mapScript.getCurrLocation();
        randomizeInfo();
        openPanelSound.Play();
        this.GetComponent<Image>().sprite = notSelected;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        this.GetComponent<Image>().sprite = selected;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        this.GetComponent<Image>().sprite = notSelected;
    }
    #endregion

    #region Script Specific Methods
    public void randomizeInfo()
    {
        var N = JSON.Parse(asset.text);
        infoCount = N["locationInfo"][location].Count;
        infoIndex = UnityEngine.Random.Range(0, infoCount);
        infoText.text = N["locationInfo"][location][infoIndex][0];
        infoImage.sprite = Resources.Load<Sprite>(N["locationInfo"][location][infoIndex][1]);
    }

    public void nextInfo()
    {
        infoIndex = Mod(infoIndex + 1, infoCount);
        var N = JSON.Parse(asset.text);
        infoText.text = N["locationInfo"][location][infoIndex][0];
        infoImage.sprite = Resources.Load<Sprite>(N["locationInfo"][location][infoIndex][1]);
    }

    public void prevInfo()
    {
        infoIndex = Mod(infoIndex - 1, infoCount);
        var N = JSON.Parse(asset.text);
        infoText.text = N["locationInfo"][location][infoIndex][0];
        infoImage.sprite = Resources.Load<Sprite>(N["locationInfo"][location][infoIndex][1]);
    }
    #endregion

    #region Set/Get Methods
    public void setJsonAsset(int countryIndex)
    {
        if (countryIndex == 0)
            asset = infoJson1;
        else if (countryIndex == 1)
            asset = infoJson2;
        else if (countryIndex == 2)
            asset = infoJson3;
    }
    #endregion

    #region Helper Methods
    int Mod(int a, int b)
    {
        return (a % b + b) % b;
    }
    #endregion    
}
