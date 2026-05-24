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
        PlayerPrefs.DeleteAll();
        audioSource = GetComponent<AudioSource>();
        Prefs();
    }

    void Update()
    {
        // 入力処理
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Submit"))
        {
            audioSource.PlayOneShot(audioClip[0]);
            SceneManager.LoadScene("StageSelect");
        }
    }

    void Prefs()
    {
        if(!PlayerPrefs.HasKey("STAGE2_Count"))PlayerPrefs.SetInt("STAGE2_Count",0);
        if(!PlayerPrefs.HasKey("STAGE2_Star"))PlayerPrefs.SetInt("STAGE2_Star",0);
        if(!PlayerPrefs.HasKey("STAGE4_Count"))PlayerPrefs.SetInt("STAGE4_Count",0);
        if(!PlayerPrefs.HasKey("STAGE4_Star"))PlayerPrefs.SetInt("STAGE4_Star",0);
        if(!PlayerPrefs.HasKey("CurrentStage"))PlayerPrefs.SetInt("CurrentStage", 0);
        PlayerPrefs.Save();
    }


}