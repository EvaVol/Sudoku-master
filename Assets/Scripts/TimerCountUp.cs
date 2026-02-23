using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*public class TimerCountUp : MonoBehaviour
{
    Text timerText; 

    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        float t = Time.timeSinceLevelLoad;
        int seconds = (int)(t % 60);
        t /= 60;
        int minutes = (int)(t % 60);

        timerText.text = string.Format("{0}:{1}", minutes.ToString("00"), seconds.ToString("00") );

    }
}*/



public class TimerCountUp : MonoBehaviour
{
    Text timerText;
    bool running = true;
    float startTime;
    float frozenTime; // čas, který zobrazíme po zastavení

    void Start()
    {
        timerText = GetComponentInChildren<Text>();
        startTime = Time.time;   // začneme od startu scény
        running = true;
    }

    void Update()
    {
        float t = running ? (Time.time - startTime) : frozenTime;

        int seconds = (int)(t % 60);
        int minutes = (int)(t / 60);

        if (timerText != null)
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StopTimer()
    {
        // ulož aktuální čas a zastav přepočet
        frozenTime = Time.time - startTime;
        running = false;
    }

    public void ResetAndStart()
    {
        startTime = Time.time;
        running = true;
    }
}
