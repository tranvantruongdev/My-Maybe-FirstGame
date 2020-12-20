using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.SceneManagement;
//Supress default null warning
#pragma warning disable 0649

namespace GreatArcStudios
{
    public class PauseManager : MonoBehaviour
    {
        /// <summary>
        /// This is the main panel holder, which holds the main panel and should be called "main panel"
        /// </summary> 
        public GameObject mainPanel;
        /// <summary>
        /// This is the video panel holder, which holds all of the controls for the video panel and should be called "vid panel"
        /// </summary>
        public GameObject vidPanel;
        /// <summary>
        /// This is the audio panel holder, which holds all of the silders for the audio panel and should be called "audio panel"
        /// </summary>
        public GameObject audioPanel;
        /// <summary>
        /// These are the game objects with the title texts like "Pause menu" and "Game Title" 
        /// </summary>
        public GameObject TitleTexts;
        /// <summary>
        /// The mask that makes the scene darker  
        /// </summary>
        public GameObject mask;
        /// <summary>
        /// Audio Panel animator
        /// </summary>
        public Animator audioPanelAnimator;
        /// <summary>
        /// Video Panel animator  
        /// </summary>
        public Animator vidPanelAnimator;
        /// <summary>
        /// Quit Panel animator  
        /// </summary>
        public Animator quitPanelAnimator;
        /// <summary>
        /// Pause menu text 
        /// </summary>
        public Text pauseMenu;

        /// <summary>
        /// Main menu level string used for loading the main menu. This means you'll need to type in the editor text box, the name of the main menu level, ie: "mainmenu";
        /// </summary>
        public String mainMenu;
        /// <summary>
        /// The main camera, assign this through the editor. 
        /// </summary>        
        public Camera mainCam;
        internal static Camera mainCamShared;
        /// <summary>
        /// The main camera game object, assign this through the editor. 
        /// </summary> 
        public GameObject mainCamObj;

        /// <summary>
        /// Timescale value. The defualt is 1 for most games. You may want to change it if you are pausing the game in a slow motion situation 
        /// </summary> 
        public float timeScale = 1f;
        /// <summary>
        /// Inital shadow distance 
        /// </summary>
        internal static float shadowDistINI;
        /// <summary>
        /// Inital AA quality 2, 4, or 8
        /// </summary>
        internal static float aaQualINI;

        /// <summary>
        /// Inital fov 
        /// </summary>
        internal static float fovINI;
        /// <summary>
        /// Inital msaa amount 
        /// </summary>
        internal static int msaaINI;
        /// <summary>
        /// AA drop down menu.
        /// </summary>
        public Dropdown aaCombo;
        /// <summary>
        /// Aniso drop down menu.
        /// </summary>
        public Dropdown afCombo;

        public Slider fovSlider;
        public Slider audioMasterSlider;
        public Slider masterTexSlider;

        [SerializeField] private GameManager gameManager;

        /// <summary>
        /// The preset text label.
        /// </summary>
        public Text presetLabel;
        /// <summary>
        /// Lod bias float array. You should manually assign these based on the quality level.
        /// </summary>
        public  float[] LODBias;
        /// <summary>
        /// Shadow distance array. You should manually assign these based on the quality level.
        /// </summary>
        public  float[] shadowDist;
        /// <summary>
        /// An array of music audio sources
        /// </summary>
        public  AudioSource[] music;
        /// <summary>
        /// An array of sound effect audio sources
        /// </summary>
        public  AudioSource[] effects;
        /// <summary>
        /// An array of the other UI elements, which is used for disabling the other elements when the game is paused.
        /// </summary>
        public GameObject[] otherUIElements;
        /// <summary>
        /// Editor boolean for hardcoding certain video settings. It will allow you to use the values defined in LOD Bias and Shadow Distance
        /// </summary>
        public Boolean hardCodeSomeVideoSettings;
        /// <summary>
        /// Event system
        /// </summary>
        public EventSystem uiEventSystem;
        /// <summary>
        /// Defualt selected on the video panel
        /// </summary>
        public GameObject defualtSelectedVideo;
        /// <summary>
        /// Defualt selected on the video panel
        /// </summary>
        public GameObject defualtSelectedAudio;
        /// <summary>
        /// Defualt selected on the video panel
        /// </summary>
        public GameObject defualtSelectedMain;
        //last music multiplier; this should be a value between 0-1
        internal static float lastMusicMult;
        //last audio multiplier; this should be a value between 0-1
        internal static float lastAudioMult;
        //Initial master volume
        internal static float beforeMaster;
        //last texture limit 
        internal static int lastTexLimit;
        //int for amount of effects
        private int _audioEffectAmt = 0;
        //Inital audio effect volumes
        private float[] _beforeEffectVol;

