using UnityEngine;

public class symbolON : MonoBehaviour
{
    public SpriteRenderer ON;
    public SpriteRenderer OFF;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ON.enabled = true;
        OFF.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("LT") > 0.5f || Input.GetKeyDown(KeyCode.Z))
        {
            ON.enabled = false;
            OFF.enabled = true;
        }
    }

}
