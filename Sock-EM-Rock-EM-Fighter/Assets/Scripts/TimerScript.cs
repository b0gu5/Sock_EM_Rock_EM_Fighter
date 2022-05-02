using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Class that checks which player wins as well as manages the in game timer
public class TimerScript : MonoBehaviour
{
    //Integers used to represent timers in the couroutine
    private int countDownTime, startUpTime, continueTime;

    //Instances of the Text class to represent text that can be seen within the scene
    //Text that will display the countdown timer
    public Text countDownDisplay;
    //Text that will display the "FIGHT!!!!" text
    public Text battleMessage;
    //Text that will display the Continue yes or no text
    public Text continueText;
    //Text that will display the arrow to show the players choice
    public Text continueArrow;
    //Text that will display the CPUs difficulty level
    public Text levelText;

    //Instances of the Healthbar class to represent both players health
    public HealthBar player1HealthBar;
    public HealthBar player2HealthBar;

    //Boolean to determine which player has won
    //False is player 1 has won, true is player 2 has won
    private bool playerWin;
    //Boolean to determine if the player wants to continue
    //True is the player wants to continue, False is the player does not wants to continue
    private bool continueType;
    //Boolean to determine if the game is single player or two player
    //If the boolean is true its 2Player and if its false its 1Player
    private bool gameMode;

    //Will store the current scene that the game is in
    Scene currentScene;
    //String that will store the name of the current scene
    string sceneName;

    private void Start()
    {
        //Sets the countdown timer to 60
        countDownTime = 60;
        //Sets the countdown for starting timer to 1
        startUpTime = 1;
        //Sets the continue time to a large number to give the player plenty of time to choose
        continueTime = 999999;
        //Sets the continue text to false so that it can't be seen until the game is over
        continueText.gameObject.SetActive(false);
        //Starts the coroutine that will update the timer and text to reflect in game progress
        StartCoroutine(CountdownToStart());
    }

