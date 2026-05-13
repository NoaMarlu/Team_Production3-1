using UnityEngine;

public class StartScript : MonoBehaviour
{
    [Header("--- アニメーションの設定 ---")]
    public Animator readyGoAnimator;
    public string animationStateName = "GameStart";

    [Header("--- 【重要】アニメーションの長さ（秒） ---")]
    [Tooltip("レディGOのアニメーションが全部で何秒あるか入力（例: 2.5 秒なら 2.5）")]
    public float animationDuration = 2.0f;

    private float elapsedUnscaledTime = 0f;

    void Awake() // ← ここを Start から Awake に変更
    {
        Time.timeScale = 0f;
        // （以下そのまま）
        if (readyGoAnimator != null)
        {
            // 2. 時間停止中も動くモードにして再生
            readyGoAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }

    void Update()
    {
        // 3. Time.timeScale = 0 でも影響を受けない「現実の時間」を計測する
        elapsedUnscaledTime += Time.unscaledDeltaTime;

        // 4. 設定したアニメーションの秒数が経過したら、強制終了処理を行う
        if (elapsedUnscaledTime >= animationDuration)
        {
            ForceStartGame();
        }
    }

    // 強制的にゲームをスタートさせて非表示にする関数
    void ForceStartGame()
    {
        Time.timeScale = 1f; // ゲームスタート！（時間を動かす）

        if (readyGoAnimator != null)
        {
            readyGoAnimator.gameObject.SetActive(false); // 確実に非表示にする
        }

        enabled = false; // このスクリプト自体をオフにして終了
    }
}