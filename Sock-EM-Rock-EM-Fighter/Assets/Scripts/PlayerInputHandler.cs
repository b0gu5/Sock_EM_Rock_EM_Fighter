using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//Class that will use user input and will call on functions to reflect these inputs on the characterScript
public class PlayerInputHandler : MonoBehaviour
{
    //Float that will represent the direction that the player will move
    float xDirection;

    //String that will update which animation state the player character will be in
    private string animationState = "";

    //Instance of the player input 
    private PlayerInput playerInput;

    //Instances of the InputAction class to be called on input
    private InputAction leftAction;
    private InputAction rightAction;
    private InputAction punchAction, kickAction, blockAction;
    private InputAction chooseContinue, continueWithGame;

    //Instance of the character script class
    private CharacterScript character;

    //Will store the current scene that the game is in
    Scene currentScene;
    //String that will store the name of the current scene
    string sceneName;

    private void Awake()
    {
        //Gets the Current Scene
        currentScene = SceneManager.GetActiveScene();
        //Gets the Current Scene name
        sceneName = currentScene.name;

        //Assings the playerInput instance to the playerinput script in the scene
        playerInput = GetComponent<PlayerInput>();
        //var that will store a character in the scene
        var characters = FindObjectsOfType<CharacterScript>();
        //var that will store the playerIndex of the character
        var index = playerInput.playerIndex;

        //checks what scene the game is in
        //This is to ensure that if multiple controllers are plugged in they will control
        //only one character in single player
        if (sceneName == "1PlayerScene")
        {
            //every instance of the character is set to Player 1
            character = characters.FirstOrDefault(m => m.getPlayerIndex() == 0);
        }
        else if(sceneName == "2PlayerScene")
        {
            //every instance of the character is set to the 2 players on screen
            character = characters.FirstOrDefault(m => m.getPlayerIndex() == index);
        }

        //Assigns the corresponding values to the correct action
        leftAction = playerInput.actions["Left_Movement"];
        rightAction = playerInput.actions["Right_Movement"];
        punchAction = playerInput.actions["Punch"];
        kickAction = playerInput.actions["Kick"];
        blockAction = playerInput.actions["Block"];
        chooseContinue = playerInput.actions["Select"];
        continueWithGame = playerInput.actions["Confirm"];
    }

    // Update is called once per frame
    void Update()
    {
        //Checks if this player has won and change the control scheme 
        //that will let the winning player choose if they want to continue
        if(character.getWinStatus())
        {
            playerInput.SwitchCurrentActionMap("TitleScreen");
        }

        //Calls on the action for the left move if the player inputs the left button
        leftAction.started += leftMove;
        //Calls on the stop action if the left move button is released
        leftAction.canceled += stopMoving;

        //Calls on the action for the right move if the player inputs the right button
        rightAction.started += rightMove;
        //Calls on the stop action if the right move button is released
        rightAction.canceled += stopMoving;

        //Calls on the block action once the player inputs the block button
        blockAction.started += blocking;
        //Calls on the stop blocking action if the block button is released
        blockAction.canceled += stopBlocking;

        //Calls on the punch action once the player inputs the punch button
        punchAction.performed += punch;
        //Calls on the kick action once the player inputs the kick button
        kickAction.performed += kick;

        //Passes in the Updated xDirection assigned from either the left, right or stop moving functions
        character.updateDirection(xDirection);

        //Calls on the action to choose if the player wants to continue if the player inputs up or down
        chooseContinue.performed += selectContinue;
        //Calls on the action to let the player continue to their choosen scene if the player inputs to continue
        continueWithGame.performed += continueGame;
    }

    //Function that will move the player to the left
    public void leftMove(InputAction.CallbackContext context)
    {
        //Updates the xDirection so that the player moves left
        xDirection = -1f;
        //Updates the animation state so that it animates the character moving
        animationState = "Move";
        //Passes in the new animation state to the character so that it is reflected in the characterScript
        character.updateAnimationState(animationState);
    }

    //Function that will move the player to the right
    private void rightMove(InputAction.CallbackContext context)
    {
        //Updates the xDirection so that the player moves right
        xDirection = 1f;
        //Updates the animation state so that it animates the character moving
        animationState = "Move";
        //Passes in the new animation state to the character so that it is reflected in the characterScript
        character.updateAnimationState(animationState);
    }

    //Function that will stop the player moving
    private void stopMoving(InputAction.CallbackContext context)
    {
        //Updates the xDirection so that the player is not moving
        xDirection = 0f;
        //Passes in an empty animation state so that the characterScript stops animating the player moving
        character.updateAnimationState("");
    }

    //Function that will make the player block
    private void blocking(InputAction.CallbackContext context)
    {
        //Updates the animation state so that it animates the character blocking
        animationState = "Block";
        //Passes in the new animation state to the character so that it is reflected in the characterScript
        character.updateAnimationState(animationState);
    }

    //Function that will make the player stop blocking
    private void stopBlocking(InputAction.CallbackContext context)
    {
        //Updates the animation state so that it animates the character not blocking
        animationState = "NotBlocking";
        //Passes in the new animation state to the character so that it is reflected in the characterScript
        character.updateAnimationState(animationState);
    }

    //Function that will make the player punch
    private void punch(InputAction.CallbackContext context)
    {
        //Updates the animation state so that it animates the character punching
        animationState = "Punch";
        //Passes in the new animation state to the character so that it is reflected in the characterScript
        character.updateAnimationState(animationState);
    }

    //Function that will make the player kick
    private void kick(InputAction.CallbackContext context)
    {
        //Updates the animation state so that it animates the character kicking
        animationState = "Kick";
        //Passes in the new animation state to the character so that it is reflected in the characterScript
        character.updateAnimationState(animationState);
    }

    //Function that will let the player change if they want to continue or not
    private void selectContinue(InputAction.CallbackContext context)
    {
        //Updates the boolean in the characterScript to reflect the players choice
        character.continueSelection();
    }

    //Function that will let the player change if they want to continue or not
    private void continueGame(InputAction.CallbackContext context)
    {
        //calls on the function that will indicate that the player has chosen
        //if they want to continue or not
        character.chooseToContinue();
    }
}
