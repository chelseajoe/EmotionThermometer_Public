using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using OpenAI;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System;


public class ScenarioController : MonoBehaviour
{
    // Scene Variables - public variables to change in unity
    public Image[] sceneImages;
    public string sceneText = "Jessica is playing on the swings at recess. She is with her best friend. As she is swinging, Jessica slips off and falls to the ground. Her eyes fill with tears, and she starts to cry.";
    public string sceneQuestion = "How is Jessica feeling?";

    [SerializeField] float wordDelay = 0.1f;
    private int currentScene = 0;
    [SerializeField] private TMP_Text scenarioscene;
    [SerializeField] private TMP_Text scenarioquestion;

    public Button jibo;
    public Button next;
    public Button drawBtn;

    public bool isDrawingScene = false;
    public string[] drawingScenes = { "Scenario3" };

    // Adding TTS
    [SerializeField] private TTSManager tTSManager;
    [SerializeField] float delayTime = 0.1f;
    public bool jiboButtonIsClicked = false;

    // Adding ASR (recording)
    public Canvas ASRCanvas;
    public OpenAI.Whisper whisper;
    bool sentMessage = false;
    public bool canvasOn = false;
    public bool recordButtonIsClicked = false;

    //Adding GPT analysis
    public string correctResponse;
    public string childResponse;
    public string characterName;
    public string feelingType;
    public string jiboCorrectResponse;
    public string jiboIncorrectResponse;
    public string jiboNRResponse;

    // TTS with timestamps
    public AudioClip audioClip;
    public TextAsset jsonFile;
    public AudioSource audioSource;

    // Classes to deserialize JSON data
    [System.Serializable]
    public class WordData
    {
        public string word;
        public float start;
        public float end;
    }

    [System.Serializable]
    public class WordDataList
    {
        public List<WordData> wordDataList;
    }


    void Start()
    {
        // if this is a drawing type of scene - check button -> draw button
        if (Array.Exists(drawingScenes, scene => scene == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name))
        {
            isDrawingScene = true;
        }

        if (!isDrawingScene)
        {
            ASRCanvas.gameObject.SetActive(false);
        }
        
        TextStreamAndAudio();

        
    }

    // Function to stream text and audio based on JSON data
    public void TextStreamAndAudio()
    {
        // Check if both audioClip and jsonFile are assigned
        if (audioClip == null || jsonFile == null)
        {
            Debug.LogError("Invalid audio clip or JSON file.");
            return;
        }

        // Load the JSON data directly into a list of WordData
        List<WordData> wordDataList = ParseJsonText(jsonFile.text);
        //print(jsonFile.text);

        // Check if word data is valid
        if (wordDataList == null)
        {
            Debug.LogError("Invalid word data.");
            return;
        }

        // Clear existing text
        scenarioscene.text = "";

        //print("near end of TextStream and Audio");

        // Play the audio clip
        StartCoroutine(PlayAudioWithText(audioClip, wordDataList));
    }

    // Audio timestamp matching function
    private IEnumerator PlayAudioWithText(AudioClip audioClip, List<WordData> wordDataList)
    {
        audioSource.clip = audioClip;
        audioSource.Play();

        foreach (WordData wordData in wordDataList)
        {
            scenarioscene.text += wordData.word + " ";

            // wait for the entire duration of the current word's audio -- debugging
            //print("END: " + wordData.end);
            //print("START: " + wordData.start);
            //print("Difference: " + (wordData.end - wordData.start));

            // needs buffer time - 0.5f, unity might lag
            yield return new WaitForSeconds(wordData.end - wordData.start + delayTime);
        }

        // only after text streaming is done, show "jibo" button
        jibo.gameObject.SetActive(true);

    }

    // Parse JSON data for timestamps
    private List<WordData> ParseJsonText(string jsonText)
    {
        // Parse JSON data into a list of WordData objects
        WordDataList wordDataListContainer = JsonUtility.FromJson<WordDataList>(jsonText);

        if (wordDataListContainer != null && wordDataListContainer.wordDataList != null)
        {
            return wordDataListContainer.wordDataList;
        }
        else
        {
            Debug.LogError("Failed to parse JSON data.");
            return null;
        }
    }



    void Update()
    {

        if (!sentMessage && !isDrawingScene && whisper.finishedRecording)
        {
            //Show ASR Canvas with slight delay
            StartCoroutine(ShowASRCanvas());

            print("SENDING SPEECH");

            // NOTE: send firebase message here
            SendSpeechFireBase();

            //Hide ASR Canvas with slight delay
            StartCoroutine(HideASRCanvas());

        }

    }


    public void record()
    {
        if (recordButtonIsClicked)
            return;

        recordButtonIsClicked = true;

        StartCoroutine(ShowASRCanvas());

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
    }


    int GetSceneNumber(string sceneName)
    {
        // Use regular expression to extract the numeric part from the scene name
        Match match = Regex.Match(sceneName, @"\d+$");

        if (match.Success)
        {
            // Extracted numeric part
            string numericPart = match.Value;

            // Convert the numeric part to an integer
            if (int.TryParse(numericPart, out int scenarioNumber))
            {
                print("SCENARIO NUMBER: " + scenarioNumber);
                return scenarioNumber;
            }
            else
            {
                Debug.LogError("Failed to parse the numeric part to an integer.");
                return -1; // Indicates failure
            }
        }
        else
        {
            Debug.LogError("Scene name doesn't contain a numeric part.");
            return -1; // Indicates failure
        }
    }


