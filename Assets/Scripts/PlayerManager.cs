using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerManager : Photon.PunBehaviour, IPunObservable {

    #region Variables
    public string playerName;
    public int time = 0, questionsRemaining = 5;
    public bool endedGame = false, endGameAction = false, isReady = false;
    public static GameObject LocalPlayerInstance;
    PanelScript canvasPanelScript;
    TimerScript timerScript;
    #endregion

    #region MonoBehaviour Methods
    // Use this for initialization
    void Awake () {
        var canvas = GameObject.Find("Canvas");
        canvasPanelScript = (PanelScript)canvas.GetComponent(typeof(PanelScript));
        var timer = GameObject.FindGameObjectWithTag("Timer");
        timerScript = (TimerScript)timer.GetComponent(typeof(TimerScript));
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.isMine)
        {
            var photonView = (PhotonView)this.GetComponent(typeof(PhotonView));
            playerName = photonView.owner.NickName;
            PlayerManager.LocalPlayerInstance = this.gameObject;
        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        if (gameOver() && !endGameAction && photonView.isMine)
        {
            endGameAction = true;
            canvasPanelScript.turnOff();
            var endMenu = GameObject.FindGameObjectWithTag("EndGameMenu");
            var endMenuPanelScript = (PanelScript)endMenu.GetComponent(typeof(PanelScript));
            endMenuPanelScript.turnOn();
            var endMenuPlayer = GameObject.FindGameObjectWithTag("EndGamePlayer");
            var endMenuPlayerText = (Text)endMenuPlayer.GetComponent(typeof(Text));
            var endScore = GameObject.FindGameObjectWithTag("EndGameScore");
            var endScoreText = (Text)endScore.GetComponent(typeof(Text));
            if (endedGame)
                endMenuPlayerText.text = "You Win!";
            else
                endMenuPlayerText.text = "You lose!";
            endScoreText.text = getScoring();       
        }
	}
    #endregion

    #region Set/Get Methods
    public void reduceQuestionCount()
    {
        if (questionsRemaining != 0)
            questionsRemaining--;
    }

    public void endPlayerGame()
    {
        endedGame = true;
    }

    public void setPlayerTime(int newTime)
    {
        time = newTime;
    }

    public void setPlayerReady()
    {
        isReady = true;
    }

    public bool getPlayerReadyStatus()
    {
        return isReady;
    }

    private string getScoring()
    {
        string score = "";

        var players = GameObject.FindGameObjectsWithTag("Player");
        PlayerManager winningPlayerScript = null;
        PlayerManager[] losingPlayersScripts = new PlayerManager[players.Length - 1];
        int i = 0;
        foreach (var player in players)
        {
            var playerController = (PlayerManager)player.GetComponent(typeof(PlayerManager));
            if (playerController.endedGame)
                winningPlayerScript = playerController;
            else
            {
                losingPlayersScripts[i] = playerController;
                i++;
            }
        }
        Array.Sort(losingPlayersScripts, delegate (PlayerManager player1, PlayerManager player2) {
            return player1.questionsRemaining.CompareTo(player2.questionsRemaining);
        });

        score += "1st - " + winningPlayerScript.playerName;
        score += ": " + timerScript.formatTime(winningPlayerScript.time) + "\n";
        for (i = 0; i < losingPlayersScripts.Length; i++)
        {
            if (i == 0)
                score += "2nd - ";
            else if (i == 1)
                score += "3rd - ";
            else
                score += "4th - ";
            score += losingPlayersScripts[i].playerName;
            score += ": " + losingPlayersScripts[i].questionsRemaining + " question(s) remaining.\n";
        }

        return score;
    }
    #endregion

    #region Helper Methods
    // If one player has captured the flag, end the game.
    bool gameOver()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach(var player in players)
        {
            var playerController = (PlayerManager)player.GetComponent(typeof(PlayerManager));
            if (playerController.endedGame)
                return true;
        }
        return false;
    }
    #endregion

    #region Photon Methods
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(endedGame);
            stream.SendNext(questionsRemaining);
            stream.SendNext(time);
            stream.SendNext(endGameAction);
            stream.SendNext(isReady);
            stream.SendNext(playerName);
        }
        else
        {
            // Network player, receive data
            this.endedGame = (bool)stream.ReceiveNext();
            this.questionsRemaining = (int)stream.ReceiveNext();
            this.time = (int)stream.ReceiveNext();
            this.endGameAction = (bool)stream.ReceiveNext();
            this.isReady = (bool)stream.ReceiveNext();
            this.playerName = (string)stream.ReceiveNext();
        }
    }
    #endregion
}
