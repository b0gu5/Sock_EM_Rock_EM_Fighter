using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

//Class that determine what actions the player will do through various parameters
//AI script
public class EnemyAI : MonoBehaviour
{
    //Float that will represent the direction that the AI will move
    float xDirection;

    //instance of the AI for the singelton pattern
    public static EnemyAI instance;
    //String that will update which animation state the player character will be in
    private string animationState = "";
    //Integers that represent the AI's health and the player's health
    private int opponentHealth, AIHealth;
    //Integer that represents the difficulty for the level
    private int difficultyLevel;
    //Boolean that determines if the AI is done resetting
    private bool resetFinished;
    //float that stores the AI's X position
    private float AIXPosition;
    //float that stores the player's x position
    private float enemyXPosition;
    //integer that determines if the player will move towards or away from you
    private int movementDecider = 30;

    //array of integers that will be checked to affect how the AI behaves
    //KEY 
    //0 is the number of times the AI has been punched by the player 
    //1 is the number of times the AI has been kicked by the player 
    //2 is the number of attacks the AI has blocked an attack from the player 
    //3 is the number of times the AI has punched the player
    //4 is the number of times the AI has kicked the player 
    //5 is the number of times the AI has had an attack blocked 
    private int[] behaviourParameters = new int[6];

    //integer that will be checked to affect how the AI attacks
    //1 is intended to be blocking
    //2 is intended to be punching
    //3 is intended to be kicking
    private int attackDecider;

    //instance of the character script to call on the functions to update it
    private CharacterScript AICharacter;
    //boolean that checks if the player is attacking
    private bool isAttacking = false;

    //integer that will store a random value
    private int randomChance;

    //Will store the current scene that the game is in
    Scene currentScene;
    //String that will store the name of the current scene
    string sceneName;

