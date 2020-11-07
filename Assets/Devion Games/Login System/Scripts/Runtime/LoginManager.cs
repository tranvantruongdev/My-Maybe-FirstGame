using DevionGames.LoginSystem.Configuration;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Firebase;
using Firebase.Auth;
using UnityEditor;

namespace DevionGames.LoginSystem
{
    public class LoginManager : MonoBehaviour
    {
        private static LoginManager m_Current;

        private static FirebaseAuth auth;

        /// <summary>
        /// The LoginManager singleton object. This object is set inside Awake()
        /// </summary>
        public static LoginManager current
        {
            get
            {
                Assert.IsNotNull(m_Current, "Requires Login Manager.Create one from Tools > Devion Games > Login System > Create Login Manager!");
                return m_Current;
            }
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if (m_Current != null)
            {
                if (DefaultSettings.debug)
                    Debug.Log("Multiple LoginManager in scene...this is not supported. Destroying instance!");
                Destroy(gameObject);
                return;
            }
            else
            {
                m_Current = this;
                if (DefaultSettings.debug)
                    Debug.Log("LoginManager initialized.");

            }
        }

        private void Start()
        {
            auth = FirebaseAuth.DefaultInstance;

            if (DefaultSettings.skipLogin)
            {
                if (DefaultSettings.debug)
                    Debug.Log("Login System is disabled...Loading " + LoginManager.DefaultSettings.sceneToLoad + " scene.");
                UnityEngine.SceneManagement.SceneManager.LoadScene(LoginManager.DefaultSettings.sceneToLoad);
            }
        }


        [SerializeField]
        private LoginConfigurations m_Configurations = null;

        /// <summary>
        /// Gets the login configurations. Configurate it inside the editor.
        /// </summary>
        /// <value>The database.</value>
        public static LoginConfigurations Configurations
        {
            get
            {
                if (current != null)
                {
                    Assert.IsNotNull(current.m_Configurations, "Please assign Login Configurations to the Login Manager!");
                    return current.m_Configurations;
                }
                return null;
            }
        }


        private static Default m_DefaultSettings;
        public static Default DefaultSettings
        {
            get
            {
                if (m_DefaultSettings == null)
                {
                    m_DefaultSettings = GetSetting<Default>();
                }
                return m_DefaultSettings;
            }
        }

        private static UI m_UI;
        public static UI UI
        {
            get
            {
                if (m_UI == null)
                {
                    m_UI = GetSetting<UI>();
                }
                return m_UI;
            }
        }

        private static Notifications m_Notifications;
        public static Notifications Notifications
        {
            get
            {
                if (m_Notifications == null)
                {
                    m_Notifications = GetSetting<Notifications>();
                }
                return m_Notifications;
            }
        }

        private static Server m_Server;
        public static Server Server
        {
            get
            {
                if (m_Server == null)
                {
                    m_Server = GetSetting<Server>();
                }
                return m_Server;
            }
        }

        private static T GetSetting<T>() where T : Configuration.Settings
        {
            if (Configurations != null)
            {
                return (T)Configurations.settings.Where(x => x.GetType() == typeof(T)).FirstOrDefault();
            }
            return default(T);
        }



        public static void CreateAccount(string username, string password, string email)
        {
            if (current != null)
            {
                current.StartCoroutine(CreateAccountInternal(username, password, email));
            }
        }

