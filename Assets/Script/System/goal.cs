using UnityEngine;
using UnityEngine.SceneManagement; // ★重要：シーン遷移を動かすために追加しました！

public class goal : MonoBehaviour
{
    [Header("--- アニメーションの設定 ---")]
    public Animator goalAnimAnimator;
    public string animationStateName = "Goal"; // ゴールアニメのステート名

    [Header("--- 演出の長さ（秒） ---")]
    public float animationDuration = 2.0f;

    [Header("--- 遷移先のシーン名 ---")]
    [Tooltip("アニメーションが終わった後に移動したいステージ（シーン）の名前を入力")]
    public string nextSceneName; // ★シーン切り替え用の変数を合体！

    private float elapsedUnscaledTime = 0f;
    private bool isGoalTriggered = false; // ゴールしたかどうかのフラグ
    private GameManager gameManager;

    /*SE関連*/
    private AudioSource audioSource;
    public AudioClip clip;
    private bool wasSE = false;


    private SheepSpawner sheepSpawner;
    public bool isGoal=false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sheepSpawner = GameObject.FindWithTag("Spawner").GetComponent<SheepSpawner>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // 開始時はアニメーション用オブジェクトを非表示にしておく（必要に応じて）
        if (goalAnimAnimator != null)
        {
            goalAnimAnimator.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // ★ゴールした後にだけ、時間を計測する
        if (isGoalTriggered)
        {
              if (wasSE != true)
             {
                audioSource.PlayOneShot(clip);
                sheepSpawner.StarRecord();
                isGoal = true;
                wasSE = true;
            }
            elapsedUnscaledTime += Time.unscaledDeltaTime;

            // 規定の秒数（2秒）が経過したら、演出を終わらせてシーンを切り替える
            if (elapsedUnscaledTime >= animationDuration)
            {
                EndGoalAnimationAndLoadScene();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // すでにゴール演出中なら、2回目以降は無視する（バグ防止）
        if (isGoalTriggered) return;

        // 触れたのがプレイヤーだったら
        if (collision.CompareTag("Player"))
        {
            isGoalTriggered = true; // タイマースタートの合図
            elapsedUnscaledTime = 0f; // タイマーをリセット

            if (gameManager != null) gameManager.isPausable = false;

            //Time.timeScale = 0f; // ゲームを停止

            if (goalAnimAnimator != null)
            {
                goalAnimAnimator.gameObject.SetActive(true); // アニメーションを表示
                goalAnimAnimator.updateMode = AnimatorUpdateMode.UnscaledTime; // 時間停止中も動くモードに
                goalAnimAnimator.Play(animationStateName, 0, 0f); // アニメーションを最初から再生
            }

            //// プレイヤーの物理挙動を止める
            //Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            //if (rb != null)
            //{
            //    rb.linearVelocity = Vector2.zero;
            //    rb.simulated = false;
            //}
        }
    }

    // ★2秒経ったときに呼び出され、次のシーンへ進む合体関数
    void EndGoalAnimationAndLoadScene()
    {
        Time.timeScale = 1f; // ゲームの時間を動かす

        if (goalAnimAnimator != null)
        {
            goalAnimAnimator.gameObject.SetActive(false); // アニメーションを非表示にする
        }

        // ★インスペクターで設定されたシーン名があれば、そこへ画面を切り替える！
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("遷移先の nextSceneName がインスペクターで設定されていません！");
        }

        enabled = false; // このスクリプト自体のUpdateを止めて軽量化
    }
}