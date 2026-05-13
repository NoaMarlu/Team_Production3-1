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

    void Start()
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

        //最初に一体生成
        sheeps.Add(Instantiate(sheepPrefab, transform.position, transform.rotation));
        sheepCount++;
        StartCoroutine(showUI());

    }
    void Update()
    {
        MaxTime();
        manager.SetGameTime(maxTime);
    }

    //スポーン処理
    public bool Spawn()
    {

        if (isHit())
        {

            sheeps.Add(Instantiate(sheepPrefab, transform.position, transform.rotation));
            sheepCount++;
            StartCoroutine(showUI());
            //StartCoroutine(showUI());

            return true;//Spawn成功
        }
        return false;
        
    }
    //吹き出し表示
    IEnumerator showUI()
    {
        if(sheepCount<10){
        speechBubble1.SetActive(true);
        yield return new WaitForSecondsRealtime(UItime);
            speechBubble1.SetActive(false);
        }
        else
        {
            speechBubble2.SetActive(true);
            yield return new WaitForSecondsRealtime(UItime);
            speechBubble2.SetActive(false);
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
            if (player.isDie==false) return true;
        }
        return false;
    }

    public void IsLoopSpawn()
    {
        foreach (GameObject sheep in sheeps)
        {
            PlayerScript player = sheep.GetComponent<PlayerScript>();
            player.isloopSpawn = false;
        }
    }

}