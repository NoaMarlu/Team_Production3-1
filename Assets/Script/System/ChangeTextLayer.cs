using TMPro;
using UnityEngine;

public class ChangeTextLayer : MonoBehaviour
{

    private TextMeshProUGUI tmp;
    public int layerValue;
    void Start()
    {
        tmp=GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        tmp.canvas.sortingOrder = layerValue;
    }
}
