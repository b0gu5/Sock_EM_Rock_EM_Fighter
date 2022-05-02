using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Main menu class that will allow the player to choose between single or two player
//And display all this information to the player
public class MainMenuScript : MonoBehaviour
{
    //Boolean that determines if the game loads the Player 1 or Player 2 mode
    private bool gameType = false;
    //Text class that stores the arrow that will point to the game mode
    private Text selectionText; 

    //Function that will move the arrow and change the variable that will determine which game mode is chosen
    //Will be called once the player inputs W, S or up and down
    public void gameSelection()
    {
        //Assign the text class to the SelectArrow text object in the scene
        selectionText = GameObject.Find("SelectArrow").GetComponent<Text>();
        //checks the boolean to determine the game mode and will move the arrow to represent
        //which game mode is being selected
        if(gameType)
        {
            selectionText.transform.position = new Vector3(-6.66f, -1.42f, 0);
        }
        else
        {
            selectionText.transform.position = new Vector3(-6.66f, -3.1f,0);
        }

        gameType = !gameType;
    }

    //Function that will load the next game mode
    //Will be called once the player inputs enter or the down face buttons
    public void TwoPlayerGameStart()
    {
        //Checks if the boolean to determine the game mode and will load the next scene
        //based on the selected game mode
        if(gameType)
        {
            SceneManager.LoadScene("2PlayerTutorial");
        }
        else
        {
            SceneManager.LoadScene("1PlayerTutorial");
        }
    }
}