    private void Awake()
    {
        //Singelton pattern
        //doesn't delete the AI script if another scene is loaded
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        //var that will store a character in the scene
        var characters = FindObjectsOfType<CharacterScript>();
        //every instance of the character is set to Player 2 in the AI scene
        AICharacter = characters.FirstOrDefault(m => m.getPlayerIndex() == 3);
        //loop that sets all the behaviour parameters to 0
        for(int i = 0; i < 6; i++)
        {
            behaviourParameters[i] = 0;
        }
        //Sets both health values to default value
        opponentHealth = 5;
        AIHealth = 5;
        //Sets the difficulty level to 1 
        difficultyLevel = 1;
        //intialise the random value to 1 to avoid dividing by 0
        randomChance = 1;
        //sets the reset to true so that it can be reset
        resetFinished = true;
        //sets the initial attack decider to 2
        attackDecider = 2;

        //Calls on the random generation function every 2 seconds
        InvokeRepeating("getRandom", 2f, 1f);
        //Calls on the function to stop blocking every 2 seconds
        //to ensure that the AI does not stop blocking
        InvokeRepeating("blockingForTooLong", 2f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        //Updates the AI's and the players position
        AIXPosition = AICharacter.getPosition();
        //This will be updated every second as it from the timer
        enemyXPosition = AICharacter.getEnemyXPosition();

        //Checks that the AI has not won and has not lost
        if (!AICharacter.getWinStatus() && !AICharacter.getLoseStatus())
        {
            //Updates the isAttacking boolean from this instance of the characterscript 
            isAttacking = AICharacter.getIsAttacking();
            //Checks that the AI isn't attacking
            if (!isAttacking)
            {
                //checks if the difference between the AI and the Player is less than 5 or is greater than -5
                if ((AIXPosition - enemyXPosition < 5) || (AIXPosition - enemyXPosition > - 5))
                {
                    //Sets the movement decider to 30 so that the AI is more likely to move
                    movementDecider = 30;
                }
                //checks if the difference between the AI and the Player is greater than 5 or is less than -5
                if ((AIXPosition - enemyXPosition > 5) || (AIXPosition - enemyXPosition < -5))
                {
                    //Sets the movement decider to -30 so that the AI is more likely to attack
                    movementDecider = -30;
                }

                //if AI has been punched frequently
                //the number of times the AI is punched is greater than the AI's health
                if(behaviourParameters[0] > AIHealth)
                {
                    //Changes the attackDecider to be in favour of blocking
                    attackDecider = 1;
                }
                //if AI has been kicked frequently
                //the number of times the AI is kicked is greater than the AI's health
                else if (behaviourParameters[1] > AIHealth)
                {
                    //Changes the attackDecider to be in favour of punching
                    attackDecider = 2;
                }
                //if AI has blocked frequently
                //the number of times the AI has blocked is greater than the AI's health
                else if (behaviourParameters[2] > AIHealth)
                {
                    //Changes the attackDecider to be in favour of punching
                    attackDecider = 2;
                }
                //if the AI has punched the player frequently
                //the number of times the AI has punched is greater than the Opponents health
                else if(behaviourParameters[3] > opponentHealth)
                {
                    //Changes the attackDecider to be in favour of kicking
                    attackDecider = 3;
                }
                //if the AI has kicked the player frequently
                //the number of times the AI has kicked is greater than the Opponents health
                else if (behaviourParameters[4] > opponentHealth)
                {
                    //Changes the attackDecider to be in favour of punching
                    attackDecider = 2;
                }
                //If AI has had attacks blocked frequently
                //the number of times the AI has had an attack blocked is greater than the times the AI has been punched
                //by the player multiplied by 2
                else if (behaviourParameters[5] > behaviourParameters[0] * 2)
                {
                    //Changes the attackDecider to be in favour of kicking
                    attackDecider = 3;
                }

                //Checks if the random number + the movemenet decider is less than 70
                //so if the movement decider is -30 they are more likely to move towards the player
                if(randomChance + movementDecider < 70)
                {
                    //Stops the AI from blocking so it can move
                    stopBlocking();
                    //checks if the random chance divided by the difficulty level is less than 30
                    //so if the difficulty level is higher the ai will be more likely to move towards you
                    if (randomChance / difficultyLevel < 30)
                    {
                        leftMove();
                    }
                    //checks if the random chance divided by the difficulty level is greater than 30
                    //so if the difficulty level is lower the ai will be more likely to move away from you
                    else if (randomChance / difficultyLevel > 30)
                    {
                        rightMove();
                    }
                }
                //Checks if the random number + the movemenet decider is greater than 70
                //so if the movement decider is 30 they are more likely to attack the player
                else if (randomChance + movementDecider > 70)
                {
                    //Stops the AI from blocking so it can move
                    stopBlocking();
                    //checks if the attackdecider multiplied by the random chance is less than 80
                    //so if the attack Decider is 1 the AI will be more likely to block
                    if (attackDecider * randomChance < 80)
                    {
                        blocking();
                    }
                    //checks if the attackdecider multiplied by the random chance is between 80 and 160
                    //so if the attack Decider is 2 the AI will be more likely to attack punch
                    else if (attackDecider * randomChance >= 80 && attackDecider * randomChance <= 160)
                    {
                        punch();
                    }
                    //checks if the attackdecider multiplied by the random chance is greater than 160
                    //so if the attack Decider is 3 the AI will be more likely to kick
                    else if (attackDecider * randomChance > 160)
                    {
                        kick();
                    }
                }

                //Checks if the game is nearly over
                if (AICharacter.getGameNearlyOver())
                {
                    //If the AI's health is greater than or equal to the players health
                    //The AI will try and run away from the player to win by time out
                    if (AIHealth >= opponentHealth)
                    {
                        rightMove();
                    }
                    //If the AI's health is less than the players health
                    //The AI will follow go after the player
                    else if (AIHealth < opponentHealth)
                    {
                        leftMove();
                    }
                }
            }
        }

        //checks if the AI has won or lost to stop them from moving
        else if (AICharacter.getWinStatus())
        {
            difficultyLevel = 1;
            stopMoving();
        }
        else if (AICharacter.getLoseStatus())
        {
            stopMoving();
        }

        //gets resetfinished from the character script
        resetFinished = AICharacter.getAIReset();
        //checks if resetfinished is true and resets the parameters for a new game
        if(resetFinished)
        {
            resetBehaviourParameters();
        }

        //Updates the behaviour parameters based on the characterscript passing in corresponding values
        updateBehaviourParameters(AICharacter.getBehaviourParameter());


        //Gets the Current Scene
        currentScene = SceneManager.GetActiveScene();
        //Gets the Current Scene name
        sceneName = currentScene.name;

        //Checks if the current scene is not the 1Player scene and destroys this instance of the class if it isn't
        if (sceneName != "1PlayerScene")
        {
            Destroy(gameObject);
        }

    }

    //Function that will generate a random number every 2 seconds
    private void getRandom()
    {
        //New integer will be generated between 1-101
        int newRandom = Random.Range(1, 101);
        //Assigns the new random number to the random chance
        randomChance = newRandom;
    }

    //Function that will move the AI to the left
    public void leftMove()
    {
        //Stop the AI from blocking so that it can move
        stopBlocking();
        //Updates the xDirection so that the AI moves left
        xDirection = -1f;
        //Passes in the Updated xDirection
        AICharacter.updateDirection(xDirection);
        //Updates the animation state so that it animates the character moving
        animationState = "Move";
        //Passes in the new animation state to the character so that it is reflected in the characterScript
        AICharacter.updateAnimationState(animationState);
    }

    //Function that will move the AI to the right
    private void rightMove()
    {
        //Stop the AI from blocking so that it can move
        stopBlocking();
        //Updates the xDirection so that the AI moves right
        xDirection = 1f;
        //Passes in the Updated xDirection
        AICharacter.updateDirection(xDirection);
        //Updates the animation state so that it animates the character moving
        animationState = "Move";
        //Passes in the new animation state to the character so that it is reflected in the characterScript
    }

    //Function that will stop the AI moving
    private void stopMoving()
    {
        //Updates the xDirection so that the player is not moving
        xDirection = 0f;
        //Passes in the Updated xDirection
        AICharacter.updateDirection(xDirection);
        //Passes in an empty animation state so that the characterScript stops animating the player moving
        AICharacter.updateAnimationState("");
    }

    //Function that will make the AI block
    private void blocking()
    {
        //Updates the animation state so that it animates the character blocking
        animationState = "Block";
        //Passes in the new animation state to the character so that it is reflected in the characterScript
        AICharacter.updateAnimationState(animationState);
    }

    //Function that will make the AI stop blocking
    private void stopBlocking()
    {
        //Updates the animation state so that it animates the character not blocking
        animationState = "NotBlocking";
        //Passes in the new animation state to the character so that it is reflected in the characterScript
        AICharacter.updateAnimationState(animationState);
    }

    //Function that will make the AI punch
    private void punch()
    {
        //Updates the animation state so that it animates the character punching
        animationState = "Punch";
        //Passes in the new animation state to the character so that it is reflected in the characterScript
        AICharacter.updateAnimationState(animationState);
    }

    //Function that will make the AI kick
    private void kick()
    {
        //Updates the animation state so that it animates the character kicking
        animationState = "Kick";
        //Passes in the new animation state to the character so that it is reflected in the characterScript
        AICharacter.updateAnimationState(animationState);
    }

    //Function that will be called every 2 seconds to check if the AI has been blocking for too long
    private void blockingForTooLong()
    {
        //Checks if the attackDecider is in favour of blocking
        if (attackDecider == 1)
        {
            //Calls on the stopBlocking function
            stopBlocking();
            //Changes the attackDecider to be in favour of punching
            attackDecider = 2;
        }
    }

    //Function that will update the parameters on how the AI should behave
    public void updateBehaviourParameters(int parameterID)
    {
        //Checks that that the parameter ID is in range of the array
        //To avoid the index of the array being out of bounds
        if(parameterID < 6)
        {
            //Adds one to the value of the parameter
            behaviourParameters[parameterID]++;

            //Checks if the parameter ID is between 3 and 4
            //I.E the AI has punched or kicked the player
            if (parameterID >= 3 && parameterID <= 4)
            {
                //Subtract one from the player's representation of their health
                opponentHealth--;
            }
            //Checks if the parameter ID is between 0 and 1
            //I.E the AI has been punched or kicked by the player
            else if (parameterID >= 0 && parameterID <= 1)
            {
                //Subtract one from the AI's representation of their health
                AIHealth--;
            }
        }
        //if the parameterID is 6 the AI will kick
        else if(parameterID == 6)
        {
            stopMoving();
            kick();
        }
        //if the parameterID is 7 the AI will punch
        else if (parameterID == 7)
        {
            stopMoving();
            punch();
        }
    }

    //Function that resets the parameters for a new game
    public void resetBehaviourParameters()
    {
        //checks that if the reset has finished
        if(resetFinished)
        {
            //makes the resetfinished to false so that this function doesn't trigger again
            resetFinished = false;
            //loop that sets all the behaviour parameters to 0
            for (int i = 0; i < 6; i++)
            {
                behaviourParameters[i] = 0;
            }
            //Increases the difficulty level by 1 to make it harder
            difficultyLevel++;
            //Increases the AI's Health representation to 5 + the difficulty level
            //I.E if it is level 3 the AI's health will be 8
            AIHealth = 5 + difficultyLevel;
            //Sets the player's health representation to 5
            //the default value
            opponentHealth = 5;
        }
        
    }
}
