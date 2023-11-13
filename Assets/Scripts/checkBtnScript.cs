using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkBtnScript : MonoBehaviour
{
    public DragHandle dragHandle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //print(dragHandle.ChosenEmoji);
    }

     public void Confirm(){
        Debug.Log("Confirm Emotion Selection");
        

        if (dragHandle.CurrentEmoji == "") {
            Debug.Log("No Emoji was Selected");
            return;
        }
        else
        {
            dragHandle.isConfirmed = true;
            
        }

        


        
    }

}
