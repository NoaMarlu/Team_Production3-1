using UnityEngine;
using UnityEngine.UI;

public class text : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

// Editor上で値をいじったらすぐに反映させるための魔法の言葉ケロ
[ExecuteAlways]
[RequireComponent(typeof(Image))] // ついでにImageコンポーネントも必須にするケロ
public class ImageChanger : MonoBehaviour
{
    [Header("--- データ設定 ---")]
    [Header("0～9の数字スプライトを順番に登録")]
    public Sprite[] sprites;

    [Header("桁ごとのImageオブジェクト（右から1桁目、2桁目...）")]
    public Image[] digitImages;

    [Header("--- デバッグ用 ---")]
    [Range(0, 99)] // スライダーで数字を選べるようにするケロ
    public int debugNumber; // ここをインスペクターでいじるケロ！

    private int lastDebugNumber = -1;

    // ゲーム中じゃなくても、インスペクターの値が変わったら呼ばれるケロ
    void Update()
    {
        // 数字が変わったときだけ、画像を更新するケロ
        if (debugNumber != lastDebugNumber)
        {
            SetImage(debugNumber);
            lastDebugNumber = debugNumber;
        }
    }

    // これがメインの関数ケロ
    public void SetImage(int num)
    {
        if (sprites == null || sprites.Length == 0 || digitImages == null || digitImages.Length == 0) return;

        // --- 【重要】まず最初にすべての桁を一旦「非表示」にするケロ ---
        foreach (var img in digitImages)
        {
            if (img != null) img.enabled = false;
        }

        // 0のときは、1桁目だけ表示して終わりケロ
        if (num == 0)
        {
            digitImages[0].enabled = true;
            digitImages[0].sprite = sprites[0];
            return;
        }

        int currentNum = num;

        for (int i = 0; i < digitImages.Length; i++)
        {
            // 数字がなくなったらループ終了ケロ
            if (currentNum <= 0) break;

            if (digitImages[i] == null) continue;

            // 桁を表示状態にするケロ
            digitImages[i].enabled = true;

            // 数字をセットするケロ
            int digit = currentNum % 10;
            if (digit >= 0 && digit < sprites.Length)
            {
                digitImages[i].sprite = sprites[digit];
            }

            currentNum /= 10;
        }
    }
}