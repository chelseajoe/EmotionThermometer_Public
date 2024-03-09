using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class DragHandle : MonoBehaviour
{
    [SerializeField] GameObject TopBoundary;
    [SerializeField] GameObject BottomBoundary;
    public string ChosenEmoji;
    public string CurrentEmoji;
    public DatabaseManager databaseManager;
    public Canvas ASRCanvas;
    public float dragSpeed = 1f;
    public OpenAI.Whisper whisper;
    private GameObject dbManager;
    public string emotion;
    public List<string> displayedStrategies;
    public GameObject strategiesCanvas;
    public GameObject noPIDCanvas;
    public GameObject strategiesDisplayed;

    public AutoRosConnector autoRos;

    [SerializeField]
    Slider slider;

    private bool light_on = false;

    public RosManager rosManager;

    public bool isConfirmed = false;
    public bool canvasOn = false;
    bool sentMessage = false;


    // Random PID Generation
    private static System.Random random = new System.Random();
    public GameObject dbManagerSpare;

    // new database attempt
    DatabaseReference reference;


    private string GenerateRandomPID(int length)
    {
        const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        char[] result = new char[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = characters[random.Next(characters.Length)];
        }

        return new string(result);
    }

    private void Start()
    {

        rosManager = autoRos.GetRosManager();
        if (rosManager == null)
        {
            Debug.LogError("ROS manager does not exist.");
        }
        SubscribeToMessage();


        noPIDCanvas.gameObject.SetActive(false);
        strategiesCanvas.gameObject.SetActive(false);
        // ASR Canvas Starts Off
        ASRCanvas.gameObject.SetActive(false);


    ////NOTE: Locating DatabaseManager which should hopefully exist
    //    dbManager = GameObject.Find("DatabaseManager");
    //    //databaseManager = dbManager.GetComponent<DatabaseManager>();


    //    if (dbManager == null)
    //    {

    //        dbManagerSpare = GameObject.Find("DatabaseManagerSpare");
    //        // dbManager = new GameObject();

    //        databaseManager = dbManagerSpare.GetComponent<DatabaseManager>();

    //        if (databaseManager != null)
    //        {
    //            ChosenEmoji = "";
    //            CurrentEmoji = "";
    //            return;
    //        }
    //        databaseManager = dbManagerSpare.AddComponent<DatabaseManager>();

    //        databaseManager.ReInitializeDatabase();

    //        string randomPID = GenerateRandomPID(5);
    //        databaseManager.userId = randomPID;

    //    }
    //    else
    //    {
    //        // Get the DatabaseManager component from the existing GameObject
    //        databaseManager = dbManager.GetComponent<DatabaseManager>();
    //    }

    //    //databaseManager = DatabaseManager.Instance;


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

            if (!canvasOn)
            {
                //Show ASR Canvas with slight delay
                StartCoroutine(ShowASRCanvas());
                canvasOn = true;


                //This works fine for Jibo speach
                Debug.Log("Speak, Jibo, Speak!!");
                rosManager.SendRosMessage(new RosMessages.JiboSpeechAction("<es name='Sway_01'> There are things I am good at, such as spelling. </es> <break size='1.0'/> And then there are things I am still learning, such as listening to words. <break size='2.0'/> <es cat='curious'> If I am not able to understand you, will you help me by repeating? </es>")).Invoke();
            }


            if (!sentMessage && whisper.finishedRecording)
            {
                //Show ASR Canvas with slight delay
                StartCoroutine(ShowASRCanvas());

                // NOTE: send firebase message here
                SendEmotionFireBase();

                // Get All Emotion Strategies
                //databaseManager.GetEmotionStrategies(databaseManager.userId, emotion);
                DatabaseManager.Instance.GetEmotionStrategies(DatabaseManager.Instance.userId, emotion);

                //Hide ASR Canvas with slight delay
                StartCoroutine(HideASRCanvas());

            }

        }
    }

    private void SendEmotionFireBase()
    {
        // convert emotion to lowercase for correct key
        emotion = ChosenEmoji.Substring(3).ToLower();
        print(ChosenEmoji);
        print(emotion);
        //GameObject dbManager = GameObject.Find("DatabaseManager");
        //DatabaseManager databaseManager = dbManager.GetComponent<DatabaseManager>();

        // Send strategy message
        string strategy = whisper.childSpeech;
        print(strategy);
        //databaseManager.EmotionIdentified(emotion, "this is a placeholder");
        //databaseManager.EmotionIdentified(emotion, strategy);

        DatabaseManager.Instance.EmotionIdentified(emotion, strategy);

        sentMessage = true;
    }

    IEnumerator ShowPIDCanvas()
    {

        Debug.Log("No DatabaseManager in scene -- redirecting to enter PID Scene");
        noPIDCanvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Enter PID");

    }

    IEnumerator ShowASRCanvas()
    {
       
        yield return new WaitForSeconds(0.75f);
        ASRCanvas.gameObject.SetActive(true);
        //Debug.Log("Coroutine finished");
    }

    IEnumerator HideASRCanvas()
    {

        yield return new WaitForSeconds(0.75f);
        ASRCanvas.gameObject.SetActive(false);
        strategiesCanvas.gameObject.SetActive(true);

        //List<string> ListOfStrategies = databaseManager.emotionStrategies;
        List<string> ListOfStrategies = DatabaseManager.Instance.emotionStrategies;

        print("length: " + ListOfStrategies.Count);

        displayedStrategies = ChooseRandomStrings(ListOfStrategies, 4);


        foreach (string str in displayedStrategies)
        {
            print("strats: " + str);
            strategiesDisplayed.GetComponent<TMP_Text>().text += "-  " + str + "\n\n";
        }
        //Debug.Log("Coroutine finished");
    }

    List<string> ChooseRandomStrings(List<string> originalList, int count)
    {
        List<string> selectedStrings = new List<string>();

        if (originalList.Count <= count)
        {
            // If the original list is smaller than or equal to the requested count,
            // simply return the entire original list.
            return originalList;
        }

        // Randomly select strings from the original list
        List<string> tempList = new List<string>(originalList);

        for (int i = 0; i < count; i++)
        {
            
            int randomIndex = Random.Range(0, tempList.Count);
            selectedStrings.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
            print("selected: " + selectedStrings[i]);
        }

        return selectedStrings;


    }




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

    public void SubscribeToMessage()
    {
        rosManager.SendRosMessage(RosbridgeUtilities.GetROSJsonSubscribeMsg(Constants.JIBO_ASR_RESULTS_ROSTOPIC, Constants.JIBO_ASR_RESULTS_MSG_TYPE)).Invoke();
    }


}