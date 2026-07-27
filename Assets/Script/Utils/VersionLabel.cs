using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class VersionLabel : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI versionText;
        private void Awake()
        {
            if (versionText != null || TryGetComponent(out versionText) )
            {
                versionText.text = "v"+Application.version;
            }
        }
    }
