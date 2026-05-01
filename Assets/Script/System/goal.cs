using UnityEngine;

public class GoalCheck : MonoBehaviour
{
    // ゴールした時に表示させたいオブジェクトをここに登録する
    public GameObject goalUI;

    void Start()
    {
        // ゲーム開始時に、念のためゴールUIを隠しておく
        if (goalUI != null)
        {
            goalUI.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 触れたのがプレイヤーだったら
        if (collision.CompareTag("Player"))
        {
            Debug.Log("ゴール！");

            // ゴールUIを表示させる
            if (goalUI != null)
            {
                goalUI.SetActive(true);
            }

            // (オプション) プレイヤーを止めたい場合は以下を追加
            // collision.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            // collision.GetComponent<Rigidbody2D>().simulated = false; 
        }
    }
}
