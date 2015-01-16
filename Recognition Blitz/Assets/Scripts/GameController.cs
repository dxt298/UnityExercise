using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour {
    //Game logic
    public List<GameObject> possibilities;
    public int objectsPerWave = 1;
    public float delay = 2.0f;
    public float startWait = 3.0f;
    public float spawnWait = 2.0f;
    private float startTime;
    //UI
    public GUIText menuText;
    public GUIText scoreText;
    public GUIText restartText;
    public GUIText gameOverText;
    public GUIText timerText;
    //Game state
    private bool menu;
    private bool gameOver;
    private bool restart;
    private int score;

	// Use this for initialization
	void Start () {
        menuText.text = "Recognition Blitz";
        scoreText.text = "";
        restartText.text = "";
        gameOverText.text = "";
        timerText.text = "Time";

        menu = true;
        gameOver = false;
        restart = false;
        score = 0;

        startTime = Time.time;

        possibilities = GameObject.FindGameObjectsWithTag("Possible").ToList();
        foreach (GameObject go in possibilities)
        {
            go.SetActive(false);
        }

        UpdateScore();
        StartCoroutine(StartWaves());
	}
	
	// Update is called once per frame
	void Update () {
        if (restart)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Application.LoadLevel(Application.loadedLevel);
            }
        }
	}

    IEnumerator StartWaves()
    {
        yield return new WaitForSeconds(startWait);
        while (!gameOver)
        {
            for (int i = 0; i < objectsPerWave; i++)
            {
                GameObject current = Pick(possibilities);
                current.SetActive(true);
                yield return new WaitForSeconds(delay);
                current.SetActive(false);
                yield return new WaitForSeconds(spawnWait);
            }

            if (gameOver)
            {
                restartText.text = "Press R to return to menu";
                restart = true;
                break;
            }
        }
        //Instantiate(go, new Vector3(0, 0, 10), Quaternion.identity);
    }

    T Pick<T>(List<T> from)
    {
        if (from.Count == 0) throw new Exception("Cannot pick from an empty list!");
        return (from[Random.Range(0, from.Count)]);
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }

    void GameOver()
    {
        gameOverText.text = "Game Over";
        gameOver = true;
    }
}
