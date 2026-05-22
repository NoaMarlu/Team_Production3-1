using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GaugeScript : MonoBehaviour
{

    private GameManager manager;
    private SheepSpawner sheepSpawner;
    private float maxTime=0;

    /*DrawIcon*/
    public GameObject icon;
    public List<GameObject> iconList = new List<GameObject>();

    /*横幅の取得*/
    private float gaugeX1,gaugeX2;
    private float gaugeLength;

    void Start()
    {
        Init();
    }
    void Update()
    {
        MaxTimeChanger();
        ChangePosition();
    }
    void Init()
    {
        GameObject obj = GameObject.Find("GameManager");
        manager = obj.GetComponent<GameManager>();
        sheepSpawner = GameObject.FindWithTag("Spawner").GetComponent<SheepSpawner>();
        //横幅の取得
        gaugeX1 = GetComponent<SpriteRenderer>().bounds.min.x;
        gaugeX2 = GetComponent<SpriteRenderer>().bounds.max.x;
        gaugeLength = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    //ゲームの最大タイムを計測
    void MaxTimeChanger(){  maxTime = manager.GetGameTime();}
    public void DrawIcon()
    {
        GameObject newIcon = Instantiate(icon,new Vector2(gaugeX1,transform.position.y),transform.rotation);
        iconList.Add(newIcon);
    }
    float GetTransform(int num)
    {
        GameObject sObj = sheepSpawner.GetSheepList(num);
        PlayerScript ps = sObj.GetComponent<PlayerScript>();
        float timeRatio =ps.timeList[0]/maxTime;
        float posX = gaugeX1+(gaugeLength * timeRatio);
        return posX;
    }
    void ChangePosition()
    {
        foreach (GameObject obj in iconList) {

        int num = iconList.IndexOf(obj);//配列番号の取得
        //iconの位置処理
        if (maxTime != 0)
        {
            obj.transform.position = new Vector2(GetTransform(num), transform.position.y);
        }
        
        }
    }

}
