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
    public SpriteRenderer speechBubble;//吹き出しアセットを用意してUnity上でアタッチしてください
    private float UItime = 1;

    /*isHit*/
    public List<GameObject> sheeps=new List<GameObject>();
    private float r = 2.6f;

    /*MaxTime*/
    private GameManager manager;
    public float maxTime= float.MaxValue;

    void Start()
    {

        GameObject obj = GameObject.Find("GameManager");
        manager = obj.GetComponent<GameManager>();

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

            return true;//Spawn成功
        }
        return false;

    }
    //吹き出し表示
    IEnumerator showUI()
    {
        speechBubble.enabled = true;
        yield return new WaitForSecondsRealtime(UItime);
        speechBubble.enabled = false;
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

    void MaxTime()
    {
        foreach(GameObject sheep in sheeps)
        {
            //全羊から死亡時間を取得
            PlayerScript player=sheep.GetComponent<PlayerScript>();
            if (maxTime < player.DieTime)
            {
                if(player.DieTime!=0)maxTime = player.DieTime;
            }
        }
    }

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