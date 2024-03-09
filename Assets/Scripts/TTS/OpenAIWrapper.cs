using System.Text;
using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

[System.Serializable]
public class TTSPayload
{
    public string model;
    public string input;
    public string voice;
    public string response_format;
    public float speed;
    
}

public class OpenAIWrapper : MonoBehaviour
{
    private string openAIKey;
    private TTSModel model = TTSModel.TTS_1;
    private TTSVoice voice = TTSVoice.Nova;
    private float speed = 1f;
    private readonly string outputFormat = "mp3";
    public bool unSuccessful;


    private void Start()
    {
        unSuccessful = false;
        openAIKey = "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";
    }
    public async Task<byte[]> RequestTextToSpeech(string text)
    {
        Debug.Log("Sending new request to OpenAI TTS.");
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openAIKey);

        TTSPayload payload = new TTSPayload
        {
            model = this.model.EnumToString(),
            input = text,
            voice = this.voice.ToString().ToLower(),
            response_format = this.outputFormat,
            speed = this.speed
        };

        string jsonPayload = JsonUtility.ToJson(payload);

        var httpResponse = await httpClient.PostAsync(
            "https://api.openai.com/v1/audio/speech", 
            new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

        byte[] response = await httpResponse.Content.ReadAsByteArrayAsync();

        if (httpResponse.IsSuccessStatusCode)
        {
            //unSuccessful = false;
            return response;
        }
        //unSuccessful = true;
        Debug.Log("Try again");

        // recursive call since firsts run doesn't always work for openai request
        return await RequestTextToSpeech(text);
        
        Debug.Log("Error: " + httpResponse.StatusCode);
        return null;
    }
    
    public async Task<byte[]> RequestTextToSpeech(string text, TTSModel model, TTSVoice voice, float speed)
    {
        this.model = model;
        this.voice = voice;
        this.speed = speed;
        return await RequestTextToSpeech(text);
    }
}
