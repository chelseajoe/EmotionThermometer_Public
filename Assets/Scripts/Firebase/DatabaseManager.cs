
using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using Newtonsoft.Json;
using SimpleJSON;
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
    private FirebaseAuth auth;
    private int sessionIdx;
    private string currentSessionKey;
    int latestSessionNumber;
    List<string> emotionStrategies;
    //public string userId;
    //public UserData userData;

    //private List<string> angryStrategies = new List<string>();
    //private List<string> sadStrategies = new List<string>();
    //private List<string> nervousStrategies = new List<string>();
    //private List<string> happyStrategies = new List<string>();
    //private List<string> calmStrategies = new List<string>(); 


    private static DatabaseManager instance;

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

            //RegisterUserFromConsole();

            //print("hello");

            ////Authenticate the user if needed
            //FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWith(authTask =>
            //{
            //    if (authTask.IsCompleted && !authTask.IsCanceled && !authTask.IsFaulted)
            //    {
            //        print("hello");
            //        currentUser = authTask.Result.User;
            //        userId = currentUser.UserId;

            //        print(userId);
            //        reference.Child("users").Child(userId).SetValueAsync("John Doe");
            //        //reference.Child("users").SetValueAsync(currentSession);

            //        // Load the emotionStrategyMap from Firebase
            //        LoadEmotionStrategyMapFromFirebase();
            //    }
            //});

            //emotionStrategyMap = new Dictionary<string, List<string>>
            //        {
            //            { "angry", new List<string>() },
            //            { "sad", new List<string>() },
            //            { "nervous", new List<string>() },
            //            { "happy", new List<string>() },
            //            { "calm", new List<string>() },
            //};

            //reference.Child("users").Child(userId).Child("emotionStrategyMap").GetValueAsync().ContinueWith(task =>
            //        { }

            //emotionStrategyMap["angry"]

            //string json = JsonUtility.ToJson(emotionStrategyMap);

            //print("hello");
            //currentUser = authTask.Result.User;

            // PID from first screen
            //userId = "John";
            //userId = readInput.PID;
            //print(readInput.PID);
            print("HIIIII");
            //print(userId);
            //currentUser = "johnny"; // placeholder

            //print(userId);
            //reference.Child("users").Child(userId).SetValueAsync("John Doe");
            //reference.Child(userId).Child("emotionStrategyMap").SetValueAsync("hiii");
            //CreateSession();

            //reference.GetValueAsync().ContinueWith(task => {
            //    if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
            //    {
            //        DataSnapshot snapshot = task.Result;
            //        if (snapshot.Exists)
            //        {
            //            string jsonData = snapshot.GetRawJsonValue();
            //            //UpdateJsonData(newJsonData); // Call a method to update the JSON data in Unity
            //            //string username = "John Doe"; // Change this to the desired username
            //            int latestSessionNumber = FindLatestSessionNumber(userId, jsonData);

            //            Debug.Log("Latest session number for " + userId + ": " + latestSessionNumber);
            //        }
            //        else
            //        {
            //            Debug.LogWarning("Data doesn't exist in Firebase.");
            //        }
            //    }
            //    else
            //    {
            //        Debug.LogError("Failed to fetch data from Firebase: " + task.Exception);
            //    }
            //});

            // find latest Session Number
            //FetchUserData(userId);
            // Create Session is called in this function ^^
            //CreateSession();
            //FetchUserData(userId);
        //    emotionStrategyMap = new Dictionary<string, List<string>>
        //            {
        //                { "angry", new List<string>() },
        //                { "sad", new List<string>() },
        //                { "nervous", new List<string>() },
        //                { "happy", new List<string>() },
        //                { "calm", new List<string>() },
        //};






        });

    }
    

    public void ReadStringInput(string input)
    {
        userId = input;
        FetchUserData(userId);
        //StartCoroutine(Sleep(4.0f));
        StartCoroutine(CreateSession());
        SceneManager.LoadScene("MainMenu");
    }

    // function to make a list of all strategies
    private void GetEmotionStrategies(string userId, string emotion)
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
                                foreach (DataSnapshot strategy in emotionSnapshot.Children)
                                    emotionStrategies.Add(strategy.ToString());
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
            findLatestSessionNumber = Int32.Parse(numericPart);

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
        //print(emotionStrategyMap[emotion]);
        //emotionStrategyMap[emotion].ForEach(Console.WriteLine);
        //foreach (string str in emotionStrategyMap[emotion])
        //{
        //    Debug.Log(str);
        //}

        List<string> strategies = emotionStrategyMap[emotion];
        string emotionKey = "Emotion Identified - " + emotion;
        reference.Child(userId).Child(currentSessionKey).Child(emotionKey).Child("Strategies").SetValueAsync(strategies);

        //// Check if Emotion has already been identified
        //reference.Child(userId).Child(currentSessionKey).Child(emotion).GetValueAsync().ContinueWith(task =>
        //{
        //    if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
        //    {
        //        reference.Child(userId).Child(currentSessionKey).Child(emotion).SetValueAsync
        //    }

        //    else
        //    {
        //        reference.Child(userId).Child(currentSessionKey).Child(emotion).Child("Strategies").SetValueAsync
        //    }

        //});
    }





    //// Save the emotionStrategyMap to Firebase
    //public void SaveEmotionStrategyMapToFirebase()
    //{
    //    print("entered SaveEmotionStrategyMapToFirebase");
    //    if (currentUser != null)
    //    {
    //        //string userId = currentUser.UserId;

    //        // Convert the emotionStrategyMap to JSON
    //        string json = JsonUtility.ToJson(emotionStrategyMap);
    //        print("json" + json);

    //        // Save the JSON data to Firebase
    //        reference.Child("users").Child(userId).Child("emotionStrategyMap").SetRawJsonValueAsync(json);
    //    }
    //}

    //// Load the emotionStrategyMap from Firebase
    //private void LoadEmotionStrategyMapFromFirebase()
    //{
    //    print("entered LoadEmotionStrategyMapFromFirebase");
    //    if (currentUser != null)
    //    {
    //        //string userId = currentUser.UserId;
    //        print("currusernotnull");

    //        reference.Child("users").Child(userId).Child("emotionStrategyMap").GetValueAsync().ContinueWith(task =>
    //        {
    //            if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
    //            {
    //                DataSnapshot snapshot = task.Result;
    //                if (snapshot.Exists)
    //                {
    //                    print("OLDLDDD");
    //                    // Deserialize the JSON data to the emotionStrategyMap
    //                    string json = snapshot.GetRawJsonValue();
    //                    //emotionStrategyMap = JsonUtility.FromJson<Dictionary<string, List<string>>(json);
    //                    //EmotionStrategyMapEntry[] dataArray = JsonConvert.DeserializeObject(json);

    //                    //// Convert the array to a Dictionary
    //                    //emotionStrategyMap = new Dictionary<string, List<string>>();

    //                    //foreach (var entry in dataArray)
    //                    //{
    //                    //    emotionStrategyMap[entry.emotion] = entry.strategies;
    //                    //}

    //                    // Load in emotionStrategyMap if it exists
    //                    emotionStrategyMap = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
    //                }
    //            }
    //            else
    //            {
    //                // Initialize your emotion to strategy mapping (load from somewhere or set it manually)
    //                emotionStrategyMap = new Dictionary<string, List<string>>
    //                {
    //                    { "Angry", new List<string>() },
    //                    { "Sad", new List<string>() },
    //                    { "Nervous", new List<string>() },
    //                    { "Happy", new List<string>() },
    //                    { "Calm", new List<string>() },
    //                };

    //                print("NEWWW");

    //            }
    //        });
    //    }
    //    print("EMOTIONSTRATGYCREATED");
    //    print(emotionStrategyMap.ToString());
    //}


    

    //// Log a new strategy to the current session
    //public void LogStrategy(string emotion, string strategy)
    //{
    //    if (currentUser != null && currentSession.sessionId != null)
    //    {
    //        // Append the strategy to the current session
    //        emotionStrategyMap[emotion].Add(strategy);

    //        // Check if the emotion exists in the current sessionEmotionStrategyMap
    //        if (!currentSession.sessionEmotionStrategyMap.ContainsKey(emotion))
    //        {
    //            currentSession.sessionEmotionStrategyMap[emotion] = new List<string>();
    //        }

    //        // Append the strategy to the emotion's list of strategies
    //        currentSession.sessionEmotionStrategyMap[emotion].Add(strategy);

    //        // Update the Firebase session with the new strategy
    //        reference.Child("sessions").Child(currentSession.sessionId).SetValueAsync(currentSession);

    //        //// Update the emotionStrategyMap with the new emotion and strategy
    //        //if (!emotionStrategyMap.ContainsKey(emotion))
    //        //{
    //        //    emotionStrategyMap[emotion] = new List<string>();
    //        //}

    //        // Now you have added the strategy to the current session and updated the emotionStrategyMap.
    //    }
    //}
}







//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Firebase;
//using Firebase.Database;
//using Firebase.Auth;


//public class DatabaseManager : MonoBehaviour
//{
//    private string userID;
//    private DatabaseReference dbReference;
//    void Start()
//    {
//        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
//        userID = SystemInfo.deviceUniqueIdentifier;
//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }

//    private void createNewUser(string userId, string username, string email) {
//        User newUser = new User(name, email);
//        string json = JsonUtility.ToJson(newUser);

//        dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json);
//    }

//    public void CreateSession(string emotion, string strategy)
//    {
//        dateTime = firebase.database.ServerValue.TIMESTAMP;
//        if (currentUser == null)
//        {
//            Debug.LogWarning("User is not authenticated.");
//            return;
//        }

//        string userId = currentUser.UserId;

//        // Create a new session node in the database
//        DatabaseReference sessionRef = reference.Child("Users").Child(userId).Child("Sessions").Push();

//        // Populate the session data
//        sessionRef.Child("dateTime").SetValueAsync(dateTime);
//        sessionRef.Child("emotion").SetValueAsync(emotion);
//        sessionRef.Child("Strategy").SetValueAsync(strategy);

//        Debug.Log("New session created and stored in the database.");
//    }
//}
//}
