using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class MapScript : MonoBehaviour
{
    #region Public Variables
    public Text locationText, questionsRemainingText, progressText;
    public PanelScript loadingPanelScript, canvasPanelScript;
    public QuestionPanelScript questionScript;
    public InfoScript infoScript;
    public TimerScript timerScript;
    public Image progressBar;
    public AudioSource menuSound, gameMusic1, gameMusic2, gameMusic3;
    public TextAsset mapJson1, mapJson2, mapJson3;
    public GameObject arrow, flag1, flag2, flag3, infoIcon;
    public Material background;
    public int countryCount;
    public bool VRMode, MultiplayerMode;
    #endregion

    #region Private Variables
    GameObject flag;
    GameObject[] arrows;
    Material[] locationTextures;
    TextAsset asset;
    AudioSource gameSound;
    private int currLocation, remainingQuestions, locationCount, loadedLocations, flagLocation;
    private IEnumerator mapCoroutine;
    private bool mapLoaded;
    #endregion

    #region MonoBehaviour Methods
    // Use this for initialization
    void Start()
    {
        resetSkybox();
        menuSound.Play();
    }
    #endregion

    #region Google Maps Methods
    public void loadLocations()
    {
        mapCoroutine = GetGoogleMap();
        StartCoroutine(mapCoroutine);
    }

    IEnumerator GetGoogleMap()
    {
        var N = JSON.Parse(asset.text);
        string location = N["locationCoordinates"][loadedLocations];
        string url1, url2, url3, url4, url5, url6;
        url1 = "https://maps.googleapis.com/maps/api/streetview?size=640x640&location=" + location + "&heading=0&pitch=0&key=AIzaSyBiesSOJEkxe2FeFPH7ELi28Pb6fnV3LBE";
        url2 = "https://maps.googleapis.com/maps/api/streetview?size=640x640&location=" + location + "&heading=90&pitch=0&key=AIzaSyBiesSOJEkxe2FeFPH7ELi28Pb6fnV3LBE";
        url3 = "https://maps.googleapis.com/maps/api/streetview?size=640x640&location=" + location + "&heading=180&pitch=0&key=AIzaSyBiesSOJEkxe2FeFPH7ELi28Pb6fnV3LBE";
        url4 = "https://maps.googleapis.com/maps/api/streetview?size=640x640&location=" + location + "&heading=270&pitch=0&key=AIzaSyBiesSOJEkxe2FeFPH7ELi28Pb6fnV3LBE";
        url5 = "https://maps.googleapis.com/maps/api/streetview?size=640x640&location=" + location + "&heading=0&pitch=90&key=AIzaSyBiesSOJEkxe2FeFPH7ELi28Pb6fnV3LBE";
        url6 = "https://maps.googleapis.com/maps/api/streetview?size=640x640&location=" + location + "&heading=0&pitch=-90&key=AIzaSyBiesSOJEkxe2FeFPH7ELi28Pb6fnV3LBE";

        WWW[] www = new WWW[6];

        www[0] = new WWW(url1);
        yield return www[0];
        www[1] = new WWW(url2);
        yield return www[1];
        www[2] = new WWW(url3);
        yield return www[2];
        www[3] = new WWW(url4);
        yield return www[4];
        www[4] = new WWW(url5);
        yield return www[4];
        www[5] = new WWW(url6);
        yield return www[5];

        Material result = createPanoramaMaterial(www);
        locationTextures[loadedLocations] = result;
        loadedLocations++;
        progressBar.fillAmount = (float)loadedLocations / (float)locationCount;
        progressText.text = loadedLocations + "/" + locationCount;
        if (loadedLocations == locationCount)
        {
            mapLoaded = true;                 
            yield return new WaitForSeconds(0.5F);
            toggleGameMusic();
            toggleMenuMusic();
            timerScript.startTimer();
            loadingPanelScript.turnOff();
            if(!MultiplayerMode)
                setLocation();
            if (VRMode)
            {
                infoIcon.SetActive(true);
                canvasPanelScript.hideCanvasLocation();
            }              
            StopCoroutine(mapCoroutine);
        }
        else
        {
            loadLocations();
        }
    }

    private Material createPanoramaMaterial(WWW[] www)
    {
        Material result = new Material(Shader.Find("Skybox/6 Sided"));
        result.SetTexture("_FrontTex", www[0].texture);
        result.SetTexture("_LeftTex", www[1].texture);
        result.SetTexture("_BackTex", www[2].texture);
        result.SetTexture("_RightTex", www[3].texture);
        result.SetTexture("_UpTex", www[4].texture);
        result.SetTexture("_DownTex", www[5].texture);
        return result;
    }
    #endregion

    #region Script Specific Methods
    public void startGame(int index)
    {
        mapLoaded = false;
        flagLocation = -1;
        if (index == -1)
            index = randomizeCountry();
        setGameAssets(index);
        questionScript.setJsonAsset(index);
        infoScript.setJsonAsset(index);
        var N = JSON.Parse(asset.text);
        locationCount = N["locations"].Count;
        locationTextures = new Material[locationCount];
        loadedLocations = 0;
        remainingQuestions = 5;
        progressBar.fillAmount = 0;
        progressText.text = "0/" + locationCount;
        randomizeLocation();
        loadLocations();
    }

    public void decreaseQuestionCount()
    {
        remainingQuestions--;
        setRemainingQuestionsText();
        if (remainingQuestions == 0)
            spawnFlag();
    }

    public int randomizeCountry()
    {
        return UnityEngine.Random.Range(0, countryCount);
    }

    public void randomizeLocation()
    {
        currLocation = UnityEngine.Random.Range(0, locationCount);
    }
    #endregion

    #region Toggle Methods
    public void toggleMenuMusic()
    {
        if (menuSound.isPlaying)
            menuSound.Stop();
        else
            menuSound.Play();
    }

    public void toggleGameMusic()
    {
        if (gameSound.isPlaying)
            gameSound.Stop();
        else
            gameSound.Play();
    }

    public void toggleElements()
    {
        toggleLocationText();
        toggleArrows();
        toggleInfoIcon();
    }

    public void toggleLocationText()
    {
        if (locationText.text.Equals(""))
            updateLocationText();
        else
            locationText.text = "";
    }

    public void toggleArrows()
    {
        foreach (var arrow in arrows)
        {
            arrow.SetActive(!arrow.activeInHierarchy);
        }
    }

    public void toggleInfoIcon()
    {
        infoIcon.SetActive(!infoIcon.activeInHierarchy);
    }

    public void hideRemainingQuestionsText()
    {
        questionsRemainingText.text = "";
    }
    #endregion

    #region Set/Get Methods
    public void setLocation()
    {
        RenderSettings.skybox = locationTextures[currLocation];
        setRemainingQuestionsText();
        updateLocationText();
        createArrows();
        arrows = GameObject.FindGameObjectsWithTag("Arrow");
        if (flagLocation != -1)
        {
            if (flagLocation == currLocation)
                flag.SetActive(true);
            else
                flag.SetActive(false);
        }
    }

    public void resetSkybox()
    {
        RenderSettings.skybox = background;
    }

    public void setGameAssets(int index)
    {
        if (index == 0)
        {
            asset = mapJson1;
            flag = flag1;
            gameSound = gameMusic1;
        }           
        else if (index == 1)
        {
            asset = mapJson2;
            flag = flag2;
            gameSound = gameMusic2;
        }
        else if (index == 2)
        {
            asset = mapJson3;
            flag = flag3;
            gameSound = gameMusic3;
        }
    }

    public void updateLocationText()
    {
        var N = JSON.Parse(asset.text);
        locationText.text = N["locations"][currLocation];
    }

    public void updateLocationIndex(int newIndex)
    {
        currLocation = newIndex;
    }

    public void setRemainingQuestionsText()
    {
        if (remainingQuestions > 0)
        {
            questionsRemainingText.text = "Questions Remaining: " + remainingQuestions;
        }
        else
        {
            questionsRemainingText.text = "Capture the Flag!";
        }
    }

    public int getCurrLocation()
    {
        return currLocation;
    }

    public bool getMapLoadedStatus()
    {
        return mapLoaded;
    }
    #endregion

    #region GameObject Creation/Deletion Methods
    public void spawnFlag()
    {
        var random = UnityEngine.Random.insideUnitCircle.normalized * 10;
        var newFlag = Instantiate(flag, new Vector3(random.x, 0, random.y), Quaternion.Euler(0, 0, 0));
        newFlag.transform.LookAt(Camera.main.transform);
        newFlag.transform.rotation = newFlag.transform.rotation * Quaternion.AngleAxis(90, Vector3.up);
        flag = newFlag;
        do
        {
            flagLocation = UnityEngine.Random.Range(0, locationCount);
        } while (flagLocation == currLocation);
        flag.SetActive(false);
    }

    public void createArrows()
    {
        var N = JSON.Parse(asset.text);
        int arrowCount = N["arrowCoordinates"][currLocation].Count;
        for (int i = 0; i < arrowCount; i++)
        {
            string coordinates = "";
            if(VRMode)
                coordinates = N["arrowVRCoordinates"][currLocation][i];
            else
                coordinates = N["arrowCoordinates"][currLocation][i];
            string[] values = coordinates.Split(',');
            int x, y, z, rotation;
            x = Int32.Parse(values[0]);
            y = Int32.Parse(values[1]);
            z = Int32.Parse(values[2]);
            rotation = Int32.Parse(values[3]);
            var newArrow = Instantiate(arrow, new Vector3(x, y, z), Quaternion.Euler(0, rotation, 0));
            ArrowScript script = newArrow.GetComponentInChildren<ArrowScript>();
            script.setText(N["arrowText"][currLocation][i]);
            script.setLocationIndex(N["arrowIndexes"][currLocation][i]);
        }
    }

    public void destroyArrows()
    {
        foreach (var arrow in arrows)
        {
            Destroy(arrow);
        }
    }
    #endregion   
}