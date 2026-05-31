using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Video;
public class GameMovie : MonoBehaviour
{
    public bool isAnimation = true;
    public RawImage RI;
    public VideoPlayer vp;
    /*アニメーション管理*/
    public bool isReplay = true;
    public bool isAlpha = false;
    public float animeTime;
    private float animeTimer=0;
    public float alphaSpeed;

    public AudioSource audioS;
    public bool wasFadeOut = false;
    public bool wasFadeIn = false;
    public float outTime=10.0f;
    public float inTime=11.0f;
    public float outDuration=4.0f;
    public float inDuration = 4.0f;

    void Start()
    {
        Debug.Log("BGM.Instance = " + BGM.Instance);
        vp = FindAnyObjectByType<VideoPlayer>();

        if (PlayerPrefs.GetInt("isAnimation") == 1)
        {
            isAnimation = false;
            Color c = RI.color;
            c.a = 0;
            RI.color = c;
            BGM.Instance?.PlayBGM();
            audioS.Stop();
            return;
        }
        if(vp!=null){
            vp.Play();
            BGM.Instance?.StopBGM();
        }
        else
        {
            isAnimation = false;
            wasFadeOut = false;
        }

    }
    void Update()
    {

        if (!isAnimation) return;
        AnimeReplay();
        AnimeAlpha();
    }
    void AnimeReplay()
    {
        if (!isReplay) return;
        animeTimer += Time.deltaTime;

        if (animeTimer >= outTime && !wasFadeOut)
        {
            StartCoroutine(FadeOut(0, outDuration));
            wasFadeOut = true;
        }
        if (animeTimer >= inTime && !wasFadeIn)
        {
            BGM.Instance?.FadeInVolume(inDuration);
            wasFadeIn = true;
        }

        if (animeTimer >= animeTime)
        {
            isReplay = false;
            isAlpha = true;
        }
    }
    void AnimeAlpha()
    {
        if (!isAlpha) return;
        Color c = RI.color;
        if (c.a <= 0)
        {
            PlayerPrefs.SetInt("isAnimation", 1);
            PlayerPrefs.Save();
            Destroy(vp.gameObject);
            isAlpha = false;
            isAnimation = false;
        }
        c.a -= alphaSpeed * Time.deltaTime;
        RI.color = c;
    }

    public System.Collections.IEnumerator FadeOut(float target, float duration)
    {
        Debug.Log("FadeRoutine開始");
        float start = audioS.volume;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioS.volume = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }
        audioS.volume = target;
        Debug.Log("FadeRoutine完了 volume=" + audioS.volume);
    }
}