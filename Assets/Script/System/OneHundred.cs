using UnityEngine;
using UnityEngine.SceneManagement;

public class OneHundred : MonoBehaviour
{

    public SpriteRenderer gamingBG;
    public float spriteSpeed=0.1f;
    private float currentSprite=0;

    private AudioSource audioSource;
    public AudioClip audioClip;
    public AudioClip titleClip;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StopBGM();
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.Play();
    }
    void Update()
    {
        GamingBG();
        InputPlayer();
    }

    void GamingBG()
    {
        currentSprite += spriteSpeed * Time.deltaTime;
        if (currentSprite > 1f) currentSprite = 0;

        gamingBG.color = Color.HSVToRGB(currentSprite, 69f/100f, 93f/100f);

    }

    void InputPlayer()
    {
        if (Input.GetButtonDown("Submit") || Input.GetKeyDown(KeyCode.Space)|| Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            if (BGM.Instance != null)
            {
                BGM.Instance.PlayBGM();
            }
            SceneManager.LoadScene("Title");
        }
    }
    void StopBGM()
    {
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource source in allAudioSources)
        {
            if (source != audioSource) source.Stop();
        }
    }

}
