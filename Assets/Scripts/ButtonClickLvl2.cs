using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickLvl2 : MonoBehaviour
{
    //скрипт, що привязаний до цього ігрового об'єкта
    private Button ThisButton;
    //зразок ігрового об'єкта - квадрат, що з'являється при кліку
    public GameObject Square;
    [HideInInspector]
    //сам ігровий об'єкт, який ми створюємо при кліку
    //він створюється як дочірній компонент
    public GameObject SquareInstantiated;
    [HideInInspector]
    public Level2Creator l2c;
    private void Start()
    {
        //отримуємо посилання на скрипт Button
        ThisButton = GetComponent<Button>();
        //назначаємо функції - обробник події для кліка оп зображенню
        ThisButton.onClick.AddListener(Click);
        SquareInstantiated = null;
    }
    //функція виконується при кліку по зображенню і відповідає за появі або видалення квадратної рамки навколо зображення
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
        //функції, що змінюють колір квадрата. Зелений буде при правильній і червоний при неправильній відповіді
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
