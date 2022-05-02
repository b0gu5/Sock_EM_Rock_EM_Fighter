using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//(AI ONLY) and (PLAYER ONLY) indicates if the variable/function are only used by the AI or Player

//Class that will update the health bar in game to reflect the players health
//and act as an inbetween for the timer and players
public class HealthBar : MonoBehaviour
{
    //Index to uniquely identify the healthbars
    [SerializeField]
    private int healthBarIndex;

    //Floats that will update the fill size of health bar
    private float maxHealth, currentHealth;
    //Instance of the scrollbar class
    private Scrollbar scrollBar;
    //Boolean variables that determine if the player has won or lost
    private bool playerLost, playerWin;
    //Boolean variables that determine if the player has chosen to continue (PLAYER ONLY)
    private bool continueSelection, chosenContinue;
    //Boolean variable that determines if the game is nearly over (AI ONLY)
    private bool gameNearlyOver;
    //Float that will update the size of the scrollbar in the bar
    private float fillValue;
    //Float that will store the players position
    private float xPosition = 0f;
    //Float that will be updated to reflect the other players position (AI ONLY)
    private float enemyXPosition = 0f;
    //Integer that will store the difficulty level of the character (AI ONLY)
    private int difficultyLevel = 0;

    
    // Start is called before the first frame update
    void Awake()
    {
        //Assigns the scrollbar instance to the one attached to the healthbar
        scrollBar = GetComponent<Scrollbar>();
        //Intialises the health variables
        maxHealth = 5;
        currentHealth = 5;

        //Intialises all the boolean values to false
        playerLost = false;
        playerWin = false;
        continueSelection = false;
        chosenContinue = false;
        gameNearlyOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Updates the fill size value based on the current health
        fillValue = currentHealth / maxHealth;
        //Assigns fillsize value to the scrollbar
        scrollBar.size = fillValue;
    }

    //Assigns the values to the health bar and the difficulty level(AI ONLY)
    public void SetHealth(int newHealth, int newLevel)
    {
        maxHealth = newHealth;
        currentHealth = maxHealth;
        difficultyLevel = newLevel;
    }

    //Function that subtracts 1 from the current health
    public void Damage()
    {
        currentHealth--;
    }

    //Function that will take in a boolean and update if the player has won or lost
    public void timeOut(bool result)
    {
        //If the boolean passed in is true set the player win bool to true
        if (result)
        {
            playerWin = true;
        }
        //If it is not true set the player lost bool to true
        else
        {
            playerLost = true;
        }
    }

    //Getter function that returns the fill value of the healthbar
    public float getFillValue()
    {
        return fillValue;
    }

    //Getter function that returns the boolean value if the player has won
    public bool getWinStatus()
    {
        return playerWin;
    }

    //Getter function that returns the boolean value if the player has lost
    public bool getLoseStatus()
    {
        return playerLost;
    }

    //Getter function that returns the xPosition of the character assigned to this health bar
    public float getPosition()
    {
        return xPosition;
    }

    //Getter function that returns the xPosition of the opposing character(AI ONLY)
    public float getEnemyXPosition()
    {
        return enemyXPosition;
    }

    //Getter function that returns the players choice if they want to continue (PLAYER ONLY)
    public bool getContinue()
    {
        return continueSelection;
    }

    //Getter function that returns if the player has chosen if they want to continue (PLAYER ONLY)
    public bool getChoiceContinue()
    {
        return chosenContinue;
    }

    //Getter function that returns the boolean that determines if the game is nearly over(AI ONLY)
    public bool getGameNearlyOver()
    {
        return gameNearlyOver;
    }

    //Getter function that returns the unique index for the healthbar
    public int gethealthBarIndex()
    {
        return healthBarIndex;
    }

    //Getter function that returns the difficulty level of the character associated with this health bar (AI ONLY)
    public int getDifficultyLevel()
    {
        return difficultyLevel;
    }

    //Setter function that takes in the current position of the player associated with this health bar
    public void setPosition(float NewXPosition)
    {
        xPosition = NewXPosition;
    }

    //Setter function that takes in the current position of the opposing player (AI ONLY)
    public void setEnemyXPosition(float NewXPosition)
    {
        enemyXPosition = NewXPosition;
    }

    //Setter function that inverts the boolean to determine if they player wants to continue or not (PLAYER ONLY)
    public void setContinue()
    {
        continueSelection = !continueSelection;
    }

    //Setter function that sets the boolean function to determine if the player has made their decision (PLAYER ONLY)
    //If they want to continue or not to true
    public void chooseContinue()
    {
        chosenContinue = true;
    }

    //Setter function that sets the boolean to determine if the game is nearly over to true (AI ONLY)
    public void gameIsNearlyOver()
    {
        gameNearlyOver = true;
    }
}
