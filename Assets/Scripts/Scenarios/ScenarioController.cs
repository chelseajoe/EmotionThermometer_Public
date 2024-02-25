using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using OpenAI;
//using UnityEditor.VersionControl;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System;

// dynamically changing the image, scene text, and scene question of scenario 1

public class ScenarioController : MonoBehaviour
{
    public Image[] sceneImages;
    // static scenes
    public string[] sceneTexts =
    {
        "Jessica is playing on the swings at recess. She is with her best friend. As she is swinging, Jessica slips off and falls to the ground. Her eyes fill with tears, and she starts to cry.",
        //"Mark is going to a park with his family. They make plans to meet a friend there. When Mark gets to the park, the sun is shining and it's warm. Mark plays with his friends and smiles a lot.",
        //"Ryan and Donnie are on a bike ride together. They ride by a baseball field. Ryan slams on his brakes. He hops off his bike and jumps up and down. Ryan asks Donnie to watch the game with him.",
        //"Hanna is working on an art project. She puts her hands on her head and takes a deep breath. She puts her head down and pushes her paper away."
    };
    public string[] sceneQuestions = { "How is Jessica feeling?"
        //,"How is Mark feeling?", "How does Ryan feel? How does he show it?", "How does Hanna feel about this art project? How does she show it?"
    };

    public float wordDelay = 0.1f;
    private int currentScene = 0;

    [SerializeField]
    private TMP_Text scenario2scene;

    [SerializeField]
    private TMP_Text scenario2question;

    public Button jibo;
    public Button next;




    // Adding TTS
    [SerializeField] private TTSManager tTSManager;
    public float delayTime = 2.0f;
    public bool jiboButtonIsClicked = false;

    // Adding ASR (recording)
    public Canvas ASRCanvas;
    public OpenAI.Whisper whisper;
    public DatabaseManager databaseManager;
    private GameObject dbManager;
    bool sentMessage = false;
    public bool canvasOn = false;
    public bool recordButtonIsClicked = false;

    // Random PID Generation
    private static System.Random random = new System.Random();
    //public GameObject dbManagerSpare;


    //Adding GPT analysis
    public string correctResponse;
    //public string keyWord;
    public string childResponse;
    public string characterName;
    public string feelingType;
    public string jiboCorrectResponse;
    public string jiboIncorrectResponse;
    public string jiboNRResponse;


    // tts with timestamps
    public AudioClip audioClip;
    public TextAsset jsonFile;
    public AudioSource audioSource;


    void Start()
    {
        //correctResponse = "Jessica is sad";

        ASRCanvas.gameObject.SetActive(false);

        // NOTE: Locating DatabaseManager which should hopefully exist

        //StartCoroutine(textStreamerFunction(currentScene));
        TextStreamAndAudio();


    }

    // Function to stream text and audio based on JSON data
    //public void TextStreamAndAudio(string audioFilePath, string jsonData)
    public void TextStreamAndAudio()
    {
        // Check if both audioClip and jsonFile are assigned
        if (audioClip == null || jsonFile == null)
        {
            Debug.LogError("Invalid audio clip or JSON file.");
            return;
        }

        // Load the JSON data directly into a list of WordData
        //List<WordData> wordDataList = JsonUtility.FromJson<List<WordData>>(jsonFile.text);
        List<WordData> wordDataList = ParseJsonText(jsonFile.text);
        print(jsonFile.text);

        // Check if word data is valid
        if (wordDataList == null)
        {
            Debug.LogError("Invalid word data.");
            return;
        }

        // Clear existing text
        scenario2scene.text = "";

        print("near end of  TextStream and Audio");

        // Play the audio clip
        StartCoroutine(PlayAudioWithText(audioClip, wordDataList));
    }


    private IEnumerator PlayAudioWithText(AudioClip audioClip, List<WordData> wordDataList)
    {
        audioSource.clip = audioClip;
        audioSource.Play();

        foreach (WordData wordData in wordDataList)
        {
            scenario2scene.text += wordData.word + " ";

            // wait for the entire duration of the current word's audio
            print("END: " + wordData.end);
            print("START: " + wordData.start);
            print("Difference: " + (wordData.end - wordData.start));

            // needs buffer , unity might lag
            yield return new WaitForSeconds(wordData.end - wordData.start + 0.05f);
        }

    }

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

