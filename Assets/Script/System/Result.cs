using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Result : MonoBehaviour
{

    public string prefsName;
    public SpriteRenderer[] star;
    public Sprite onStar;
    public Sprite offStar;

    public AudioClip select;
    private AudioSource audioSource;

    /*DispSocre*/
    public GameObject Text1;
    public GameObject Text2;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        LoadScene();
        StarChange();
    }
    
    //ÉVÅ[ÉìëJà⁄
    void LoadScene()
    {
        if(Input.GetButtonDown("Submit") || Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.PlayOneShot(select);
            SceneManager.LoadScene("StageSelect");
        }
    }
    void StarChange()
    {
        switch (PlayerPrefs.GetInt(prefsName))
        {
            case 0:
                star[0].GetComponent<SpriteRenderer>().sprite = offStar;
                star[1].GetComponent<SpriteRenderer>().sprite = offStar;
                star[2].GetComponent<SpriteRenderer>().sprite = offStar;
                break;
            case 1:
                star[0].GetComponent<SpriteRenderer>().sprite = onStar;
                star[1].GetComponent<SpriteRenderer>().sprite = offStar;
                star[2].GetComponent<SpriteRenderer>().sprite = offStar;
                break;
            case 2:
                star[0].GetComponent<SpriteRenderer>().sprite = onStar;
                star[1].GetComponent<SpriteRenderer>().sprite = onStar;
                star[2].GetComponent<SpriteRenderer>().sprite = offStar;
                break;
            case 3:
                star[0].GetComponent<SpriteRenderer>().sprite = onStar;
                star[1].GetComponent<SpriteRenderer>().sprite = onStar;
                star[2].GetComponent<SpriteRenderer>().sprite = onStar;
                break;
            default:
                Debug.Log("StarÇÃêîílÇ™3Çè„âÒÇËÇ‹ÇµÇΩ");
                break;
        }
    }
    void DispScore()
    {

    }

}
