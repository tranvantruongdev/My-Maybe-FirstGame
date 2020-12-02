using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters;
using VoxelBusters.NativePlugins;

public class ShareScoreManager : MonoBehaviour
{
    private bool isSharing = false;

    public void ShareSocialMedia()
    {
        isSharing = true;
    }

    private void LateUpdate()
    {
        if (isSharing)
        {
            isSharing = false;
            StartCoroutine(CaptureScreenShoot());
        }
    }

    private IEnumerator CaptureScreenShoot()
    {
        yield return new WaitForEndOfFrame();
        Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
        ShareSheet(texture);
        Destroy(texture);
    }

    public void ShareSheet(Texture2D texture)
    {
        ShareSheet shareSheet = new ShareSheet();
        shareSheet.Text = "Share Sheet";
        shareSheet.AttachImage(texture);
        shareSheet.URL = "https://twiiter.com/RoixoGames";

        NPBinding.Sharing.ShowView(shareSheet, FinishSharing);
    }

    private void FinishSharing(eShareResult _result)
    {
        Debug.Log(_result);
    }
}
