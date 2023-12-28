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
    // Start is called before the first frame update
    void Start()
    {
        dbManager = GameObject.Find("DatabaseManager");
    }

    // Update is called once per frame
    void Update()
    {
        if (checkBtn.pidIsEntered)
        {
            dbManager.GetComponent<DatabaseManager>().userId = inputField.text;
            dbManager.GetComponent<DatabaseManager>().ReadStringInput();
        }
    }
}
