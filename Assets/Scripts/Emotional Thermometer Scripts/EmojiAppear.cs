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
    //[SerializeField] List<GameObject> OtherEmojiTexts;
    Color myColor;
    float fontSize;

    [SerializeField] GameObject NextEmojiTrigger;
    [SerializeField] GameObject BottomTrigger;
    public DragHandle dragHandle;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        //EmojiText.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
        EmojiText.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;

        myColor = EmojiText.transform.GetChild(0).GetComponent<TextMeshPro>().color;
        EmojiText.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(myColor.r, myColor.g, myColor.b, 0.2f);
        fontSize = EmojiText.transform.GetChild(0).GetComponent<TextMeshPro>().fontSize;
    }

    void CheckConfirmation()
    {
        if (dragHandle.isConfirmed)
        {
            //dragHandle.ChosenEmoji = EmojiText.transform.GetChild(0).GetComponent<TextMeshPro>().text;
            dragHandle.ChosenEmoji = dragHandle.CurrentEmoji;
            //print(dragHandle.ChosenEmoji);
            //dragHandle.ChosenEmoji = EmojiText.ToString();
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
            //dragHandle.CurrentEmoji = "hello";
            dragHandle.CurrentEmoji = EmojiText.transform.GetChild(0).GetComponent<TextMeshPro>().text;
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
            //EmojiText.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;

            EmojiText.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(myColor.r, myColor.g, myColor.b, 1f);
            EmojiText.transform.GetChild(0).GetComponent<TextMeshPro>().fontSize = fontSize + 1.5f;
            //print(EmojiText.transform.GetChild(0).GetComponent<TextMeshPro>().color);
            //for (int i = 0; i < OtherEmojiTexts.Count; i++)
            //{
            //    print("hello");
            //    Color currColor = OtherEmojiTexts[i].transform.GetChild(0).GetComponent<TextMeshPro>().color;
            //    OtherEmojiTexts[i].transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(currColor.r, currColor.g, currColor.b, 0.2f);
            //}
        }
        else
        {
            //dragHandle.CurrentEmoji = "";
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            //EmojiText.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
            EmojiText.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(myColor.r, myColor.g, myColor.b, 0.3f);
            EmojiText.transform.GetChild(0).GetComponent<TextMeshPro>().fontSize = fontSize;

        }

        if (MaskHandle.transform.position.y < BottomTrigger.transform.position.y)
        {
            dragHandle.CurrentEmoji = "";
        }

    }
}
