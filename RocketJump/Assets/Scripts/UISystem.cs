using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class UISystem : MonoBehaviour {
    //Statistics in game
    [SerializeField] private Text timeText;
    [SerializeField] private Text levelNrText;

    //Panels
    [SerializeField] private GameObject PPause;
    [SerializeField] private GameObject POptions, PCredits;

    [SerializeField] private GameSystem gameSystem;
    [SerializeField] private CameraSystem cameraSystem;
    [SerializeField] private SoundSystem soundSystem;
    private MusicBackground musicBackground = MusicBackground.instance;

    [HideInInspector] public float timeValue;
    [HideInInspector] public bool isPaused;
    [HideInInspector] public bool gameComplete;

    //All text to translate
    [SerializeField] private Text time, jumps, falls, deaths, wiseMovement, playerSkin, secretRoom;
    [SerializeField] private Text Ttime, Tjumps, Tfalls, Tdeaths, Tresolution, TfullScreen, Tlanguage, Tsounds, Tmusic, TnewGame, TloadGame, TwarningLine1, TwarningLine2, Tyes, Tno, Texit, TtrailerMusic, TsoundsInGame, TsoundsInGame_Info, Tgraphics, TBack;
    [SerializeField] private Text[] TmainMenu, Toptions, Tcredits;

    [SerializeField] private Animator newGameWarning;

    //Info panel
    [SerializeField] private Animator PInfo;
    [SerializeField] private Image PInfo_icon;
    [SerializeField] private Sprite[] PInfo_icons;
    [SerializeField] private Text PInfo_text;

    //Circle
    public Animator circle;
    public Color32[] circleColor;

    public Transform playerT;
    public Animator gameCompletedAnim;

    //Settings
    private int _resolutionID;
    private string[] resolutions = new string[10] { "1920x1080", "1280x720", "1600x900", "1024x576", "960x720", "1024x768", "1600x1200", "1920x1440", "3840x2160", "4096x2160" };

    [SerializeField] private Slider soundSlider, musicSlider;
    [SerializeField] private GameObject[] languageFlags;
    public int languageID;

    public Text TResolution;
    public Toggle TFullScreen;


    private void Start() {
        if(SceneManager.GetActiveScene().name != "Game") {
            Time.timeScale = 1;
            ShowCircle(false);
        }
        isPaused = false;
        LoadSettings();
    }

    private void Update() {
        if(SceneManager.GetActiveScene().name != "Game") { return; }

        ChangeTime();
        if(Input.GetKeyDown(KeyCode.Escape) && !gameComplete) {
            PauseGame();
        }

        void ChangeTime() {
            if(gameComplete) { return; }

            timeValue += Time.deltaTime;

            int hour = (int)timeValue / 3600;
            int minute = (int)((timeValue / 60) - (hour * 60));
            int second = (int)timeValue % 60;

            string timeMinute = "00";
            string timeSecond = "00";

            if(hour > 0) {
                if(minute < 10) {
                    timeMinute = "0" + minute;
                } else {
                    timeMinute = minute.ToString();
                }
                if(second < 10) {
                    timeSecond = "0" + second;
                } else {
                    timeSecond = second.ToString();
                }

                timeText.text = hour + ":" + timeMinute + ":" + timeSecond;
            } else {
                if(minute < 10) {
                    timeMinute = "0" + minute;
                } else {
                    timeMinute = minute.ToString();
                }
                if(second < 10) {
                    timeSecond = "0" + second;
                } else {
                    timeSecond = second.ToString();
                }
                timeText.text = timeMinute + ":" + timeSecond;
            }
        }
    }

    public void PauseGame() {
        isPaused = !isPaused;

        if(isPaused) {
            Time.timeScale = 0f;
            PPause.SetActive(true);
        } else {
            Time.timeScale = 1f;
            PPause.SetActive(false);
        }
    }

    public void StartNewGame(int value) {
        switch(value) {
            case 0:
                if(FileManager.LoadData("currentGame/time") != "0") {
                    newGameWarning.SetBool("Show", true);
                } else {
                    FileManager.StartNewGame();
                    LoadGame();
                }
                break;
            case 1:
                FileManager.StartNewGame();
                LoadGame();
                break;
            case 2:
                newGameWarning.SetBool("Show", false);
                break;
        }
    }
    public void LoadGame() {
        if(FileManager.LoadData("currentGame/levelIsCompleted") == "1") {
            StartNewGame(1);
            return;
        }

        ShowCircle(true);

        StartCoroutine(wait());
        IEnumerator wait() {
            yield return new WaitForSecondsRealtime(0.4f);
            SceneManager.LoadScene("Game");
        }
    }
    public void ExitGame() {
        Application.Quit();
    }


    public void ChangeLevelNr(int levelNr) {
        levelNrText.text = levelNr.ToString(); ;
    }
    public void ShowCircle(bool value, int circleColorID = -1) {
        if(SceneManager.GetActiveScene().name == "Game") {
            Time.timeScale = 0f;
        }

        ChangeCircleColor();

        if(value) {
            circle.transform.localScale = Vector3.zero;
        } else {
            circle.transform.localScale = new Vector3(50, 50, 0);
        }

        circle.gameObject.SetActive(true);

        if(SceneManager.GetActiveScene().name == "Game") {
            Vector2 playerPos = playerT.position;
            Vector2 viewportPoint = Camera.main.WorldToViewportPoint(playerPos);

            circle.GetComponent<RectTransform>().anchorMin = viewportPoint;
            circle.GetComponent<RectTransform>().anchorMax = viewportPoint;
        }

        if(value) {
            circle.Play("ShowCircle");
        } else {
            circle.Play("HideCircle");

            StartCoroutine(wait());
            IEnumerator wait() {
                yield return new WaitForSeconds(3f);
                circle.gameObject.SetActive(false);
            }
        }

        if(!value) {
            StartCoroutine(wait2());
            IEnumerator wait2() {
                yield return new WaitForSecondsRealtime(0.8f);
                Time.timeScale = 1f;
            }
        }

        void ChangeCircleColor() {
            if(circleColorID == -1) {
                float posY = float.Parse(FileManager.LoadData("currentGame/player/posY"));

                int i = (int)posY / 154;
                circle.gameObject.GetComponent<Image>().color = circleColor[i];
            } else {
                circle.gameObject.GetComponent<Image>().color = circleColor[circleColorID];
            }
        }
    }
    public void ShowOptions(bool value) {
        POptions.SetActive(value);
        if(!value) {
            SaveSettings();
        }
    }
    public void ShowCredits(bool value) {
        PCredits.SetActive(value);
    }


    public void ShowGameCompleted() {
        isPaused = true;
        SetTime();
        jumps.text = SteamAchievements.JumpAmountCurrentGame.ToString();
        falls.text = SteamAchievements.FallAmountCurrentGame.ToString();
        deaths.text = SteamAchievements.DeathAmountCurrentGame.ToString();

        wiseMovement.text = CountData("wiseMovement", 3) + " / 3";
        secretRoom.text = CountData("secretRoom", 4) + " / 4";
        playerSkin.text = CountData("skin", 12) + " / 12";

        ShowCircle(true, 2);
        gameCompletedAnim.gameObject.SetActive(true);
        gameCompletedAnim.Play("GameCompleted");

        void SetTime() {
            int hour = (int)timeValue / 3600;
            int minute = (int)timeValue / 60;
            int second = (int)timeValue % 60;

            string timeMinute = "00";
            string timeSecond = "00";

            if(hour > 0) {
                if(minute < 10) {
                    timeMinute = "0" + minute;
                } else {
                    timeMinute = minute.ToString();
                }
                if(second < 10) {
                    timeSecond = "0" + second;
                } else {
                    timeSecond = second.ToString();
                }

                time.text = hour + ":" + timeMinute + ":" + timeSecond;
            } else {
                if(minute < 10) {
                    timeMinute = "0" + minute;
                } else {
                    timeMinute = minute.ToString();
                }
                if(second < 10) {
                    timeSecond = "0" + second;
                } else {
                    timeSecond = second.ToString();
                }
                time.text = timeMinute + ":" + timeSecond;
            }
        }

        int CountData(string elementName, int maxValue) {
            int counter = 0;
            for(int i = 0; i < maxValue; i++) {
                if(int.Parse(FileManager.LoadData("currentGame/" + elementName + i)) == 0) {
                    counter++;
                }
            }

            return counter;
        }
    }

    public void GoToMainMenu() {
        gameSystem.SaveGame();
        ShowCircle(true);

        StartCoroutine(wait());
        IEnumerator wait() {
            yield return new WaitForSecondsRealtime(0.8f);
            SceneManager.LoadScene("MainMenu");
        }
    }
    public void SaveGame() {
        gameSystem.SaveGame();
    }

    public void ShowPInfo(int infoID, int currentLevel = 0) {
        PInfo.Play("ShowPInfo");

        if(infoID == 1) //skin
        {
            PInfo_text.text = FileManager.CountData("skin") + "/12";
            PInfo_icon.sprite = PInfo_icons[0];

        } else if(infoID == 2) //secretRoom
          {
            PInfo_text.text = FileManager.CountData("secretRoom") + "/4";
            PInfo_icon.sprite = PInfo_icons[1];

        } else if(infoID == 3) //wiseMovement
          {
            PInfo_text.text = FileManager.CountData("wiseMovement") + "/3";
            PInfo_icon.sprite = PInfo_icons[2];
        }
    }

    //***----------------------------------------SETTINGS----------------------------------------***//
    public void LoadSettings() {
        ResolutionID = int.Parse(FileManager.LoadData("settings/resolutionID"));
        TResolution.text = resolutions[ResolutionID];

        if(FileManager.LoadData("settings/fullScreen") == "1") {
            TFullScreen.isOn = true;
        } else {
            TFullScreen.isOn = false;
        }
        ChangeScreenResolution();

        if(PlayerPrefs.GetInt("FirstLoad") == 0) {
            languageID = SteamLanguage.GetSteamLanguageID();
            FileManager.SaveData("settings/languageID", languageID);
        } else {
            languageID = int.Parse(FileManager.LoadData("settings/languageID"));
        }

        MusicBackground.instance.ChangeMusicVolume(float.Parse(FileManager.LoadData("settings/musicVolume")));
        musicSlider.value = float.Parse(FileManager.LoadData("settings/musicVolume"));
        soundSystem.soundVolume = float.Parse(FileManager.LoadData("settings/soundVolume"));
        soundSlider.value = soundSystem.soundVolume;

        ChangeLanguage();
    }
    public void SaveSettings() {
        ChangeScreenResolution();
        FileManager.SaveData("settings/resolutionID", ResolutionID);

        if(TFullScreen.isOn) {
            FileManager.SaveData("settings/fullScreen", 1);
        } else {
            FileManager.SaveData("settings/fullScreen", 0);
        }

        FileManager.SaveData("settings/soundVolume", soundSystem.soundVolume);

        FileManager.SaveData("settings/musicVolume", musicBackground.GetVolume());
    }

    //Resolution
    public void ChangeScreenResolution() {
        int xIndex = resolutions[ResolutionID].IndexOf("x");
        int.TryParse(resolutions[ResolutionID].Substring(0, xIndex), out int x);
        int.TryParse(resolutions[ResolutionID].Substring(xIndex + 1), out int y);

        Screen.SetResolution(x, y, TFullScreen.isOn);

        if(SceneManager.GetActiveScene().name != "Game") { return; }
        if(ResolutionID == 4 || ResolutionID == 5 || ResolutionID == 6) {
            cameraSystem.MaxXPos = 20f;
            cameraSystem.MinXPos = -4f;
        } else {
            cameraSystem.MaxXPos = 18f;
            cameraSystem.MinXPos = -2f;
        }
    }
    public void ChangeResolutionID(bool next) {
        if(next) {
            ResolutionID++;
        } else {
            ResolutionID--;
        }
        TResolution.text = resolutions[ResolutionID];
    }

    //Audio
    public void SetAudioVolume(float vol) {
        soundSystem.soundVolume = vol;
    }
    public void SetMusicVolume(float vol) {
        if(musicBackground == null) {
            musicBackground = MusicBackground.instance;
        }

        musicBackground.ChangeMusicVolume(vol);
    }

    //Language
    public void SetLanguage(int languageID) {
        this.languageID = languageID;
        ChangeLanguage();
        FileManager.SaveData("settings/languageID", languageID);
    }
    public void ChangeLanguage(int languageID) {
        if(SceneManager.GetActiveScene().name == "Game") {
            Ttime.text = TranslateSystem.LoadValue("time");
            Tjumps.text = TranslateSystem.LoadValue("jumps");
            Tfalls.text = TranslateSystem.LoadValue("falls");
            Tdeaths.text = TranslateSystem.LoadValue("deaths");
            TmainMenu[1].text = TranslateSystem.LoadValue("mainMenu");
            TBack.text = TranslateSystem.LoadValue("back");
        } else {
            TnewGame.text = TranslateSystem.LoadValue("newGame");
            TloadGame.text = TranslateSystem.LoadValue("loadGame");
            Tyes.text = TranslateSystem.LoadValue("yes");
            Tno.text = TranslateSystem.LoadValue("no");
            TwarningLine1.text = TranslateSystem.LoadValue("newGameWarningLine1");
            TwarningLine2.text = TranslateSystem.LoadValue("newGameWarningLine2");
            Texit.text = TranslateSystem.LoadValue("exit");
            TmainMenu[1].text = TranslateSystem.LoadValue("mainMenu");
            Tcredits[0].text = TranslateSystem.LoadValue("credits");
            Tcredits[1].text = TranslateSystem.LoadValue("credits");
            TtrailerMusic.text = TranslateSystem.LoadValue("trailerMusic");
            TsoundsInGame.text = TranslateSystem.LoadValue("soundsInGame");
            TsoundsInGame_Info.text = TranslateSystem.LoadValue("soundsInGame-Info");
            Tgraphics.text = TranslateSystem.LoadValue("graphics");
        }

        Tresolution.text = TranslateSystem.LoadValue("resolution");
        TfullScreen.text = TranslateSystem.LoadValue("fullScreen");
        TmainMenu[0].text = TranslateSystem.LoadValue("mainMenu");
        Toptions[0].text = TranslateSystem.LoadValue("options");
        Toptions[1].text = TranslateSystem.LoadValue("options");
        Tlanguage.text = TranslateSystem.LoadValue("language");
        Tsounds.text = TranslateSystem.LoadValue("sounds");
        Tmusic.text = TranslateSystem.LoadValue("music");
    }
    public void ChangeLanguage() {
        for(int i = 0; i < languageFlags.Length; i++) {
            languageFlags[i].transform.localScale = Vector3.one;
        }
        languageFlags[languageID].transform.localScale = new Vector3(1.2f, 1.2f, 1f);

        if(languageID == 0) {
            TranslateSystem.localeID = "en_GB";
        } else if(languageID == 1) {
            TranslateSystem.localeID = "pl_PL";
        } else if(languageID == 2) {
            TranslateSystem.localeID = "de_DE";
        } else if(languageID == 3) {
            TranslateSystem.localeID = "es_ES";
        } else if(languageID == 4) {
            TranslateSystem.localeID = "fr_FR";
        }

        ChangeLanguage(languageID);
    }

    public int ResolutionID {
        set {
            _resolutionID = value;
            if(_resolutionID >= 10) {
                _resolutionID = 0;
            }
            if(_resolutionID < 0) {
                _resolutionID = 9;
            }
        }
        get {
            return _resolutionID;
        }
    }
}
