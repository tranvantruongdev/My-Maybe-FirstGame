using BayatGames.SaveGameFree;
using BayatGames.SaveGameFree.Encoders;
using BayatGames.SaveGameFree.Serializers;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//Supress default null warning
#pragma warning disable 0649

namespace SpeedTutorMainMenuSystem
{
    public class MenuController : MonoBehaviour
    {
        #region Default Values
        [Header("Levels To Load")]
        public string stage1;
        public string stage2;
        public string stage3;
        private string levelToLoad; //use when load saved game

        private int menuNumber; //1 - default menu || 2 - option || 3 - graphics
        #endregion              //4 - sounds || 5 - gameplay || 6 - controls
        //7 - new game || 8 - load game || 9 - stage select

        #region Menu Dialogs
        [Header("Main Menu Components")]
        [SerializeField] private GameObject menuDefaultCanvas;
        [SerializeField] private GameObject confirmationBox;
        [SerializeField] private GameObject stageCanvas;
        [Space(10)]
        [Header("Menu Popout Dialogs")]
        [SerializeField] private GameObject noSaveDialog;
        [SerializeField] private GameObject newGameDialog;
        [SerializeField] private GameObject loadGameDialog;
        #endregion        

        #region Initialisation - Button Selection & Menu Order
        private void Start()
        {
            menuNumber = 1;

            try
            {
                var a = SaveGame.Load<DailyChallenge>(dailyIdentifier, null, encode, encodePassword,
                                                serializer, encoder, encoding, savePath);

                if (a.challengeDate.Date == System.DateTime.Now.Date &&
                    a.challengeDate.Month == System.DateTime.Now.Month &&
                    a.challengeDate.Year == System.DateTime.Now.Year)
                {
                    if (a.checkComplete == false)
                    {
                        challengeName.text = a.challengeName;
                        challengeProgress.text = string.Format("Progress: {0}/{1}", a.numberEliminated, a.numberRequired);
                        challengeProgress.enabled = true;
                    }
                    if (a.checkComplete == true)
                    {
                        challengeName.text = "You've complete Daily challenge";
                        challengeProgress.enabled = false;
                    }
                }
                else
                {
                    challengeName.text = "Your Challenge to day is:\nEliminate 15 monsters in one game at any difficulty";
                    challengeProgress.text = string.Format("Progress: 0/15");
                    challengeProgress.enabled = true;
                    //override data
                    SaveGame.Save<DailyChallenge>(
                    dailyIdentifier,
                    new DailyChallenge(),
                    encode,
                    encodePassword,
                    serializer,
                    encoder,
                    encoding,
                    savePath);
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e.ToString());
                challengeName.text = "Your Challenge to day is:\nEliminate 15 monsters in one game at any difficulty";
                challengeProgress.text = string.Format("Progress: 0/15");
                challengeProgress.enabled = true;
                //create new data
                SaveGame.Save<DailyChallenge>(
                    dailyIdentifier,
                    new DailyChallenge(),
                    encode,
                    encodePassword,
                    serializer,
                    encoder,
                    encoding,
                    savePath);
            }

        }
        #endregion

