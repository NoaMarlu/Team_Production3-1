using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour
{

    [System.Serializable]
    public struct StageSelectObj{
        public GameObject stageObj;
        public SpriteRenderer spriteRenderer;
        public GameObject[] star;
        public GameObject[] tutorialStar;
        public string sceneName;
        public string prefsName;//星の数を取得
        public bool hasStar;
        public bool hasTutorialStar;
        public GameObject SpeechBubble;
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

    public GameMovie gameMovie;

    public GameObject s1Sheep1;
    public GameObject s1Sheep2;
    public GameObject s1Sheep3;
    public GameObject s2Sheep1;
    public GameObject s2Sheep2;
    public GameObject s2Sheep3;
    public GameObject s3Sheep1;
    public GameObject s3Sheep2;
    public GameObject s3Sheep3;
    public GameObject s4Sheep1;
    public GameObject s4Sheep2;
    public GameObject s4Sheep3;

    void Start()
    {
        Init();
    }
    void Update()
    {
        SpriteChanger();
        ScrollStage();
        DispSheep();
        if (gameMovie.isAnimation) return;
        SelectStage();
        ChangeStar();
        LoadScene();
        ChangeCurrent();
    }
    void SelectStage()
    {
        //Lスティック入力が左向きなら
        if (Input.GetAxis("Horizontal") < -0.5f)
        {
            if (!isInput)
            {
                if (currentStage <= 0) currentStage = 0;
                else
                {
                    currentStage--;
                    audioSource.PlayOneShot(stageChange);
                }
                isInput = true;
            }
        }//Lスティック入力が右向きなら
        else if (Input.GetAxis("Horizontal") > 0.5f)
        {
            if (!isInput)
            {
                if (currentStage >= maxStage) currentStage = maxStage;
                else
                {
                    currentStage++;
                    audioSource.PlayOneShot(stageChange);
                }
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
            SS[i].tutorialStar = new GameObject[3];
            if (SS[i].hasStar)
            {
                SS[i].star[0] = SS[i].stageObj.transform.GetChild(0).gameObject;
                SS[i].star[1] = SS[i].stageObj.transform.GetChild(1).gameObject;
                SS[i].star[2] = SS[i].stageObj.transform.GetChild(2).gameObject;
            }
            if (SS[i].hasTutorialStar)
            {
                SS[i].tutorialStar[0] = SS[i].stageObj.transform.GetChild(0).gameObject;
                SS[i].tutorialStar[1] = SS[i].stageObj.transform.GetChild(1).gameObject;
                SS[i].tutorialStar[2] = SS[i].stageObj.transform.GetChild(2).gameObject;
            }
        }

        currentStage = PlayerPrefs.GetInt("CurrentStage");
        if (currentStage >= 3)
        {
            BG.transform.position = new Vector2(-17.92f, 0);
        }
        maxStage = SS.Length-1;
    }
    void SpriteChanger()
    {
        for(int i = 0; i < SS.Length; i++)
        {
            if (currentStage == i)//現在選択しているステージなら
            {
                SS[i].spriteRenderer.sprite = isSelectSpr;
                SS[i].SpeechBubble.SetActive(true);
            }
            else
            {
                SS[i].spriteRenderer.sprite = normalSpr;
                SS[i].SpeechBubble.SetActive(false);
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

        for (int i = 0; i < SS.Length; i++)
        {
            if (SS[i].hasTutorialStar)
            {
                if (PlayerPrefs.GetInt(SS[i].prefsName) == 1)
                {
                    SS[i].tutorialStar[0].GetComponent<SpriteRenderer>().sprite = onStar;
                    SS[i].tutorialStar[1].GetComponent<SpriteRenderer>().sprite = onStar;
                    SS[i].tutorialStar[2].GetComponent<SpriteRenderer>().sprite = onStar;
                }
                else
                {
                    SS[i].tutorialStar[0].GetComponent<SpriteRenderer>().sprite = offStar;
                    SS[i].tutorialStar[1].GetComponent<SpriteRenderer>().sprite = offStar;
                    SS[i].tutorialStar[2].GetComponent<SpriteRenderer>().sprite = offStar;
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
    void ChangeCurrent() { PlayerPrefs.SetInt("CurrentStage", currentStage); }
    //星の数で羊のイラストを追加
    void DispSheep()
    {

        //STAGE1
        if (PlayerPrefs.GetInt("Stage1") == 1) { s1Sheep1.SetActive(true); s1Sheep2.SetActive(true); s1Sheep3.SetActive(true); }
        else { s1Sheep1.SetActive(false); s1Sheep2.SetActive(false); s1Sheep3.SetActive(false); }

        //STAGE2

        switch (PlayerPrefs.GetInt("STAGE2_Star"))
        {
            case 0:
                s2Sheep1.SetActive(false);
                s2Sheep2.SetActive(false);
                s2Sheep3.SetActive(false);
                break;
            case 1:
                s2Sheep1.SetActive(true);
                s2Sheep2.SetActive(false);
                s2Sheep3.SetActive(false);
                break;
            case 2:
                s2Sheep1.SetActive(true);
                s2Sheep2.SetActive(true);
                s2Sheep3.SetActive(false);
                break;
            case 3:
                s2Sheep1.SetActive(true);
                s2Sheep2.SetActive(true);
                s2Sheep3.SetActive(true);
                break;
            default:
                Debug.Log("Starの数値が3を上回りました");
                break;
        }

        //STAGE3
        if (PlayerPrefs.GetInt("Stage3") == 1) { s3Sheep1.SetActive(true); s3Sheep2.SetActive(true); s3Sheep3.SetActive(true); }
        else { s3Sheep1.SetActive(false); s3Sheep2.SetActive(false); s3Sheep3.SetActive(false); }

        //STAGE4
        switch (PlayerPrefs.GetInt("STAGE4_Star"))
        {
            case 0:
                s4Sheep1.SetActive(false);
                s4Sheep2.SetActive(false);
                s4Sheep3.SetActive(false);
                break;
            case 1:
                s4Sheep1.SetActive(true);
                s4Sheep2.SetActive(false);
                s4Sheep3.SetActive(false);
                break;
            case 2:
                s4Sheep1.SetActive(true);
                s4Sheep2.SetActive(true);
                s4Sheep3.SetActive(false);
                break;
            case 3:
                s4Sheep1.SetActive(true);
                s4Sheep2.SetActive(true);
                s4Sheep3.SetActive(true);
                break;
            default:
                Debug.Log("Starの数値が3を上回りました");
                break;
        }

    }

}
