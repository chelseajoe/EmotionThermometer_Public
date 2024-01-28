using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PIDInput : MonoBehaviour
{
    public TMP_InputField inputField;
    public checkBtnScript checkBtn;
    private GameObject dbManager;

    void Start()
    {
        // NOTE: where DatabaseManager is searched for 
        dbManager = GameObject.Find("DatabaseManager");
    }

    void Update()
    {
        if (checkBtn.pidIsEntered)
        {
            dbManager.GetComponent<DatabaseManager>().userId = inputField.text;
            dbManager.GetComponent<DatabaseManager>().ReadStringInput();

        }
    }
}
