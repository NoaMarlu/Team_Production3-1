using UnityEngine;

public class E_Spawn : MonoBehaviour
{

    private float E_Timer = 0;
    private float E_Time = 0.2f;
    private int E_Count = 0;

    private Sprite[] E_spr;
    private SpriteRenderer spr;

    void Start()
    {
        E_spr = Resources.LoadAll<Sprite>("E_Spawn");
        spr = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        E_Timer += Time.deltaTime;
        if (E_Timer > E_Time)
        {
            E_Count++;

            E_Timer = 0;
        }
        if (E_Count >= E_spr.Length)
        {
            Destroy(gameObject);
        }
        else
        {
            spr.sprite = E_spr[E_Count];
        }

    }

}
