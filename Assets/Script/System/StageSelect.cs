using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour
{

    [System.Serializable]
    public struct StageSelectObj{
        public GameObject stageObj;
        public SpriteRenderer spriteRenderer;
        public GameObject[] star;
        public string sceneName;
        public string prefsName;//星の数を取得
        public bool hasStar;
     }
    public StageSelectObj[] SS;

    /*SelectStage*/
    private int currentStage;
    private int maxStage;

    /*SpriteChanger*/
    public Sprite isSelectSpr;
    public Sprite normalSpr;

    /*SelectStage*/
    private bool isInput=false;
    public AudioClip stageChange;
    private AudioSource audioSource;

    /*ChangeStar*/
    public Sprite offStar;
    public Sprite onStar;

    /*LoadScene*/
    public AudioClip select;

    /*ScrollStage*/
    public GameObject BG;
    public float LerpSpeed = 4.0f;
    public SpriteRenderer right;
    public SpriteRenderer left;

    void Start()
    {
        Init();
    }
    void Update()
    {
        SpriteChanger();
        SelectStage();
        ChangeStar();
        LoadScene();
        ScrollStage();
    }
    void SelectStage()
    {
        //Lスティック入力が左向きなら
        if (Input.GetAxis("Horizontal") < -0.5f)
        {
            if (!isInput)
            {
                if (currentStage <= 0) currentStage = 0;
                else currentStage--;
                isInput = true;
            }
        }//Lスティック入力が右向きなら
        else if (Input.GetAxis("Horizontal") > 0.5f)
        {
            if (!isInput)
            {
                if (currentStage >= maxStage) currentStage = maxStage;
                else currentStage++;
                isInput = true;
            }
        }//入力されていないなら
        else isInput = false;
    }
    void Init()
    {
        audioSource = GetComponent<AudioSource>();
        //StageSelectObj
        for(int i = 0; i < SS.Length; i++)
        {
            SS[i].spriteRenderer=SS[i].stageObj.GetComponent<SpriteRenderer>();
            //星取得
            SS[i].star = new GameObject[3];
            if (SS[i].hasStar)
            {
                SS[i].star[0] = SS[i].stageObj.transform.GetChild(0).gameObject;
                SS[i].star[1] = SS[i].stageObj.transform.GetChild(1).gameObject;
                SS[i].star[2] = SS[i].stageObj.transform.GetChild(2).gameObject;
            }
        }

        currentStage = 0;
        maxStage = SS.Length-1;
    }
    void SpriteChanger()
    {
        for(int i = 0; i < SS.Length; i++)
        {
            if (currentStage == i)//現在選択しているステージなら
            {
                SS[i].spriteRenderer.sprite = isSelectSpr;
            }
            else
            {
                SS[i].spriteRenderer.sprite = normalSpr;
            }
        }
    }
    void ChangeStar()
    {
        for(int i = 0; i < SS.Length; i++)
        {
            if (SS[i].hasStar)
            {

                switch (PlayerPrefs.GetInt(SS[i].prefsName))
                {
                    case 0:
                        SS[i].star[0].GetComponent<SpriteRenderer>().sprite = offStar;
                        SS[i].star[1].GetComponent<SpriteRenderer>().sprite = offStar;
                        SS[i].star[2].GetComponent<SpriteRenderer>().sprite = offStar;
                        break;
                    case 1:
                        SS[i].star[0].GetComponent<SpriteRenderer>().sprite = onStar;
                        SS[i].star[1].GetComponent<SpriteRenderer>().sprite = offStar;
                        SS[i].star[2].GetComponent<SpriteRenderer>().sprite = offStar;
                        break;
                    case 2:
                        SS[i].star[0].GetComponent<SpriteRenderer>().sprite = onStar;
                        SS[i].star[1].GetComponent<SpriteRenderer>().sprite = onStar;
                        SS[i].star[2].GetComponent<SpriteRenderer>().sprite = offStar;
                        break;
                    case 3:
                        SS[i].star[0].GetComponent<SpriteRenderer>().sprite = onStar;
                        SS[i].star[1].GetComponent<SpriteRenderer>().sprite = onStar;
                        SS[i].star[2].GetComponent<SpriteRenderer>().sprite = onStar;
                        break;
                    default:
                        Debug.Log("Starの数値が3を上回りました");
                        break;
                }


            }
        }
    }
    void LoadScene()
    {
        if(Input.GetButtonDown("Submit") || Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.PlayOneShot(select);
            SceneManager.LoadScene(SS[currentStage].sceneName);
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            audioSource.PlayOneShot(select);
            SceneManager.LoadScene("TITLE");
        }
    }
    //currentStageが3以上でスクロール
    void ScrollStage()
    {
        if (currentStage >= 3)
        {
            BG.transform.position = Vector2.Lerp(BG.transform.position, new Vector2(-17.92f, 0), LerpSpeed);
            left.enabled = true;
            right.enabled = false;
        }
        else
        {
            BG.transform.position = Vector2.Lerp(BG.transform.position, new Vector2(0, 0),LerpSpeed);
            left.enabled = false;
            right.enabled = true;
        }
    }

}
