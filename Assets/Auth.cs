using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class Auth : MonoBehaviour {

    public InputField nameInput;
    public InputField passwordInput;

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;
	// Use this for initialization
	void Start () {
        if (nameInput == null || passwordInput == null)
        {
            Debug.LogError("Set Input for name and password in " + this.name);
            Destroy(gameObject);
            return;
        }
        InitializeFirebase();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void AuthMe()
    {
        Debug.Log("Auth Pressed");
        Debug.LogAssertion("Name: " + nameInput.text);
        Debug.LogAssertion("Password: " + passwordInput.text);

        auth.CreateUserWithEmailAndPasswordAsync(nameInput.text, passwordInput.text).ContinueWith(HandleCreateResult); 
    }
    void HandleCreateResult(System.Threading.Tasks.Task<Firebase.Auth.FirebaseUser> authTask) { 
        if (authTask.IsCanceled) {
            Debug.Log("User Creation canceled.");
        } else if (authTask.IsFaulted) {
            Debug.Log("User Creation encountered an error.");
            Debug.Log(authTask.Exception.ToString());
        } else if (authTask.IsCompleted) {
            Debug.Log("User Creation completed.");
            if (auth.CurrentUser != null) {
                Debug.Log("User Info: " + auth.CurrentUser.Email + "   " + auth.CurrentUser.ProviderId + "  UID: " + auth.CurrentUser.PhotoUrl);
            
                Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile();
                profile.DisplayName = "Jane Q. User";
                profile.PhotoUri =  new System.Uri("https://example.com/jane-q-user/profile.jpg");
                user.UpdateUserProfileAsync(profile).ContinueWith(task => {
                    if (task.IsCompleted) {
                        Debug.Log("User profile updated.");
                    }
                });
            }
        }
    }
    public void SignIn(){
        Debug.Log("SignIn Pressed");
        Debug.LogAssertion("Name: " + nameInput.text);
        Debug.LogAssertion("Password: " + passwordInput.text);

        auth.SignInWithEmailAndPasswordAsync(nameInput.text, passwordInput.text).ContinueWith(task => {
            if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted) { 
                user = task.Result; 
                Debug.Log("User has been Signed In: " + auth.CurrentUser.DisplayName + " " + auth.CurrentUser.PhotoUrl); 
            }
        });
    }
    // initialization
    void InitializeFirebase()
    {

        Debug.Log("Setting up Firebase Auth");

        FirebaseApp app = FirebaseApp.DefaultInstance; 
        //The FirebaseAuth class is the gateway for all API calls. It is accessable through FirebaseAuth.DefaultInstance.
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;  
        auth.StateChanged += AuthStateChanged;
    }
    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            if (user == null && auth.CurrentUser != null)
            {
                Debug.Log("Signed in " + auth.CurrentUser.DisplayName); 
            }
            else if (user != null && auth.CurrentUser == null)
            {
                Debug.Log("Signed out " + user.DisplayName);
            }
            user = auth.CurrentUser;
        }
    }
    // cleanup
    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }
}
