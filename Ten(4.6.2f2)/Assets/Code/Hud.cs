using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Hud : MonoBehaviour {


	public Image[] _images = null;
	public Sprite[] _number = null;
	public Text _score;
	public void Awake(){
		foreach (Image i in _images) {
			i.enabled = false;		
		}
	}


	public int score {
		
		set{
			this._score.text = value.ToString();
		}
	}
	public void setNext(int x, int number){
		Debug.Log ("x:" + x + "number" + number);
		foreach (Image i in _images) {
			i.enabled = false;		
		}
		_images [x].sprite = _number [number-1];
		_images [x].enabled = true;
	}
}
