using UnityEngine;

public class TextLayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MeshRenderer mesh=GetComponent<MeshRenderer>();
        mesh.sortingLayerName="Text";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
