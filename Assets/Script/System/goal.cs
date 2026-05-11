using UnityEngine;
using UnityEngine.SceneManagement; // ★シーン遷移に必要です

public class goal : MonoBehaviour
{
    // ★インスペクターで設定する遷移先のシーン名
    [Header("遷移先のシーン名")]
    public string SceneName;

    // --- 【コメントアウト】UI関連の変数 -------------------------
    // // ゴールした時に表示させたいオブジェクトをここに登録する
    // public GameObject goalUI;
    // -----------------------------------------------------------

    void Start()
    {
        // --- 【コメントアウト】初期化処理 -------------------------
        // // ゲーム開始時に、念のためゴールUIを隠しておく
        // if (goalUI != null)
        // {
        //     goalUI.SetActive(false);
        // }
        // -----------------------------------------------------------
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 触れたのがプレイヤーだったら
        if (collision.CompareTag("Player"))
        {
            Debug.Log("ゴール！");

            // ★インスペクターで設定したシーン名へ遷移する
            if (!string.IsNullOrEmpty(SceneName))
            {
                SceneManager.LoadScene(SceneName);
            }
            else
            {
                Debug.LogWarning("遷移先のSceneNameが設定されていません！");
            }

            // --- 【コメントアウト】UI表示関連の処理 -----------------
            // // ゴールUIを表示させる
            // if (goalUI != null)
            // {
            //     goalUI.SetActive(true);
            // }
            // -----------------------------------------------------------

            // (オプション) プレイヤーを止めたい場合は以下を追加
            // collision.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            // collision.GetComponent<Rigidbody2D>().simulated = false; 
        }
    }
}