        private static IEnumerator CreateAccountInternal(string username, string password, string email)
        {
            if (Configurations == null)
            {
                EventHandler.Execute("OnFailedToCreateAccount");
                yield break;
            }
            if (DefaultSettings.debug)
                Debug.Log("[CreateAccount]: Trying to register a new account with username: " + username + " and password: " + password + "!");

            //Call the Firebase auth signin function passing the email and password
            var task = auth.CreateUserWithEmailAndPasswordAsync(email, password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => task.IsCompleted);

            if (task.Exception != null)
            {
                //If there are errors handle them
                if (DefaultSettings.debug) 
                    Debug.LogWarning(message: $"Failed to register task with {task.Exception}");
                FirebaseException firebaseEx = task.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                if (DefaultSettings.debug) 
                    Debug.Log(message);
                EventHandler.Execute("OnFailedToCreateAccount");
            }
            else
            {
                //User has now been created
                //Now get the result
                var User = task.Result;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        if (DefaultSettings.debug) 
                            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        if (DefaultSettings.debug) 
                            Debug.Log("Username Set Failed!");
                        EventHandler.Execute("OnFailedToCreateAccount");
                    }
                    else
                    {
                        if (DefaultSettings.debug)
                            Debug.Log("Register successfully");
                        EventHandler.Execute("OnAccountCreated");
                    }
                }
            }
            #region old code
            //WWWForm newForm = new WWWForm();
            //newForm.AddField("name", username);
            //newForm.AddField("password", password);
            //newForm.AddField("email", email);

