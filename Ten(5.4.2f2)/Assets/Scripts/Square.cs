using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Square : MonoBehaviour
{
    public Text number = null;
    public int Number
    {
        set
        {
            number.text = value.ToString();
        } 
    }  

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Show()
    {        
        this.gameObject.SetActive(true);
    }
}
