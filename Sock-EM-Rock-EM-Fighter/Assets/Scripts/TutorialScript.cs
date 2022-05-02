using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Class that stores displays the tutorial information and allows the player to progress to the respective game scene
public class TutorialScript : MonoBehaviour
{
    //Will store the current scene that the game is in
    Scene currentScene;
    //String that will store the name of the current scene
    string sceneName;

    //Function that will transition the game scene based on the tutorial screen
    public void gameStart()
    {
        //Gets the Current Scene
        currentScene = SceneManager.GetActiveScene();
        //Gets the Current Scene name
        sceneName = currentScene.name;
        //Checks if the scene is the Player 1 Tutorial Scene
        //And transitions to the Player 1 Scene
        if(sceneName == "1PlayerTutorial")
        {
            SceneManager.LoadScene("1PlayerScene");
        }
        //Checks if the scene is the Player 2 Tutorial Scene
        //And transitions to the Player 2 Scene
        if (sceneName == "2PlayerTutorial")
        {
            SceneManager.LoadScene("2PlayerScene");
        }
    }
}
