using UnityEngine;
using System.Collections;
using GDGeek;
using UnityEngine.UI;

public class Play : MonoBehaviour {

	public Square _phototype;
	public GameObject _root = null;
	private Square[] list_ = null;
	public Hud _hud = null;
	void Awake(){
		if(list_ == null){
			list_ = this._root.GetComponentsInChildren<Square> ();
			foreach (Square square in list_) {
				square.hide();
			}
		}
	}

	public Task moveTask (int number, Vector2 begin, Vector2 end)
	{
		TaskSet ts = new TaskSet ();
		Square s = (Square)GameObject.Instantiate (_phototype);
		Square b = this.getSquare((int)(begin.x), (int)(begin.y));
		Square e = this.getSquare((int)(end.x), (int)(end.y));
		
		s.transform.SetParent(b.transform.parent);
		s.transform.localScale = b.transform.localScale;
		s.transform.localPosition = b.transform.localPosition;
		s.show ();
		s.number = number;
		b.hide ();
		TweenTask tt = new TweenTask (delegate() {
			return TweenLocalPosition.Begin(s.gameObject, 0.5f, e.transform.localPosition);
		});

		TweenTask t2 = new TweenTask (delegate() {
			return TweenRotation.Begin(s.gameObject, 0.5f, Quaternion.AngleAxis((begin.x - end.x) * 90.0f, Vector3.up));
		});
	
		ts.push (tt);
		ts.push (t2);
		
		TaskManager.PushBack (ts, delegate {
			GameObject.DestroyObject(s.gameObject);
		});
		return ts;
	}

	public Square getSquare (int x, int y){

		int n = x + y * 4;
		return list_[n];
	}

}
