using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpeedTutorMainMenuSystem
{
    public class Init_LoadPreferences : MonoBehaviour
    {
        #region Variables        
        [Space(20)]
        [SerializeField] private bool canUse = false;
        [SerializeField] private MenuController menuController;
        #endregion

        private void Awake()
        {
            Debug.Log("Loading player prefs test");
            if (canUse)
            {
                
            }
        }
    }
}
