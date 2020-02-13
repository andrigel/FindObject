using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickLvl3 : MonoBehaviour
{
    private Button ThisButton;
    [HideInInspector]
    public Level3Creator l3c;
    private void Start()
    {
        ThisButton = GetComponent<Button>();
        ThisButton.onClick.AddListener(Click);
    }

    public void Click()
    {
            l3c.SelectObject(this.gameObject);
    }

}
