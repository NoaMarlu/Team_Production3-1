using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

/// </summary>
public class SheepSpawner : MonoBehaviour
{

    /*Spawn*/
    public GameObject sheepPrefab;
    public int sheepCount = 0;

    /*UI*/
    public GameObject speechBubble1;//吹き出しアセットを用意してUnity上でアタッチしてください
    public GameObject speechBubble2;//10桁目
    private float UItime = 2;

    /*isHit*/
    public List<GameObject> sheeps=new List<GameObject>();
    private float r = 2.6f;

    /*MaxTime*/
    private GameManager manager;
    public float maxTime= 0;

    /*レディGO!アニメーション*/
    public bool isStartAnime;
    private bool isStart = true;

    /*スケール管理*/
    public float pScale=1;
    public float mountOffset=1.0f;
    public float jumpPower = 7.0f;
    public float moveSpeed = 3.0f;

    /*レイヤー変更管理*/
    private int layerNum = 1;

    void Start()
    {
        Init();
    }
    void Update()
    {

        isStartAnime = manager.isStartAnimation;
        if (isStartAnime) return;

        StartFunc();
        MaxTime();
        manager.SetGameTime(maxTime);

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
        GameObject obj = GameObject.Find("GameManager");
        manager = obj.GetComponent<GameManager>();

        /*showUI*/
        speechBubble1 = GameObject.Find("speechBubble1");
        speechBubble2 = GameObject.Find("speechBubble2");
        if (speechBubble1 == null) Debug.Log("speechBubble1はnullです");
        if (speechBubble2 == null) Debug.Log("speechBubble2はnullです");
        speechBubble1.SetActive(false);
        speechBubble2.SetActive(false);
        isStartAnime = manager.isStartAnimation;
    }
    //スタートアニメーション終了後に一回呼び出す処理
    void StartFunc()
    {
        if (!isStart) return;
        //最初に一体生成
        InstansPlayer();
        StartCoroutine(showUI());
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

        //変数調整
        script.JumpForceChanger(jumpPower);
        script.MoveSpeedChanger(moveSpeed);
        script.MountOffsetChanger(mountOffset);

        //list追加
        sheeps.Add(newSheep);
    }

}