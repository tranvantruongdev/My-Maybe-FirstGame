using System;
using System.IO;
using UnityEngine;
/// <summary>
///  Copyright (c) 2016 Eric Zhu 
/// </summary>
namespace GreatArcStudios
{
    [System.Serializable]
    public class SaveSettings
    {
        /// <summary>
        /// Change the file name if something else floats your boat
        /// </summary>
        public string fileName = "GameSettings.json";
        /// <summary>
        /// Master volume
        /// </summary>
        public float masterVolume;
        /// <summary>
        /// Shadow Distance
        /// </summary>
        public float shadowDistINI;
        /// <summary>
        /// MSAA quality
        /// </summary>
        public float aaQualINI;
        /// <summary>
        /// Camera FOV
        /// </summary>
        public float fovINI;
        public int msaaINI;
        /// <summary>
        /// Texture quality
        /// </summary>
        public int textureLimit;
        /// <summary>
        /// Quality preset
        /// </summary>
        public int curQualityLevel;
        /// <summary>
        /// Aniso texture level
        /// </summary>
        public int anisoLevel;
        /// <summary>
        /// The string that will be saved.
        /// </summary>
        static string jsonString;
        /// <summary>
        /// Create the JSON object needed to save settings.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static object createJSONOBJ(string jsonString)
        {
            return JsonUtility.FromJson<SaveSettings>(jsonString);

        }
        /// <summary>
        /// Read the game settings from the file
        /// </summary>
        /// <param name="readString"></param>
        public void LoadGameSettings(String readString)
        {
            try
            {

                SaveSettings read = (SaveSettings)createJSONOBJ(readString);
                QualitySettings.antiAliasing = (int)read.aaQualINI;
                QualitySettings.shadowDistance = read.shadowDistINI;
                PauseManager.mainCamShared.fieldOfView = read.fovINI;
                QualitySettings.antiAliasing = read.msaaINI;
                PauseManager.lastTexLimit = read.textureLimit;
                QualitySettings.masterTextureLimit = read.textureLimit;
                AudioListener.volume = read.masterVolume;
                QualitySettings.SetQualityLevel(read.curQualityLevel);
                if (read.anisoLevel == 0)
                {
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                }
                else if (read.anisoLevel == 1)
                {
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                }
                else if (read.anisoLevel == 2)
                {
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                }
            }
            catch (FileNotFoundException)
            {
                Debug.Log("Game settings not found in: " + Application.persistentDataPath + "/" + fileName);
            }

        }
        /// <summary>
        /// Get the quality/music settings before saving 
        /// </summary>
        public void SaveGameSettings()
        {
            if (File.Exists(Application.persistentDataPath + "/" + fileName))
            {
                File.Delete(Application.persistentDataPath + "/" + fileName);
            }
            aaQualINI = QualitySettings.antiAliasing;
            shadowDistINI = PauseManager.shadowDistINI;
            fovINI = PauseManager.mainCamShared.fieldOfView;
            msaaINI = QualitySettings.antiAliasing;
            textureLimit = PauseManager.lastTexLimit;
            masterVolume = PauseManager.beforeMaster;
            curQualityLevel = QualitySettings.GetQualityLevel();
            if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.Disable)
            {
                anisoLevel = 0;
            }
            else if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.ForceEnable)
            {
                anisoLevel = 1;
            }
            else if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.Enable)
            {
                anisoLevel = 2;
            }
            jsonString = JsonUtility.ToJson(this);
            Debug.Log(jsonString);
            File.WriteAllText(Application.persistentDataPath + "/" + fileName, jsonString);
        }
    }
}