// This class is a high level abstraction of dealing with Ros, to separate ROS
// logic from GameController.
//
// RosManager heavily relies on RosbridgeUtilities and RosbridgeWebSocketClient.

using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using MiniJSON;
using Newtonsoft.Json;

public class RosManager
{

    public GameObject testObject; // For UI feedback when things happen.

    private RosbridgeWebSocketClient rosClient;
    // TODO: note that for now only one handler can be registered per command.
    //private Dictionary<StorybookCommand, Action<Dictionary<string, object>>> commandHandlers;
    private bool connected;

    // Constructor.
    public RosManager(string rosIP, string portNum)
    {
        Debug.Log("RosManager constructor");

        this.rosClient = new RosbridgeWebSocketClient(rosIP, portNum);


        //this.rosClient.SendMessage(subMessage)
        this.rosClient.receivedMsgEvent += this.onMessageReceived;
        //this.commandHandlers = new Dictionary<StorybookCommand, Action<Dictionary<string, object>>>();
    }

    public void onRosConnect()
    {
        Debug.Log("Successfully connected to ROS");
        this.connected = true;
    }

    public bool Connect()
    {
        // If the client disconnects then reconnects, make sure to readvertise our topics to make
        // sure we're not being ignored.
        this.rosClient.OnReconnectSuccess(this.onRosConnect);

        if (!this.rosClient.SetupSocket())
        {
            Debug.Log("Failed to set up socket");
            return false;
        }

        // If you want to advertise publishing/subscribing to other topics, do so here
        this.connected = true;

        return this.connected;
    }

    public bool isConnected()
    {
        return this.connected;
    }

    public void CloseConnection()
    {
        this.rosClient.CloseSocket();
        this.connected = false;
    }

    public void stopWebsocketTimer()
    {
        this.rosClient.StopTimer();
    }

    public Action SendRosMessage(RosMessages.RosMessage message)
    {
        Debug.Log("sending " + message);
        return () => {
            Thread t = new Thread(() => {
                Dictionary<string, object> publish = new Dictionary<string, object>{
                    { "topic", message.GetTopic()},
                    { "op", "publish"},
                };
                Dictionary<string, object> data = message.GetData();
                publish.Add("msg", data);
                Debug.Log("Sending event ROS message: " + Json.Serialize(publish));
                bool sent = false;
                sent = this.rosClient.SendMessage(Json.Serialize(publish));
                while (!sent && this.isConnected())
                {
                    Debug.Log("Sending again...");
                    // Thread.Sleep(500);
                    sent = this.rosClient.SendMessage(Json.Serialize(publish));
                }
            });
            t.Start();

        };
    }

    public Action SendRosMessage(string message)
    {
        Debug.Log("sending " + message);
        return () => {
            Thread t = new Thread(() => {
                bool sent = false;
                sent = this.rosClient.SendMessage(message);
                while (!sent && this.isConnected())
                {
                    Debug.Log("Sending again...");
                    // Thread.Sleep(500);
                    sent = this.rosClient.SendMessage(message);
                }
            });
            t.Start();

        };
    }

    private void onMessageReceived(Dictionary<string, object> msg)
    {
        Debug.Log(msg["transcription"]);
    }
}
