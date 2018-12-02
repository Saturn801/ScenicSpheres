using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionScript : Photon.PunBehaviour {

    #region Public Variables
    public PanelScript incorrectPanelScript, questionPanelScript;
    public TimerScript timerScript;
    public AudioSource correctSound, incorrectSound;
    #endregion

    #region Private Variables
    private bool isCorrectAnswer;
    private ArrowScript arrowSelected;
    #endregion 

    #region Script Specific Methods
    public void checkAnswer()
    {
        if (isCorrectAnswer)
        {
            correctSound.Play();
            arrowSelected.moveLocation();
            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in players)
            {
                var playerView = (PhotonView)player.GetComponent(typeof(PhotonView));
                if (playerView.isMine)
                {
                    var playerController = (PlayerManager)player.GetComponent(typeof(PlayerManager));
                    playerController.reduceQuestionCount();
                }
            }
        }
        else
        {
            incorrectSound.Play();
            questionPanelScript.turnOff();
            incorrectPanelScript.turnOn();
            timerScript.setTime(timerScript.getTime() + 10);
        }
    }
    #endregion

    #region Set/Get Methods
    public void setArrow(GameObject arrow)
    {
        arrowSelected = (ArrowScript)arrow.GetComponent(typeof(ArrowScript));
    }

    public void setCorrectValue(bool isCorrect)
    {
        isCorrectAnswer = isCorrect;
    }
    #endregion
}
