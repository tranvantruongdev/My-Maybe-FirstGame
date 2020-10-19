using AIBehavior;
using BayatGames.SaveGameFree;
using BayatGames.SaveGameFree.Encoders;
using BayatGames.SaveGameFree.Serializers;
using BayatGames.SaveGameFree.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

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

    [SerializeField] private string stageName;

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
    /// The score identifier.
    /// </summary>
    public string scoreIdentifier = "enter the score identifier";

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
    public Vector3 defaultPosition = Vector3.zero;

    [Tooltip("Default Rotation Value")]
    /// <summary>
    /// The default rotation.
    /// </summary>
    public Vector3 defaultRotation = Quaternion.identity.eulerAngles;

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
    }

    /// <summary>
    /// Save this instance.
    /// </summary>
    public virtual void Save()
    {
        SaveGame.Save<Vector3Save>(
            positionIdentifier,
            transform.position,
            encode,
            encodePassword,
            serializer,
            encoder,
            encoding,
            savePath);

        SaveGame.Save<QuaternionSave>(
            rotationIdentifier,
            transform.rotation,
            encode,
            encodePassword,
            serializer,
            encoder,
            encoding,
            savePath);
    }

    /// <summary>
    /// Load this instance.
    /// </summary>
    public virtual void Load()
    {
        transform.position = SaveGame.Load<Vector3Save>(
            positionIdentifier,
            defaultPosition,
            encode,
            encodePassword,
            serializer,
            encoder,
            encoding,
            savePath);

        transform.rotation = SaveGame.Load<QuaternionSave>(
            rotationIdentifier,
            Quaternion.Euler(defaultRotation),
            encode,
            encodePassword,
            serializer,
            encoder,
            encoding,
            savePath);
    }
}
