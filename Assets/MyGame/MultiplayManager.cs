using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayManager : MonoBehaviour
{
    [SerializeField]
    private string mainMenuScene;

    DatabaseReference reference;

    // Start is called before the first frame update
    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackToSingle()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
