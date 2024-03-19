using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    public static string JIBO_ASR_RESULTS_ROSTOPIC = "/jibo_asr_result";
    public static string JIBO_ASR_RESULTS_MSG_TYPE = "jibo_msgs/JiboAsrResult";

    public static string JIBO_ACTION_ROSTOPIC = "/jibo";
    public static string JIBO_ACTION_ROSTOPIC_MSG_TYPE = "jibo_msgs/JiboAction";

    public static string JIBO_STATE_ROSTOPIC = "/jibo_state";
    public static string JIBO_STATE_MSG_TYPE = "jibo_msgs/JiboState";

    public static string JIBO_ASR_COMMAND_ROSTOPIC = "/jibo_asr_command";
    public static string JIBO_ASR_COMMAND_MSG_TYPE = "jibo_msgs/JiboAsrCommand";
}
