using AIBehavior;
using System;
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
    [SerializeField] Transform[] enemySpawnPosArr;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] private float waitTime;

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
            yield return new WaitForSeconds(waitTime);
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
}
