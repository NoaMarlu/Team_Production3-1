using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

/// </summary>
public class SheepSpawner : MonoBehaviour
{

    /*Spawn*/
    public GameObject sheepPrefab;
    public int sheepCount = 0;
    private GaugeScript gaugeScript;

    /*UI*/
    public GameObject speechBubble1;//吹き出しアセットを用意してUnity上でアタッチしてください
    public GameObject speechBubble2;//10桁目
    private float UItime = 2;

    /*isHit*/
    public List<GameObject> sheeps=new List<GameObject>();

    /*MaxTime*/
    private GameManager manager;
    public float maxTime= 0;

    /*レディGO!アニメーション*/
    public bool isStartAnime;
    private bool isStart = true;

    /*プレイヤー変数管理*/
    public float pScale=1;
    public float mountOffset=1.0f;
    public float jumpPower = 7.0f;
    public float moveSpeed = 3.0f;
    public bool pMoveControl = false;
    public float[] pControlValue;//0がX左、1がX右

    /*レイヤー変更管理*/
    private int layerNum = 1;

    /*最初はスポーンしない*/
    public bool farstSpawn = true;
    private bool spawnOnce = false;

    /*MaxTime*/
    private float maxLiveTime=0;

    /*StarRecord*/
    public bool hasStar=false;
    public string prefsName;
    public string prefsNameSheepCount;
    public int[] scoreSheep;

    /*看板*/
    public GameObject symbolON;
    public GameObject symbolOFF;

    void Start()
    {
        Init();
    }
    void Update()
    {

        if (sheepCount >= 50)
        {
            SceneManager.LoadScene("OneHundredStage");
        }

        isStartAnime = manager.isStartAnimation;
        if (isStartAnime) return;

        if (farstSpawn != true)
        {

            if (Input.GetAxis("LT") > 0.5f || Input.GetKeyDown(KeyCode.Z))
            {
                if (spawnOnce) return;
                InstansPlayer();
                sheepCount = 1;
                StartCoroutine(showUI());
                spawnOnce =true;
            }

        }



        StartFunc();
        MaxTime();
        manager.SetGameTime(maxTime);


        if (symbolOFF != null)
        {
            if (isNotDieSheep())
            {
                symbolOFF.SetActive(true);
                symbolON.SetActive(false);
            }
            else
            {
                symbolOFF.SetActive(false);
                symbolON.SetActive(true);
            }
        }

    }