    IEnumerator CountdownToStart()
    {
        //Gets the Current Scene
        currentScene = SceneManager.GetActiveScene();
        //Gets the Current Scene name
        sceneName = currentScene.name;

        //If it is the 2 Player Scene set the boolean to true to show the game mode
        //and does not display the level text
        if (sceneName == "2PlayerScene")
        {
            gameMode = true;
            levelText.gameObject.SetActive(false);
        }
        //If it is the 1 Player Scene set the boolean to false to show the game mode
        //and does display the level text
        else
        {
            gameMode = false;
        }

        //Plays the Background music using the Audio manager
        FindObjectOfType<AudioManager>().Play("BGM");

        //loops while the value for the startup timer is greater than 0
        while (startUpTime > 0)
        {
            //Displays the text FIGHT
            battleMessage.text = "FIGHT !!!";
            //Plays the round announcer sound 
            FindObjectOfType<AudioManager>().Play("RoundStart");
            //repeats this loop after one second
            yield return new WaitForSeconds(1f);

            //subtracts the startup time by 1
            startUpTime--;
        }

        //Displays text to reflect the AI's difficulty level
        levelText.text = "Level " + player2HealthBar.getDifficultyLevel().ToString();

        //Sets the battle message text to blank so that there is not text on screen
        battleMessage.text = "";

        //loops while the value for the countdown is greater than 0
        while (countDownTime > 0)
        {
            //Passes the player 1's position to player 2(The AI) so that they can track that
            player2HealthBar.setEnemyXPosition(player1HealthBar.getPosition());
            //Display the countdown value to the timer
            countDownDisplay.text = countDownTime.ToString();

            //repeats this loop after one second
            yield return new WaitForSeconds(1f);

            //Checks if the player 1's health bar is empty
            if (player1HealthBar.getFillValue() <= 0)
            {
                //Sets the player 1 to lose and player 2 to win
                player1HealthBar.timeOut(false);
                player2HealthBar.timeOut(true);
                //Set bool to show player 2 has won
                playerWin = true;
                //Checks if the game is in single player or 2 player
                if (gameMode)
                {
                    //displays message saying player 2 has won
                    battleMessage.text = "Player 2 Wins!!!";
                    //Play the winning announcer, the winning theme and stopping the background music
                    FindObjectOfType<AudioManager>().Stop("BGM");
                    FindObjectOfType<AudioManager>().Play("RoundWin");
                    FindObjectOfType<AudioManager>().Play("VictoryBGM");
                }
                else
                {
                    //displays a message saying game over
                    battleMessage.text = "Game Over!!!";
                    //Play the losing announcer, the losing theme and stopping the background music
                    FindObjectOfType<AudioManager>().Stop("BGM");
                    FindObjectOfType<AudioManager>().Play("RoundLose");
                    FindObjectOfType<AudioManager>().Play("GameOver");
                    //Sets the startup timer to 5 for a game over
                    startUpTime = 5;
                }
                break;
            }

            //Checks if the player 2's health bar is empty
            if (player2HealthBar.getFillValue() <= 0)
            {
                //Sets the player 2 to lose and player 1 to win
                player2HealthBar.timeOut(false);
                player1HealthBar.timeOut(true);
                //displays message saying player 1 has won
                battleMessage.text = "Player 1 Wins!!!";
                //Play the winning announcer, the winning theme and stopping the background music
                FindObjectOfType<AudioManager>().Stop("BGM");
                FindObjectOfType<AudioManager>().Play("RoundWin");
                FindObjectOfType<AudioManager>().Play("VictoryBGM");
                //Set bool to show player 1 has won
                playerWin = false;
                break;
            }

            //if the timer has 10 seconds left make both players aware that the game is nearly over
            if(countDownTime == 10)
            {
                player1HealthBar.gameIsNearlyOver();
                player2HealthBar.gameIsNearlyOver();
            }

            //subtracts the countdown by 1
            countDownTime--;
        }

        startUpTime = 2;

        //Checks if the timer is at 0
        //AKA no player's health is a 0
        if (countDownTime <= 0)
        {
            //Changes counter text to XX
            countDownDisplay.text = "XX";

            //change condition for implementation of player 2
            if (player1HealthBar.getFillValue() > player2HealthBar.getFillValue())
            {
                //Sets the player 2 to lose and player 1 to win
                player2HealthBar.timeOut(false);
                player1HealthBar.timeOut(true);
                //displays message saying player 1 has won
                battleMessage.text = "Player 1 Wins!!!";
                //Play the winning announcer, the winning theme and stopping the background music
                FindObjectOfType<AudioManager>().Stop("BGM");
                FindObjectOfType<AudioManager>().Play("RoundWin");
                FindObjectOfType<AudioManager>().Play("VictoryBGM");
                playerWin = false;
            }
            else
            {
                //Sets the player 1 to lose and player 2 to win
                player1HealthBar.timeOut(false);
                player2HealthBar.timeOut(true);
                //Checks if the game is in single player or 2 player
                if (gameMode)
                {
                    //displays message saying player 2 has won
                    battleMessage.text = "Player 2 Wins!!!";
                    //Play the winning announcer, the winning theme and stopping the background music
                    FindObjectOfType<AudioManager>().Stop("BGM");
                    FindObjectOfType<AudioManager>().Play("RoundWin");
                    FindObjectOfType<AudioManager>().Play("VictoryBGM");
                }
                else
                {
                    //displays a message saying game over
                    battleMessage.text = "Game Over!!!";
                    //Play the losing announcer, the losing theme and stopping the background music
                    FindObjectOfType<AudioManager>().Stop("BGM");
                    FindObjectOfType<AudioManager>().Play("RoundLose");
                    FindObjectOfType<AudioManager>().Play("GameOver");
                    //Sets the startup timer to 5 for a game over
                    startUpTime = 5;
                }
                //Set bool to show player 2 has won
                playerWin = true;
            }
        }

        //loops while the value for the startupTime is greater than 0
        while (startUpTime > 0)
        {
            //repeats this loop after one second
            yield return new WaitForSeconds(1f);

            //subtracts the startUpTime by 1
            startUpTime--;
        }

        //Checks if the game mode is two player or player 1 has won
        if(gameMode || !playerWin)
        {
            //Display the arrow text to show the players decision to continue
            continueText.gameObject.SetActive(true);
            continueArrow.text = "->";
        }


        //loops while the value for the continueTime is greater than 0
        while (continueTime > 0)
        {
            //Checks if the game mode is one player and player 1 has lost
            if (!gameMode && playerWin)
            {
                //Sets the continue type to not continue
                continueType = false;
                break;
            }

            //Checks if player 1 or player 2 has won
            if (playerWin)
            {
                //Gets if player 2 wants to continue
                continueType = player2HealthBar.getContinue();
            }
            else
            {
                //Gets if player 1 wants to continue
                continueType = player1HealthBar.getContinue();
            }

            //checks the boolean to determine the continue mode and will move the arrow to represent
            //their decision to continue
            if (continueType)
            {
                continueArrow.transform.position = new Vector3(-4.5f, -1.99f, 0);
            }
            else
            {
                continueArrow.transform.position = new Vector3(-4.55f, -3.5f, 0);
            }

            //Checks if player 1 has made their decision to continue
            if(player1HealthBar.getChoiceContinue())
            {
                break;
            }

            //Checks if player 2 has made their decision to continue
            if (player2HealthBar.getChoiceContinue())
            {
                break;
            }

            //repeats this loop after 0.1 of a second
            //this is to let the timer go on for longer and make the input be more responsive
            yield return new WaitForSeconds(0.1f);
            //subtracts the countdown by 1
            continueTime--;
        }

        //Checks the players decision to continue
        if (continueType)
        {
            //Checks which game mode the game is in
            if(gameMode)
            {
                //Loads the 2Player scene
                SceneManager.LoadScene("2PlayerScene");
            }
            else
            {
                //Loads the 1Player scene
                SceneManager.LoadScene("1PlayerScene");
            }
        }
        else
        {
            //Load the hint screen
            SceneManager.LoadScene("HintScene");
        }

    }
}
