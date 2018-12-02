using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInputField : MonoBehaviour {

    #region Variables
    public PanelScript priorInput;
    public Launcher launcher;
    public Text errorText, connectingText;
    static string playerNamePrefKey = "PlayerName";
    #endregion

    #region MonoBehaviour Methods
    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    void Start()
    {
        string defaultName = "";
        InputField _inputField = this.GetComponent<InputField>();
        if (_inputField != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey.ToUpper());
                _inputField.text = defaultName;
            }
        }
        PhotonNetwork.playerName = defaultName;
    }
    #endregion

    #region Script Specific Methods
    /// <summary>
    /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
    /// </summary>
    /// <param name="value">The name of the Player</param>
    public void SetPlayerName(string value)
    {
        value = value.ToUpper();
        // #Important
        PhotonNetwork.playerName = value; 
        PlayerPrefs.SetString(playerNamePrefKey, value);
    }

    public void submitButtonCall()
    {
        InputField _inputField = this.GetComponent<InputField>();
        if (isInputValid(_inputField.text))
        {
            launcher.Connect();
            priorInput.turnOff();
            connectingText.text = "Connecting...";
        }        
        else
            errorText.text = "Please enter 3 letters\nas your name";
    }
    #endregion

    #region Helper Methods
    public bool isInputValid(string input)
    {
        input = input.ToUpper();
        if (input.Length != 3)
            return false;
        else
        {
            char[] inputArray = input.ToCharArray();
            for (int i = 0; i < 3; i++)
            {
                if (inputArray[i] < 65 || inputArray[i] > 90)
                {
                    return false;
                }
            }
            return true;
        }
    }
    #endregion
}
