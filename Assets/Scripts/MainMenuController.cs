using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public void PlayGame(){
        Debug.Log("button click");
        SceneManager.LoadScene("ScenariosMenu");
    }

    public void emotionalThermometerLearn()
    {
        Debug.Log("going to emotional thermometer - learn");
        SceneManager.LoadScene("Emotional Thermometer");
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }

    public void scenario2Learn(){
        Debug.Log("going to scenario 2 - learn");
        SceneManager.LoadScene("Scenario2");
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }
    public void scenario3Learn(){
        Debug.Log("going to scenario 3 - learn");
        SceneManager.LoadScene("Scenario3");
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }

    public void scenario4Learn()
    {
        Debug.Log("going to scenario 4 - learn");
        SceneManager.LoadScene("Scenario4");
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }

    public void scenario5Learn()
    {
        Debug.Log("going to scenario 5 - learn");
        SceneManager.LoadScene("Scenario5");
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }

    public void scenario6Learn()
    {
        Debug.Log("going to scenario 6 - learn");
        SceneManager.LoadScene("Scenario6");
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }

}
