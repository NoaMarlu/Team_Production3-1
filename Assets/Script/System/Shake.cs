using UnityEngine;
using System.Collections;

public class Shake : MonoBehaviour
{  
    /* signShake */
    public float waitTime = 1.0f;     // 待機時間
    public float shakeSpeed = 10.0f;  // 揺れるスピード（少し大きめがおすすめ）
    public float shakeRadian = 3.0f; // 揺れる振幅（度数法）
    public int shakeCount = 4;        // 往復回数

    private float startRotationZ;

    void Start()
    {
        startRotationZ = transform.localEulerAngles.z;
        StartCoroutine(ShakeLoop());
    }
    IEnumerator ShakeLoop()
    {
        while (true)
        {
            for (int i = 0; i < shakeCount; i++)
            {
                yield return StartCoroutine(RotateToZ(startRotationZ + shakeRadian));
                yield return StartCoroutine(RotateToZ(startRotationZ - shakeRadian));
            }

            yield return StartCoroutine(RotateToZ(startRotationZ));
            yield return new WaitForSeconds(waitTime);
        }
    }

    //指定したZ角度まで回転させるサブコルーチン
    IEnumerator RotateToZ(float targetZ)
    {
        while (Mathf.Abs(Mathf.DeltaAngle(transform.localEulerAngles.z, targetZ)) > 0.1f)
        {
            float newZ = Mathf.MoveTowardsAngle(transform.localEulerAngles.z, targetZ, shakeSpeed * Time.deltaTime * 60f);
            transform.localEulerAngles = new Vector3(0, 0, newZ);
            yield return null;
        }
    }


}
