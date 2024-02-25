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

    public DatabaseManager databaseManager;

    //void Awake()
    //{
    //    databaseManager = DatabaseManager.Instance;
    //    Debug.Log("PIDInput Awake");
    //}

    void Start()
    {
        // NOTE: where DatabaseManager is searched for 
        dbManager = GameObject.Find("DatabaseManager");
        databaseManager = DatabaseManager.Instance;
    }

    void Update()
    {
        if (checkBtn.pidIsEntered)
        {
            dbManager.GetComponent<DatabaseManager>().userId = inputField.text;
            dbManager.GetComponent<DatabaseManager>().ReadStringInput();


            //DatabaseManager.Instance.userId = inputField.text;
            //DatabaseManager.Instance.ReadStringInput(); 

        }
    }
}
