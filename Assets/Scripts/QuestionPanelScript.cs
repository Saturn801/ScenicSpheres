using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class QuestionPanelScript : MonoBehaviour {

    #region Public Variables
    public TextAsset questionJson1, questionJson2, questionJson3;
    #endregion

    #region Private Variables
    TextAsset asset;
    #endregion

    #region Script Specific Methods
    public void randomizeQuestion(int location)
    {
        var N = JSON.Parse(asset.text);
        int questionCount = N["locationQuestions"][location].Count;
        int selectedQuestion = UnityEngine.Random.Range(0, questionCount);
        setQuestionText(location, selectedQuestion);
        setButtonValues(location, selectedQuestion);
        setQuestionImage(location, selectedQuestion);
    }
    #endregion

    #region Set/Get Methods
    public void setSelectedArrow(GameObject arrow)
    {
        var buttons = GameObject.FindGameObjectsWithTag("AnswerButton");
        foreach (GameObject button in buttons)
        {
            var buttonScript = (QuestionScript)button.GetComponent(typeof(QuestionScript));
            buttonScript.setArrow(arrow);
        }
    }

    public void setJsonAsset(int countryIndex)
    {
        if (countryIndex == 0)
            asset = questionJson1;
        else if (countryIndex == 1)
            asset = questionJson2;
        else if (countryIndex == 2)
            asset = questionJson3;
    }

    public void setQuestionText(int location, int selectedQuestion)
    {
        var N = JSON.Parse(asset.text);
        string question = N["locationQuestions"][location][selectedQuestion];
        var questionTextObject = GameObject.FindGameObjectWithTag("QuestionText");
        var questionText = (Text)questionTextObject.GetComponent(typeof(Text));
        questionText.text = question;
    }

    public void setButtonValues(int location, int selectedQuestion)
    {
        var N = JSON.Parse(asset.text);
        var buttons = GameObject.FindGameObjectsWithTag("AnswerButton");
        int buttonIndex = 0;
        foreach (GameObject button in buttons)
        {
            var buttonText = (Text)button.GetComponentInChildren(typeof(Text));
            var buttonScript = (QuestionScript)button.GetComponent(typeof(QuestionScript));
            buttonText.text = N["locationAnswers"][location][selectedQuestion][buttonIndex];
            buttonScript.setCorrectValue(N["locationAnswerResults"][location][selectedQuestion][buttonIndex]);
            buttonIndex++;
        }
    }

    public void setQuestionImage(int location, int selectedQuestion)
    {
        var N = JSON.Parse(asset.text);
        var questionImageObject = GameObject.FindGameObjectWithTag("QuestionImage");
        var questionImage = (Image)questionImageObject.GetComponent(typeof(Image));
        questionImage.sprite = Resources.Load<Sprite>(N["locationImages"][location][selectedQuestion]);
    }
    #endregion
}
