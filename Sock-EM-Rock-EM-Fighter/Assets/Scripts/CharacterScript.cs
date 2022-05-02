using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

//Class that will update the players animation and allows the position to be manipulated
//either by AI or player input
public class CharacterScript : MonoBehaviour
{
    //float variable that will affect the speed of the player characters movements
    private float speed = 5;

    //float variable that will affect the direction of the characters movement
    float xDirection;

    //instance of the characterScript for the singelton pattern
    public static CharacterScript instance;

    //Index to uniquely identify the healthbars
    [SerializeField]
    private int playerIndex;

    //Rigidbody to give the player character a collision box
    private Rigidbody2D characterBody;
    //Animator to play the character animations
    private Animator characterAnim;
    //Sprite Renderer to render the character animations
    private SpriteRenderer characterSpriteRenderer;
    //Instance of the Healthbar class to show the players health to the player
    public HealthBar healthBar;

    //Strings used to represent the character animations for readability and to remove errors
    private string FORWARD_ANIMATION = "Forward";
    private string BACK_ANIMATION = "Back";
    private string KICK_ANIMATION = "Kick";
    private string PUNCH_ANIMATION = "Punch";
    private string BLOCK_ANIMATION = "Block";
    private string HIT_ANIMATION = "Hit";
    private string KO_ANIMATION = "KO";
    private string WIN_ANIMATION = "Win";
    private string BROKE_ANIMATION = "BlockBroken";

    //Boolean to signal if the character is attacking
    private bool isAttacking = false;
    //Boolean to signal if the character is blocking
    private bool isBlocking = false;
    //Boolean to signal if the character is being hit
    private bool isBeingHit = false;
    //Boolean to signal if the character has won
    private bool hasWon = false;
    //Boolean to signal if the character has lost
    private bool hasLost = false;
    //Boolean to signal if the in game timer is nearly over
    private bool gameIsNearlyOver = false;
    //String to be compared to see what animation state the character should be in
    private string animationState = "";
    //Integer that will store the health of the player
    private int health = 5;
    //Float to alter the direction of the knockback based on which side the player is on
    float knockBackDirection = -1.0f;

    //Variables the AI will use
    //Vector to store the starting position of the AI
    private Vector2 startingPosition;
    //Boolean to determine if the AI needs to reset its values for a new game
    private bool AIReset = false;
    //Integer that will store the difficulty level of the AI (Only used by AI)
    private int difficultyLevel = 0;
    //Boolean that indicates that there has been a change to the AI's parameters
    private bool parameterChange = false;
    //Integer to determine which parameter needs updating
    private int parameterID = 7;

    //Will store the current scene that the game is in
    Scene currentScene;
    //String that will store the name of the current scene
    string sceneName;

