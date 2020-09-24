using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool gameOver = false;
    [SerializeField] GameObject gameOverPanel;

    public void GameOver()
    {
        gameOver = true;
        gameOverPanel.SetActive(true);
    }
}
