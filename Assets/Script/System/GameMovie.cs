using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
public class GameMovie : MonoBehaviour
{
    public bool isAnimation = true;
    public RawImage RI;
    /*アニメーション管理*/
    public bool isReplay = true;
    public bool isAlpha = false;
    public float animeTime;
    private float animeTimer=0;
    public float alphaSpeed;
    void Start()
    {

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
            isAlpha = false;
            isAnimation = false;
        }
        c.a -= alphaSpeed * Time.deltaTime;
        RI.color = c;
    }
}