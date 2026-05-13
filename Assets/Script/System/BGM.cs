using UnityEngine;

public class BGM : MonoBehaviour
{
    // どこからでもアクセスできるようにシングルトン（静的変数）にする
    public static BGM Instance { get; private set; }

    private AudioSource audioSource;

    void Awake()
    {
        // --- 重複防止の処理（超重要！） ---
        if (Instance != null && Instance != this)
        {
            // すでにシーンに存在している場合は、新しく生まれた方を削除する
            Destroy(gameObject);
            return;
        }

        // 自分が唯一無二のマネージャーならインスタンスを保持
        Instance = this;

        // シーンを切り替えてもこのオブジェクトを破壊しないようにする
        DontDestroyOnLoad(gameObject);

        // AudioSourceを取得しておく
        audioSource = GetComponent<AudioSource>();
    }

    // 外部のプログラムから音量を変更したり、再生/停止できるようにする関数（例）
    public void PlayBGM()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopBGM()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}