        //MAIN SECTION

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //new game, load game, stage select
                if (menuNumber == 7 || menuNumber == 8 || menuNumber == 9)
                {
                    GoBackToMainMenu();
                    ClickSound();
                }
            }
        }

        private void ClickSound()
        {
            GetComponent<AudioSource>().Play();
        }

        #region Menu Mouse Clicks
        public void MouseClick(string buttonType)
        {
            if (buttonType == "Exit")
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif

                Application.Quit();
            }

            if (buttonType == "LoadGame")
            {
                menuDefaultCanvas.SetActive(false);
                loadGameDialog.SetActive(true);
                menuNumber = 8;
            }

            if (buttonType == "NewGame")
            {
                menuDefaultCanvas.SetActive(false);
                newGameDialog.SetActive(true);
                menuNumber = 7;
                //set time scale back to normal in case return from pause menu or gameover
                Time.timeScale = 1;
            }
        }
        #endregion

        #region Dialog Options - This is where we load what has been saved in player prefs!
        public void ClickNewGameDialog(string ButtonType)
        {
            //Hide main menu and show stageCanvas
            //Choose Easy difficult
            if (ButtonType == "Easy")
            {
                GameSetting.loadType = GameSetting.LoadType.New;
                GameSetting.difficult = GameSetting.Difficult.Easy;
                menuDefaultCanvas.SetActive(false);
                stageCanvas.SetActive(true);
                menuNumber = 9;
            }

            //Choose Normal difficult
            if (ButtonType == "Normal")
            {
                GameSetting.loadType = GameSetting.LoadType.New;
                GameSetting.difficult = GameSetting.Difficult.Normal;
                menuDefaultCanvas.SetActive(false);
                stageCanvas.SetActive(true);
                menuNumber = 9;
            }

            //Choose Hard difficult
            if (ButtonType == "Hard")
            {
                GameSetting.loadType = GameSetting.LoadType.New;
                GameSetting.difficult = GameSetting.Difficult.Hard;
                menuDefaultCanvas.SetActive(false);
                stageCanvas.SetActive(true);
                menuNumber = 9;
            }

            if (ButtonType == "No")
            {
                GoBackToMainMenu();
            }

            if (ButtonType == "1")
            {
                SceneManager.LoadScene(stage1);
            }

            if (ButtonType == "2")
            {
                SceneManager.LoadScene(stage2);
            }

            if (ButtonType == "3")
            {
                SceneManager.LoadScene(stage3);
            }
        }

        public enum SaveFormat
        {
            JSON,
        }

        [Header("Save/Load Settings")]
        [Space]


        [Tooltip("stage path.")]
        /// <summary>
        /// The score identifier.
        /// </summary>
        public string stageIdentifier = "enter the stage identifier";

        [Tooltip("daily path.")]
        /// <summary>
        /// The score identifier.
        /// </summary>
        public string dailyIdentifier = "enter the daily identifier";

        [Tooltip("Encode the data?")]
        /// <summary>
        /// The encode.
        /// </summary>
        public bool encode = false;

        [Tooltip("If you leave it blank this will reset to it's default value.")]
        /// <summary>
        /// The encode password.
        /// </summary>
        public string encodePassword = "";

        [Tooltip("Which serialization format?")]
        public SaveFormat format = SaveFormat.JSON;

        [Tooltip("If you leave it blank this will reset to it's default value.")]
        /// <summary>
        /// The serializer.
        /// </summary>
        public ISaveGameSerializer serializer;

        [Tooltip("If you leave it blank this will reset to it's default value.")]
        /// <summary>
        /// The encoder.
        /// </summary>
        public ISaveGameEncoder encoder;

        [Tooltip("If you leave it blank this will reset to it's default value.")]
        /// <summary>
        /// The encoding.
        /// </summary>
        public Encoding encoding;

        [Tooltip("Where to save? (PersistentDataPath highly recommended).")]
        /// <summary>
        /// The save path.
        /// </summary>
        public SaveGamePath savePath = SaveGamePath.PersistentDataPath;

        [Tooltip("Reset the empty fields to their default value.")]
        /// <summary>
        /// The reset blanks.
        /// </summary>
        public bool resetBlanks = true;

        private void Awake()
        {
#if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#else
            Debug.unityLogger.logEnabled = false;
#endif
            if (resetBlanks)
            {
                if (string.IsNullOrEmpty(encodePassword))
                {
                    encodePassword = SaveGame.EncodePassword;
                }
                if (serializer == null)
                {
                    serializer = SaveGame.Serializer;
                }
                if (encoder == null)
                {
                    encoder = SaveGame.Encoder;
                }
                if (encoding == null)
                {
                    encoding = SaveGame.DefaultEncoding;
                }
            }
            switch (format)
            {
                case SaveFormat.JSON:
                    serializer = new SaveGameJsonSerializer();
                    break;
            }
        }

        public Text challengeName;
        public Text challengeProgress;

        public void ClickLoadGameDialog(string ButtonType)
        {
            if (ButtonType == "Yes")
            {
                GameSetting.loadType = GameSetting.LoadType.Load;

                if (GameSetting.loadType == GameSetting.LoadType.Load)
                {
                    Debug.Log("I WANT TO LOAD THE SAVED GAME");
                    levelToLoad = SaveGame.Load<string>(stageIdentifier, "", encode, encodePassword,
                                                        serializer, encoder, encoding, savePath);

                    //if dont have any saved scene then show dialog and return
                    if (levelToLoad == "" || levelToLoad == null)
                    {
                        loadGameDialog.SetActive(false);
                        noSaveDialog.SetActive(true);
                        return;
                    }
                    SceneManager.LoadScene(levelToLoad);
                }
                else
                {
                    Debug.Log("Load Game Dialog");
                    menuDefaultCanvas.SetActive(false);
                    loadGameDialog.SetActive(false);
                    noSaveDialog.SetActive(true);
                }
            }

            if (ButtonType == "No")
            {
                GoBackToMainMenu();
            }
        }
        #endregion

        #region Back to Menus        
        public void GoBackToMainMenu()
        {
            menuDefaultCanvas.SetActive(true);
            newGameDialog.SetActive(false);
            loadGameDialog.SetActive(false);
            noSaveDialog.SetActive(false);
            stageCanvas.SetActive(false);
            menuNumber = 1;
        }

        public void ClickNoSaveDialog()
        {
            GoBackToMainMenu();
        }
        #endregion
    }
}

public static class GameSetting
{
    public enum LoadType
    {
        New,
        Load
    }
    public enum Difficult
    {
        Easy,
        Normal,
        Hard
    }

    public static LoadType loadType;
    public static string username = "test";
    public static string uid = "123456";
    public static Difficult difficult;
}

public class DailyChallenge
{
    public System.DateTime challengeDate = System.DateTime.Now;
    public bool checkComplete = false;
    public string challengeName = "Your Challenge to day is:\nEliminate 15 monsters in one game at any difficulty";
    public int numberRequired = 15;
    public int numberEliminated = 0;
}