    private void Awake()
    {
        //Assign the Unity classes to instances in game
        characterBody = GetComponent<Rigidbody2D>();
        characterAnim = GetComponent<Animator>();
        characterSpriteRenderer = GetComponent<SpriteRenderer>();
        //Singelton Pattern
        //Checks if the playerIndex is 3 (the one the AI will have)
        //And doesn't delete it upon loading a new scene
        if (playerIndex == 3)
        {
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Tags the game object with the player tag
        this.gameObject.tag = "Player";
        //Intialises the health as 5
        health = 5;
        //passes the health and the difficulty level to the health bar
        healthBar.SetHealth(health, difficultyLevel);
        //Checks that the playerIndex is greater than or equal to 1
        //Only player 1 on the left, will have an index less than 1
        if (playerIndex >= 1)
        {
            //Inverts the knockback direction
            knockBackDirection = 1.0f;
        }
        //Assigns the starting value
        startingPosition = new Vector2(6.45f, -2.28f);
    }

    // Update is called once per frame
    void Update()
    {
        //Sets the AI's reset to false so that it doesn't reset everyframe
        AIReset = false;
        //passes in the characters current position to the healthbar
        healthBar.setPosition(transform.position.x);

        //checks that the player is not being hit
        if (!isBeingHit)
        {
            //checks that player is in the animating state that lets them move
            if (animationState == "Move")
            {
                //Updates the player's position based on xDirection
                moveController();
            }
        }

        //Checks the lose and win status from the healthbar
        hasWon = healthBar.getWinStatus();
        hasLost = healthBar.getLoseStatus();

        //Animates the player based on the Animtation controller
        animatePlayer();

        //checks if the health bar instance is null
        if(healthBar == null)
        {
            //Gets the Current Scene
            currentScene = SceneManager.GetActiveScene();
            //Gets the Current Scene name
            sceneName = currentScene.name;

            //This is intended for when the player hits continue and the 1Player scene is reloaded
            //Checks if the current scene is not the 1Player scene and destroys this instance of the class if it isn't
            if (sceneName != "1PlayerScene")
            {
                Destroy(gameObject);
            }
            else
            {
                //Sets the AI's reset to true so that it can reset
                AIReset = true;
                //Sets the AI's position back to the start
                characterBody.transform.position = startingPosition;
                //var that looks through avaliable health bars in the scene
                var healthbars = FindObjectsOfType<HealthBar>();
                //Assign the instance of the healthbar in this class to the healthbar with the same index
                //as this player
                healthBar = healthbars.FirstOrDefault(m => m.gethealthBarIndex() == playerIndex);
                //Adds 1 to this players health
                health++;
                //Adds 1 to this players difficulty
                difficultyLevel++;
                //passes the health and the difficulty level to the health bar
                healthBar.SetHealth(health, difficultyLevel);
            }
        }
    }

    //Function that moves the characters position
    public void moveController()
    {
        //vector that will be used to move the players
        Vector2 movePos = transform.position;

        //updates the x position of the current position based on the xDirection
        movePos.x += xDirection * speed * Time.deltaTime;

        //checks if the x position is less than -7.5 or greater than 7.5 and sets it to that number
        //this is to stop the player going past the boundaries
        if (movePos.x < -7.5f)
        {
            movePos.x = -7.5f;
        }
        if (movePos.x > 7.5f)
        {
            movePos.x = 7.5f;
        }

        if (!isBlocking || isAttacking)
        {
            //transforms the new vector to the players current position
            transform.position = movePos;
        }
    }

    void animatePlayer()
    {
        //Checks that the player is not being hit
        if (isBeingHit)
        {
            //sets the animation state to blank
            animationState = "";
            //Assigns the corresponding animation bools to the "being hit" animation
            characterAnim.SetBool(HIT_ANIMATION, true);
            characterAnim.SetBool(BACK_ANIMATION, false);
            characterAnim.SetBool(FORWARD_ANIMATION, false);
            characterAnim.SetBool(KICK_ANIMATION, false);
            characterAnim.SetBool(PUNCH_ANIMATION, false);
            characterAnim.SetBool(BLOCK_ANIMATION, false);
            //Checks if the player hasLost value is true and plays the losing animation
            if (hasLost)
            {
                characterAnim.SetBool(KO_ANIMATION, true);
            }
        }
        //Checks if the player has won
        else if (hasWon)
        {
            //Assigns the corresponding animation bools to the "winning" animation
            characterAnim.SetBool(HIT_ANIMATION, false);
            characterAnim.SetBool(BACK_ANIMATION, false);
            characterAnim.SetBool(FORWARD_ANIMATION, false);
            characterAnim.SetBool(KICK_ANIMATION, false);
            characterAnim.SetBool(PUNCH_ANIMATION, false);
            characterAnim.SetBool(BLOCK_ANIMATION, false);
            characterAnim.SetBool(WIN_ANIMATION, true);
        }
        //checks if the player has lost
        else if (hasLost)
        {
            //Assigns the corresponding animation bools to the "losing" animation
            characterAnim.SetBool(BACK_ANIMATION, false);
            characterAnim.SetBool(FORWARD_ANIMATION, false);
            characterAnim.SetBool(KICK_ANIMATION, false);
            characterAnim.SetBool(PUNCH_ANIMATION, false);
            characterAnim.SetBool(BLOCK_ANIMATION, false);
            characterAnim.SetBool(HIT_ANIMATION, true);
            characterAnim.SetBool(KO_ANIMATION, true);
        }
        else
        {
            //Assigns the corresponding animation bools to stop the
            //winning, losing, hit and block breaking animtion
            characterAnim.SetBool(KO_ANIMATION, false);
            characterAnim.SetBool(WIN_ANIMATION, false);
            characterAnim.SetBool(BROKE_ANIMATION, false);
            characterAnim.SetBool(HIT_ANIMATION, false);
            //Checks if the player should be punching and is not moving
            if (animationState == "Punch" && xDirection == 0)
            {
                //Plays the hit sound for punch attacks
                FindObjectOfType<AudioManager>().Play("HitSound");
                //Stops the player animating both walking animations
                characterAnim.SetBool(BACK_ANIMATION, false);
                characterAnim.SetBool(FORWARD_ANIMATION, false);
                //Sets the boolean to signify that the player is attacking
                isAttacking = true;
                //Tags the game object with the punch tag
                this.gameObject.tag = "Punch";
                //Assigns the kick animation bool to animate the punch
                characterAnim.SetBool(PUNCH_ANIMATION, true);
            }
            //Checks if the player should be kicking and is not moving
            else if (animationState == "Kick" && xDirection == 0)
            {
                //Plays the hit sound for punch attacks
                FindObjectOfType<AudioManager>().Play("HitSound");
                //Stops the player animating both walking animations
                characterAnim.SetBool(BACK_ANIMATION, false);
                characterAnim.SetBool(FORWARD_ANIMATION, false);
                //Sets the boolean to signify that the player is attacking
                isAttacking = true;
                //Assigns the kick animation bool to animate the kick
                characterAnim.SetBool(KICK_ANIMATION, true);
            }
            //Checks if the player should be blocking and is not moving
            else if (animationState == "Block" && xDirection == 0)
            {
                //Sets the boolean to signify that the player is blocking
                isBlocking = true;
                //Tags the game object with the block tag
                this.gameObject.tag = "Block";
                //Assigns the kick animation bool to animate blocking
                characterAnim.SetBool(BLOCK_ANIMATION, true);

            }
            //Checks if the player should be not be blocking and is not moving
            else if (animationState == "NotBlocking" && xDirection == 0)
            {
                //Set the animation state to blank 
                animationState = "";
                //Sets the boolean to signify that the player is not blocking
                isBlocking = false;
                //Tags the game object with the player tag
                this.gameObject.tag = "Player";
                //Assigns the kick animation bool to stop animating the player blocking
                characterAnim.SetBool(BLOCK_ANIMATION, false);
            }
            //Checks if the xDirection is greater than 0 and the animation state is move
            else if (xDirection > 0 && animationState == "Move")
            {
                //Tags the game object with the player tag
                this.gameObject.tag = "Player";
                //checks if the knockback direction is less than or greater than 1
                //and animates the corresponding direction
                if (knockBackDirection < 0)
                {
                    characterAnim.SetBool(FORWARD_ANIMATION, true);
                }
                else
                {
                    characterAnim.SetBool(BACK_ANIMATION, true);
                }
            }
            //Checks if the xDirection is less than 0 and the animation state is move
            else if (xDirection < 0 && animationState == "Move")
            {
                //Tags the game object with the player tag
                this.gameObject.tag = "Player";
                //checks if the knockback direction is less than or greater than 1
                //and animates the corresponding direction
                if (knockBackDirection < 0)
                {
                    characterAnim.SetBool(BACK_ANIMATION, true);
                }
                else
                {
                    characterAnim.SetBool(FORWARD_ANIMATION, true);
                }
            }
            else
            {
                //Assigns the corresponding animation bools to stop the
                //back, forward, kick and punch animation when nothing is being passed in
                characterAnim.SetBool(BACK_ANIMATION, false);
                characterAnim.SetBool(FORWARD_ANIMATION, false);
                characterAnim.SetBool(KICK_ANIMATION, false);
                characterAnim.SetBool(PUNCH_ANIMATION, false);
            }
        }
    }

    //Collision checker function
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Checks if they collide with an object of block type
        if (collision.gameObject.CompareTag("Block"))
        {
            //This checks if this instance punched the opponent but the opponent blocked it
            if (this.gameObject.tag == "Punch")
            {
                //Knock the player back by the knockBackDirection
                characterBody.AddForce(new Vector2(knockBackDirection * 2, 2f), ForceMode2D.Impulse);
                //Assigns the parmeterID to signify that the AI punch was blocked
                parameterID = 5;
                //Sets the boolean to true to signify that there has been a parameter change
                parameterChange = true;
            }
            else if (this.gameObject.tag == "Player")
            {
                //Assigns the parmeterID to signify that the AI should Kick
                parameterID = 6;
                //Sets the boolean to true to signify that there has been a parameter change
                parameterChange = true;
            }
        }
        //Checks if they collide with an object of punch type
        else if (collision.gameObject.CompareTag("Punch"))
        {
            //This checks if the player got hit by a punch and was not blocking
            if (!isBlocking)
            {
                //Plays the grunt sound to signify that they got hit
                FindObjectOfType<AudioManager>().Play("Grunt");
                //Changes the boolean to true to signify that the player is being hit
                isBeingHit = true;
                //Damages the health bar of this player
                healthBar.Damage();
                //Assigns the parmeterID to signify that the AI has been punched
                parameterID = 0;
                //Sets the boolean to true to signify that there has been a parameter change
                parameterChange = true;
                characterBody.AddForce(new Vector2(knockBackDirection, 2f), ForceMode2D.Impulse);
            }
            //This checks if the player got hit by a punch but was blocking
            else
            {
                //Knock the player back by the knockBackDirection
                characterBody.AddForce(new Vector2(knockBackDirection * 2, 2f), ForceMode2D.Impulse);
                //Assigns the parmeterID to signify that the AI blocked a punch
                parameterID = 2;
                //Sets the boolean to true to signify that there has been a parameter change
                parameterChange = true;
            }
        }
        //This checks if the player got hit by a kick
        else if (collision.gameObject.CompareTag("Kick"))
        {
            //Plays the grunt sound to signify that they got hit
            FindObjectOfType<AudioManager>().Play("Grunt");
            //Tags the game object with the player tag
            this.gameObject.tag = "Player";
            //Sets the Animation bool so that the block breaking animation plays
            characterAnim.SetBool(BROKE_ANIMATION, true);
            //sets the animation state to blank
            animationState = "";
            //Changes the boolean to true to signify that the player isn't blocking
            isBlocking = false;
            //Assigns the parmeterID to signify that the AI was kicked
            parameterID = 1;
            //Sets the boolean to true to signify that there has been a parameter change
            parameterChange = true;
            //Changes the boolean to true to signify that the player is being hit
            isBeingHit = true;
            //Damages the health bar of this player
            healthBar.Damage();

            //Knock the player back by the knockBackDirection
            characterBody.AddForce(new Vector2(knockBackDirection, 2f), ForceMode2D.Impulse);
        }
        //This checks if the player collided with a player
        else if (collision.gameObject.CompareTag("Player"))
        {
            //Checks if this instance of the character is tagged with punch
            if (this.gameObject.CompareTag("Punch"))
            {
                //Assigns the parmeterID to signify that the AI has punched the player
                parameterID = 3;
                //Sets the boolean to true to signify that there has been a parameter change
                parameterChange = true;
            }
            //Checks if this instance of the character is tagged with kick
            else if (this.gameObject.CompareTag("Kick"))
            {
                //Assigns the parmeterID to signify that the AI kicked the player
                parameterID = 4;
                //Sets the boolean to true to signify that there has been a parameter change
                parameterChange = true;
            }
            //Checks if this instance of the character is tagged with player
            else if (this.gameObject.CompareTag("Player"))
            {
                //Assigns the parmeterID to signify that the AI should punch the player
                parameterID = 7;
                //Sets the boolean to true to signify that there has been a parameter change
                parameterChange = true;
            }
        }
    }

