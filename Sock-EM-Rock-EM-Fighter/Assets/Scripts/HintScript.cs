using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Class that will show a random hint and will load the title screen after 7 seconds
public class HintScript : MonoBehaviour
{
    //Text class that will store the random hint
    public Text hintText;
    //Integer that will store the random number that will determine which hint is shown
    private int hintID;
    //Float that will be used to countdown to load the next scene
    private float hintCountdown;

    // Start is called before the first frame update
    void Start()
    {
        //Sets the text to false so that the hint does not change when loading into the scene
        hintText.gameObject.SetActive(false);
        //Assigns the value for the countdown to 7
        hintCountdown = 7f;
        //Starts the coroutine that will display the random hint for a set amount of time
        //then transition back to the title screen after the countdown is over
        StartCoroutine(DisplayHint());
    }

    IEnumerator DisplayHint()
    {
        //Randomly chooses a number between 1-5
        int newRandom = Random.Range(1, 5);
        //Assigns random number to previously generated number
        hintID = newRandom;

        //Checks the random number and displays a hint based on that number
        if (hintID == 1)
        {
            hintText.text = "Kick attacks can't be Blocked";
        }
        if(hintID == 2)
        {
            hintText.text = "Kick attacks can't be Blocked but they are slow, so you can counterattack before they kick";
        }
        if(hintID == 3)
        {
            hintText.text = "In single Player the CPU will have more overall health than you and get more the more times you continue";
        }
        if(hintID == 4)
        {
            hintText.text = "If you block too often the CPU will start kicking more";
        }
        if(hintID == 5)
        {
            hintText.text = "The CPU will attack more often once they get close to you";
        }

        //Sets the text to true so that the hint is displayed on the screen
        hintText.gameObject.SetActive(true);

        //loops while the value for the countdown is greater than 0
        while (hintCountdown > 0)
        {
            //repeats this loop after one second
            yield return new WaitForSeconds(1f);
            //subtracts the countdown by 1
            hintCountdown--;
        }

        //Loads in the Titlescreen scene once the countdown has finished
        SceneManager.LoadScene("TitleScreen");
    }
}
