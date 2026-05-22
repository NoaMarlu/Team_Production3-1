using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeScript : MonoBehaviour
{

    private GameManager manager;
    private SheepSpawner sheepSpawner;
    private float maxTime=0;

    /*DrawIcon*/
    public GameObject icon;
    public List<GameObject> iconList = new List<GameObject>();
    private float upValue = 0.3f;
    private float sideValue = 0.5f;
    public SpriteRenderer dieIcon;

    /*横幅の取得*/
    private float gaugeX1,gaugeX2;
    private float gaugeLength;

    /*Arrow*/
    public GameObject arrow;
    public GameObject arrowInstance;
    public float arrowPosY=0.5f;

    void Start()
    {
        Init();
    }
    void Update()
    {
        IconSprite();
        MaxTimeChanger();
        ChangePosition();
        ArrowFunc();
    }
    void Init()
    {
        GameObject obj = GameObject.Find("GameManager");
        manager = obj.GetComponent<GameManager>();
        sheepSpawner = GameObject.FindWithTag("Spawner").GetComponent<SheepSpawner>();
        //横幅の取得
        gaugeX1 = GetComponent<SpriteRenderer>().bounds.min.x + sideValue;
        gaugeX2 = GetComponent<SpriteRenderer>().bounds.max.x - sideValue;
        gaugeLength = GetComponent<SpriteRenderer>().bounds.size.x;
        //Arrow取得
        arrowInstance = Instantiate(arrow, new Vector2(gaugeX1, transform.position.y + arrowPosY), transform.rotation); ;
    }
    //ゲームの最大タイムを計測
    void MaxTimeChanger(){  maxTime = sheepSpawner.GetLiveTimer();}
    //Iconの描画（SheepSpawnerで呼び出し）
    public void DrawIcon()
    {
        GameObject newIcon = Instantiate(icon,new Vector2(gaugeX1,transform.position.y+upValue),transform.rotation);
        iconList.Add(newIcon);
    }
    //Iconの位置を計算
    float GetTransform(int num)
    {
        if (num >= sheepSpawner.sheeps.Count) return gaugeX1;
        GameObject sObj = sheepSpawner.GetSheepList(num);
        PlayerScript ps = sObj.GetComponent<PlayerScript>();

        if (ps.timeList == null || ps.timeList.Count == 0) return gaugeX1;
        float timeRatio =ps.timeList[0]/maxTime;
        float posX = gaugeX1+(gaugeLength * timeRatio);
        return posX;
    }
    //Iconの位置を変更
    void ChangePosition()
    {
        foreach (GameObject obj in iconList) {

        int num = iconList.IndexOf(obj);//配列番号の取得
        //iconの位置処理
        if (maxTime != 0)
        {
            obj.transform.position = new Vector2(GetTransform(num), transform.position.y+ upValue);
        }

            //位置制御
            if (obj.transform.position.x < gaugeX1) obj.transform.position = new Vector2(gaugeX1, transform.position.y+upValue) ;
            if (obj.transform.position.x > gaugeX2) obj.transform.position = new Vector2(gaugeX2, transform.position.y+upValue) ;
        
        }
    }
    //Arrow表示関連
    void ArrowFunc()
    {
        if (maxTime == 0) return;
        if (manager.GetGameTimer() == maxTime) arrowInstance.SetActive(false);
        else arrowInstance.SetActive(true);
        float ratio = manager.GetGameTimer() / maxTime;
        float arrowPosX= gaugeX1 + (gaugeLength * ratio);
        arrowInstance.transform.position = new Vector2(arrowPosX, transform.position.y+arrowPosY);

        if (arrowInstance.transform.position.x < gaugeX1) arrowInstance.transform.position = new Vector2(gaugeX1, transform.position.y+ arrowPosY);
        if (arrowInstance.transform.position.x > gaugeX2) arrowInstance.transform.position = new Vector2(gaugeX2, transform.position.y+ arrowPosY);


    }
    void IconSprite()
    {
        foreach(GameObject obj in iconList)
        {
            int num = iconList.IndexOf(obj);
            if (sheepSpawner.GetSheepList(num).GetComponent<PlayerScript>().isDie)
            {
                SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
                sprite = dieIcon;
            }
        }
    }

}
