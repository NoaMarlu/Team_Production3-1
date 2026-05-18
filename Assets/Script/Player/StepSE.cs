using UnityEngine;

public class StepSE : MonoBehaviour
{

    private AudioSource audioSource;
    public AudioClip stepSE;
    public 

    void Start()
    {
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