    //スポーン処理
    public bool Spawn()
    {

        if (isHit())
        {

            InstansPlayer();
            sheepCount++;
            StartCoroutine(showUI());

            return true;//Spawn成功
        }
        return false;
        
    }
    //吹き出し表示
    IEnumerator showUI()
    {
        if(sheepCount<10){

            if (speechBubble1 != null) speechBubble1.SetActive(true);
        yield return new WaitForSecondsRealtime(UItime);
            if (speechBubble1 != null) speechBubble1.SetActive(false);
        }
        else
        {
            if (speechBubble2 != null) { speechBubble2.SetActive(true); }
            
            yield return new WaitForSecondsRealtime(UItime);
            if (speechBubble2 != null) { speechBubble2.SetActive(false); }
        }
    }
    //スポナーの半径r内にPlayerがいるか判定
    public bool  isHit()
    {
        //foreach (GameObject sheep in sheeps)
        //{
        //    Vector2 v = transform.position - sheep.transform.position;
        //    if (v.SqrMagnitude() < r * r) return false;//半径内にいる
        //}
        return true;
    }
    //羊のMaxTimeを取得
    void MaxTime()
    {
        foreach(GameObject sheep in sheeps)
        {

            //全羊から死亡時間を取得
            PlayerScript player=sheep.GetComponent<PlayerScript>();
            if (player == null) continue;
            if ( player.DieTime> maxTime && player.DieTime != 0)
            {
                maxTime = player.DieTime;
            }

            //全羊から最大生存時間を取得
            if (maxLiveTime <= player.GetLiveTimer())
            {
                maxLiveTime =player.GetLiveTimer();
            }

        }
    }
    //現状で死んでいない羊がいるかどうか
    public bool isNotDieSheep()
    {
        foreach (GameObject sheep in sheeps)
        {
            PlayerScript player = sheep.GetComponent<PlayerScript>();
            if (player == null) continue;
            if (player.isDie==false) return true;
        }
        return false;
    }
    public void IsLoopSpawn()
    {
        foreach (GameObject sheep in sheeps)
        {
            PlayerScript player = sheep.GetComponent<PlayerScript>();
            if (player == null) continue;
            player.isloopSpawn = false;
        }
    }
    //初期化
    void Init()
    {
        symbolON = GameObject.Find("symbol_ON");
        symbolOFF = GameObject.Find("symbol_OFF");
        GameObject obj = GameObject.Find("GameManager");
        manager = obj.GetComponent<GameManager>();
        GameObject gaugeObj = GameObject.FindWithTag("gauge");
        if(gaugeObj!=null)gaugeScript =gaugeObj.GetComponent<GaugeScript>();

        /*showUI*/
        speechBubble1 = GameObject.Find("speechBubble1");
        speechBubble2 = GameObject.Find("speechBubble2");
        if (speechBubble1 == null) Debug.Log("speechBubble1はnullです");
        if (speechBubble2 == null) Debug.Log("speechBubble2はnullです");
        if(speechBubble1!=null)speechBubble1.SetActive(false);
        if(speechBubble2!=null)speechBubble2.SetActive(false);
        isStartAnime = manager.isStartAnimation;
    }
    //スタートアニメーション終了後に一回呼び出す処理
    void StartFunc()
    {
        if (!isStart) return;

        //最初に一体生成
        if (farstSpawn) {
            Debug.Log("初回生成");
           InstansPlayer();
           StartCoroutine(showUI());
        }

        isStart = false;
    }
    //プレイヤー生成関連
    void InstansPlayer()
    {
        //生成
        GameObject newSheep = Instantiate(sheepPrefab, transform.position, transform.rotation);
        newSheep.transform.localScale = new Vector3(pScale, pScale, pScale);
        PlayerScript script = newSheep.GetComponent<PlayerScript>();
        SpriteRenderer sprite = newSheep.GetComponent<SpriteRenderer>();

        //付け焼き刃
        sprite.sortingOrder += layerNum;
        layerNum++;

        //list追加
        sheeps.Add(newSheep);

        //ゲージ管理
        if (gaugeScript!=null)gaugeScript.DrawIcon();

        //変数調整
        script.JumpForceChanger(jumpPower);
        script.MoveSpeedChanger(moveSpeed);
        script.MountOffsetChanger(mountOffset);
        if (pMoveControl) script.MoveContorolChanger(pControlValue);

    }
    public GameObject GetSheepList(int num) { return sheeps[num]; }
    public float GetLiveTimer() { return maxLiveTime; }
    public void StarRecord()
    {
        if (!hasStar) return;
        PlayerPrefs.SetInt(prefsNameSheepCount,sheepCount);
        if (sheepCount <= scoreSheep[0])
        {
            if (sheepCount <= scoreSheep[1])
            {
                if (sheepCount <= scoreSheep[2])
                {
                    PlayerPrefs.SetInt(prefsName, 3);
                    PlayerPrefs.Save();
                    return;
                }
                PlayerPrefs.SetInt(prefsName, 2);
                PlayerPrefs.Save();
                return;
            }
            PlayerPrefs.SetInt(prefsName, 1);
            PlayerPrefs.Save();
            return;
        }
        PlayerPrefs.SetInt(prefsName, 0);
        PlayerPrefs.Save();
        return;

    }
    //プレイヤーが死亡するのが途中の場合
    public void SheepReset()
    {
        foreach (GameObject sheep in sheeps)
        {
            PlayerScript player= sheep.GetComponent<PlayerScript>();
            if (player == null) continue;
            player.loopDie = true;
            player.isloopSpawn = false;
            player.num = 0;
            Debug.Log("ループしたよん");
            sheep.transform.position = new Vector2(300, 300);
            Rigidbody2D rb=sheep.GetComponent<Rigidbody2D>();
            if(rb!=null)rb.linearVelocity=Vector2.zero;
        }
    }

}