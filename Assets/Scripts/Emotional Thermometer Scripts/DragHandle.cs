using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandle : MonoBehaviour
{
    [SerializeField] GameObject TopBoundary;
    [SerializeField] GameObject BottomBoundary;
    public string ChosenEmoji;
    public string CurrentEmoji;
    public DatabaseManager databaseManager;


    public bool isConfirmed = false;
    bool sentMessage = false;

    private void Start()
    {
        ChosenEmoji = "";
        CurrentEmoji = "";
    }
    void Update()
    {
        //print(CurrentEmoji);
        if (Input.GetMouseButtonDown(0))
        {
            // Tap Functionality
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null && hit.collider.tag == "Tap")
            {
                Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 6.33f);
                Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                objPosition.x = transform.position.x;
                objPosition.z = transform.position.z;
                transform.position = objPosition;

            }

            
        }
        if (isConfirmed)
        {
            dragSpeed = 0;
            this.enabled = false;

            if (!sentMessage)
            {
                // NOTE: send firebase message here

                // convert emotion to lowercase for correct key
                string emotion = ChosenEmoji.Substring(3).ToLower();
                print(ChosenEmoji);
                print(emotion);
                GameObject dbManager = GameObject.Find("DatabaseManager");
                DatabaseManager databaseManager = dbManager.GetComponent<DatabaseManager>();
                databaseManager.EmotionIdentified(emotion, "this is a placeholder");
                //databaseManager.
                
                sentMessage = true;
            }

        }
    }



    public float dragSpeed = 1f;


    void OnMouseDrag()
    {
        if (isConfirmed)
        {
            return;
        }


        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 6.33f); //
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        objPosition.x = transform.position.x;
        objPosition.z = transform.position.z;
        transform.position = objPosition;

        if (transform.position.y >= TopBoundary.gameObject.transform.position.y)
        {
            transform.position = TopBoundary.gameObject.transform.position;
        }

        if (transform.position.y <= BottomBoundary.gameObject.transform.position.y)
        {
            transform.position = BottomBoundary.gameObject.transform.position;
        }

    }


    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Boundary")
        {
            transform.position = other.gameObject.transform.position;
        }
           
    }


}