using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutsceneHelper : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public float timeToSkip = 3f;
    public SceneReference sceneToLoad;
    public Image skipImage;
    private float remainingTime;
    private double currentTimeVideo;
    private double videoLength;

    void Start()
    {
        remainingTime = timeToSkip;
        videoLength = videoPlayer.clip.length;
        videoPlayer.loopPointReached += EndReached;
        SoundManager.Instance.StopAll();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            remainingTime -= Time.deltaTime;
            skipImage.fillAmount += Time.deltaTime / timeToSkip;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            remainingTime = timeToSkip;
            skipImage.fillAmount = 0;
        }

        if (remainingTime <= 0.0f)
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