    private void SendSpeechFireBase()
    {
        // convert scenario number as key
        //int scenarioNumber = currentScene + 1; // increment by 1 since index #

        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        int scenarioNumber = GetSceneNumber(currentSceneName);

        if (scenarioNumber != -1)
        {
            // Now, scenarioNumber holds the last number in your scene name
            Debug.Log("Scenario Number: " + scenarioNumber);
        }
        else
        {
            Debug.LogError("Failed to get the scenario number from the scene name.");
        }


        string scenarioName = "Scenario " + scenarioNumber.ToString();

        print(scenarioName);

        // Send strategy message to firebase
        string response = whisper.childSpeech;
        print(response);

        string prompt = "Given the scenario:" + "'" + sceneText + "'" + "Please analyze the student's response stored in the following string:" + "[" + response + "]. Evaluate whether the response is correct, incorrect, or a non-response. If correct, confirm the emotional state expressed, considering the correct response: [" + correctResponse + "] Responses similar to [" + correctResponse + "] or using synonyms for [" + feelingType + "] are also considered correct, as long as the response mentions something similar to [" + feelingType + "], it is also considered correct. If the response is a non-response (e.g., 'I don't know,' 'I don't care,' etc.), classify it as 'non-response.' If incorrect, simply output 'incorrect.' Your only output text should consist of one word, and should be one of three choices ONLY: 'correct', 'incorrect', or 'non-response'";
        print("prompt: " + prompt);

        DatabaseManager.Instance.AddScenarioSpeech(scenarioName, response);


        sentMessage = true;

        // Send child message to GPT
        SendRequest(prompt);


    }

    // GPT REQUEST function
    private async void SendRequest(string prompt)
    {
        var openai = new OpenAIApi("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");
        var req = new CreateChatCompletionRequest
        {
            Model = "gpt-4",
            Messages = new List<ChatMessage>()
        {
            new ChatMessage()
            {
                Role = "user",
                Content = prompt
            }
        }
        };

        var res = await openai.CreateChatCompletion(req);

        if (res.Choices != null && res.Choices.Count > 0)
        {
            var message = res.Choices[0].Message;
            message.Content = message.Content.Trim();

            print("gpt output message:" + message.Content);

            childResponse = message.Content;
        }
        else
        {
            Debug.LogWarning("No text was generated from this prompt.");
            childResponse = "ERROR";
        }


        // Analyze the response
        StartCoroutine(AnalyzeResponse());
    }

    IEnumerator AnalyzeResponse()
    {
        // add delay time just in case
        yield return new WaitForSeconds(0.5f);

        if (childResponse == "correct")
        {
            // Handle correct response
            print("Correct response");
            print("JIBO: " + jiboCorrectResponse);
            //print("JIBO: You are right! I have also thought that the " + characterName + " is feeling " + feelingType + ". Why do you think the character is feeling this emotion? What might they be thinking?");
        }
        else if (childResponse == "incorrect")
        {
            // Handle incorrect response
            print("Incorrect response");
            print("JIBO: " + jiboIncorrectResponse);
            //print("JIBO: That is an interesting way to look at it! I can see why you might think that. Emotions can be a bit tricky sometimes, but we're here to figure them out together! Let me share what I noticed about the character's feelings in this story. They seem to be feeling " + feelingType + ". Why do you think the character is feeling this emotion? What might they be thinking? \n");
        }
        else if (childResponse == "non-response")
        {
            // Handle non-response
            print("Non-response");
            print("JIBO: " + jiboNRResponse);
            //print("JIBO: Oh, I thought that the " + characterName + " is feeling " + feelingType + ". Why do you think the character is feeling this emotion? What might they be thinking?");
        }
        else
        {
            // Handle other cases or unexpected responses
            print("Unexpected response: " + childResponse);
        }
    }


    // onclick of the "jibo" button
    public void jiboQuestion()
    {
        if (jiboButtonIsClicked)
            return;

        jiboButtonIsClicked = true;
        StartCoroutine(questionStreamerFunction(currentScene));
    }
    // question streaming
    private IEnumerator questionStreamerFunction(int currentScene)
    {
        int currentIndex = 0;
        string[] words = sceneQuestion.Split(' ');
        while (currentIndex < words.Length)
        {
            scenarioquestion.text += words[currentIndex];
            scenarioquestion.text += " ";
            currentIndex++;
            yield return new WaitForSeconds(wordDelay);
        }
        // only after question streaming is done, show "next" button to show the next scene
        if (isDrawingScene)
        {
            drawBtn.gameObject.SetActive(true);
        } else
        {
            next.gameObject.SetActive(true);
        }
        
    }

    public void draw()
    {
        // nextScene();
        Debug.Log("Going to drawing screen");
        SceneManager.LoadScene("Drawing");
    }
}
