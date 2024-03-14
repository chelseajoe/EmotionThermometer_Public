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
    public GameObject StatusCanvas;
    public bool savedImage;
    void Start ()
    {
        StatusCanvas.SetActive(false);
        savedImage = false;
    }
    

    public string firebaseStoragePath = "images/Scenario3";

    public void SaveAndGoBack()
    {
        Debug.Log("Saving Scene Screenshot to Firebase");

        StartCoroutine(CaptureAndSaveScreenshot());
    }

    private IEnumerator CaptureAndSaveScreenshot()
    {
        if (!savedImage)
        {
            savedImage = true;
            // Capture the screenshot of the entire screen
            yield return CaptureSceneScreenshot();

            // Define the file path
            //string filePath = Application.persistentDataPath + "Assets/Screenshots/screenshot.png";
            string filePath = Application.persistentDataPath + "/Screenshot.png";

            print(filePath);

            // Wait until the file exists
            while (!File.Exists(filePath))
            {
                print("hello");
                yield return new WaitForSeconds(0.5f);
                StatusCanvas.SetActive(true);
                //yield return null;
            }

            byte[] bytes = File.ReadAllBytes(filePath);



            // wait for screenshot to update -- not necessary unless large delay in build
            //yield return new WaitForSeconds(1f);

            print("2 seconds passed -- to wait for screenshot to load");

            // Upload the saved image to Firebase Storage
            UploadToFirebaseStorage(bytes);

            // Clean up the temporary file
            File.Delete(filePath);

            // Go back to menu
            SceneManager.LoadScene("ScenariosMenu");

        }

    }

    private IEnumerator CaptureSceneScreenshot()
    {

        //string folderPath = Application.persistentDataPath + "Assets/Screenshots/";

        //// if this path does not exist yet, it will get created
        //if (!System.IO.Directory.Exists(folderPath))

        //    System.IO.Directory.CreateDirectory(folderPath);
        //    print("folder does not exist and is being created");

        //print("waiting to make directory?");
        //yield return new WaitForSeconds(2.5f);

        //var screenshotName = "screenshot.png";
        //// takes the sceenshot, the "2" is for the scaled resolution, you can put this to 600 but it will take really long to scale the image up
        //ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName), 2);
        //Debug.Log(folderPath + screenshotName);


        // Unity Editor needs different filepath than android build to save screenshot
        string filePath = Application.persistentDataPath + "/Screenshot.png";

#if UNITY_EDITOR

        ScreenCapture.CaptureScreenshot(filePath, 2);
#else
        ScreenCapture.CaptureScreenshot("Screenshot.png", 2);
#endif

        //ScreenCapture.CaptureScreenshot("Screenshot.png", 2);


        // Wait for a frame to ensure the screenshot is taken
        yield return new WaitForSeconds(2.5f);

    }

    private void UploadToFirebaseStorage(byte[] bytes)
    {

        string userId = DatabaseManager.Instance.userId;
        string currentSessionKey = DatabaseManager.Instance.currentSessionKey;
        string loginTime = DatabaseManager.Instance.loginTime;

        // Get a reference to the storage service, using the default Firebase App
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;

        // Create a storage reference from our storage service
        StorageReference storageRef =
            storage.GetReferenceFromUrl("gs://emotion-thermometer.appspot.com");

        string currentTime = System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");

        StorageReference screenshotImagesRef =
            storageRef.Child($"images/{userId}/{currentSessionKey}/scenario3/{loginTime}/{currentTime}.jpg");

        screenshotImagesRef.PutBytesAsync(bytes)
            .ContinueWith((Task<StorageMetadata> task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log(task.Exception.ToString());
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
