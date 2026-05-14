using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleManager : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] audioClip;



    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // 入力処理
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Submit"))
        {
            audioSource.PlayOneShot(audioClip[0]);
            SceneManager.LoadScene("STAGE1");
        }
    }


}