        //Initial music volume
        private float _beforeMusic;
        //Preset level
        private int _currentLevel;
        //Presets 
        private String[] presets;

        private SaveSettings saveSettings = new SaveSettings();

        [SerializeField] GameObject gunScript;

        private void Awake()
        {
#if UNITY_EDITOR
			Debug.unityLogger.logEnabled = true;
#else
            Debug.unityLogger.logEnabled = false;
#endif
        }

        /// <summary>
        /// The start method; you will need to place all of your inital value getting/setting here. 
        /// </summary>
        public void Start()
        {           
            mainCamShared = mainCam;
            //Set the first selected item
            uiEventSystem.firstSelectedGameObject = defualtSelectedMain;
            //Get the presets from the quality settings 
            presets = QualitySettings.names;
            presetLabel.text = presets[QualitySettings.GetQualityLevel()].ToString();
            _currentLevel = QualitySettings.GetQualityLevel();
            //get all specified audio source volumes
            _beforeEffectVol = new float[_audioEffectAmt];
            beforeMaster = AudioListener.volume;
            //get all ini values
            aaQualINI = QualitySettings.antiAliasing;
            shadowDistINI = QualitySettings.shadowDistance;
            fovINI = mainCam.fieldOfView;
            msaaINI = QualitySettings.antiAliasing;
            //enable titles
            TitleTexts.SetActive(true);
            //Disable other panels
            mainPanel.SetActive(false);
            vidPanel.SetActive(false);
            audioPanel.SetActive(false);
            //Enable mask
            mask.SetActive(false);
            //set last texture limit
            lastTexLimit = QualitySettings.masterTextureLimit;
            saveSettings.LoadGameSettings(File.ReadAllText(Application.persistentDataPath + "/" + saveSettings.fileName));
        }
        /// <summary>
        /// Restart the level by loading the loaded level.
        /// </summary>
        public void Restart()
        {
            //make the reload stage is new prevent load game -> restart -> start new scene with save value
            GameSetting.loadType = GameSetting.LoadType.New;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            uiEventSystem.firstSelectedGameObject = defualtSelectedMain;
            Time.timeScale = 1;
        }
        /// <summary>
        /// Method to resume the game, so disable the pause menu and re-enable all other ui elements
        /// </summary>
        public void Resume()
        {
            Time.timeScale = timeScale;

            mainPanel.SetActive(false);
            vidPanel.SetActive(false);
            audioPanel.SetActive(false);
            TitleTexts.SetActive(false);
            mask.SetActive(false);
            for (int i = 0; i < otherUIElements.Length; i++)
            {
                otherUIElements[i].gameObject.SetActive(true);
            }

            //My code
            //Enable gun script
            gunScript.GetComponent<HandgunScriptLPFP>().enabled = true;
        }
        /// <summary>
        /// All the methods relating to qutting should be called here.
        /// </summary>
        public void quitOptions()
        {
            vidPanel.SetActive(false);
            audioPanel.SetActive(false);
            quitPanelAnimator.enabled = true;
            quitPanelAnimator.Play("QuitPanelIn");
        }
        /// <summary>
        /// Method to quit the game. Call methods such as auto saving before qutting here.
        /// </summary>
        public void quitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        public void SaveAndQuitGame()
        {
            gameManager.Save();
            quitGame();
        }
        /// <summary>
        /// Cancels quittting by playing an animation.
        /// </summary>
        public void quitCancel()
        {
            quitPanelAnimator.Play("QuitPanelOut");
        }
        /// <summary>
        ///Loads the main menu scene.
        /// </summary>
        public void returnToMenu()
        {
            SceneManager.LoadScene(mainMenu);
            uiEventSystem.SetSelectedGameObject(defualtSelectedMain);
        }

        public void SaveAndReturnToMenu()
        {
            gameManager.Save();
            returnToMenu();
        }

