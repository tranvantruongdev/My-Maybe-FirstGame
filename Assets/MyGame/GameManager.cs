using AIBehavior;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool gameOver = false;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject gunScript;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] AudioSource audioSource;

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
}
