using AIBehavior;
using AIBehaviorExamples;
using BayatGames.SaveGameFree;
using BayatGames.SaveGameFree.Encoders;
using BayatGames.SaveGameFree.Serializers;
using BayatGames.SaveGameFree.Types;
using GreatArcStudios;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool gameOver = false;

    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject gunScript;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Transform[] enemySpawnPosArr;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] private float minWaitTime;
    [SerializeField] private float maxWaitTime;

    private string stageName;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject player;
    [SerializeField] private int enemiesLeft;

    private void Start()
    {
        StartCoroutine(EnemySpawn());
    }

    private IEnumerator EnemySpawn()
    {
        while (true)
        {
            var enemySpawnPos = enemySpawnPosArr[UnityEngine.Random.Range(0, enemySpawnPosArr.Length)];
            Instantiate(enemyPrefab, enemySpawnPos.position, Quaternion.identity);
            yield return new WaitForSeconds(UnityEngine.Random.Range(minWaitTime, maxWaitTime));
        }
    }

    public void GameOver()
    {
        audioSource.PlayOneShot(gameOverSound);
        gameOver = true;
        gameOverPanel.SetActive(true);

        //Disable gunScript
        gunScript.GetComponent<HandgunScriptLPFP>().enabled = false;

        //Disable pauseMenu
        pauseMenu.gameObject.SetActive(false);

        //Set time scale to 0
        Time.timeScale = 0;

        //Unlock the mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public enum SaveFormat
    {

        /// <summary>
        /// The XML.
        /// </summary>
        XML,

        /// <summary>
        /// The JSON.
        /// </summary>
        JSON,

        /// <summary>
        /// The Ninary.
        /// </summary>
        Binary

    }


    [Header("Save/Load Settings")]
    [Space]


    [Tooltip("You must specify a value for this to be able to save it.")]
    /// <summary>
    /// The position identifier.
    /// </summary>
    public string positionIdentifier = "enter the position identifier";

    [Tooltip("You must specify a value for this to be able to save it.")]
    /// <summary>
    /// The rotation identifier.
    /// </summary>
    public string rotationIdentifier = "enter the rotation identifier";

    [Tooltip("You must specify a value for this to be able to save it.")]
    /// <summary>
    /// The rotation identifier.
    /// </summary>
    public string hpIdentifier = "enter the hp identifier";

    [Tooltip("You must specify a value for this to be able to save it.")]
    /// <summary>
    /// The score identifier.
    /// </summary>
    public string scoreIdentifier = "enter the score identifier";

    [Tooltip("You must specify a value for this to be able to save it.")]
    /// <summary>
    /// The score identifier.
    /// </summary>
    public string stageIdentifier = "enter the flag identifier";

    [Tooltip("You must specify a value for this to be able to save it.")]
    /// <summary>
    /// The score identifier.
    /// </summary>
    public string enemyCounterIdentifier = "enter the enemy counter identifier";

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

    [Header("Defaults")]
    [Space]

    [Tooltip("Default Position Value")]
    /// <summary>
    /// The default position.
    /// </summary>
    public Vector3 defaultPosition;

    [Tooltip("Default Rotation Value")]
    /// <summary>
    /// The default rotation.
    /// </summary>
    public Vector3 defaultRotation = Quaternion.identity.eulerAngles;

    [Tooltip("Default Score Value")]
    /// <summary>
    /// The default score.
    /// </summary>
    public int defaultScore = 0;

    protected virtual void Awake()
    {
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
            case SaveFormat.Binary:
                serializer = new SaveGameBinarySerializer();
                break;
            case SaveFormat.JSON:
                serializer = new SaveGameJsonSerializer();
                break;
            case SaveFormat.XML:
                serializer = new SaveGameXmlSerializer();
                break;
        }

        switch (GameSetting.loadType)
        {
            case GameSetting.LoadType.Load:
                Load();
                break;
            case GameSetting.LoadType.New:
                break;
        }
    }

    /// <summary>
    /// Save this instance.
    /// </summary>
    public virtual void Save()
    {
        SaveGame.Save<Vector3Save>(
            positionIdentifier,
            player.transform.position,
            encode,
            encodePassword,
            serializer,
            encoder,
            encoding,
            savePath);

        SaveGame.Save<QuaternionSave>(
            rotationIdentifier,
            player.transform.rotation,
            encode,
            encodePassword,
            serializer,
            encoder,
            encoding,
            savePath);

        SaveGame.Save<float>(
            hpIdentifier,
            playerStats.Health,
            encode,
            encodePassword,
            serializer,
            encoder,
            encoding,
            savePath);

        SaveGame.Save<int>(
            scoreIdentifier,
            playerStats.Score1,
            encode,
            encodePassword,
            serializer,
            encoder,
            encoding,
            savePath);

        var stage = SceneManager.GetActiveScene();

        SaveGame.Save<string>(
            stageIdentifier,
            stage.name,
            encode,
            encodePassword,
            serializer,
            encoder,
            encoding,
            savePath);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemiesLeft = enemies.Length;

        SaveGame.Save<int>(
            enemyCounterIdentifier,
            enemiesLeft,
            encode,
            encodePassword,
            serializer,
            encoder,
            encoding,
            savePath);

        Debug.Log("Saved!");

        pauseMenu.GetComponent<PauseManager>().returnToMenu();
    }

    /// <summary>
    /// Load this instance.
    /// </summary>
    public virtual void Load()
    {
        player.transform.position = SaveGame.Load<Vector3Save>(
            positionIdentifier,
            defaultPosition,
            encode,
            encodePassword,
            serializer,
            encoder,
            encoding,
            savePath);

        player.transform.rotation = SaveGame.Load<QuaternionSave>(
            rotationIdentifier,
            Quaternion.Euler(defaultRotation),
            encode,
            encodePassword,
            serializer,
            encoder,
            encoding,
            savePath);

        playerStats.Health = SaveGame.Load<float>(
            hpIdentifier,
            playerStats.Health,
            encode,
            encodePassword,
            serializer,
            encoder,
            encoding,
            savePath);

        playerStats.Score1 = SaveGame.Load<int>(
            scoreIdentifier,
            defaultScore,
            encode,
            encodePassword,
            serializer,
            encoder,
            encoding,
            savePath);

        //update score
        playerStats.Score(0);

        enemiesLeft = SaveGame.Load<int>(
            enemyCounterIdentifier,
            0,
            encode,
            encodePassword,
            serializer,
            encoder,
            encoding,
            savePath);

        while (enemiesLeft > 0)
        {
            var enemySpawnPos = enemySpawnPosArr[UnityEngine.Random.Range(0, enemySpawnPosArr.Length)];
            Instantiate(enemyPrefab, enemySpawnPos.position, Quaternion.identity);
            enemiesLeft--;
        }

        Time.timeScale = 1;
    }
}