        // Update is called once per frame
        /// <summary>
        /// The update method. This mainly searches for the user pressing the escape key.
        /// </summary>
        public void Update()
        {
            //colorCrossfade();
            if (vidPanel.activeInHierarchy == true)
            {
                pauseMenu.text = "Video Menu";
            }
            else if (audioPanel.activeInHierarchy == true)
            {
                pauseMenu.text = "Audio Menu";
            }
            else if (mainPanel.activeInHierarchy == true)
            {
                pauseMenu.text = "Pause Menu";
            }

            if (Input.GetKeyDown(KeyCode.Escape) && mainPanel.activeInHierarchy == false)
            {
                //Original code
                uiEventSystem.SetSelectedGameObject(defualtSelectedMain);
                mainPanel.SetActive(true);
                vidPanel.SetActive(false);
                audioPanel.SetActive(false);
                TitleTexts.SetActive(true);
                mask.SetActive(true);
                Time.timeScale = 0;
                for (int i = 0; i < otherUIElements.Length; i++)
                {
                    otherUIElements[i].gameObject.SetActive(false);
                }

                //My code
                //Disable gun script => make the gun dont shoot when click in pause mode
                gunScript.GetComponent<HandgunScriptLPFP>().enabled = false;
            }
            else if(Input.GetKeyDown(KeyCode.Escape) && mainPanel.activeInHierarchy == true) {
                //Original code
                Time.timeScale = timeScale;
                mainPanel.SetActive(false);
                vidPanel.SetActive(false);
                audioPanel.SetActive(false);
                TitleTexts.SetActive(false);
                mask.SetActive(false);
                for (int i = 0; i < otherUIElements.Length; i++)
                {
                    otherUIElements[i].gameObject.SetActive(true);
                }

                //My code
                //Re-enable gun script
                gunScript.GetComponent<HandgunScriptLPFP>().enabled = true;
            }
        }

        /////Audio Options

        /// <summary>
        /// Show the audio panel 
        /// </summary>
        public void Audio()
        {
            mainPanel.SetActive(false);
            vidPanel.SetActive(false);
            audioPanel.SetActive(true);
            audioPanelAnimator.enabled = true;
            audioIn();
            pauseMenu.text = "Audio Menu";
        }

        /// <summary>
        /// Play the "audio panel in" animation.
        /// </summary>
        public void audioIn()
        {
            uiEventSystem.SetSelectedGameObject(defualtSelectedAudio);
            audioPanelAnimator.Play("Audio Panel In");
            audioMasterSlider.value = AudioListener.volume;
        }

        /// <summary>
        /// Audio Option Methods
        /// </summary>
        /// <param name="f"></param>
        public void updateMasterVol(float f)
        {
            //Controls volume of all audio listeners 
            AudioListener.volume = f;
        }

        /// <summary>
        /// Update music effects volume
        /// </summary>
        /// <param name="f"></param>
        public void updateMusicVol(float f)
        {
            try
            {
                for (int _musicAmt = 0; _musicAmt < music.Length; _musicAmt++)
                {
                    music[_musicAmt].volume *= f;
                }
            }
            catch
            {
                Debug.Log("Please assign music sources in the manager");
            }
        }

        /// <summary>
        /// Update the audio effects volume
        /// </summary>
        /// <param name="f"></param>
        public void updateEffectsVol(float f)
        {
            try
            {
                for (_audioEffectAmt = 0; _audioEffectAmt < effects.Length; _audioEffectAmt++)
                {
                    //get the values for all effects before the change
                    _beforeEffectVol[_audioEffectAmt] = effects[_audioEffectAmt].volume;

                    //lower it by a factor of f because we don't want every effect to be set to a uniform volume
                    effects[_audioEffectAmt].volume *= f;
                }
            }
            catch
            {
                Debug.Log("Please assign audio effects sources in the manager.");
            }

        }

        /// <summary> 
        /// The method for changing the applying new audio settings
        /// </summary>
        public void applyAudio()
        {
            StartCoroutine(applyAudioMain());
            uiEventSystem.SetSelectedGameObject(defualtSelectedMain);
           
        }

        /// <summary>
        /// Use an IEnumerator to first play the animation and then change the audio settings
        /// </summary>
        /// <returns></returns>
        internal IEnumerator applyAudioMain()
        {
            audioPanelAnimator.Play("Audio Panel Out");
            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime((float)audioPanelAnimator.GetCurrentAnimatorClipInfo(0).Length));
            mainPanel.SetActive(true);
            vidPanel.SetActive(false);
            audioPanel.SetActive(false);
            beforeMaster = AudioListener.volume;
            saveSettings.SaveGameSettings();
        }

