using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class PonPon : MonoBehaviour
{

    public float posY = 0f;
    public float ponPonSpeed = 0f;
    private float startY;
    public float ponPonTime=0;
    private float ponPonTimer;
    public float waitTime = 0;
    private float waitTimer = 0;
    private bool isPonPon=true;
    private float NuruNuruTime = 0;
    public float startWaitTime = 0;
    private float startWaitTimer = 0;

    void Start()
    {
        startY = transform.position.y;
    }

    void Update()
    {
        startWaitTimer += Time.deltaTime;
        if (startWaitTimer <= startWaitTime) return;

        if (isPonPon)
        {
            NuruNuruTime += Time.deltaTime * ponPonSpeed;
            float currentY = startY + Mathf.PingPong(NuruNuruTime, posY);
            transform.position = new Vector2(transform.position.x, currentY);
        }

        //PonPonが許される時間
        if (ponPonTimer <= ponPonTime && isPonPon)
        {
            ponPonTimer += Time.deltaTime;
            isPonPon = true;
            waitTimer = 0;
        }
        if (ponPonTimer >= ponPonTime)
        {
            isPonPon = false;
        }

        //PonPonが許されない時間
        if (waitTimer <= waitTime && !isPonPon)
        {
            waitTimer += Time.deltaTime;
            isPonPon = false;
            ponPonTimer = 0;
        }
        if (waitTimer >= waitTime)
        {
            isPonPon = true;
        }

    }
}