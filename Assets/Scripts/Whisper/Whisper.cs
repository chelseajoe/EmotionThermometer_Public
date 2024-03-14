using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using System.Collections;

namespace OpenAI
{
    public class Whisper : MonoBehaviour
    {
        [SerializeField] private Button recordButton;
        [SerializeField] private Image progressBar;
        [SerializeField] private Button stopButton;
        [SerializeField] private GameObject text;
        
        private TMP_Text recordText;
        [SerializeField] private Dropdown dropdown;

        private readonly string fileName = "output.wav";
        private readonly int duration = 10;
        
        private AudioClip clip;
        private bool isRecording;
        private float time;
        private string apiKey;
        public string childSpeech;
        public bool finishedRecording;
        public bool currentlyRecording;

        // Debugging
        [SerializeField] private GameObject outputText;
        private TMP_Text output;


        private void Start()
        {

            recordText = text.GetComponent<TMP_Text>();
            finishedRecording = false;
            progressBar.enabled = false;
            stopButton.gameObject.SetActive(false);

            dropdown.enabled = false;
            currentlyRecording = false;

            stopButton.onClick.AddListener(EndRecording);

#if UNITY_WEBGL && !UNITY_EDITOR
            dropdown.options.Add(new Dropdown.OptionData("Microphone not supported on WebGL"));
#else
            foreach (var device in Microphone.devices)
            {
               dropdown.options.Add(new Dropdown.OptionData(device));
            }
            recordButton.onClick.AddListener(StartRecording);
            dropdown.onValueChanged.AddListener(ChangeMicrophone);
            
            var index = PlayerPrefs.GetInt("user-mic-device-index");
            dropdown.SetValueWithoutNotify(index);
            #endif
        }

        private void ChangeMicrophone(int index)
        {
            PlayerPrefs.SetInt("user-mic-device-index", index);
        }
        
        private void StartRecording()
        {
            isRecording = true;
            progressBar.enabled = true;
            stopButton.gameObject.SetActive(true);
            recordButton.gameObject.SetActive(false);
            recordText.text = "Recording...";
            currentlyRecording = true;

            // var index = PlayerPrefs.GetInt("user-mic-device-index");

            #if !UNITY_WEBGL
                        // clip = Microphone.Start(dropdown.options[index].text, false, duration, 44100);
                        clip = Microphone.Start(Microphone.devices[0], false, duration, 44100);
            #endif
        }

        IEnumerator EmptyProgressBar()
        {
            yield return new WaitForSeconds(1f);
            progressBar.fillAmount = 0;
        }

            private async void EndRecording()
        {
            currentlyRecording = false;
            var openai = new OpenAIApi("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");
            recordText.text = "Loading...";

#if !UNITY_WEBGL
            Microphone.End(null);
            #endif
            
            byte[] data = SaveWav.Save(fileName, clip);
            
            var req = new CreateAudioTranscriptionsRequest
            {
                FileData = new FileData() {Data = data, Name = "audio.wav"},
                // File = Application.persistentDataPath + "/" + fileName,
                Model = "whisper-1",
                Language = "en"
            };
            //progressBar.color = Color.blue;
            var res = await openai.CreateAudioTranscription(req);

            StartCoroutine(EmptyProgressBar());
            finishedRecording = true;
            //message.text = res.Text;
            recordButton.enabled = true;
            print("Recorded: " + res.Text);

            // redirect res.Text output to Firebase
            //finishedRecording = true;
            childSpeech = res.Text;
            //print("strategy in whisper" + strategy);

        }

        private void Update()
        {
            if (isRecording)
            {
                if (currentlyRecording) {
                    time += Time.deltaTime;
                    progressBar.fillAmount = time / duration;
                }

                // times up, end recording
                if (time >= duration && currentlyRecording)
                {
                    time = 0;
                    isRecording = false;
                    EndRecording();
                }
            }
        }

    }
}
