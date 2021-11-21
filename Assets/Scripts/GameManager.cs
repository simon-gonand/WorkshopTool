using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isEndGame = false;

    // Start is called before the first frame update
    void Start()
    {
        if (instance) Destroy(gameObject);
        else instance = this;
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        isEndGame = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
        isEndGame = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
