using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class backBtnScript : MonoBehaviour
{
    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    // void Update()
    // {
        
    // }
    public void goBack(){
        Debug.Log("going back all scenarios menu");
        SceneManager.LoadScene("ScenariosMenu");
        //Debug.Log("going back to previous scene");
        // SceneManager.LoadScene("MainMenu");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -1);
    }
    public void goBackToAllScenarios(){
        Debug.Log("going back all scenarios menu");
        SceneManager.LoadScene("ScenariosMenu");
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -1);
    }
}
