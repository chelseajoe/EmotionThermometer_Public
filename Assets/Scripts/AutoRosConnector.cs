using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading;

public class AutoRosConnector : MonoBehaviour
{
	private RosManager rosManager;
	private bool connected;

	private float timeToRetry = 2.0f;
	private float timer = 0.0f;
	// Use this for initialization
	void Awake()
	{
		Debug.Log("Auto connector started");
		rosManager = new RosManager("10.99.0.1", "9090");

		string ros_ip = "10.99.0.1";

		if (ros_ip != "")
		{
			tryToConnect();
		}
	}

	private void tryToConnect()
	{
		if (!rosManager.isConnected())
		{
			// Try to connect~
			connected = AutoRosConnect();
		}
	}


    private bool AutoRosConnect()
    {
        Debug.Log("Auto connecting to ros....");
        string rosbridgeIp = "10.99.0.1";
        // If user entered a different IP, use it, otherwise stick to default.
        Debug.Log("Trying to connect to roscore at " + rosbridgeIp);
        if (rosManager == null)
        {
            this.rosManager = new RosManager(rosbridgeIp, "9090");
        }
        if (!this.rosManager.isConnected())
        {

            if (this.rosManager.Connect())
            {
                Thread.Sleep(1000); // Wait for a bit to make sure connection is established.
                Debug.Log("Connection confirmed");
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Debug.Log("Already connected to ROS, not trying to connect again");
            return true;
        }
    }

    public RosManager GetRosManager()
    {
        return rosManager;
    }
}