    //Getter function to change if the player wants to continue or not
    public void continueSelection()
    {
        healthBar.setContinue();
    }

    //Getter function to change if the player has chosen if they want to continue
    public void chooseToContinue()
    {
        healthBar.chooseContinue();
    }

    //The following 3 functions are triggered by animation triggers in the animation

    //Function that signifys that the Player is kicking
    void kickStart()
    {
        //Plays the hit sound for punch attacks
        FindObjectOfType<AudioManager>().Play("HitSound");
        //Tags the game object with the kick tag
        this.gameObject.tag = "Kick";
    }

    //Function that signifys that the Player has stopped attacking
    void attackEnd()
    {
        //Sets the boolean to signify that the player has stopped attacking
        isAttacking = false;
        //Tags the game object with the player tag
        this.gameObject.tag = "Player";
        //sets the animation state to blank
        animationState = "";
        //Stops the player animating both attacking animations
        characterAnim.SetBool(PUNCH_ANIMATION, false);
        characterAnim.SetBool(KICK_ANIMATION, false);
    }

    //Function that signifys that the Player's hit animation has ended
    void hitStunEnd()
    {
        //Checks if the players health is greater than 0 and sets
        //the bool to signify that the player is being hit
        if (health > 0)
        {
            isBeingHit = false;
        }
    }

