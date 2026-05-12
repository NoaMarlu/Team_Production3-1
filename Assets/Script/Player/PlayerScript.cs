
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private bool isJump=false;

    /*SheepIsDie*/
    private SheepSpawner sheepSpawner;
    public bool isDie = false;//trueで一度死亡している判定

    /*AddPos*/
    public List<float> timeList = new List<float>();
    public List<int> actionList = new List<int>();//0をジャンプ、1を方向転換、2を生成タイミング、3で羊に接着、4でgroundに着地
    public List<Vector2> positionList = new List<Vector2>();
    public List<GameObject> nearestSheep = new List<GameObject>();


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
    public bool isloopSpawn = false;
    private GameObject E_Spawn;//煙エフェクト

    /*ChangeSprite*/
    private Sprite[] S_Standby;
    private Sprite[] S_Standby_Death;
    private Sprite[] S_Jump;
    private Sprite[] S_Jump_Death;
    public Vector2 Velocity;
    private float standbyTime = 0.1f;
    private float standbyTimer = 0;
    private bool isStandby = false;
    float x = 5.8f;

    /*StartAnimation*/
    private bool isAnimation = true;
    private Sprite S_Dash;
    public float startDistance = 1.0f;//小屋からスポーン距離までの長さ
    public float startMoveSpeed = 0.3f;

    /*OnTrigger*/
    List<GameObject> triggerPlayer = new List<GameObject>();

    /*SE*/
    private AudioSource audioSource;//0=ジャンプ,1=スポーン、3=踏みつけ
    public AudioClip[] audioClip;
    public float[] SEVolume;

    /*MountOnLoopSheep*/
    public float mountRadius = 3.0f; // Unity上で設定可能な円の半径
    public float mountOffset = 1.0f; // 羊の上に乗るためのYオフセット
    public bool isMount=false;
    public GameObject nearestCol = null;

    /*当たり判定処理*/
    public bool isOverRaped = false;

    /*LinkForce*/
    public float rayRadius=0.5f;
    public float rayDistance = 0.5f;
    List<RaycastHit2D> hits = new List<RaycastHit2D>();

    /*setArrow*/
    private SpriteRenderer arrow;

    void Start()
    {

        audioSource = GetComponent<AudioSource>();
        GameObject obj = GameObject.Find("GameManager");
        manager = obj.GetComponent<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        sheepSpawner = GameObject.FindWithTag("Spawner").GetComponent<SheepSpawner>();
        spr = GetComponent<SpriteRenderer>();
        E_Spawn = Resources.Load<GameObject>("E_Spawn");

        SheepIsLive();
        SetSprite();

        //StartAnimation
        //spr.sprite = S_Dash;
        startPos = transform.position;
        AddList(2);

        //Debug
        SpawnTiming = manager.GetGameTimer();


    }
    void Update()
    {
        Debug.Log(rb.linearVelocityX + " " + rb.linearVelocityY);
        ///StartAnimation///

        //       if (isAnimation)
        //       {
        //           transform.position =new Vector2(transform.position.x+startMoveSpeed * Time.deltaTime, transform.position.y);
        //           if (transform.position.x >= startPos.x + startDistance)
        //           {
        //               isAnimation = false;
        //               startPos=transform.position;
        //               SheepIsLive();
        //               AddList(2);
        //           }
        //           return;
        //       }
        isAnimation = false;
        //羊小屋のレイヤーが21で、スピーカーが22、「羊がn匹」フォントのレイヤーが19なため、羊のレイヤー順を変える必要がある
        if (isAnimation)
        {
            SpriteRenderer spr=gameObject.GetComponent<SpriteRenderer>();
            spr.sortingOrder = 20;
        }
        else
        {
            SpriteRenderer spr = gameObject.GetComponent<SpriteRenderer>();
            spr.sortingOrder = 23;
        }
        //////////////////////
        ///

        if (Input.GetKeyDown(KeyCode.JoystickButton10)||Input.GetKeyDown(KeyCode.Escape))
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }


        GameTimerDayo = manager.GetGameTimer();

        ChangeSprite();
        LinkForce();

        if (isRemind)
        {
            //if (isGrounded)
            //{ 
            //    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            //}
            SheepIsDie();
            RemindAction();
        }
        else
        {

            SheepIsDie();
            FindNearSheep();

            //if(isGrounded)
            //{
            //    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            //}

            //死亡済みなら操作を取りやめる
            if (isDie) return;

            if (Input.GetKeyDown(KeyCode.JoystickButton2) || Input.GetKeyDown(KeyCode.X)) { ChangeDirection(); }
            if (Input.GetButtonDown("Submit") || Input.GetKeyDown(KeyCode.Space))
            {
                if (isGrounded) isJump = true;
            }
            if (Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKeyDown(KeyCode.C))
            {
                MountOnNearestLoopSheep();
            }
        }

        if (isSpawn == false)
        {
            Spawn();
        }

        //Debug
        Velocity = rb.linearVelocity;

    }
    void FixedUpdate()
    {
        if (isJump)
        {
            Jump();
            isJump = false;
        }
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
        triggerPlayer.Clear();
        IgnoreReset();
        if (isAnimation != true) transform.position = startPos;//位置
        if (isDie)
        {
            audioSource.PlayOneShot(audioClip[1], SEVolume[1]);
            Instantiate(E_Spawn, transform.position, transform.rotation);
        }
    }
    void SheepIsDie()
    {
        if (this.transform.position.y <= -5.47f)
        {
            gameObject.tag = "ground";
            loopDie = true;
            FouceReceiveLayer();
            if (DieTime == 0) DieTime = manager.GetGameTimer();
            if (isRemind == false || isDie == false)
            {
                isDie = true;
                isRemind = true;
            }
        }
    }
    //ジャンプ処理
    void Jump()
    {
        AddList(0);

        IgnoreReset();

        //ジャンプSE
        audioSource.PlayOneShot(audioClip[0], SEVolume[0]);

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

        //Triggerで当たり判定を取っている羊も同じ動きをさせる
        foreach (GameObject p in triggerPlayer)
        {
            if (p.transform.position.y > transform.position.y)
            {
                Rigidbody2D prb = p.GetComponent<Rigidbody2D>();
                prb.gravityScale = 1;

                if (isDirection)//右向き
                {
                    prb.linearVelocity = rb.linearVelocity;
                }
                else
                {
                    prb.linearVelocity = rb.linearVelocity;
                }
            }
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
        positionList.Add(transform.position);
        if (isMount && nearestCol != null)
        {
            nearestSheep.Add(nearestCol);
        }
        else nearestSheep.Add(null);
    }
    //記録された位置を再生する
    void RemindAction()
    {
        if (gameObject.layer == 3) gameObject.layer = 6;

        if (loopDie && (manager.GetGameTimer() >= timeList[0]))
        {
            SheepIsLive();
            num = 1;
            return;
        }


        if (num >= timeList.Count) return;
        if (manager.GetGameTimer() >= timeList[num])
        {
            switch (actionList[num])
            {
                case 0://0をジャンプ
                    rb.gravityScale = 1;
                    Jump();
                    num++;
                    break;
                case 1://1を方向転換
                    ChangeDirection();
                    num++;
                    break;
                case 3://3で羊に接着
                    MountOnNearestLoopSheep();
                    num++;
                    break;
                case 4://4でgroundに着地
                    rb.gravityScale = 0;
                    transform.position = positionList[num];
                    rb.linearVelocity = new Vector2(0, 0);
                    num++;
                    break;
            }
        }

    }
    //LBでスポーン
    void Spawn()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton4) || Input.GetKeyDown(KeyCode.Z))
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
                standbyTimer = 0;
            }
            if (isStandby)
            {
                if (isDie == false) spr.sprite = S_Standby[0];
                else { spr.sprite = S_Standby_Death[0]; }
            }
            else
            {
                if (isDie == false) spr.sprite = S_Standby[1];
                else { spr.sprite = S_Standby_Death[1]; }
            }
        }
        else if (rb.linearVelocityY > 0 && rb.linearVelocityY < x)//ジャンプ
        {
            if (isDie == false) spr.sprite = S_Jump[2];
            else { spr.sprite = S_Jump_Death[2]; }
        }
        else if (rb.linearVelocityY > 0 && rb.linearVelocityY >= x)
        {
            if (isDie == false) spr.sprite = S_Jump[1];
            else { spr.sprite = S_Jump_Death[1]; }
        }
        else if (rb.linearVelocityY < 0 && rb.linearVelocityY > -x)//滞空
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
        S_Standby_Death = Resources.LoadAll<Sprite>("S_Standby_Death");
        S_Jump = Resources.LoadAll<Sprite>("S_Jump");
        S_Jump_Death = Resources.LoadAll<Sprite>("S_Jump_Death");
        S_Dash = Resources.Load<Sprite>("S_Dash");

    }
    //当たり判定処理
    void FouceReceiveLayer()
    {
        //BoxCollider2D collider = GetComponent<BoxCollider2D>();
        //LayerMask layerName = LayerMask.GetMask("Player");

        //if (isDie)
        //{
        //    collider.forceReceiveLayers = layerName;
        //}
    }
    //近くのループ羊に乗る関数やつぁ
    void MountOnNearestLoopSheep()
    {
        AddList(3);
        PlayerScript nearest = null;
        float nearestDist = float.MaxValue;

        /*内田加筆*/
        IgnoreReset();
        /////////////

        if (isRemind != true)//ループしていないなら
        {
            foreach (GameObject sheep in sheepSpawner.sheeps)
            {
                PlayerScript ps = sheep.GetComponent<PlayerScript>();
                if (ps == null || ps == this.GetComponent<PlayerScript>()) continue;
                if (!ps.isRemind) continue; // ループ羊のみ対象

                float dist = Vector2.Distance(transform.position, sheep.transform.position);
                if (dist <= mountRadius && dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = ps;
                    nearestCol = sheep;
                }
            }
        }
        else//ループ中にこの関数が呼ばれている場合
        {
            if (nearestSheep[num]!=null)nearest = nearestSheep[num].GetComponent<PlayerScript>();
            nearestCol = nearestSheep[num];
        }


        if (nearest != null)
        {
            isMount = true;
            isGrounded = true;

            /*内田加筆*/
            //nearestとの衝突判定をONにする
            Physics2D.IgnoreCollision(this.GetComponent<BoxCollider2D>(), nearestCol.GetComponent<BoxCollider2D>(), false);
            /////////////

            // 最も近いループ羊の真上に位置をセット
            transform.position = new Vector2(
                nearest.transform.position.x,
                nearest.transform.position.y + mountOffset
            );
            

        }
    }
    //羊が上にいる場合に、力を連動させる
    void LinkForce()
    {
        hits.Clear();
        triggerPlayer.Clear();

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, rayRadius, Vector2.up, rayDistance);
        if(hit.collider != null)
        {
            hits.Add(hit);
            triggerPlayer.Add(hit.collider.gameObject);
        }
    }
    //衝突判定を全て無効にする
    void IgnoreReset()
    {
        if(nearestCol!=null)Physics2D.IgnoreCollision(gameObject.GetComponent<BoxCollider2D>(), nearestCol.GetComponent<BoxCollider2D>(), true);
        isMount = false;
        nearestCol = null;
    }
    //矢印UIの表示管理
    void FindNearSheep()
    {
        float nearestDist = mountRadius;
        bool b = false;
        foreach (GameObject sheep in sheepSpawner.sheeps)
        {
            if (sheep == this.gameObject) continue;
            PlayerScript ps = sheep.GetComponent<PlayerScript>();

            if (ps == null || ps == this.GetComponent<PlayerScript>()) continue;

            if (ps==null||!ps.isRemind) continue; // ループ羊のみ対象

            float dist = Vector2.Distance(transform.position, sheep.transform.position);
            if (dist <= mountRadius && dist < nearestDist)
            {
                nearestDist = dist;
                b = true;
            }
        }
        setArrow(b);
    }
    void setArrow(bool b)
    {
        if (b) { arrow.enabled = true; }
        else { arrow.enabled = false; }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 地面との接触判定
        if (collision.gameObject.CompareTag("ground"))
        {
            Debug.Log("地面とプレイヤーが衝突");
            AddList(4);
            rb.linearVelocity = Vector2.zero;
            audioSource.PlayOneShot(audioClip[2], SEVolume[2]);
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


}