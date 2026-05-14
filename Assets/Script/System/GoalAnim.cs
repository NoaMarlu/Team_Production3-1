using UnityEngine;

public class GoalAnim : MonoBehaviour
{
    [Header("--- アニメーションの設定 ---")]
    public Animator goalAnimAnimator;
    public string animationStateName = "Goal"; // ★ゴールアニメのステート名

    [Header("--- 演出の長さ（秒） ---")]
    public float animationDuration = 2.0f;

    private float elapsedUnscaledTime = 0f;
    private bool isGoalTriggered = false; // ★ゴールしたかどうかのフラグ

    void Start()
    {
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
            elapsedUnscaledTime += Time.unscaledDeltaTime;

            // 規定の秒数が経過したら、演出を終了してゲームを戻す
            if (elapsedUnscaledTime >= animationDuration)
            {
                EndGoalAnimation();
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

            Time.timeScale = 0f; // ゲームを停止

            if (goalAnimAnimator != null)
            {
                goalAnimAnimator.gameObject.SetActive(true); // アニメーションを表示
                goalAnimAnimator.updateMode = AnimatorUpdateMode.UnscaledTime; // 時間停止中も動くモードに
                goalAnimAnimator.Play(animationStateName, 0, 0f); // アニメーションを最初から再生
            }

            // プレイヤーの物理挙動を止める
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.simulated = false;
            }
        }
    }

    // 演出が終わった時に呼び出す関数
    void EndGoalAnimation()
    {
        Time.timeScale = 1f; // ゲームの時間を動かす

        if (goalAnimAnimator != null)
        {
            goalAnimAnimator.gameObject.SetActive(false); // アニメーションを非表示にする
        }

        enabled = false; // このスクリプト自体のUpdateを止めて軽量化
    }
}