    //Getter function to return the playerIndex
    public int getPlayerIndex()
    {
        return playerIndex;
    }

    //Getter function to return the enemy's position
    public float getEnemyXPosition()
    {
        return healthBar.getEnemyXPosition();
    }

    //Getter function to return the current position
    public float getPosition()
    {
        return transform.position.x;
    }

    //Getter function to return the has won bool
    public bool getWinStatus()
    {
        return hasWon;
    }

    //Getter function to return the has lost bool
    public bool getLoseStatus()
    {
        return hasLost;
    }

    //Getter function to return if the game is nearly over
    public bool getGameNearlyOver()
    {
        return gameIsNearlyOver;
    }

    //Getter function to return if the AI needs to be reset
    public bool getAIReset()
    {
        return AIReset;
    }

    //Getter function to return the AI parameter if it has changed
    public int getBehaviourParameter()
    {
        if(!parameterChange)
        {
            return 8;
        }
        else
        {
            parameterChange = false;
            return parameterID;
        }
    }

    //Getter function to return the boolean is Attacking
    public bool getIsAttacking()
    {
        return isAttacking;
    }

    //Setter function to set the new animation state
    public void updateAnimationState(string newState)
    {
        animationState = newState;
    }

    //Setter function to set the xDirection
    public void updateDirection(float newDirection)
    {
        xDirection = newDirection;
    }
}
