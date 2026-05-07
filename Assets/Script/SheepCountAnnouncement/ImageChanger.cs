using UnityEngine;

[ExecuteAlways]
public class SpriteDigitChanger : MonoBehaviour
{
    [Header("0～9のスプライトを登録")]
    public Sprite[] sprites;

    [Header("1の位、10の位のSpriteRendererを登録")]
    public SpriteRenderer[] digitRenderers;

    [Header("表示したい数字")]
    [Range(0, 99)]
    public int debugNumber;

    private int lastDebugNumber = -1;

    void Update()
    {
        if (debugNumber != lastDebugNumber)
        {
            SetDigits(debugNumber);
            lastDebugNumber = debugNumber;
        }
    }

    public void SetDigits(int num)
    {
        if (sprites == null || digitRenderers == null) return;

        // 全ての桁を一旦オフにしつつ、レイヤーを100に固定する
        foreach (var sr in digitRenderers)
        {
            if (sr != null)
            {
                sr.enabled = false;
                sr.sortingOrder = 100; // ここでレイヤーを100に固定！
            }
        }

        // 数字を各桁に割り当てる処理
        if (num == 0)
        {
            digitRenderers[0].enabled = true;
            digitRenderers[0].sprite = sprites[0];
            return;
        }

        int tempNum = num;
        for (int i = 0; i < digitRenderers.Length; i++)
        {
            if (tempNum <= 0) break;
            if (digitRenderers[i] == null) continue;

            digitRenderers[i].enabled = true;
            digitRenderers[i].sprite = sprites[tempNum % 10];
            tempNum /= 10;
        }
    }
}