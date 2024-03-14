using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    public void PlayGame(){
        Debug.Log("button click");
        SceneManager.LoadScene("ScenariosMenu");
    }

    public void emotionThermometerLearn()
    {
        Debug.Log("going to emotional thermometer - learn");
        SceneManager.LoadScene("Emotion Thermometer");
    }

    public void scenario2Learn()
    {
        Debug.Log("going to scenario 2 - learn");
        SceneManager.LoadScene("Scenario2");
    }
    public void scenario3Learn()
    {
        Debug.Log("going to scenario 3 - learn");
        SceneManager.LoadScene("Scenario3");
    }

    public void scenario4Learn()
    {
        Debug.Log("going to scenario 4 - learn");
        SceneManager.LoadScene("Scenario4");
    }

    public void scenario5Learn()
    {
        Debug.Log("going to scenario 5 - learn");
        SceneManager.LoadScene("Scenario5");
    }

    public void scenario6Learn()
    {
        Debug.Log("going to scenario 6 - learn");
        SceneManager.LoadScene("Scenario6");
    }

}
