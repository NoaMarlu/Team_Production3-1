
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
    public bool isGrounded;//地面に触れているか
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
    public bool isMountFunc = false;
    public GameObject nearestCol = null;
    public PlayerScript nearestColScript = null;
    private bool isTop=true;//trueであれば単体または重なっている場合の最上段
    public GameObject nearestUpCol = null;

    /*当たり判定処理*/
    public bool isOverRaped = false;

    /*LinkForce*/
    public float rayRadius=0.5f;
    public float rayDistance = 0.5f;
    List<RaycastHit2D> hits = new List<RaycastHit2D>();

    /*チュートリアル用の制御変数*/
    public bool directionOK = true;
    public bool mountOK = true;

    /*初回から死んでいる場合*/
    public bool farstDie = false;

    /*Debug*/
    public float VelocityY;

    /*行けなくなる範囲を設定する場合*/
    public bool moveControl = false;//基本はなし
    public float[] controlValue;//0がX左、1がX右
    /*contorolValueを設定する際は、両方の軸をいれないといけない*/

    /*isCeiling*/
    private BoxCollider2D boxCol;
    private float colW;
    private float colH;
    public bool isCeiling;

    void Start()
    {
        Init();
        FirstDieInit();
        if (farstDie) return;
        StartFunc();
    }
    void Update()
    {

        VelocityY = rb.linearVelocityY;
        if (farstDie)
        {
            MountOnNearestLoopSheep();
            LinkForce();
            return;
        }

        IsCeiling();
        getTopSheep();
        DebugFunc();
        AnimationFunc();
        ChangeSprite();
        MountOnNearestLoopSheep();
        LinkForce();
        MoveControler();

        if (isRemind)
        {
            SheepIsDie();
            RemindAction();
        }
        else
        {
            SheepIsDie();
            FindNearSheep();
            PlayerInput();
        }

        Spawn();

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
        if (directionOK != true) return;
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
        if (this.transform.position.y <= -6.53f)
        {
            gameObject.tag = "ground";
            nearestCol = null;
            loopDie = true;
            rb.linearVelocityY = -10;
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
        isMountFunc = false;
        isMount = false;
        isTop = true;//単体になるとtrue
        if(isMountFunc && nearestColScript!=null)nearestColScript.NullNearestUpCol();
        nearestColScript = null;
        nearestCol = null;
        //ジャンプした瞬間に接地判定をオフにする（二段ジャンプ防止）
        isGrounded = false;


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
                PlayerScript prbS = p.GetComponent<PlayerScript>();
                if (prb == null) continue;
                if (prbS.nearestCol != this.gameObject) continue;

                prb.gravityScale = 1;
                Debug.Log("連動させました");
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



    }
    //プレイヤーの位置を記録
    void AddList(int num) //0をジャンプ、1を方向転換とする
    {
        if (isRemind) return;
        timeList.Add(manager.GetGameTimer());
        actionList.Add(num);
        positionList.Add(transform.position);
        if (nearestCol != null)
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
                    isMountFunc = true;
                    isMount = true;
                    nearestCol = nearestSheep[num];
                    num++;
                    break;
                case 4://4でgroundに着地
                    rb.gravityScale = 0;
                    rb.linearVelocity = new Vector2(0, 0);
                    transform.position = positionList[num];
                    num++;
                    break;
            }
        }

    }
    //LBでスポーン
    void Spawn()
    {
        if (isSpawn) return;
            if (Input.GetAxis("LT")>0.5f || Input.GetKeyDown(KeyCode.Z))
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
    //近くのループ羊に乗る関数やつぁ
    void MountOnNearestLoopSheep()
    {
        if (!mountOK) return;
        if (!isMountFunc) return;
        

        /*内田加筆*/
        float nearestDist = float.MaxValue;
        /////////////

        if (isRemind != true)//ループしていないなら
        {
            if (nearestCol != null) nearestColScript = nearestCol.GetComponent<PlayerScript>();
            else
            {
                foreach (GameObject sheep in sheepSpawner.sheeps)
                {
                    PlayerScript ps = sheep.GetComponent<PlayerScript>();
                    if (ps == null || ps == this.GetComponent<PlayerScript>()) continue;
                    if (!ps.isRemind) continue; // ループ羊のみ対象
                    if (ps.getIsTop() == false) continue;

                    float dist = Vector2.Distance(transform.position, sheep.transform.position);
                    if (dist <= mountRadius && dist < nearestDist)
                    {
                        if (ps.getIsTop() == false) continue;//相手が単体ではない、または最上段ではないなら
                        if (ps.GetCeiling() == true) continue;
                        nearestCol = sheep;
                        nearestColScript = ps;
                        nearestColScript.SetNearestUpCol(this.gameObject);

                    }
                }
            }

        }
        else//ループ中にこの関数が呼ばれている場合
        {
            if (nearestCol == null)
            {
                isMountFunc = false;
                return;
            }
            if(nearestCol!=null)
            { 
                nearestColScript = nearestCol.GetComponent<PlayerScript>();
                nearestColScript.SetNearestUpCol(this.gameObject);
            }
        }


        if (nearestColScript != null)
        {

            /*内田加筆*/
            if (isMount)
            {
                setIsTop(true);
                nearestColScript.setIsTop(false);
                AddList(3);
                IgnoreReset();
                rb.linearVelocity = Vector2.zero;
                isMount = false;
                isGrounded = true;
            }
            /////////////

            // 最も近いループ羊の真上に位置をセット
            transform.position = new Vector2(
                nearestColScript.transform.position.x,
                nearestColScript.transform.position.y + mountOffset
            );


        }
        else
        {
            isMountFunc = false;
            isMount = false;
        }

    }
    public void MountOffsetChanger(float num) { mountOffset = num; }
    //上に乗っている羊を取得、削除（羊がプレイヤーである場合セットしない）
    public void SetNearestUpCol(GameObject col) {
        PlayerScript pS=col.GetComponent<PlayerScript>();
        if(pS.isDie)//プレイヤーをUpColにセットするとnearestUpColが二体いる状態に対応できないので除外
        {
            nearestUpCol = col; 
        }
    }
    public void NullNearestUpCol() { nearestUpCol = null; }
    //段差の一番上を取得
    private void getTopSheep()
    {
        if (isDie) return;
        if (!isMountFunc) return;
        if (nearestCol != null)//下に羊がいるなら
        {
            GameObject upObj=nearestCol;//まずは下にいる羊
            PlayerScript upScript = upObj.GetComponent<PlayerScript>() ;
            int count=0;

            while (true)//一番上にいる羊を探る
            {
                count++;
                if (upScript.nearestUpCol !=null)//下にいる羊の上にnearestUpColがいるなら
                {
                    upObj = upScript.nearestUpCol;
                    upScript=upObj.GetComponent<PlayerScript>() ;
                    //どんどん上の羊を取得する
                    if (count>100)
                    {
                        break;
                    }
                }
                else {  break; }//上に羊がいないならbreak
            }
            nearestCol = upObj;
            AddList(3);
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
            if (hit.collider.gameObject.tag == "stepObj") return;
            hits.Add(hit);
            triggerPlayer.Add(hit.collider.gameObject);
        }
    }
    //衝突判定を全て無効にする
    void IgnoreReset()
    {
        if(nearestCol!=null)Physics2D.IgnoreCollision(gameObject.GetComponent<BoxCollider2D>(), nearestCol.GetComponent<BoxCollider2D>(), true);
        isMount = false;
        //nearestCol = null;
    }
    //矢印UIの表示管理
    void FindNearSheep()
    {
        float nearestDist = mountRadius;
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
                return;
            }
        }
    }
    //Debug関連
    void DebugFunc()
    {
        //Debug.Log(rb.linearVelocityX + " " + rb.linearVelocityY);
        GameTimerDayo = manager.GetGameTimer();
    }
    //プレイヤー入力関連
    void PlayerInput()
    {
        if (isDie) return;

        //方向転換
        if (Input.GetKeyDown(KeyCode.JoystickButton2) || Input.GetKeyDown(KeyCode.X)) ChangeDirection();
        //ジャンプ
        if (Input.GetButtonDown("Submit") || Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded) isJump = true;
        }
        //羊の乗る
        if (Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKeyDown(KeyCode.C))
        {
             isMountFunc = true;
             isMount = true;
        }

    }
    //アニメーション管理
    void AnimationFunc()
    {
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
        ////羊小屋のレイヤーが21で、スピーカーが22、「羊がn匹」フォントのレイヤーが19なため、羊のレイヤー順を変える必要がある
        //if (isAnimation)
        //{
        //    SpriteRenderer spr = gameObject.GetComponent<SpriteRenderer>();
        //    spr.sortingOrder = 20;
        //}
        //else
        //{
        //    SpriteRenderer spr = gameObject.GetComponent<SpriteRenderer>();
        //    spr.sortingOrder = 23;
        //}
        //////////////////////
        ///
    }
    //初期化
    void Init()
    {
        boxCol = GetComponent<BoxCollider2D>();
        colW = boxCol.size.x*transform.localScale.x/2.0f;
        colH= boxCol.size.y*transform.localScale.y;
        audioSource = GetComponent<AudioSource>();
        GameObject obj = GameObject.Find("GameManager");
        manager = obj.GetComponent<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        sheepSpawner = GameObject.FindWithTag("Spawner").GetComponent<SheepSpawner>();
        spr = GetComponent<SpriteRenderer>();
        E_Spawn = Resources.Load<GameObject>("E_Spawn");
    } 
    //Start関数に入れる予定だったもの
    void StartFunc()
    {

        Init();
        SheepIsLive();
        SetSprite();
        startPos = transform.position;
        AddList(2);

        //Debug
        SpawnTiming = manager.GetGameTimer();

    }
    //最上段または単体であることを取得する
    public void setIsTop(bool top) { isTop = top; }
    public bool getIsTop (){ return isTop; }
    //ジャンプ力・移動速度の変更
    public void JumpForceChanger(float num) { jumpForce = num; }
    public void MoveSpeedChanger(float num) { moveSpeed = num; }
    //最初から死亡
    void FirstDieInit()
    {
        if (farstDie == false) return;
        isDie = true;
        isRemind = true;
        DieTime = 0; // MaxTimeに影響させたくない場合は0のまま
        gameObject.tag = "ground";
        sheepSpawner.sheeps.Add(this.gameObject);
        SetSprite();
        spr.sprite = S_Standby_Death[0];
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            isGrounded = true;
            Debug.Log("地面とプレイヤーが衝突");
            AddList(4);
            rb.linearVelocity = Vector2.zero;

        }
    }
    //横軸移動範囲の正業
    void MoveControler()
    {
        //移動範囲を制限する場合
        if (moveControl)
        {
            if (gameObject.transform.position.x < controlValue[0])
            {
                gameObject.transform.position = new Vector3(controlValue[0], transform.position.y, transform.position.z);
            }
            if (gameObject.transform.position.x > controlValue[1])
            {
                gameObject.transform.position = new Vector3(controlValue[1], transform.position.y, transform.position.z);
            }
        }
    }
    //天井判定取得
    void IsCeiling()
    {

        Vector2 origin = transform.position;
        Vector2 direction = Vector2.up;
        RaycastHit2D hit = Physics2D.CircleCast(origin, colW, direction,colH, LayerMask.GetMask("Ground"));

        if (hit.collider != null)  isCeiling = true;
        else  isCeiling = false;
    
    }
    bool GetCeiling() { return isCeiling; }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 pos = (Vector2)transform.position+(Vector2.up*colH/2.0f);
        Gizmos.DrawWireSphere(transform.position, colW);
        Gizmos.DrawWireSphere(pos, colW);
    }

}