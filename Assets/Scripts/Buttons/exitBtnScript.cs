using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class exitBtnScript : MonoBehaviour
{

    public void exitApp()
    {
        Debug.Log("Exiting App");
        Application.Quit();
    }
}
