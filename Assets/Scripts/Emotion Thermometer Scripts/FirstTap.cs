using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstTap : MonoBehaviour
{
    // [SerializeField] GameObject SwipeUpAnim;
    bool firstTap;
    // Start is called before the first frame update
    void Start()
    {
        firstTap = false;
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetMouseButtonDown(0) && !firstTap) {
          firstTap = true;
          gameObject.SetActive(false);
        //   Debug.log("Clicked!");
        }
    }
}
