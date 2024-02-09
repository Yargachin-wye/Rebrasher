using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaler : MonoBehaviour
{
    bool scaler = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            scaler = !scaler;
            if (scaler)
            {
                Time.timeScale = 0.2f;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }
}
