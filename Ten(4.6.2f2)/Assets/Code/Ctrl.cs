using UnityEngine;
using System.Collections;
using GDGeek;
using System.Collections.Generic;


public class Ctrl : MonoBehaviour {

	public Sound _sound = null;
	public int _nextNummber;
	
	public int _nextX;

	public View _view = null;
	public Model _model = null;
	public int _toX = -1;
	private Cube curr_ = null;
	public int _currX = 0;
	public int _currY = 0;
	private int number_ = 0;

	
	public float _speed = 1.0f;
	public Task endTask ()
	{
		TweenTask tt = new TweenTask (delegate {
			return TweenGroupAlpha.Begin(_view.end.gameObject, 0.3f, 1.0f);
				});
		TaskManager.PushFront (tt, delegate {
			_view.end._cg.alpha = 0;
			_view.end._score.text = _model.score.ToString();
			_view.end.gameObject.SetActive(true);
				});
		return tt;
	}

	public Task removeTask (List<Cube> remove)
	{
		TaskSet ts = new TaskSet ();
		if (remove.Count > 0) {
			TaskManager.PushFront(ts, delegate {
				_sound.remove.Play();
			});
			foreach (Cube c in remove) {
				c.isEnabled = false;
				Square s = _view.play.getSquare((int)(c.position.x), (int)(c.position.y));
				ts.push(s.removeTask());
				this._model.score++;
				float a = ((float)(this._model.score))/500.0f;
				_speed = 1.0f*(1-a) + 10.0f*a;
				Time.timeScale = _speed;
				Debug.Log ("speed" + _speed);
			}


		}
		TaskManager.PushBack (ts, delegate {

			refreshModel2View ();
				});
		return ts;
	}

	public Task overTask(){
		TaskSet tl = new TaskSet ();
		for(int y = 0; y<_model.height; ++y){
			for (int x =0; x < _model.width; ++x) {
				
				Cube c = _model.getCube(x, y);
				if(c.isEnabled){
					Square s = _view.play.getSquare(x, y);
					tl.push (s.overTask());
				}
				
			}		
		}
		TaskWait tw = new TaskWait ();
		tw.setAllTime (0.5f);
		tl.push (tw);
		return tl;
	}
	public Task doFallTask ()
	{
		
		TaskSet ts = new TaskSet ();
		
		for (int x =0; x < _model.width; ++x) {
			for(int y = _model.height - 1; y >= 0; --y){
				
				Cube c = _model.getCube(x, y);
				Cube end = null;
				int endY = 0;
				if(c.isEnabled){
					for(int n = y+1; n < _model.height; ++n){
						Cube fall = _model.getCube(x, n);
						if(fall == null || fall.isEnabled == true){
							break;
						}else{
							end = fall;
							endY = n;
						}
					}
					if(end != null)
					{
						end.number = c.number;
						end.isEnabled = true;
						c.isEnabled = false;
						ts.push (_view.play.moveTask(c.number, new Vector2(x, y), new Vector2(x, endY)));
						TaskManager.PushBack(ts, delegate {
							_sound.fall.Play();
						});
					}
				}
				
				
			}		
		}
		TaskManager.PushBack (ts, delegate() {
			refreshModel2View ();
		});
		return ts;
	}
	public List<Cube> checkRemove ()
	{
		List<Cube> remove = new List<Cube> ();
		for (int x =0; x <_model.width; ++x) {
			for(int y = 0; y<_model.height; ++y){
				
				Cube c = _model.getCube(x, y);
				if(c.isEnabled == true){
					Debug.LogWarning("x" + x +"y"+y);
					Cube up = _model.getCube (x, y-1);
					Cube down = _model.getCube (x, y+1);
					Cube left = _model.getCube (x-1, y);
					Cube right = _model.getCube (x+1, y);
					if(up != null && up.isEnabled == true && up.number + c.number == 10){
						remove.Add(c);
					}else if(down != null && down.isEnabled == true && down.number + c.number == 10){
						remove.Add(c);
					}else if(left != null && left.isEnabled == true && left.number + c.number == 10){
						remove.Add(c);
					}else if(right != null && right.isEnabled == true && right.number + c.number == 10){
						remove.Add(c);
					}
				}
			}		
		}
	
		return remove;
	}


	public Task moveXTask ()
	{
		Task task = this._view.play.moveTask(this.curr_.number, new Vector2(_currX, _currY), new Vector2(_toX, _currY));
		TaskManager.PushFront (task, delegate {
			this.curr_.isEnabled = false;
			_currX = _toX;
			
			this.curr_ = this._model.getCube(_currX, _currY);
			this.curr_.isEnabled = true;
			this.curr_.number = number_;

				});
		TaskManager.PushBack (task, delegate {
			refreshModel2View();
		});
		return task;
	}


	



	public void refreshModel2View (){
		_view.play._hud.score = this._model.score;
		for (int x =0; x <_model.width; ++x) {
			for(int y = 0; y<_model.height; ++y){
				Square s = _view.play.getSquare(x, y);
				Cube c = _model.getCube(x, y);
				if(c.isEnabled){
					s.number = c.number;
					s.show();
				}else{
					s.hide();
				}
			}		
		}
	}
	public void getNext(){
		if (this._model.score < 1) {
			_nextNummber = 5;
		}else if (this._model.score < 20) {
						_nextNummber = Random.Range (4, 7);
				} else if (this._model.score < 100) {
			
						_nextNummber = Random.Range (3, 8);
				} else if (this._model.score < 200) {
			
						_nextNummber = Random.Range (2, 9);
				} else {
			
			_nextNummber = Random.Range (1, 10);
		}
		_nextX =  Random.Range(0,3);
		this._view.play._hud.setNext (_nextX, _nextNummber);
	}



	public Task createTask ()
	{
	
		Task task = new Task();

		TaskManager.PushBack (task, delegate {
			_currX =  _nextX;
			number_ = _nextNummber;
			_currY = 0;
			curr_ = _model.getCube(_currX, _currY);
			curr_.number = number_;
			curr_.isEnabled = true;
			refreshModel2View ();
			getNext();
		});
	
		return task;
	}

	public bool check ()
	{
		Cube next  = this._model.getCube(_currX, _currY +1);
		if (next == null || next.isEnabled == true) {
			return false;		
		}
		return true;
	}

	public bool checkToX (int toX)
	{
		if(toX > _currX){
			for (int x = _currX+1; x<= toX; ++x) {
				Cube c = _model.getCube(x, _currY);
				if(c == null || c.isEnabled == true){

					_sound.error.Play();
					return false;
				}
			}

		}else if(toX < _currX){
			for (int x = _currX-1; x>= toX; --x) {
				Cube c = _model.getCube(x, _currY+1);
				if(c == null || c.isEnabled == true){
					_sound.error.Play();
					return false;
				}
			}
			
		}
		return true;
	}





	public Task fallOneTask ()
	{
		Task task = this._view.play.moveTask(this.curr_.number, new Vector2(_currX, _currY), new Vector2(_currX, _currY+1));
		TaskManager.PushFront (task, delegate {
			this.curr_.isEnabled = false;
			_currY = _currY+1;
			
			this.curr_ = this._model.getCube(_currX, _currY);
			this.curr_.isEnabled = true;
			this.curr_.number = number_;
				});
		TaskManager.PushBack (task, delegate {
		
			refreshModel2View();
				});
		return task;
	}




}
