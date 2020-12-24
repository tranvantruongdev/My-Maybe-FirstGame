using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
//Supress default null warning
#pragma warning disable 0649

public class ShareScoreManager : MonoBehaviour
{
    [SerializeField] GameObject Panel_share;
    [SerializeField] Text txtPanelScore;
    [SerializeField] Text txtHomeScore;
    [SerializeField] Text txtDate;

    private void Awake()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
		Debug.unityLogger.logEnabled = false;
#endif
    }

    public void ShareScore()
    {
        //get the same score in home sceen
        txtPanelScore.text = txtHomeScore.text; 
        //get the current date
        System.DateTime dt = System.DateTime.Now; 

        txtDate.text = string.Format("{0}/{1}/{2}", dt.Day, dt.Month, dt.Year);

        //open the score panel
        //show the panel
        Panel_share.SetActive(true);
        StartCoroutine("TakeScreenShotAndShare");
    }

    IEnumerator TakeScreenShotAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D tx = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        tx.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        tx.Apply();
        //image name
        string path = Path.Combine(Application.temporaryCachePath, "sharedImage.png");
        File.WriteAllBytes(path, tx.EncodeToPNG());
        //avoid memory leaks
        Destroy(tx); 

        new NativeShare()
            .AddFile(path)
            .SetSubject("This is my score")
            .SetText("Check this awesome game!\nhttps://tranvantruongdev.itch.io/deadly-house")
            .Share();
        //hide the panel
        Panel_share.SetActive(false); 
    }
}
