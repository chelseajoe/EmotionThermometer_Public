
using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using Newtonsoft.Json;
// using SimpleJSON;
using Google.MiniJSON;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

//using System.Text.Json.Serialization;


public class DatabaseManager : MonoBehaviour
{


    private DatabaseReference reference;
    //private FirebaseUser currentUser;
    private string currentUser;
    public Dictionary<string, List<string>> emotionStrategyMap;
    public string userId;
    private int sessionIdx;
    private string currentSessionKey;
    int latestSessionNumber;
    public List<string> emotionStrategies;
    

    //public string userId;
    //public UserData userData;

    //private List<string> angryStrategies = new List<string>();
    //private List<string> sadStrategies = new List<string>();
    //private List<string> nervousStrategies = new List<string>();
    //private List<string> happyStrategies = new List<string>();
    //private List<string> calmStrategies = new List<string>(); 


    private static DatabaseManager instance;
    private FirebaseAuth auth;

    private void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this;
        DontDestroyOnLoad(gameObject);
        //sessionIdx = 0;
        //FetchUserData(userId);

        //user

        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
    }

    private void Start()
    {
        
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            reference = FirebaseDatabase.DefaultInstance.RootReference;

        });
        //auth = FirebaseAuth.DefaultInstance;

    }

    //public void SignIn()
    //{
    //    string pID = userId;

    //    // Check if the user with the given username exists
    //    CheckUsernameAvailability(pID, userExists =>
    //    {
    //        if (userExists)
    //        {
    //            Debug.Log("User signed in successfully: " + pID);
    //        }
    //        else
    //        {
    //            CreateUser(pID);
    //            Debug.Log("Creating User");
    //        }
    //    });
    //}

    //void CheckUsernameAvailability(string pID, System.Action<bool> callback)
    //{
    //    reference.Child("PIDs").Child(pID).GetValueAsync().ContinueWithOnMainThread(task =>
    //    {
    //        if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
    //        {
    //            callback(!task.Result.Exists);
    //        }
    //        else
    //        {
    //            callback(false);
    //        }
    //    });
    //}

    //void CreateUser(string pID)
    //{
    //    // Create a new user entry in the database
    //    reference.Child("PIDs").Child(pID).SetValueAsync(true);
    //}


    public void ReadStringInput()
    {
        //SignIn();
        //userId = input;
        FetchUserData(userId);
        //StartCoroutine(Sleep(4.0f));
        StartCoroutine(CreateSession());
        SceneManager.LoadScene("MainMenu");
    }

    // function to make a list of all strategies
    public void GetEmotionStrategies(string userId, string emotion)
    {
        emotionStrategies = new List<string>();
        reference.Child(userId).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
            {
                string emotionKey = "Emotion Identified - " + emotion;
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    foreach (DataSnapshot sessionSnapshot in snapshot.Children)
                    {
                        foreach (DataSnapshot emotionSnapshot in sessionSnapshot.Children)
                        {
                            if (emotionSnapshot.Key == emotionKey)
                            {
                                foreach (DataSnapshot strategyList in emotionSnapshot.Children)
                                {
                                    foreach (DataSnapshot strategy in strategyList.Children)
                                    {

                                        //print(strategy.Value.ToString());
                                        emotionStrategies.Add(strategy.Value.ToString());
                                    }
                                }
                                    
                            }
                        }
                    }
                }


                else
                {
                    Debug.LogWarning("User data for " + userId + " doesn't exist in Firebase.");

                }
            }

            else
            {
                Debug.LogError("Failed to fetch data from Firebase: " + task.Exception);
            }
        });
    }

    // get all data to find latest session
    private void FetchUserData(string userId)
    {
        reference.Child(userId).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {

                    // Iterate through sessions to find the latest session number
                    latestSessionNumber = FindLatestSessionNumber(snapshot);
                    print("latestSessinoNumber" + latestSessionNumber);
                    // Create Session is Called HERE
                    //CreateSession();
                    //return snapshot
                    Debug.Log("Latest session number for " + userId + ": " + latestSessionNumber);
                }
                else
                {
                    Debug.LogWarning("User data for " + userId + " doesn't exist in Firebase.");
                    latestSessionNumber = 0;

                }
            }
            else
            {
                Debug.LogError("Failed to fetch data from Firebase: " + task.Exception);
                latestSessionNumber = 0;
            }
        });
    }

    private int FindLatestSessionNumber(DataSnapshot userDataSnapshot)
    {
        int findLatestSessionNumber = -1;

        foreach (DataSnapshot sessionSnapshot in userDataSnapshot.Children)
        {
            //print("SessionSnapshot: " + sessionSnapshot);
            

            string numericPart = sessionSnapshot.Key.Replace("Session ", "");
            print("numericPart: " + numericPart);

            findLatestSessionNumber = Mathf.Max(Int32.Parse(numericPart), findLatestSessionNumber);

            //if (int.TryParse(sessionSnapshot.Key, out int sessionNumber))
            //{
            //    latestSessionNumber = Math.Max(latestSessionNumber, sessionNumber);
            //}
        }

        print("latestSessionNumber : " + findLatestSessionNumber);
        return findLatestSessionNumber;
    }


// Log session information
//public void CreateSession()
    IEnumerator CreateSession()
    {
        FetchUserData(userId);
        yield return new WaitForSeconds(2.0f);
        print("latestSessionNumber Create : " + latestSessionNumber);

        emotionStrategyMap = new Dictionary<string, List<string>>
                    {
                        { "angry", new List<string>() },
                        { "sad", new List<string>() },
                        { "nervous", new List<string>() },
                        { "happy", new List<string>() },
                        { "calm", new List<string>() },
        };

        sessionIdx = latestSessionNumber + 1;
        if (userId != null)
        {
            string loginTime = DateTime.Now.ToString();

            currentSessionKey = "Session " + sessionIdx.ToString();
            //currentSessionKey = sessionKey;

            print("sessionKey " + currentSessionKey);
            reference.Child(userId).Child(currentSessionKey).Child("DATETIME").SetValueAsync(loginTime);
        }
    }


    public void EmotionIdentified(string emotion, string strategy)
    {
        //FetchUserData(userId);

        // Add strategy to dictionary -> list of strategies for that emotion identified
        if (emotionStrategyMap == null)
        {
            print("NULLLLL");
        }
        emotionStrategyMap[emotion].Add(strategy);


        List<string> strategies = emotionStrategyMap[emotion];
        string emotionKey = "Emotion Identified - " + emotion;
        reference.Child(userId).Child(currentSessionKey).Child(emotionKey).Child("Strategies").SetValueAsync(strategies);

    }




}




