using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HiscoresScript : MonoBehaviour {

    #region Public Variables
    public PlayerNameInputField inputField;
    public InputField playerNameText;
    public PanelScript priorPanelScript, postPanelScript, errorPanelScript, keyboardPanelScript;
    public TimerScript timerScript;
    public Text hiscoresMenuText, hiscoresPanelScore, hiscoresPanelText;
    public bool VRMode;
    #endregion

    #region Script Specific Methods
    public void submitButtonCall()
    {
        string input = playerNameText.text;
        input = input.ToUpper();
        if (inputField.isInputValid(input))
        {
            playerNameText.text = "";
            bool isUpdated = updateHiscores(input);
            if (isUpdated)
                hiscoresPanelText.text = "Congratulations " + input + ", you made the leaderboard!";
            else
                hiscoresPanelText.text = "Unlucky " + input + ", you didn't make the leaderboard this time.";
            setHiscoresText();
            priorPanelScript.turnOff();
            postPanelScript.turnOn();
            errorPanelScript.turnOff();
            if(VRMode)
                keyboardPanelScript.hideCanvasLocation();
        }
        else
        {
            errorPanelScript.turnOn();
        }
    }

    public bool updateHiscores(string playerName)
    {
        bool isUpdated = false;
        int[] hiscores = getHiscoresIntegerArray();
        string[] hiscoresNames = getHiscoresStringArray();
        int playerTime = timerScript.getTime();
        int tempNum, tempNum2;
        string tempString, tempString2;
        for (int i = 0; i < 10; i++)
        {
            if (hiscores[i] == -1)
            {
                isUpdated = true;
                hiscores[i] = playerTime;
                hiscoresNames[i] = playerName;
                break;
            }
            if (playerTime < hiscores[i])
            {
                isUpdated = true;
                tempNum = hiscores[i];
                tempString = hiscoresNames[i];
                hiscores[i] = playerTime;
                hiscoresNames[i] = playerName;
                for (int j = i + 1; j < 10; j++) // Loop through scores after and set num.
                {
                    tempNum2 = hiscores[j];
                    tempString2 = hiscoresNames[j];
                    hiscores[j] = tempNum;
                    hiscoresNames[j] = tempString;
                    tempNum = tempNum2;
                    tempString = tempString2;
                }
                break;
            }
        }
        setHiscoresArrays(hiscores, hiscoresNames);
        setHiscoresText();
        return isUpdated;
    }
    #endregion

    #region Set/Get Methods
    public void setHiscoresText()
    {
        string hiscoresDisplay = getHiscoresString();
        hiscoresMenuText.text = hiscoresDisplay;
        hiscoresPanelScore.text = hiscoresDisplay;
    }

    private string getHiscoresString()
    {
        string hiscoresDisplay = "";
        int[] hiscores = getHiscoresIntegerArray();
        string[] hiscoresNames = getHiscoresStringArray();

        for (int i = 1; i <= 10; i++)
        {
            if (!hiscoresNames[i - 1].Equals(""))
                hiscoresDisplay += hiscoresNames[i - 1] + ": ";
            else
                hiscoresDisplay += "---: ";
            if (hiscores[i - 1] != -1)
                hiscoresDisplay += timerScript.formatTime(hiscores[i - 1]);
            else
                hiscoresDisplay += "--:--";
            if (i != 10)
                hiscoresDisplay += "\n";
        }
        return hiscoresDisplay;
    }

    public void setHiscoresArrays(int[] newHiscores, string[] newHiscoresNames)
    {
        for (int i = 1; i <= 10; i++)
        {
            PlayerPrefs.SetInt("hiscore" + i, newHiscores[i - 1]);
            PlayerPrefs.SetString("hiscoreName" + i, newHiscoresNames[i - 1]);
        }
    }

    private int[] getHiscoresIntegerArray()
    {
        int[] hiscores = new int[10];
        for (int i = 1; i <= 10; i++)
        {
            if (PlayerPrefs.HasKey("hiscore" + i))
                hiscores[i - 1] = PlayerPrefs.GetInt("hiscore" + i);
            else
                hiscores[i - 1] = -1;
        }
        return hiscores;
    }

    private string[] getHiscoresStringArray()
    {
        string[] hiscoresNames = new string[10];
        for (int i = 1; i <= 10; i++)
        {
            if (PlayerPrefs.HasKey("hiscoreName" + i))
                hiscoresNames[i - 1] = PlayerPrefs.GetString("hiscoreName" + i);
            else
                hiscoresNames[i - 1] = "";
        }
        return hiscoresNames;
    }
    #endregion
}
