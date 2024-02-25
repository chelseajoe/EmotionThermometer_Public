using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class Scenario3SceneController : MonoBehaviour
{
   public Image[] sceneImages;
    // static scenes
    public string[] sceneTexts = 
    {
        "Rosy and Brad both want to play with the basketball at recess. Brad got to the court first, but Rosy got the ball first. How should they solve this problem? ",
        "scenario3 scene2 text",
        "scenario3 scene3 text",
        "scenario3 scene4 text"
    };
    public string[] sceneQuestions = {"Draw a picture to show what they should do.", "scenario3 scene2 question", "scenario3 scene3 question", "scenario3 scene4 question"};
    public float wordDelay = 0.2f;

    private int currentScene = 0;

    [SerializeField]  
    private TMP_Text scenario3scene;

    [SerializeField]  
    private TMP_Text scenario3question;

    public Button jibo;
    // public Button next;
    public Button drawBtn;

    // Adding TTS
    [SerializeField] private TTSManager tTSManager;
    public float delayTime = 2.0f;
    public bool jiboButtonIsClicked = false;


    void Start() {
        StartCoroutine(textStreamerFunction(currentScene));
    }
    void Update(){}
    
    // text streaming
    private IEnumerator textStreamerFunction(int currentScene){

        // read scene TTS
        tTSManager.SynthesizeAndPlay(sceneTexts[currentScene]);

        // wait for model to synthesize text
        yield return new WaitForSeconds(delayTime);

        int currentIndex = 0;
        string[] words = sceneTexts[currentScene].Split(' '); // split the scene text into words
        // show the words with delay
        while (currentIndex < words.Length){
            scenario3scene.text += words[currentIndex];
            scenario3scene.text += " ";
            currentIndex++;
            yield return new WaitForSeconds(wordDelay);
        }
        // only after text streaming is done, show "jibo" button
        jibo.gameObject.SetActive(true);
    }

    // onclick of the "next" button
    public void draw(){
        // nextScene();
        Debug.Log("Going to drawing screen");
        SceneManager.LoadScene("Drawing");
    }
    // public void nextScene(){
    //     next.gameObject.SetActive(false); // hide "next" button
    //     jibo.gameObject.SetActive(false); // hide "jibo" button
    //     // hide the prev image and show the next
    //     sceneImages[currentScene].gameObject.SetActive(false);
    //     currentScene = (currentScene + 1) % sceneImages.Length; // update the current scene
    //     sceneImages[currentScene].gameObject.SetActive(true);
    //     // hide the prev text and set it to an empty string
    //     scenario3scene.gameObject.SetActive(false);
    //     scenario3scene.text = "";
    //     scenario3scene.gameObject.SetActive(true);
    //     // hide the prev question and set it to an empty string
    //     scenario3question.gameObject.SetActive(false);
    //     scenario3question.text = "";
    //     scenario3question.gameObject.SetActive(true);
    //     // start streaming the next text
    //     StartCoroutine(textStreamerFunction(currentScene));
    // }

    // onclick of the "jibo" button
    public void jiboQuestion(){
        if (jiboButtonIsClicked)
            return;

        jiboButtonIsClicked = true;

        StartCoroutine(questionStreamerFunction(currentScene));
    }
    // question streaming
    private IEnumerator questionStreamerFunction(int currentScene){
        int currentIndex = 0;
        string[] words = sceneQuestions[currentScene].Split(' ');
        while (currentIndex < words.Length){
            scenario3question.text += words[currentIndex];
            scenario3question.text += " ";
            currentIndex++;
            yield return new WaitForSeconds(wordDelay);
        }
        // only after question streaming is done, show "draw" button to show the drawing scene
        drawBtn.gameObject.SetActive(true);
    }
}
