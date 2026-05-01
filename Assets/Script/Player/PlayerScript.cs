using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class PlayerScript : MonoBehaviour
{

    /*Move*/
    private Rigidbody2D rb;
    public float moveSpeed = 5.0f;//移動スピード

    /*ChangeDirection*/
    private bool isDirection;//方向判定 false=左移動,true=右移動

    /*Jump*/
    public float jumpForce = 5.0f;
    private bool isGrounded;//地面に触れているか

    /*SheepIsDie*/
    private SheepSpawner sheepSpawner;
    public bool isDie = false;//trueで一度死亡している判定

    /*AddPos*/
    public List<float> timeList = new List<float>();
    public List<int> actionList = new List<int>();//0をジャンプ、1を方向転換、2を生成タイミング

    /*Spawn*/
    public bool isSpawn = false;

    /*RemindAction*/
    public bool isRemind = false;
    public int num = 0;

    /*Start*/
    private Vector2 startPos;

    /*プロトタイプ*/
    private SpriteRenderer spr;

    /*ループ関連*/
    public float DieTime;//死亡した時間
    private GameManager manager;
    public bool loopDie = false;
    public float GameTimerDayo;
    public float SpawnTiming;
    public bool isloopSpawn=false;
    private GameObject E_Spawn;//煙エフェクト

    /*ChangeSprite*/
    private Sprite[] S_Standby;
    private Sprite[] S_Standby_Death;
    private Sprite[] S_Jump;
    private Sprite[] S_Jump_Death;
    private float Velocity;
    private float standbyTime=0.1f;
    private float standbyTimer = 0;
    private bool isStandby = false;
    float x = 5.8f;

    /*StartAnimation*/
    private bool isAnimation = true;
    private Sprite S_Dash;
    public float startDistance = 1.0f;//小屋からスポーン距離までの長さ

    /*OnTrigger*/
    List<PlayerScript> triggerPlayer=new List<PlayerScript>();


    void Start()
    {

        GameObject obj = GameObject.Find("GameManager");
        manager = obj.GetComponent<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        sheepSpawner = GameObject.FindWithTag("Spawner").GetComponent<SheepSpawner>();
        spr = GetComponent<SpriteRenderer>();
        E_Spawn = Resources.Load<GameObject>("E_Spawn");

        //StartAnimation
        spr.sprite = S_Dash;

        startPos = transform.position;
        AddList(2);
        SheepIsLive();

        SetSprite();

        spr.sprite= S_Dash;
        //Debug
        SpawnTiming = manager.GetGameTimer();

    }
    void Update()
    {

        /*
        ///StartAnimation///
        
        transform.position = new Vector2(transform.position.y+0.1f, transform.position.y);
        if (transform.position.x >= startPos.x + startDistance)
        {
            SheepIsLive();
            isAnimation = false;
        }

        if (isAnimation) return;
        //////////////////////
        ///*/
        GameTimerDayo = manager.GetGameTimer();

        ChangeSprite();
        FouceSendLayer();

        if (isRemind)
        {
            if (isGrounded)
            { 
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
            SheepIsDie();
            RemindAction();
        }
        else
        {

            SheepIsDie();

            if(isGrounded)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }

            //死亡済みなら操作を取りやめる
            if (isDie) return;

            if (Input.GetKeyDown(KeyCode.JoystickButton2)||Input.GetKeyDown(KeyCode.X)) { ChangeDirection(); }
            if (Input.GetButtonDown("Submit")||Input.GetKeyDown(KeyCode.Space))
            {
                if(isGrounded)Jump();
            }

        }

        if (isSpawn == false)
        {
            Spawn();
        }

        //Prototype
        Velocity = rb.linearVelocityY;

    }
    //Xでの方向転換処理
    void ChangeDirection()
    {
        AddList(1);
        isDirection = !isDirection;
        spr.flipX = !spr.flipX;

    }
    //死亡判定
    void SheepIsLive()
    {
        if (isloopSpawn) return;
            isloopSpawn = true;
            loopDie = false;
            rb.linearVelocity = Vector2.zero;//落下の力をリセット
            spr.flipX = false;
            isDirection = true;
            transform.position = startPos;//位置
        if (isDie)
        {
            Instantiate(E_Spawn, transform.position, transform.rotation);
        }
    }
    void SheepIsDie()
    {
        if (this.transform.position.y <= -5.47f)
        {
            gameObject.tag = "ground";
            loopDie = true;
            if(DieTime==0)DieTime = manager.GetGameTimer();
            if (isRemind == false|| isDie == false) { 
            isDie = true;
            isRemind = true;
            }
        }
    }
    //自動ジャンプ処理
    void Jump()
    {
        AddList(0);
        //現在の横移動速度を維持しつつ、縦方向の速度を上書きする
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);


        //移動
       if (isDirection)//右向き
       {
           rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
       }
       else
       {
           rb.linearVelocity = new Vector2(-1 * moveSpeed, rb.linearVelocity.y);
       }


        //ジャンプした瞬間に接地判定をオフにする（二段ジャンプ防止）
        isGrounded = false;

    }
    //プレイヤーの位置を記録
    void AddList(int num) //0をジャンプ、1を方向転換とする
    {
        if (isRemind) return;
        timeList.Add(manager.GetGameTimer());
        actionList.Add(num);
    }
    //記録された位置を再生する
    void RemindAction()
    {
        if (gameObject.layer == 3) gameObject.layer = 6;

        if (loopDie && manager.GetGameTimer() >= timeList[0])
        {
            if(sheepSpawner.isNotDieSheep()==false){
                SheepIsLive();
                num = 1;
                return;
            }
        }

        
        if (num >= timeList.Count) return;
            if (manager.GetGameTimer() >= timeList[num])
        {
            switch (actionList[num])
            {
                case 0:
                    Jump();
                    num++;
                    break;
                case 1:
                    ChangeDirection();
                    num++;
                    break;
            }
        }

    }
    //LBでスポーン
    void Spawn()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton4)||Input.GetKeyDown(KeyCode.Z))
        {
            if (isDie == false) return;
            gameObject.tag = "ground";
            isSpawn = sheepSpawner.Spawn();
        }
    }
    //Spriteの変更
    void ChangeSprite()
    {

        if (rb.linearVelocityY == 0)//待機
        {
            standbyTimer += Time.deltaTime;
            if (standbyTimer > standbyTime)
            {
                isStandby = !isStandby;
                standbyTimer = 0 ;
            }
            if (isStandby)
            {
                if (isDie == false) spr.sprite = S_Standby[0];
                else { spr.sprite = S_Standby_Death[0]; }
            }
            else {
                if (isDie == false) spr.sprite = S_Standby[1];
                else { spr.sprite = S_Standby_Death[1]; }
            }
        } 
        else if (rb.linearVelocityY>0 && rb.linearVelocityY < x)//ジャンプ
        {
            if (isDie == false) spr.sprite = S_Jump[2];
            else { spr.sprite = S_Jump_Death[2]; }
        }
        else if (rb.linearVelocityY > 0 && rb.linearVelocityY >= x)
        {
            if (isDie == false) spr.sprite = S_Jump[1];
            else { spr.sprite = S_Jump_Death[1]; }
        }
        else if (rb.linearVelocityY < 0&&rb.linearVelocityY>-x)//滞空
        {
            if (isDie == false) spr.sprite = S_Jump[3];
            else { spr.sprite = S_Jump_Death[3]; }
        }
        else if (rb.linearVelocityY < 0 && rb.linearVelocityY <= -x)//滞空
        {
            if (isDie == false) spr.sprite = S_Jump[4];
            else { spr.sprite = S_Jump_Death[4]; }
        }

    }
    //Spriteの登録
    void SetSprite()
    {

        S_Standby = Resources.LoadAll<Sprite>("S_Standby");
        S_Standby_Death= Resources.LoadAll<Sprite>("S_Standby_Death");
        S_Jump = Resources.LoadAll<Sprite>("S_Jump");
        S_Jump_Death = Resources.LoadAll<Sprite>("S_Jump_Death");
        S_Dash= Resources.Load<Sprite>("S_Dash");

    }
    //クリア処理
    void Goal()
    {

    }
    //当たり判定処理
    void FouceSendLayer()
    {
        //死亡しているなら
        if (isDie)
        {
            Collider2D collider = GetComponent<BoxCollider2D>();
            collider.forceSendLayers |= (1 << LayerMask.NameToLayer("PlayerDie"));
            collider.forceSendLayers |= (1 << LayerMask.NameToLayer("Player"));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 地面との接触判定
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        // 地面との接触判定
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (LayerMask.LayerToName(collider.gameObject.layer)=="Player"|| LayerMask.LayerToName(collider.gameObject.layer) == "PlayerDie")
        {  
             triggerPlayer.Add(collider.gameObject.GetComponent<PlayerScript>());
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (LayerMask.LayerToName(collider.gameObject.layer) == "Player" || LayerMask.LayerToName(collider.gameObject.layer) == "PlayerDie")
        { 
            triggerPlayer.Remove(collider.gameObject.GetComponent<PlayerScript>());
        }
    }

}
