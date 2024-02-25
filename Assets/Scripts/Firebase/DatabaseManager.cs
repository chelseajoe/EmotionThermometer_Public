
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
using System.Threading.Tasks;

//using System.Text.Json.Serialization;


public class DatabaseManager : MonoBehaviour
{


    private DatabaseReference reference;
    public Dictionary<string, List<string>> emotionStrategyMap;
    public string userId;
    public int sessionIdx;
    public string currentSessionKey;
    public int latestSessionNumber;
    public List<string> emotionStrategies;
    public string loginTime;

    public static DatabaseManager Instance;


    private void Awake()
    {
        // NOTE: where Database Manager is set to dontdestroyonload
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //private void Awake()
    //{
    //    Instance = this;
    //    Debug.Log("DatabaseManager Awake");
    //}

    private void Start()
    {

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            //FirebaseApp app = FirebaseApp.DefaultInstance;
            //reference = FirebaseDatabase.DefaultInstance.RootReference;
            //print("getting firebase reference");
            if (task.IsCompleted && !task.IsFaulted)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                reference = FirebaseDatabase.DefaultInstance.RootReference;

                // Now you can use the 'reference' or perform other actions dependent on Firebase initialization
            }
            else
            {
                // Handle the error if Firebase initialization fails
                Debug.LogError("Firebase initialization failed: " + task.Exception);
            }

        });


        //auth = FirebaseAuth.DefaultInstance;

    }

    public DatabaseReference getRef()
    {
        return reference;
    }



    public void ReInitializeDatabase()
    {
        StartCoroutine(InitializeFirebaseAndContinue());
    }

    private IEnumerator InitializeFirebaseAndContinue()
    {
        Task<Firebase.DependencyStatus> dependencyStatusTask = FirebaseApp.CheckAndFixDependenciesAsync();

        yield return new WaitUntil(() => dependencyStatusTask.IsCompleted);
        print("BYEE???");
        if (dependencyStatusTask.Result == Firebase.DependencyStatus.Available)
        {
            print("Firebase initialization successful");

            FirebaseApp app = FirebaseApp.DefaultInstance;
            reference = FirebaseDatabase.DefaultInstance.RootReference;

            // Now you can use the 'reference' or perform other actions dependent on Firebase initialization
            print("HELLO???");
            if (!string.IsNullOrEmpty(userId))
            {
                print("CREATING SESSION");
                FetchUserData(userId);
                StartCoroutine(CreateSession());
            }
        }
        else
        {
            // Handle the error if Firebase initialization fails
            Debug.LogError("Firebase initialization failed: " + dependencyStatusTask.Exception);
        }
    }

    public void ReadStringInput()
    {
        //SignIn();
        //userId = input;
        FetchUserData(userId);
        //StartCoroutine(Sleep(4.0f));
        print("CREATE SESSION AFTER INPUT");
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
        print("fetching");
        if (reference == null)
        {
            print("reference is null");
        }
        print(userId);
        reference.Child(userId).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
            {
                DataSnapshot snapshot = task.Result;

                print("Snapshot");
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
            loginTime = DateTime.Now.ToString();
            loginTime = loginTime.Replace("/", "-"); // Replace slashes with underscores - firebase pathing issues with images, used for consistency

            currentSessionKey = "Session " + sessionIdx.ToString();
            //currentSessionKey = sessionKey;

            print("sessionKey " + currentSessionKey);
            reference.Child(userId).Child(currentSessionKey).Child("DATETIME").SetValueAsync(loginTime);

            ////added so that we can reference this elsewhere
            //reference.Child(userId).Child(currentSessionKey).SetValueAsync(emotionStrategyMap);
        }
    
    }


    public void EmotionIdentified(string emotion, string strategy)
    {
        //FetchUserData(userId);

        //emotionStrategyMap = reference.

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

    //public void EmotionIdentified(string emotion, string strategy)
    //{
    //    // FetchUserData(userId); // You might not need to call FetchUserData here, as it might have been called before when creating the session

    //    // Assuming your data is stored at the path userId/currentSessionKey/DATETIME
    //    //string path = $"{userId}/{currentSessionKey}";
    //    string path = $"{userId}/{currentSessionKey}";

    //    reference.Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
    //    {
    //        if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
    //        {
    //            DataSnapshot snapshot = task.Result;

    //            if (snapshot.Exists)
    //            {
    //                // Deserialize the data from Firebase into your emotionStrategyMap dictionary
    //                string json = snapshot.GetRawJsonValue();
    //                emotionStrategyMap = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);

    //                //// Ensure the dictionary is not null before adding the new strategy
    //                //if (emotionStrategyMap == null)
    //                //{
    //                //    emotionStrategyMap = new Dictionary<string, List<string>>
    //                //{
    //                //    { "angry", new List<string>() },
    //                //    { "sad", new List<string>() },
    //                //    { "nervous", new List<string>() },
    //                //    { "happy", new List<string>() },
    //                //    { "calm", new List<string>() },
    //                //};
    //                //}

    //                // Add strategy to dictionary -> list of strategies for that emotion identified
    //                emotionStrategyMap[emotion].Add(strategy);

    //                List<string> strategies = emotionStrategyMap[emotion];
    //                string emotionKey = "Emotion Identified - " + emotion;
    //                reference.Child(userId).Child(currentSessionKey).Child(emotionKey).Child("Strategies").SetValueAsync(strategies);

    //                //// Update the data back to Firebase
    //                //reference.Child(path).SetValueAsync(emotionStrategyMap);
    //            }
    //            else
    //            {
    //                Debug.LogWarning("Data not found at path: " + path);

    //                //// Ensure the dictionary is not null before adding the new strategy
    //                //if (emotionStrategyMap == null)
    //                //{
    //                //    emotionStrategyMap = new Dictionary<string, List<string>>
    //                //{
    //                //    { "angry", new List<string>() },
    //                //    { "sad", new List<string>() },
    //                //    { "nervous", new List<string>() },
    //                //    { "happy", new List<string>() },
    //                //    { "calm", new List<string>() },
    //                //};
    //                //}

    //                //reference.Child(path).
    //            }
    //        }
    //        else
    //        {
    //            Debug.LogError("Failed to fetch data from Firebase: " + task.Exception);
    //        }
    //    });
    //}


    public void AddScenarioSpeech(string scenarioName, string response)
    {

        // Get the reference to the list
        DatabaseReference listReference = reference.Child(userId).Child(currentSessionKey).Child(scenarioName);

        // Fetch the existing list
        listReference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                // Check if the list already exists
                if (snapshot.Exists)
                {
                    // Get the existing list as a List<string>
                    List<string> existingList = new List<string>();

                    foreach (var childSnapshot in snapshot.Children)
                    {
                        existingList.Add(childSnapshot.Value.ToString());
                    }

                    // Add the new response to the list
                    existingList.Add(response);

                    // Set the updated list back to Firebase
                    listReference.SetValueAsync(existingList);
                }
                else
                {
                    // If the list doesn't exist, create a new list with the response
                    List<string> newList = new List<string> { response };
                    listReference.SetValueAsync(newList);
                }
            }
            else
            {
                Debug.LogError("Failed to fetch list data: " + task.Exception);
            }
        });
    }




}




