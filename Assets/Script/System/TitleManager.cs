using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    void Start()
    {
        
    }
    void Update()
    {

        //もしスペースキーorAが押されたら
        if (Input.GetKeyDown(KeyCode.Space)|| Input.GetButtonDown("Submit"))
        {
            SceneManager.LoadScene("STAGE1");
        }
        
    }
}
