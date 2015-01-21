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

    //UI
    public GUIText menuText;
    public GUIText scoreText;
    public GUIText restartText;
    public GUIText gameOverText;
    public GUIText instructionText;
    public GUIText gameText;
    public GUIText titleText;

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
        //Override the place holder text with blanks so they're not in the way
        menuText.text = "";
        titleText.text = "";
        scoreText.text = "";
        restartText.text = "";
        gameOverText.text = "";
        instructionText.text = "";
        gameText.text = "";

        //Fresh game, gamestate set to menu and score is set to 0
        _gameState = GameState.menu;
        score = 0;

        //Load all assets(target possibilities) into a list and deactivate them
        //The reason for this is because Unity seems to have problems loading things not in the scene so I went with this alternative
        //Have them in the scene but turn them off immediately so they aren't visible/active
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
                //Set UI texts
                titleText.text = "Recognition Blitz";
                menuText.text = "Press a number 1-9 for trial amount: ";
                menuText.text += waves.ToString();
                menuText.text += "\nPress space to start";
                instructionText.text = "Your target will be shown at the beginning of a wave.\nPress the spacebar if your target shows up to gain points.\nIf you're wrong, you lose points.  Aim for the high score!";
                score = 0;

                //Wave/Trial choices
                if (Input.GetKeyDown(KeyCode.Alpha1)) { waves = 1; }
                if (Input.GetKeyDown(KeyCode.Alpha2)) { waves = 2; }
                if (Input.GetKeyDown(KeyCode.Alpha3)) { waves = 3; }
                if (Input.GetKeyDown(KeyCode.Alpha4)) { waves = 4; }
                if (Input.GetKeyDown(KeyCode.Alpha5)) { waves = 5; }
                if (Input.GetKeyDown(KeyCode.Alpha6)) { waves = 6; }
                if (Input.GetKeyDown(KeyCode.Alpha7)) { waves = 7; }
                if (Input.GetKeyDown(KeyCode.Alpha8)) { waves = 8; }
                if (Input.GetKeyDown(KeyCode.Alpha9)) { waves = 9; }
                //Don't let them start if they haven't chosen an amount!
                if (Input.GetKeyDown(KeyCode.Space) && waves != 0)
                {
                    //Begin the game, score updating, and start spawning waves
                    _gameState = GameState.inGame;
                    menuText.text = "";
                    titleText.text = "";
                    instructionText.text = "";
                    UpdateScore();
                    StartCoroutine(StartWaves());
                }
                break;
            case GameState.inGame:
                //Parameters:
                //First, if they pressed space
                //Second, "pressed" bool.  If they got it correct, this bool is turned on so that they can't continue to keep getting points from that target
                /*Third, time out condition.  Two seconds is the limit for an object to be on screen,
                so if their reaction time is greater than that, then there isn't an object to react to, so they shouldn't gain points*/
                if (Input.GetKeyDown(KeyCode.Space) && !pressed && ((Time.time - startTime) <= 2.0f))
                {
                    //Calculate and display reaction time
                    reactionTime = Time.time - startTime;
                    gameText.text = "Reaction: " + reactionTime.ToString() + " seconds";
                    //If the current object on screen was the target object, add score, else decrease it
                    //The score formula is based on the following logic:
                    //The faster they press it on a correct object, the more points they gain
                    //The faster they press it on an incorrect object, they more points they lose
                    //This deters simply mashing the spacebar for points
                    if (current.GetComponent<Object>().winner)
                    {
                        gameText.text += "\nCorrect!";
                        pressed = true;
                        score +=  (1/reactionTime) * 100;
                    }
                    else
                    {
                        gameText.text += "\nWrong!";
                        pressed = false;
                        score -= (1/reactionTime) * 25;
                    }
                }
                UpdateScore();
                break;
            case GameState.gameOver:
                 restartText.text = "Your score was " + (int)score + "!\nPress Space to return to menu";
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
        //Don't actually start the trials until we're in the game!
        if(_gameState == GameState.inGame)
        {
            //Nested for loops, one for the wave count and one for the amount of objects per wave
            for (int j = 0; j < waves; j++)
            {
                /*Turn pressed on, we dont want them gaining points/being able to press spacebar while
                we're showing them the target*/
                pressed = true;
                //Pick a random object from the list to be our target, turn on its win bool, and show it to the player
                current = Pick(possibilities);
                current.GetComponent<Object>().winner = true;
                current.SetActive(true);
                gameText.text = "This is your new target";
                //It gets shown on screen for the delay amount, then turned off, and then the game begins in the same delay amount later
                yield return new WaitForSeconds(delay);
                current.SetActive(false);
                gameText.text = "Get ready...";
                yield return new WaitForSeconds(delay);
                gameText.text = "";

                //Shuffle list
                for (int i = 0; i < possibilities.Count(); i++)
                {
                    GameObject temp = possibilities[i];
                    int randomIndex = Random.Range(i, possibilities.Count());
                    possibilities[i] = possibilities[randomIndex];
                    possibilities[randomIndex] = temp;
                }

                /*The target may only be shown once per wave maximum, and sometimes not at all. In order to reach this goal, I came up with the following solution.
                 It's very simple, first we shuffle the list, which is a few items bigger than the amount of objects shown per wave.
                 Then we simply choose the first X items and show them in order where X is the amount of objects per wave.  The target object may or may not
                 show up within the first six, and if it does, it won't show up again because no items were duplicated, simply shuffled.
                 This also beats simply randomly picking something from the list as that may result in duplicate targets per wave*/

                //The for loop turns off the pressed bool, allowing the player to have input, shows an object for 2 seconds, then disapperas for 2 seconds
                for (int i = 0; i < objectsPerWave; i++)
                {
                    pressed = false;
                    current = possibilities[i];
                    current.SetActive(true);
                    startTime = Time.time;
                    yield return new WaitForSeconds(delay);
                    current.SetActive(false);
                    yield return new WaitForSeconds(delay);
                }
                //The wave is over, so we reset all of their "correct" bools to false
                foreach(GameObject go in possibilities)
                {
                    go.GetComponent<Object>().winner = false;
                }
            }
            //When the waves are done, the game is over
            _gameState = GameState.gameOver;
        }
    }

    //Update score method
    void UpdateScore()
    {
        scoreText.text = "Score: " + (int)score;
    }

    //Game over method
    void GameOver()
    {
        gameOverText.text = "Game Over";
        _gameState = GameState.gameOver;
    }

    //Pick extension method
    public static T Pick<T>(List<T> from)
    {
        if (from.Count == 0) throw new Exception("Cannot pick from an empty list!");
        return (from[Random.Range(0, from.Count)]);
    }

}
