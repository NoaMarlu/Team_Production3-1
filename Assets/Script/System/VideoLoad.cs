using UnityEngine;
using UnityEngine.Video;

public class VideoLoad : MonoBehaviour
{

    public VideoPlayer videoPlayer;

    void Awake()
    {
        DontDestroyOnLoad(videoPlayer.gameObject);
        videoPlayer.Prepare();
        videoPlayer.Pause();
    }
    void Update()
    {
        
    }
}
