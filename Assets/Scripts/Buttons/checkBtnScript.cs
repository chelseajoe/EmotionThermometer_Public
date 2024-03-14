using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkBtnScript : MonoBehaviour
{
    public GameObject etc;
    public EmotionThermometerController emotionThermometerController;
    public bool pidIsEntered;
    public bool confirmedOnce;
    

    // Start is called before the first frame update
    void Start()
    {
        confirmedOnce = false;
        etc = GameObject.Find("EmotionThermometerController");
        pidIsEntered = false;
    }

    // Update is called once per frame
    void Update()
    {
    }


     public void ConfirmEmoji(){
        if (confirmedOnce)
        {
            return;
        }
        emotionThermometerController = etc.GetComponent<EmotionThermometerController>();

        if (emotionThermometerController == null)
        {
            print("No EmotionThermometerController in Scene");
            return;
        }

        Debug.Log("Confirm Emotion Selection");
        

        // Edge Case Checking
        if (emotionThermometerController.CurrentEmoji == "") {
            Debug.Log("No Emoji was Selected");
            return;
        }
        else
        {
            emotionThermometerController.isConfirmed = true;
            confirmedOnce = true;
        }

    }

    public void pidSumbit()
    {
        pidIsEntered = true;
    }


}
