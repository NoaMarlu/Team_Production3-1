using UnityEngine;

public class getMainCamera : MonoBehaviour
{
    private GameObject cameraObj;
    private Camera m_Camera;
    private Canvas canvas;
    void Start()
    {
        cameraObj = GameObject.FindWithTag("MainCamera");
        m_Camera = cameraObj.GetComponent<Camera>();
        canvas=GetComponent<Canvas>();
        canvas.worldCamera=m_Camera;
    }
    void Update()
    {
        
    }
}
