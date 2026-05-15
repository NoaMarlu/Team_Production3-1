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
        if (collider.gameObject.tag == "ground")
        {
            audioSource.PlayOneShot(stepSE);
        }
    }
}
