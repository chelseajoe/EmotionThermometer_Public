using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// dynamically changing the image, scene text, and scene question of scenario 1

public class showNextScene : MonoBehaviour
{
    public Image[] sceneImages;
    // static scenes
    public string[] sceneTexts = 
    {
        "Jessica is playing on the swings at recess. She is with her best friend. As she is swinging, Jessica slips off and falls to the ground. Her eyes fill with tears, and she starts to cry.",
        "Mark is going to a park with his family. They make plans to meet a friend there. When Mark gets to the park, the sun is shining and it's warm. Mark plays with his friends and smiles a lot.",
        "Ryan and Donnie are on a bike ride together. They ride by a baseball field. Ryan slams on his brakes. He hops off his bike and jumps up and down. Ryan asks Donnie to watch the game with him.",
        "Hanna is working on an art project. She puts her hands on her head and takes a deep breath. She puts her head down and pushes her paper away."
    };
    public string[] sceneQuestions = {"How is Jessica feeling?", "How is Mark feeling?", "How does Ryan feel? How does he show it?", "How does Hanna feel about this art project? How does she show it?"};
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
            scenario2scene.text += words[currentIndex];
            scenario2scene.text += " ";
            currentIndex++;
            yield return new WaitForSeconds(wordDelay);
        }
        // only after text streaming is done, show "jibo" button
        jibo.gameObject.SetActive(true);
    }

    // onclick of the "next" button
    public void nextScene(){
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

    // onclick of the "jibo" button
    public void jiboQuestion(){
        StartCoroutine(questionStreamerFunction(currentScene));
    }
    // question streaming
    private IEnumerator questionStreamerFunction(int currentScene){
        int currentIndex = 0;
        string[] words = sceneQuestions[currentScene].Split(' ');
        while (currentIndex < words.Length){
            scenario2question.text += words[currentIndex];
            scenario2question.text += " ";
            currentIndex++;
            yield return new WaitForSeconds(wordDelay);
        }
        // only after question streaming is done, show "next" button to show the next scene
        next.gameObject.SetActive(true);
    }
}