        /// <summary>
        /// Cancel the audio setting changes
        /// </summary>
        public void cancelAudio()
        {
            uiEventSystem.SetSelectedGameObject(defualtSelectedMain);
            StartCoroutine(cancelAudioMain());
        }

        /// <summary>
        /// Use an IEnumerator to first play the animation and then change the audio settings
        /// </summary>
        /// <returns></returns>
        internal IEnumerator cancelAudioMain()
        {
            audioPanelAnimator.Play("Audio Panel Out");
            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime((float)audioPanelAnimator.GetCurrentAnimatorClipInfo(0).Length));
            mainPanel.SetActive(true);
            vidPanel.SetActive(false);
            audioPanel.SetActive(false);
            AudioListener.volume = beforeMaster;
            try
            {


                for (_audioEffectAmt = 0; _audioEffectAmt < effects.Length; _audioEffectAmt++)
                {
                    //get the values for all effects before the change
                    effects[_audioEffectAmt].volume = _beforeEffectVol[_audioEffectAmt];
                }
                for (int _musicAmt = 0; _musicAmt < music.Length; _musicAmt++)
                {
                    music[_musicAmt].volume = _beforeMusic;
                }
            }
            catch
            {
                Debug.Log("please assign the audio sources in the manager");
            }
        }

        /////Video Options
        /// <summary>
        /// Show video
        /// </summary>
        public void Video()
        {

            mainPanel.SetActive(false);
            vidPanel.SetActive(true);
            audioPanel.SetActive(false);
            vidPanelAnimator.enabled = true;
            videoIn();
            pauseMenu.text = "Video Menu";

        }

        /// <summary>
        /// Play the "video panel in" animation
        /// </summary>
        public void videoIn()
        {
            uiEventSystem.SetSelectedGameObject(defualtSelectedVideo);
            vidPanelAnimator.Play("Video Panel In");

            if (QualitySettings.antiAliasing == 0)
            {
                aaCombo.value = 0;
            }
            else if (QualitySettings.antiAliasing == 2)
            {
                aaCombo.value = 1;
            }
            else if (QualitySettings.antiAliasing == 4)
            {
                aaCombo.value = 2;
            }
            else if (QualitySettings.antiAliasing == 8)
            {
                aaCombo.value = 3;
            }
            if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.ForceEnable)
            {
                afCombo.value = 1;
            }
            else if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.Disable)
            {
                afCombo.value = 0;
            }
            else if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.Enable)
            {
                afCombo.value = 2;
            }
            presetLabel.text = presets[QualitySettings.GetQualityLevel()].ToString();
            fovSlider.value = mainCam.fieldOfView;
            masterTexSlider.value = QualitySettings.masterTextureLimit;      
        }

        /// <summary>
        /// Cancel the video setting changes 
        /// </summary>
        public void cancelVideo()
        {
            uiEventSystem.SetSelectedGameObject(defualtSelectedMain);
            StartCoroutine(cancelVideoMain());
        }

        /// <summary>
        /// Use an IEnumerator to first play the animation and then changethe video settings
        /// </summary>
        /// <returns></returns>
        internal IEnumerator cancelVideoMain()
        {
            vidPanelAnimator.Play("Video Panel Out");

            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime((float)vidPanelAnimator.GetCurrentAnimatorClipInfo(0).Length));
            try
            {
                mainCam.fieldOfView = fovINI;
                mainPanel.SetActive(true);
                vidPanel.SetActive(false);
                audioPanel.SetActive(false);
                QualitySettings.shadowDistance = shadowDistINI;
                QualitySettings.antiAliasing = (int)aaQualINI;
                QualitySettings.antiAliasing = msaaINI;
                QualitySettings.masterTextureLimit = lastTexLimit;
            }
            catch
            {
                Debug.Log("A problem occured (chances are the terrain was not assigned )");
                mainCam.fieldOfView = fovINI;
                mainPanel.SetActive(true);
                vidPanel.SetActive(false);
                audioPanel.SetActive(false);
                QualitySettings.shadowDistance = shadowDistINI;
                QualitySettings.antiAliasing = (int)aaQualINI;
                QualitySettings.antiAliasing = msaaINI;
                QualitySettings.masterTextureLimit = lastTexLimit;
            }

        }

        //Apply the video prefs
        /// <summary>
        /// Apply the video settings
        /// </summary>
        public void apply()
        {
            StartCoroutine(applyVideo());
            uiEventSystem.SetSelectedGameObject(defualtSelectedMain);
  
        }

        /// <summary>
        /// Use an IEnumerator to first play the animation and then change the video settings.
        /// </summary>
        /// <returns></returns>
        internal IEnumerator applyVideo()
        {
            vidPanelAnimator.Play("Video Panel Out");
            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime((float)vidPanelAnimator.GetCurrentAnimatorClipInfo(0).Length));
            mainPanel.SetActive(true);
            vidPanel.SetActive(false);
            audioPanel.SetActive(false);
            shadowDistINI = QualitySettings.shadowDistance;
            Debug.Log("Shadow dist ini" + shadowDistINI);
            fovINI = mainCam.fieldOfView;
            lastTexLimit = QualitySettings.masterTextureLimit;
            saveSettings.SaveGameSettings();
        }

        /// <summary>
        /// Change the lod bias using
        /// <c>
        /// QualitySettings.lodBias = LoDBias / 2.15f;
        /// </c> 
        /// LoDBias is only divided by 2.15 because the max is set to 10 on the slider, and dividing by 2.15 results in 4.65, our desired max. However, deleting or changing 2.15 is compeletly fine.
        /// </summary>
        /// <param name="LoDBias"></param>
        public void lodBias(float LoDBias)
        {
            QualitySettings.lodBias = LoDBias / 2.15f;
        }

        /// <summary>
        /// Update the texture quality using  
        /// <c>QualitySettings.masterTextureLimit </c>
        /// </summary>
        /// <param name="qual"></param>
        public void updateTex(float qual)
        {

            int f = Mathf.RoundToInt(qual);
            QualitySettings.masterTextureLimit = f;
        }

        /// <summary>
        /// Change the fov using a float. The defualt should be 60.
        /// </summary>
        /// <param name="fov"></param>
        public void updateFOV(float fov)
        {
            mainCam.fieldOfView = fov;
        }

        /// <summary>
        /// Force aniso on using quality settings
        /// </summary>
        //Force the aniso on.
        public void forceOnANISO()
        {
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
        }

        /// <summary>
        /// Per texture aniso using quality settings
        /// </summary>
        //Use per texture aniso settings.
        public void perTexANISO()
        {
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
        }

        /// <summary>
        /// Disable aniso using quality setttings
        /// </summary>
        //Disable aniso all together.
        public void disableANISO()
        {
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
        }

        /// <summary>
        /// The method for changing aniso settings
        /// </summary>
        /// <param name="anisoSetting"></param>
        public void updateANISO(int anisoSetting)
        {
            if (anisoSetting == 0)
            {
                disableANISO();
            }
            else if (anisoSetting == 1)
            {
                forceOnANISO();
            }
            else if (anisoSetting == 2)
            {
                perTexANISO();
            }
        }

        /// <summary>
        /// Update MSAA quality using quality settings
        /// </summary>
        /// <param name="msaaAmount"></param>
        public void updateMSAA(int msaaAmount)
        {
            if (msaaAmount == 0)
            {
                disableMSAA();
            }
            else if (msaaAmount == 1)
            {
                twoMSAA();
            }
            else if (msaaAmount == 2)
            {
                fourMSAA();
            }
            else if (msaaAmount == 3)
            {
                eightMSAA();
            }

        }

        /// <summary>
        /// Set MSAA to 0x (disabling it) using quality settings
        /// </summary>
        public void disableMSAA()
        {

            QualitySettings.antiAliasing = 0;
            // aaOption.text = "MSAA: " + QualitySettings.antiAliasing.ToString();
        }

        /// <summary>
        /// Set MSAA to 2x using quality settings
        /// </summary>
        public void twoMSAA()
        {

            QualitySettings.antiAliasing = 2;
            // aaOption.text = "MSAA: " + QualitySettings.antiAliasing.ToString();
        }

        /// <summary>
        /// Set MSAA to 4x using quality settings
        /// </summary>
        public void fourMSAA()
        {

            QualitySettings.antiAliasing = 4;

            // aaOption.text = "MSAA: " + QualitySettings.antiAliasing.ToString();
        }

        /// <summary>
        /// Set MSAA to 8x using quality settings
        /// </summary>
        public void eightMSAA()
        {

            QualitySettings.antiAliasing = 8;
            // aaOption.text = "MSAA: " + QualitySettings.antiAliasing.ToString();
        }

        /// <summary>
        /// Set the quality level one level higher. This is done by getting the current quality level, then using 
        /// <c> 
        /// QualitySettings.IncreaseLevel();
        /// </c>
        /// to increase the level. The current level variable is set to the new quality setting, and the label is updated.
        /// </summary>
        public void nextPreset()
        {
            _currentLevel = QualitySettings.GetQualityLevel();
            QualitySettings.IncreaseLevel();
            _currentLevel = QualitySettings.GetQualityLevel();
            presetLabel.text = presets[_currentLevel].ToString();
            if (hardCodeSomeVideoSettings == true)
            {
                QualitySettings.shadowDistance = shadowDist[_currentLevel];
                QualitySettings.lodBias = LODBias[_currentLevel];
            }
        }

        /// <summary>
        /// Set the quality level one level lower. This is done by getting the current quality level, then using 
        /// <c> 
        /// QualitySettings.DecreaseLevel();
        /// </c>
        /// to decrease the level. The current level variable is set to the new quality setting, and the label is updated.
        /// </summary>
        public void lastPreset()
        {
            _currentLevel = QualitySettings.GetQualityLevel();
            QualitySettings.DecreaseLevel();
            _currentLevel = QualitySettings.GetQualityLevel();
            presetLabel.text = presets[_currentLevel].ToString();
            if (hardCodeSomeVideoSettings == true)
            {
                QualitySettings.shadowDistance = shadowDist[_currentLevel];
                QualitySettings.lodBias = LODBias[_currentLevel];
            }

        }

        /// <summary>
        /// Hard code the minimal settings
        /// </summary>
        public void setMinimal()
        {
            QualitySettings.SetQualityLevel(0);
            //QualitySettings.shadowDistance = 12.6f;
            QualitySettings.shadowDistance = shadowDist[0];
            //QualitySettings.lodBias = 0.3f;
            QualitySettings.lodBias = LODBias[0];
        }

        /// <summary>
        /// Hard code the very low settings
        /// </summary>
        public void setVeryLow()
        {
            QualitySettings.SetQualityLevel(1);
            //QualitySettings.shadowDistance = 17.4f;
            QualitySettings.shadowDistance = shadowDist[1];
            //QualitySettings.lodBias = 0.55f;
            QualitySettings.lodBias = LODBias[1];
        }

        /// <summary>
        /// Hard code the low settings
        /// </summary>
        public void setLow()
        {
            QualitySettings.SetQualityLevel(2);
            //QualitySettings.shadowDistance = 29.7f;
            //QualitySettings.lodBias = 0.68f;
            QualitySettings.lodBias = LODBias[2];
            QualitySettings.shadowDistance = shadowDist[2];
        }

        /// <summary>
        /// Hard code the normal settings
        /// </summary>
        public void setNormal()
        {
            QualitySettings.SetQualityLevel(3);
            //QualitySettings.shadowDistance = 82f;
            //QualitySettings.lodBias = 1.09f;
            QualitySettings.shadowDistance = shadowDist[3];
            QualitySettings.lodBias = LODBias[3];
        }

        /// <summary>
        /// Hard code the very high settings
        /// </summary>
        public void setVeryHigh()
        {
            QualitySettings.SetQualityLevel(4);
            //QualitySettings.shadowDistance = 110f;
            //QualitySettings.lodBias = 1.22f;
            QualitySettings.shadowDistance = shadowDist[4];
            QualitySettings.lodBias = LODBias[4];
        }

        /// <summary>
        /// Hard code the ultra settings
        /// </summary>
        public void setUltra()
        {
            QualitySettings.SetQualityLevel(5);
            //QualitySettings.shadowDistance = 338f;
            //QualitySettings.lodBias = 1.59f;
            QualitySettings.shadowDistance = shadowDist[5];
            QualitySettings.lodBias = LODBias[5];
        }

        /// <summary>
        /// Hard code the extreme settings
        /// </summary>
        public void setExtreme()
        {
            QualitySettings.SetQualityLevel(6);
            //QualitySettings.shadowDistance = 800f;
            //QualitySettings.lodBias = 4.37f;
            QualitySettings.shadowDistance = shadowDist[6];
            QualitySettings.lodBias = LODBias[6];
        }
    }
}
