using UnityEngine;

[ExecuteAlways]
public class SpriteDigitChanger : MonoBehaviour
{
    [Header("0～9のスプライトを登録")]
    public Sprite[] sprites;

    [Header("1の位、10の位のSpriteRendererを登録")]
    public SpriteRenderer[] digitRenderers;

    [Header("このスクリプトが反応する範囲")]
    public int minCount;
    public int maxCount;

    private int lastNumber = -1;
    public SheepSpawner spawner;

    void Start()
    {
        // ヒエラルキーから Spawner を探す
        GameObject spw = GameObject.FindWithTag("Spawner");
        if (spw != null) spawner = spw.GetComponent<SheepSpawner>();
    }

    void Update()
    {
        if (spawner == null) return;

        int currentCount = spawner.sheepCount;

        // ★重要：自分の範囲内（0~9など）の時だけ表示する
        bool isInRange = (currentCount >= minCount && currentCount <= maxCount);

        // 子要素（吹き出しの見た目）をまとめて表示・非表示にする
        // ※このスクリプトがついている親オブジェクト自体をオフにすると、Updateが止まるので注意
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(isInRange);
        }

        // 範囲内で、かつ数字が変わった時だけ画像を更新
        if (isInRange && currentCount != lastNumber)
        {
            SetDigits(currentCount);
            lastNumber = currentCount;
        }
    }

    public void SetDigits(int num)
    {
        if (sprites == null || sprites.Length < 10 || digitRenderers == null) return;

        // 全ての桁を一旦リセット
        foreach (var sr in digitRenderers)
        {
            if (sr != null) sr.enabled = false;
        }

        // 1桁（0-9）の表示
        if (num < 10)
        {
            if (digitRenderers.Length > 0 && digitRenderers[0] != null)
            {
                digitRenderers[0].enabled = true;
                digitRenderers[0].sprite = sprites[num % 10];
            }
        }
        // 2桁（10-99）の表示
        else
        {
            int tempNum = num;
            for (int i = 0; i < digitRenderers.Length; i++)
            {
                if (digitRenderers[i] == null) continue;
                digitRenderers[i].enabled = true;
                digitRenderers[i].sprite = sprites[tempNum % 10];
                tempNum /= 10;
                if (tempNum == 0) break;
            }
        }
    }
}