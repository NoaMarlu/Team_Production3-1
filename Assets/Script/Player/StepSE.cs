using UnityEngine;

public class StepSE : MonoBehaviour
{

    private AudioSource audioSource;
    public AudioClip stepSE;

    void Start()
    {
        audioSource=GetComponent<AudioSource>();
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        //Groundレイヤーだったら

        // 地面との接触判定
        if (collider.gameObject.CompareTag("ground"))
        {
            Transform parent = transform.parent;
            if (parent != null)
            {
                PlayerScript player=parent.GetComponent<PlayerScript>();
                player.groundCollision();
            }
        }
            if (collider.gameObject.layer==8)
        {
            audioSource.PlayOneShot(stepSE);
        }
    }
}