            //using (UnityWebRequest www = UnityWebRequest.Post(LoginManager.Server.serverAddress + "/" + LoginManager.Server.createAccount, newForm)) {
            //	yield return www.SendWebRequest();
            //	if (www.isNetworkError || www.isHttpError)
            //	{
            //		Debug.Log(www.error);
            //	}
            //	else {
            //		if (www.downloadHandler.text.Contains("true"))
            //		{
            //			if (LoginManager.DefaultSettings.debug)
            //				Debug.Log("[CreateAccount] Account creation was successfull!");
            //			EventHandler.Execute("OnAccountCreated");
            //		}else {
            //			if (LoginManager.DefaultSettings.debug)
            //				Debug.Log("[CreateAccount] Failed to create account.");
            //			EventHandler.Execute("OnFailedToCreateAccount");
            //		}
            //	}
            //}
            #endregion
        }

        /// <summary>
        /// Logins the account.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        public static void LoginAccount(string username, string password)
        {
            if (current != null)
            {
                current.StartCoroutine(LoginAccountInternal(username, password));
            }
        }

        private static IEnumerator LoginAccountInternal(string username, string password)
        {
            if (LoginManager.Configurations == null)
            {
                EventHandler.Execute("OnFailedToLogin");
                yield break;
            }
            if (LoginManager.DefaultSettings.debug)
                Debug.Log("[LoginAccount] Trying to login using username: " + username + " and password: " + password + "!");

            //Call the Firebase auth signin function passing the email and password
            var task = auth.SignInWithEmailAndPasswordAsync(username, password);

            //Wait until the task completes
            yield return new WaitUntil(predicate: () => task.IsCompleted);

            if (task.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {task.Exception}");
                FirebaseException firebaseEx = task.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Login Failed!";
                switch (errorCode)
                {
                    case AuthError.WrongPassword:
                        message = "Wrong Password";
                        EventHandler.Execute("OnFailedToLogin");
                        break;
                    case AuthError.UserNotFound:
                        message = "Account does not exist";
                        EventHandler.Execute("OnFailedToLoginAccountNotExist");
                        break;
                }
                if (DefaultSettings.debug)
                    Debug.Log(message);
            }
            else
            {
                FirebaseUser newUser = task.Result;

                PlayerPrefs.SetString(Server.accountKey, username);

                if (DefaultSettings.debug)
                    Debug.LogFormat("User signed in successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);

                //hold info of user
                GameSetting.username = newUser.DisplayName;
                GameSetting.uid = newUser.UserId;

                EventHandler.Execute("OnLogin");
            }
            #region old code
            //WWWForm newForm = new WWWForm();
            //newForm.AddField("name", username);
            //newForm.AddField("password", password);


            //using (UnityWebRequest www = UnityWebRequest.Post(LoginManager.Server.serverAddress + "/" + LoginManager.Server.login, newForm))
            //{
            //	yield return www.SendWebRequest();
            //	if (www.isNetworkError || www.isHttpError)
            //	{
            //		Debug.Log(www.error);
            //	}
            //	else
            //	{
            //		if (www.downloadHandler.text.Contains("true"))
            //		{
            //			PlayerPrefs.SetString(LoginManager.Server.accountKey, username);
            //			if (LoginManager.DefaultSettings.debug)
            //				Debug.Log("[LoginAccount] Login was successfull!");
            //			EventHandler.Execute("OnLogin");
            //		}
            //		else
            //		{
            //			if (LoginManager.DefaultSettings.debug)
            //				Debug.Log("[LoginAccount] Failed to login.");
            //			EventHandler.Execute("OnFailedToLogin");
            //		}
            //	}
            //}
            #endregion
        }

        /// <summary>
        /// Recovers the password.
        /// </summary>
        /// <param name="email">Email.</param>
        public static void RecoverPassword(string email)
        {
            if (current != null)
            {
                current.StartCoroutine(RecoverPasswordInternal(email));
            }
        }

        private static IEnumerator RecoverPasswordInternal(string email)
        {
            if (Configurations == null)
            {
                EventHandler.Execute("OnFailedToRecoverPassword");
                yield break;
            }
            if (DefaultSettings.debug)
                Debug.Log("[RecoverPassword] Trying to recover password using email: " + email + "!");

            WWWForm newForm = new WWWForm();
            newForm.AddField("email", email);

            using (UnityWebRequest www = UnityWebRequest.Post(LoginManager.Server.serverAddress + "/" + LoginManager.Server.recoverPassword, newForm))
            {
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    if (www.downloadHandler.text.Contains("true"))
                    {
                        EventHandler.Execute("OnPasswordRecovered");
                        if (LoginManager.DefaultSettings.debug)
                            Debug.Log("[RecoverPassword] Password recovered successfull!");
                    }
                    else
                    {
                        if (LoginManager.DefaultSettings.debug)
                            Debug.Log("[RecoverPassword] Failed to recover password.");
                        EventHandler.Execute("OnFailedToRecoverPassword");
                    }
                }
            }
        }

        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        public static void ResetPassword(string username, string password)
        {
            if (LoginManager.current != null)
            {
                LoginManager.current.StartCoroutine(ResetPasswordInternal(username, password));
            }
        }

        private static IEnumerator ResetPasswordInternal(string username, string password)
        {
            if (LoginManager.Configurations == null)
            {
                EventHandler.Execute("OnFailedToResetPassword");
                yield break;
            }
            if (LoginManager.DefaultSettings.debug)
                Debug.Log("[ResetPassword] Trying to reset password using username: " + username + " and password: " + password + "!");

            WWWForm newForm = new WWWForm();
            newForm.AddField("name", username);
            newForm.AddField("password", password);

            using (UnityWebRequest www = UnityWebRequest.Post(LoginManager.Server.serverAddress + "/" + LoginManager.Server.resetPassword, newForm))
            {
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    if (www.downloadHandler.text.Contains("true"))
                    {
                        if (LoginManager.DefaultSettings.debug)
                            Debug.Log("[RecoverPassword] Password resetted!");
                        EventHandler.Execute("OnPasswordResetted");

                    }
                    else
                    {
                        if (LoginManager.DefaultSettings.debug)
                            Debug.Log("Failed to reset password.");
                        EventHandler.Execute("OnFailedToResetPassword");
                    }
                }
            }
        }

        /// <summary>
        /// Validates the email.
        /// </summary>
        /// <returns><c>true</c>, if email was validated, <c>false</c> otherwise.</returns>
        /// <param name="email">Email.</param>
        public static bool ValidateEmail(string email)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            System.Text.RegularExpressions.Match match = regex.Match(email);
            if (match.Success)
            {
                if (LoginManager.DefaultSettings.debug)
                    Debug.Log("Email validation was successfull for email: " + email + "!");
            }
            else
            {
                if (LoginManager.DefaultSettings.debug)
                    Debug.Log("Email validation failed for email: " + email + "!");
            }

            return match.Success;
        }
    }
}