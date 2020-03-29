using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeResolution : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        // Switch to ... x ... full-screen
        Screen.SetResolution(1280, 720, true);
        //Screen.SetResolution(256, 144, true);
    }
}
