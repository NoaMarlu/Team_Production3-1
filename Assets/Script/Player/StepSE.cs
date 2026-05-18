using UnityEngine;

public class StepSE : MonoBehaviour
{

    private AudioSource audioSource;
    public AudioClip stepSE;
    public GameObject parent;
    public PlayerScript player;

    void Start()
    {
        parent = transform.parent.gameObject;
        player=parent.GetComponent<PlayerScript>();
        audioSource=GetComponent<AudioSource>();
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        //Groundレイヤーだったら
        if (collider.gameObject.layer==8)
        {
            audioSource.PlayOneShot(stepSE);
        }
    }
}
