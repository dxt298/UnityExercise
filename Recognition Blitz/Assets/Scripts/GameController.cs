using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour {
    //Game logic
    public List<GameObject> possibilities;
    public int objectsPerWave = 10;
    public int waves = 0;
    public float delay = 1.0f;
    public float startWait = 1.0f;
    private float startTime;
    private float reactionTime;
    public GameObject current;
    //UI
    public GUIText menuText;
    public GUIText scoreText;
    public GUIText restartText;
    public GUIText gameOverText;
    public GUIText timerText;
    public GUIText gameText;
    //Game logic
    private GameState _gameState;
    private enum GameState
    {
        menu,
        inGame,
        gameOver,
        restart
    }
    private int score;

	// Use this for initialization
	void Start () {
        menuText.text = "Recognition Blitz \n1-9 for trial amount";
        scoreText.text = "";
        restartText.text = "";
        gameOverText.text = "";
        gameText.text = "";
        timerText.text = "Time";

        _gameState = GameState.menu;
        score = 0;

        possibilities = GameObject.FindGameObjectsWithTag("Possible").ToList();
        foreach (GameObject go in possibilities)
        {
            go.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
        switch (_gameState)
        {
            case GameState.menu:
                if (Input.GetKeyDown(KeyCode.Alpha1)) { waves = 1; }
                if (Input.GetKeyDown(KeyCode.Alpha2)) { waves = 2; }
                if (Input.GetKeyDown(KeyCode.Alpha3)) { waves = 3; }
                if (Input.GetKeyDown(KeyCode.Alpha4)) { waves = 4; }
                if (Input.GetKeyDown(KeyCode.Alpha5)) { waves = 5; }
                if (Input.GetKeyDown(KeyCode.Alpha6)) { waves = 6; }
                if (Input.GetKeyDown(KeyCode.Alpha7)) { waves = 7; }
                if (Input.GetKeyDown(KeyCode.Alpha8)) { waves = 8; }
                if (Input.GetKeyDown(KeyCode.Alpha9)) { waves = 9; }
                if (Input.GetKeyDown(KeyCode.Space) && waves != 0)
                {
                    _gameState = GameState.inGame;
                    menuText.text = "";
                    UpdateScore();
                    StartCoroutine(StartWaves());
                }
                break;
            case GameState.inGame:
                float test = Math.Abs(startTime - Time.time);
                timerText.text = test.ToString();
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    reactionTime = Time.time - startTime;
                    //Debug.Log(reactionTime);
                    if (current.GetComponent<Object>().winner)
                    {
                        Debug.Log("Winner");
                    }
                    else
                    {
                        Debug.Log("Loser");
                    }
                }
                UpdateScore();
                break;
            case GameState.gameOver:
                 restartText.text = "Press R to return to menu";
                _gameState = GameState.restart;
                break;
            case GameState.restart:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Application.LoadLevel(Application.loadedLevel);
                }
                break;
        }
	}

    IEnumerator StartWaves()
    {
        if(_gameState == GameState.inGame)
        {
            for (int j = 0; j < waves; j++)
            {
                //Pick a target
                current = Pick(possibilities);
                current.GetComponent<Object>().winner = true;
                current.SetActive(true);
                gameText.text = "This is your target";
                yield return new WaitForSeconds(startWait);
                current.SetActive(false);
                gameText.text = "Get ready...";
                yield return new WaitForSeconds(startWait);
                gameText.text = "";

                for (int i = 0; i < objectsPerWave; i++)
                {
                    current = Pick(possibilities);
                    current.SetActive(true);
                    startTime = Time.time;
                    yield return new WaitForSeconds(delay);
                    current.SetActive(false);
                    yield return new WaitForSeconds(delay);
                }
                foreach(GameObject go in possibilities)
                {
                    go.GetComponent<Object>().winner = false;
                }
            }
        }
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }

    void GameOver()
    {
        gameOverText.text = "Game Over";
        _gameState = GameState.gameOver;
    }

    T Pick<T>(List<T> from)
    {
        if (from.Count == 0) throw new Exception("Cannot pick from an empty list!");
        return (from[Random.Range(0, from.Count)]);
    }

}