    void Update()
    {
        //if (!canvasOn)
        //{
        //    //Show ASR Canvas with slight delay
        //    StartCoroutine(ShowASRCanvas());
        //    canvasOn = true;
        //}


        //if (!sentMessage && whisper.finishedRecording)
        //{
        //    //Show ASR Canvas with slight delay
        //    StartCoroutine(ShowASRCanvas());

        //    print("SENDING SPEECH");

        //    // NOTE: send firebase message here
        //    SendSpeechFireBase();

        //    //Hide ASR Canvas with slight delay
        //    StartCoroutine(HideASRCanvas());

        //}

    }

    // text streaming
    private IEnumerator textStreamerFunction(int currentScene)
    {

        // read scene TTS
        tTSManager.SynthesizeAndPlay(sceneTexts[currentScene]);

        // wait for model to synthesize text
        yield return new WaitForSeconds(delayTime);

        int currentIndex = 0;
        string[] words = sceneTexts[currentScene].Replace("\\n", "\n").Split(' '); // split the scene text into words
        // show the words with delay
        while (currentIndex < words.Length)
        {
            scenario2scene.text += words[currentIndex];
            scenario2scene.text += " ";
            currentIndex++;
            yield return new WaitForSeconds(wordDelay);
        }
        // only after text streaming is done, show "jibo" button
        jibo.gameObject.SetActive(true);
    }

    // onclick of the "next" button
    public void nextScene()
    {
        // NOTE: TEMPORARY Until Scenes are finalized
        //SceneManager.LoadScene("Scenario3");

        // ORIGINAL
        next.gameObject.SetActive(false); // hide "next" button
        jibo.gameObject.SetActive(false); // hide "jibo" button
        // hide the prev image and show the next
        sceneImages[currentScene].gameObject.SetActive(false);
        currentScene = (currentScene + 1) % sceneImages.Length; // update the current scene
        sceneImages[currentScene].gameObject.SetActive(true);
        // hide the prev text and set it to an empty string
        scenario2scene.gameObject.SetActive(false);
        scenario2scene.text = "";
        scenario2scene.gameObject.SetActive(true);
        // hide the prev question and set it to an empty string
        scenario2question.gameObject.SetActive(false);
        scenario2question.text = "";
        scenario2question.gameObject.SetActive(true);
        // start streaming the next text
        StartCoroutine(textStreamerFunction(currentScene));
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

        // Send strategy message
        string response = whisper.childSpeech;
        print(response);

        string prompt = "Given the scenario:" + "'" + sceneTexts[0] + "'" + "Please analyze the student's response stored in the following string:" + "[" + response + "]. Evaluate whether the response is correct, incorrect, or a non-response. If correct, confirm the emotional state expressed, considering the correct response: [" + correctResponse + "] Responses similar to [" + correctResponse + "] or using synonyms for [" + feelingType + "] are also considered correct, as long as the response mentions something similar to [" + feelingType + "], it is also considered correct. If the response is a non-response (e.g., 'I don't know,' 'I don't care,' etc.), classify it as 'non-response.' If incorrect, simply output 'incorrect.' Your only output text should consist of one word, and should be one of three choices ONLY: 'correct', 'incorrect', or 'non-response'";
        print("prompt: " + prompt);
        //databaseManager.EmotionIdentified(emotion, "this is a placeholder");
        //databaseManager.AddScenarioSpeech(scenarioName, response);
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

        //print("hell0");
        var res = await openai.CreateChatCompletion(req);

        //print("bye");
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
        string[] words = sceneQuestions[currentScene].Split(' ');
        while (currentIndex < words.Length)
        {
            scenario2question.text += words[currentIndex];
            scenario2question.text += " ";
            currentIndex++;
            yield return new WaitForSeconds(wordDelay);
        }
        // only after question streaming is done, show "next" button to show the next scene
        next.gameObject.SetActive(true);
    }
}
