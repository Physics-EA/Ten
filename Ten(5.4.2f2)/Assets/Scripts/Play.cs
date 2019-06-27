using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using GDGeek;

public class Play : MonoBehaviour
{
    public Square phototype;
    private Square[] listSquare = null;

    private void Awake()
    {
        listSquare = this.GetComponentsInChildren<Square>();
        foreach (var item in listSquare)
        {
            item.Hide();
        }
    }

    public Square GetSquare(int x, int y)
    {
        int n = Mathf.Clamp((x - 1), 0, 6) * 4 + Mathf.Clamp((y - 1), 0, 3);
        return listSquare[n];
    }

    public Task MoveTask(int number, Vector2 begin, Vector2 end)
    {
        Square s = GameObject.Instantiate(phototype);
        Square b = this.GetSquare((int)begin.x, (int)begin.y);        
        Square e = this.GetSquare((int)end.x, (int)end.y);
           
        s.transform.parent = b.transform.parent;      
        s.transform.localPosition = b.transform.localPosition;
        s.transform.localScale = b.transform.localScale;
        s.Number = number;
        s.Show();       
        b.Hide();

        TweenTask tt = new TweenTask(delegate
        {
            return TweenLocalPosition.Begin(s.gameObject, 1f, e.transform.localPosition);
        });

        TaskManager.PushBack(tt, delegate
        {
            GameObject.Destroy(s.gameObject);
        });

        return tt;
    }
}
