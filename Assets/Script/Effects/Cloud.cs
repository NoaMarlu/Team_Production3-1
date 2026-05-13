using UnityEngine;

public class Cloud : MonoBehaviour
{
    [Header("--- 向き（CG相談用） ---")]
    [Tooltip("true: 左から右 / false: 右から左")]
    public bool isLeftToRight = true;

    [Header("--- 雲の設定 ---")]
    public GameObject[] cloudPrefabs;
    public float spawnInterval = 3f;  // 生成間隔（秒）

    [Header("--- 出現位置（高さ） ---")]
    public float minY = 2f;
    public float maxY = 5f;

    [Header("--- 移動スピードのランダム範囲 ---")]
    public float minSpeed = 1f;       // ★スピードの最小値
    public float maxSpeed = 4f;       // ★スピードの最大値

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnCloud();
            timer = 0f;
        }
    }

    void SpawnCloud()
    {
        if (cloudPrefabs == null || cloudPrefabs.Length == 0) return;

        // 雲の種類・高さ・スピードをランダムに決定
        GameObject selectedPrefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)];
        float randomY = Random.Range(minY, maxY);
        float randomSpeed = Random.Range(minSpeed, maxSpeed); // ★ここでランダム決定

        // 出現するX座標の決定
        float startX = isLeftToRight ? -15f : 15f;
        Vector3 spawnPosition = new Vector3(startX, randomY, 0f);

        // 生成して移動用コンポーネントをアタッチ
        GameObject newCloud = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        MovableCloud moveComponent = newCloud.AddComponent<MovableCloud>();

        // 向きを考慮したスピードをセット
        float finalSpeed = isLeftToRight ? randomSpeed : -randomSpeed;
        moveComponent.Setup(finalSpeed);
    }
}

// ==========================================
// 雲の移動と自動消去を管理する補助クラス
// ==========================================
public class MovableCloud : MonoBehaviour
{
    private float speed;
    private bool hasBeenVisible = false;

    public void Setup(float moveSpeed) => speed = moveSpeed;

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    void OnBecameVisible() => hasBeenVisible = true;

    void OnBecameInvisible()
    {
        if (hasBeenVisible) Destroy(gameObject);
    }
}