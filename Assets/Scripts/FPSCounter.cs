using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;

    // Update is called once per frame
    void Update()
    {
        text.text = "" + (int)(1f / Time.unscaledDeltaTime);
    }
}
