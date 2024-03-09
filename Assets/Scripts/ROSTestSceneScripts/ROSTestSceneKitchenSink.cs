using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ROSTestSceneButtons : MonoBehaviour
{

    public AutoRosConnector autoRos;

    [SerializeField]
    Slider slider;

    private bool light_on = false;

    private RosManager rosManager;

    private void Start()
    {
        rosManager = autoRos.GetRosManager();
        if(rosManager == null)
        {
            Debug.LogError("ROS manager does not exist.");
        }
        SubscribeToMessage();
    }

    public void OnSpeakButtonPressed()
    {
        Debug.Log("Speak, Jibo, Speak!!");

        rosManager.SendRosMessage(new RosMessages.JiboSpeechAction("<es name='Body_Lean_Forward_01'/> Press this button when you don’t know what to do and would like me to give you some ideas. <break size='1.0'> <es name='Body_Lean_Back_01'/> If you don’t like my ideas, just press it again, and I will give you some more.")).Invoke();
    }

    public void OnVolumeSliderValueChanged(float newValue)
    {
        Debug.Log("Changing Jibo's volume");

        rosManager.SendRosMessage(new RosMessages.JiboVolumeAction((slider.value + 1) / 10.0f)).Invoke();
    }


    public void OnToggleLEDLightPressed()
    {
        if(light_on)
        {
            Debug.Log("Turning ring light off");
            rosManager.SendRosMessage(new RosMessages.JiboRingLightAction(new Vector3(0, 0, 0))).Invoke();
        }
        else
        {
            Debug.Log("Turning ring light on");
            rosManager.SendRosMessage(new RosMessages.JiboRingLightAction(new Vector3(0, 0, 255))).Invoke();
        }
        light_on = !light_on;
    }

    public void OnLookAtPressed()
    {
        rosManager.SendRosMessage(new RosMessages.JiboLookAtAction(new Vector3(0, 0, 0))).Invoke();
    }

    public void Listen()
    {
        rosManager.SendRosMessage(new RosMessages.JiboASRCommand(false, false, false, false, false, "", 1)).Invoke();
    }

    public void SubscribeToMessage()
    {
        rosManager.SendRosMessage(RosbridgeUtilities.GetROSJsonSubscribeMsg(Constants.JIBO_ASR_RESULTS_ROSTOPIC, Constants.JIBO_ASR_RESULTS_MSG_TYPE)).Invoke();
    }
}
