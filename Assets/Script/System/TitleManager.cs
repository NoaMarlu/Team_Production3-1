using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{

    private AudioSource audioSource;
    public AudioClip[] audioClip;
    public float[] SEVolume;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {

        //もしスペースキーorAが押されたら
        if (Input.GetKeyDown(KeyCode.Space)|| Input.GetButtonDown("Submit"))
        {
             audioSource.PlayOneShot(audioClip[0]);
            SceneManager.LoadScene("STAGE1");
        }
        
    }
}
