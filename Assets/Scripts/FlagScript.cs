using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FlagScript : MonoBehaviour, IPointerClickHandler {

    #region Public Variables
    public bool VRMode;
    #endregion

    #region Private Variables
    PanelScript canvasPanelScript, keyboardPanelScript;
    TimerScript timerScript;
    MapScript mapScript;
    Text winText;
    AudioSource winSound;
    #endregion

    #region MonoBehaviour Methods
    // Use this for initialization
    void Start()
    {        
        var timer = GameObject.FindGameObjectWithTag("Timer");
        timerScript = (TimerScript)timer.GetComponent(typeof(TimerScript));      
        var canvas = GameObject.Find("Canvas");
        mapScript = (MapScript)canvas.GetComponent(typeof(MapScript));
        if (VRMode)
        {
            var keyboard = GameObject.FindGameObjectWithTag("Keyboard");
            keyboardPanelScript = (PanelScript)keyboard.GetComponent(typeof(PanelScript));
            canvasPanelScript = (PanelScript)canvas.GetComponent(typeof(PanelScript));
        }
        var audio = GameObject.FindGameObjectWithTag("AudioWin");
        winSound = (AudioSource)audio.GetComponent(typeof(AudioSource));
    }
    #endregion

    #region MouseEvent Methods
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        bool multiplayer = false;
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            var playerView = (PhotonView)player.GetComponent(typeof(PhotonView));
            if (playerView.isMine)
            {
                multiplayer = true;
                var playerController = (PlayerManager)player.GetComponent(typeof(PlayerManager));
                playerController.endPlayerGame();
                playerController.setPlayerTime(timerScript.getTime());
            }
        }
        if (!multiplayer)
        {
            var winPanel = GameObject.FindGameObjectWithTag("WinPanel");
            var panelScript = (PanelScript)winPanel.GetComponent(typeof(PanelScript));
            panelScript.turnOn();
            var winPanelText = GameObject.FindGameObjectWithTag("WinText");
            var winText = (Text)winPanelText.GetComponent(typeof(Text));
            winText.text = "You win! You scored a time of: " + timerScript.getTimeString();
        }
        winSound.Play();       
        timerScript.stopTimer();
        mapScript.hideRemainingQuestionsText();
        mapScript.toggleElements();
        if (VRMode)
        {
            canvasPanelScript.toggleCanvasLocation(this.transform.position.x / 2, this.transform.position.y + 1.5f, this.transform.position.z / 2);
            keyboardPanelScript.setCanvasCoordinates(this.transform.position.x / 2, this.transform.position.y - 1.5f, this.transform.position.z / 2);
        }      
        mapScript.destroyArrows();          
        Destroy(GameObject.FindGameObjectWithTag("Flag"));
    }
    #endregion
}
