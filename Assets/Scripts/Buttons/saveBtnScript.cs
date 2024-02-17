using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using Firebase;
using Firebase.Storage;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.Assertions;
using System.Threading.Tasks;
using System.Collections;


public class SaveBtnScript : MonoBehaviour
{
    public DatabaseManager databaseManager;
    private GameObject dbManager;

    void Start ()
    {
        dbManager = GameObject.Find("DatabaseManager");
        databaseManager = dbManager.GetComponent<DatabaseManager>();
    }
    

    public string firebaseStoragePath = "images/Scenario3";

    public void SaveAndGoBack()
    {
        Debug.Log("Saving Scene Screenshot to Firebase");

        StartCoroutine(CaptureAndSaveScreenshot());
    }

    private IEnumerator CaptureAndSaveScreenshot()
    {
        // Capture the screenshot of the entire screen
        yield return CaptureSceneScreenshot();

        // Define the file path
        //string filePath = Application.persistentDataPath + "/Assets/Screenshots/screenshot.png";

        string filePath = "Assets/Screenshots/screenshot.png";

        print(filePath);

        // Wait until the file exists
        while (!File.Exists(filePath))
        {
            print("hello");
            yield return new WaitForSeconds(0.5f);
            //yield return null;
        }

        print("HIII");

        byte[] bytes = File.ReadAllBytes(filePath);

        print("BYTES" + bytes);

        // Upload the saved image to Firebase Storage
        UploadToFirebaseStorage(bytes);

        //// Clean up the temporary file
        //File.Delete(filePath);

        // Load the desired scene after capturing the screenshot
        SceneManager.LoadScene("ScenariosMenu");
    }

    private IEnumerator CaptureSceneScreenshot()
    {
        //// Capture the entire screen, including UI
        //ScreenCapture.CaptureScreenshot("scene_screenshot.png");

        //print("CAPTURING SCREENSHOT");


        string folderPath = "Assets/Screenshots/"; // the path of your project folder

        if (!System.IO.Directory.Exists(folderPath)) // if this path does not exist yet
            System.IO.Directory.CreateDirectory(folderPath);  // it will get created

        //var screenshotName =
        //                        "Screenshot_" +
        //                        System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + // puts the current time right into the screenshot name
        //                        ".png"; // put youre favorite data format here

        var screenshotName = "screenshot.png";
        ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName), 2); // takes the sceenshot, the "2" is for the scaled resolution, you can put this to 600 but it will take really long to scale the image up
        Debug.Log(folderPath + screenshotName); // You get instant feedback in the console


        // Wait for a frame to ensure the screenshot is taken
        yield return new WaitForSeconds(2.5f);

        //// Define the file path
        //string filePath = Application.persistentDataPath + "/scene_screenshot.png";

        //// Wait until the file exists
        //while (!File.Exists(filePath))
        //{
        //    yield return null;
        //}

        //byte[] bytes = File.ReadAllBytes(filePath);

        //// Upload the saved image to Firebase Storage
        //UploadToFirebaseStorage(bytes);

        //// Clean up the temporary file
        //File.Delete(filePath);
    }

    private void UploadToFirebaseStorage(byte[] bytes)
    {
        string userId = databaseManager.userId;
        string currentSessionKey = databaseManager.currentSessionKey;
        string loginTime = databaseManager.loginTime;
        // Get a reference to the storage service, using the default Firebase App
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;

        // Create a storage reference from our storage service
        StorageReference storageRef =
            storage.GetReferenceFromUrl("gs://emotion-thermometer.appspot.com");

        //// Create a root reference
        //StorageReference storageRef = storage.RootReference;

        // Create a reference to 'images/mountains.jpg'
        //StorageReference screenshotImagesRef =
        //    storageRef.Child("images/help.jpg");

        StorageReference screenshotImagesRef =
            storageRef.Child($"images/{userId}/{currentSessionKey}/scenario3/{loginTime}.jpg");

        // Upload the file to the path "images/mountains.jpg"
        screenshotImagesRef.PutBytesAsync(bytes)
            .ContinueWith((Task<StorageMetadata> task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log(task.Exception.ToString());
                    // Uh-oh, an error occurred!
                }
                else
                {
                    // Metadata contains file metadata such as size, content-type, and md5hash.
                    StorageMetadata metadata = task.Result;
                    string md5Hash = metadata.Md5Hash;
                    Debug.Log("Finished uploading...");
                    Debug.Log("md5 hash = " + md5Hash);
                }
            });
    }
}
