using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour {
    //Game variables
    public int objectsPerWave = 6;
    public int waves = 0;
    public float delay = 2.0f;
    private float startTime;
    private float reactionTime;
    private float score;
    private float finalScore;
    private int rightCount = 0;

    //UI
    public GUIText menuText;
    public GUIText scoreText;
    public GUIText restartText;
    public GUIText gameOverText;
    public GUIText timerText;
    public GUIText gameText;

    //Game state
    private List<GameObject> possibilities;
    private GameObject current;
    private bool pressed = true;
    private GameState _gameState;
    private enum GameState
    {
        menu,
        inGame,
        gameOver,
        restart
    }

	// Use this for initialization
	void Start () {
        menuText.text = "";
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
                menuText.text = "Recognition Blitz \n1-9 for trial amount";
                score = 0;

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
                if (Input.GetKeyDown(KeyCode.Space) && !pressed)
                {
                    reactionTime = Time.time - startTime;
                    if (current.GetComponent<Object>().winner)
                    {
                        pressed = true;
                        rightCount++;
                        score +=  (1/reactionTime) * 100;
                        finalScore = score / rightCount;
                    }
                    else
                    {
                        pressed = false;
                        score -= (1/reactionTime) * 25;
                        finalScore = score;
                    }
                }
                UpdateScore();
                break;
            case GameState.gameOver:
                 restartText.text = "Press Space to return to menu";
                _gameState = GameState.restart;
                break;
            case GameState.restart:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _gameState = GameState.menu;
                    restartText.text = "";
                }
                break;
        }
	}

    IEnumerator StartWaves()
    {
        if(_gameState == GameState.inGame)
        {
            rightCount = 0;
            for (int j = 0; j < waves; j++)
            {
                pressed = true;
                current = Pick(possibilities);
                current.GetComponent<Object>().winner = true;
                current.SetActive(true);
                gameText.text = "This is your target";
                yield return new WaitForSeconds(delay);
                current.SetActive(false);
                gameText.text = "Get ready...";
                yield return new WaitForSeconds(delay);
                gameText.text = "";

                for (int i = 0; i < objectsPerWave; i++)
                {
                    pressed = false;
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
            _gameState = GameState.gameOver;
        }
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + (int)finalScore;
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
