using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class TimerScript : MonoBehaviour {

    #region Private Variables
    Text timeText;
    private int time;
    #endregion

    #region MonoBehaviour Methods
    // Use this for initialization
    void Start()
    {
        timeText = (Text)GetComponent(typeof(Text));
    }
    #endregion

    #region Script Specific Methods
    public void startTimer()
    {
        time = 0;
        InvokeRepeating("updateTime", 0.0f, 1.0f);
    }

    public void stopTimer()
    {
        timeText.text = "";
        CancelInvoke();
    }

    private void updateTime()
    {
        time += 1;
        timeText.text = formatTime(time);
    }
    #endregion

    #region Helper Methods
    public string formatTime(int timeToFormat)
    {
        int minutes, seconds;
        minutes = timeToFormat / 60;
        seconds = timeToFormat % 60;
        return minutes.ToString("D2") + ":" + seconds.ToString("D2");
    }
    #endregion

    #region Set/Get Methods
    public int getTime()
    {
        return time;
    }

    public string getTimeString()
    {
        return formatTime(time);
    }

    public void setTime(int newTime)
    {
        time = newTime;
        timeText.text = formatTime(time);
    }
    #endregion
}
