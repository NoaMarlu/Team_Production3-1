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
    public SpriteRenderer Text1;
    public SpriteRenderer Text2_1;//1Œ…–Ú
    public SpriteRenderer Text2_2;//2Œ…–Ú
    public string prefsNameCount;
    public int sheepCount;
    public Sprite[] num;

    /*Start‚ÌSE*/
    public AudioClip resultSE;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sheepCount=PlayerPrefs.GetInt(prefsNameCount);
        audioSource.PlayOneShot(resultSE);
    }
    void Update()
    {
        DispScore();
        LoadScene();
        StarChange();
    }
    
    //ƒV[ƒ“‘JˆÚ
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
                Debug.Log("Star‚Ì”’l‚ª3‚ğã‰ñ‚è‚Ü‚µ‚½");
                break;
        }
    }
    void DispScore()
    {
        //1Œ…–Ú
        if (sheepCount < 10)
        {
            Text1.enabled = true;
            Text2_1.enabled = false;
            Text2_2.enabled = false;

            Text1.sprite = NumSwitch(sheepCount);
        }
        //2Œ…–Ú
        if (sheepCount >= 10)
        {
            Text1.enabled = false;
            Text2_1.enabled = true;
            Text2_2.enabled = true;

            Text2_2.sprite = NumSwitch((sheepCount/10)%10);
            Text2_1.sprite = NumSwitch(sheepCount%10);
        }
    }
   Sprite NumSwitch(int x)
    {
        switch (x) {
            case 0:
                return num[0];
            case 1:
                return num[1];
            case 2:
                return num[2];
            case 3:
                return num[3];
            case 4:
                return num[4];
            case 5:
                return num[5];
            case 6:
                return num[6];
            case 7:
                return num[7];
            case 8:
                return num[8];
            case 9:
                return num[9];
        }
        return null;
    }

}
