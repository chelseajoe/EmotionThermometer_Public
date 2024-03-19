using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EmojiAppear : MonoBehaviour
{
    [SerializeField] GameObject EmojiText;
    public GameObject MaskHandle;
    [SerializeField] GameObject EmojiTrigger;
    Color myColor;
    float fontSize;

    [SerializeField] GameObject NextEmojiTrigger;
    [SerializeField] GameObject BottomTrigger;
    public EmotionThermometerController emotionThermometerController;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        EmojiText.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;

        myColor = EmojiText.transform.GetChild(0).GetComponent<TextMeshPro>().color;
        EmojiText.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(myColor.r, myColor.g, myColor.b, 0.2f);
        fontSize = EmojiText.transform.GetChild(0).GetComponent<TextMeshPro>().fontSize;
    }

    void CheckConfirmation()
    {
        if (emotionThermometerController.isConfirmed)
        {
            emotionThermometerController.ChosenEmoji = emotionThermometerController.CurrentEmoji;
        }
    }


    // Update is called once per frame
    void Update()
    {
        CheckConfirmation();


        if (MaskHandle.transform.position.y > NextEmojiTrigger.transform.position.y)
        {

            EmojiText.transform.GetChild(0).GetComponent<TextMeshPro>().fontSize = fontSize;
            EmojiText.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(myColor.r, myColor.g, myColor.b, 0.3f);
        }

        else if (MaskHandle.transform.position.y > EmojiTrigger.transform.position.y)
        {
            emotionThermometerController.CurrentEmoji = EmojiText.transform.GetChild(0).GetComponent<TextMeshPro>().text;
            gameObject.GetComponent<SpriteRenderer>().enabled = true;

            EmojiText.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(myColor.r, myColor.g, myColor.b, 1f);
            EmojiText.transform.GetChild(0).GetComponent<TextMeshPro>().fontSize = fontSize + 1.5f;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            EmojiText.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(myColor.r, myColor.g, myColor.b, 0.3f);
            EmojiText.transform.GetChild(0).GetComponent<TextMeshPro>().fontSize = fontSize;

        }

        if (MaskHandle.transform.position.y < BottomTrigger.transform.position.y)
        {
            emotionThermometerController.CurrentEmoji = "";
        }

    }
}
