using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RosMessages
{
    public abstract class RosMessage
    {
        protected string _topic;
        protected int _command;
        public string GetTopic() { return _topic; }
        public abstract Dictionary<string, object> GetData();
    }

    public class JiboAction : RosMessage
    {
        protected bool _do_motion;
        protected string _motion;

        protected bool _do_lookat;
        protected Dictionary<string, float> _lookat;

        protected bool _do_tts;
        protected string _tts_text;

        protected bool _do_mim;
        protected string _mim_body;
        protected string _mim_rule;

        protected bool _do_sound_playback;
        protected string _audio_filename;

        protected bool _do_led;
        protected Dictionary<string, float> _led_color;

        protected bool _do_volume;
        protected float _volume;

        protected bool _do_anim_transition;
        protected int _anim_transition;

        protected bool _do_attention_mode;
        protected int _attention_mode;
        protected float _tts_duration_stretch;
        protected float _tts_pitch;

        public JiboAction(bool do_motion = false, string motion = "",
                          bool do_lookat = false, Dictionary<string, float> lookat = default,
                          bool do_tts = false, string tts_text = "", float tts_duration_stretch = 0, float tts_pitch = 0,
                          bool do_mim = false, string mim_body = "", string mim_rule = "",
                          bool do_sound_playback = false, string audio_filename = "",
                          bool do_led = false, Dictionary<string, float> led_color = default,
                          bool do_volume = false, float volume = 0,
                          bool do_anim_transition = false, int anim_transition = 0,
                          bool do_attention_mode = false, int attention_mode = 0)
        {
            _topic = Constants.JIBO_ACTION_ROSTOPIC;
            _do_motion = do_motion;
            _motion = motion;
            _do_lookat = do_lookat;
            if (lookat == null || !lookat.ContainsKey("x") || !lookat.ContainsKey("y") || !lookat.ContainsKey("z"))
            {
                _lookat = new Dictionary<string, float>();
                _lookat.Add("x", 0);
                _lookat.Add("y", 0);
                _lookat.Add("z", 0);
            }
            else
            {
                _lookat = lookat;
            }
            _do_tts = do_tts;
            _tts_text = tts_text;
            _do_mim = do_mim;
            _mim_body = mim_body;
            _mim_rule = mim_rule;
            _do_sound_playback = do_sound_playback;
            _audio_filename = audio_filename;
            _do_led = do_led;
            if (led_color == null || !led_color.ContainsKey("x") || !led_color.ContainsKey("y") || !led_color.ContainsKey("z"))
            {
                _led_color = new Dictionary<string, float>();
                _led_color.Add("x", 0);
                _led_color.Add("y", 0);
                _led_color.Add("z", 0);
            }
            else
            {
                _led_color = led_color;
            }
            _do_volume = do_volume;
            _volume = volume;
            _do_anim_transition = do_anim_transition;
            _anim_transition = anim_transition;
            _do_attention_mode = do_attention_mode;
            _attention_mode = attention_mode;
            _tts_duration_stretch = tts_duration_stretch;
            _tts_pitch = tts_pitch;
            Debug.Log("Actually calling parent constructor: do volume = " + do_volume.ToString());
        }

        public override Dictionary<string, object> GetData()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("header", RosbridgeUtilities.GetROSHeader());
            data.Add("do_motion", _do_motion);
            data.Add("motion", _motion);
            data.Add("do_lookat", _do_lookat);
            data.Add("lookat", _lookat);
            data.Add("do_tts", _do_tts);
            data.Add("tts_text", _tts_text);
            data.Add("do_mim", _do_mim);
            data.Add("mim_body", _mim_body);
            data.Add("mim_rule", _mim_rule);
            data.Add("do_sound_playback", _do_sound_playback);
            data.Add("audio_filename", _audio_filename);
            data.Add("do_led", _do_led);
            data.Add("led_color", _led_color);
            data.Add("do_volume", _do_volume);
            data.Add("volume", _volume);
            data.Add("do_anim_transition", _do_anim_transition);
            data.Add("anim_transition", _anim_transition);
            data.Add("do_attention_mode", _do_attention_mode);
            data.Add("attention_mode", _attention_mode);
            data.Add("tts_duration_stretch", _tts_duration_stretch);
            data.Add("tts_pitch", _tts_pitch);

            return data;
        }
    }

    public class JiboVolumeAction : JiboAction
    {
        public JiboVolumeAction(float volume) : base(do_volume: true, volume: volume) { }
    }

    public class JiboSpeechAction : JiboAction
    {
        public JiboSpeechAction(string tts_text, float tts_duration_stretch = 0, float tts_pitch = 0) :
            base(do_tts: true, tts_text: tts_text, tts_duration_stretch: tts_duration_stretch, tts_pitch: tts_pitch)
        { }
    }

    public class JiboRingLightAction : JiboAction
    {
        public JiboRingLightAction(Vector3 color) : base(do_led: true) {
            _led_color = new Dictionary<string, float>();
            _led_color.Add("x", color.x);
            _led_color.Add("y", color.y);
            _led_color.Add("z", color.z);
        }
    }

    public class JiboLookAtAction : JiboAction
    {
        public JiboLookAtAction(Vector3 pos) : base(do_lookat: true)
        {
            _lookat = new Dictionary<string, float>();
            _lookat.Add("x", pos.x);
            _lookat.Add("y", pos.y);
            _lookat.Add("z", pos.z);
        }
    }


    // Needed to properly encode the Vec3 type over ROS

    public struct JiboVec3
    {
        float x;
        float y;
        float z;
    }

    public class JiboASRCommand : RosMessage
    {

        bool _heyjibo;
        bool _continuous;
        bool _incremental;
        bool _alternatives;
        bool _detectend;
        string _rule;
        sbyte _start_or_stop;

        public JiboASRCommand(bool heyjibo = true, bool continuous = false, bool incremental = false,
                              bool alternatives = false, bool detectend = false, string rule = "",
                              sbyte command = 0)
        {
            _heyjibo = heyjibo;
            _continuous = continuous;
            _incremental = incremental;
            _alternatives = alternatives;
            _detectend = detectend;
            _rule = rule;
            _start_or_stop = command;
            _topic = Constants.JIBO_ASR_COMMAND_ROSTOPIC;
        }

        public override Dictionary<string, object> GetData()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("header", RosbridgeUtilities.GetROSHeader());
            data.Add("heyjibo", _heyjibo);
            data.Add("continuous", _continuous);
            data.Add("incremental", _incremental);
            data.Add("alternatives", _alternatives);
            data.Add("detectend", _detectend);
            data.Add("rule", _rule);
            data.Add("command", _start_or_stop);

            return data;
        }
    }
}
