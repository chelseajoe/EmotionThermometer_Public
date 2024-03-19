using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class EmotionThermometerController : MonoBehaviour
{
    // Thermometer UI variables
    [SerializeField] GameObject TopBoundary;
    [SerializeField] GameObject BottomBoundary;
    public string ChosenEmoji;
    public string CurrentEmoji;
    public float dragSpeed = 1f;

    // Databse reference
    public DatabaseManager databaseManager;
    
    // Audio 
    public OpenAI.Whisper whisper;

    // Strategies text
    public string emotion;
    public List<string> displayedStrategies;
    public GameObject strategiesDisplayed;

    // Canvases
    public GameObject strategiesCanvas;
    public Canvas ASRCanvas;
    public GameObject noPIDCanvas;

    // ROS
    public AutoRosConnector autoRos;

    [SerializeField]
    Slider slider;

    private bool light_on = false;

    public RosManager rosManager;

    public bool isConfirmed = false;
    public bool canvasOn = false;
    bool sentMessage = false;


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
        ASRCanvas.gameObject.SetActive(false);

        ChosenEmoji = "";
        CurrentEmoji = "";
    }
    void Update()
    {
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

        // if an emotion has already been selected on thermometer
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

            // send message once and wait for whisper to finish
            if (!sentMessage && whisper.finishedRecording)
            {
                //Show ASR Canvas with slight delay
                StartCoroutine(ShowASRCanvas());

                // NOTE: send firebase message here
                SendEmotionFireBase();

                // Get All Emotion Strategies
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

        // Send strategy message
        string strategy = whisper.childSpeech;
        print(strategy);
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

        // display strategies
        List<string> ListOfStrategies = DatabaseManager.Instance.emotionStrategies;
        print("length: " + ListOfStrategies.Count);
        displayedStrategies = ChooseRandomStrings(ListOfStrategies, 4);

        foreach (string str in displayedStrategies)
        {
            print("strats: " + str);
            strategiesDisplayed.GetComponent<TMP_Text>().text += "-  " + str + "\n\n";
        }
    }

    // Choose 4 random strategies in the database
    List<string> ChooseRandomStrings(List<string> originalList, int count)
    {
        List<string> selectedStrings = new List<string>();

        if (originalList.Count <= count)
        {
            // If the original list is smaller than or equal to the requested count, return the entire original list.
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

    // Dragging on Thermometer + Boundaries
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

        // within boundaries of top and bottom of thermometer
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