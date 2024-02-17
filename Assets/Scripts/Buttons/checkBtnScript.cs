using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkBtnScript : MonoBehaviour
{
    public GameObject dh;
    public DragHandle dragHandle;
    public bool pidIsEntered;
    public bool confirmedOnce;
    

    // Start is called before the first frame update
    void Start()
    {
        confirmedOnce = false;
        dh = GameObject.Find("DragHandle");
        pidIsEntered = false;
    }

    // Update is called once per frame
    void Update()
    {
        //print(dragHandle.ChosenEmoji);
    }

     public void ConfirmEmoji(){
        if (confirmedOnce)
        {
            return;
        }
        dragHandle = dh.GetComponent<DragHandle>();

        if (dragHandle == null)
        {
            print("No DragHandle in Scene");
            return;
        }

        Debug.Log("Confirm Emotion Selection");
        

        // Edge Case Checking
        if (dragHandle.CurrentEmoji == "") {
            Debug.Log("No Emoji was Selected");
            return;
        }
        else
        {
            dragHandle.isConfirmed = true;
            confirmedOnce = true;
            
        }

        


        
    }

    public void pidSumbit()
    {
        pidIsEntered = true;
    }


}
