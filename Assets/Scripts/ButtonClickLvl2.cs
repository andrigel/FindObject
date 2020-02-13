using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickLvl2 : MonoBehaviour
{
    private Button ThisButton;
    public GameObject Square;
    [HideInInspector]
    public GameObject SquareInstantiated;
    [HideInInspector]
    public Level2Creator l2c;
    private void Start()
    {
        ThisButton = GetComponent<Button>();
        ThisButton.onClick.AddListener(Click);
        SquareInstantiated = null;
    }

    public void Click()
    {
        if(l2c.SelectedObjects.Count < 2)
        {
            if (SquareInstantiated == null)
            {
                SquareInstantiated = Instantiate(Square, this.transform);
                
            }
            else
            {
                Destroy(SquareInstantiated);
                SquareInstantiated = null;
            }
            l2c.SelectObject(this.gameObject);
        }
    }
        
    public void SetSquareGreen()
    {
        if(SquareInstantiated != null)
        {
            SquareInstantiated.GetComponent<Image>().color = Color.green;
        }
    }
    public void SetSquareRed()
    {
        if (SquareInstantiated != null)
        {
            SquareInstantiated.GetComponent<Image>().color = Color.red;
        }
    }

}
