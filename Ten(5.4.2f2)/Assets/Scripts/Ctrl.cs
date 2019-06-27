using UnityEngine;
using System.Collections;
using GDGeek;
using System;

public class Ctrl : MonoBehaviour
{
    private FSM fsm = new FSM();
    public View view = null;
    public Model model = null;

    void Start()
    {
        fsm.addState("begin", BeginState());
        fsm.addState("play", PlayState());
        fsm.addState("end", EndState());
        fsm.addState("input", InputState(), "play");
        fsm.addState("fall", FallState(), "play");
        fsm.addState("remove", RemoveState(), "play");
        fsm.init("input");
    }

    //输入状态
    private State InputState()
    {
        int number = 0;
        StateWithEventMap state = new StateWithEventMap();

        state.onStart += delegate
        {
            Debug.Log("进入输入状态！");
            number = UnityEngine.Random.Range(3, 8);
            //number = 5;
            Cube c = model.GetCube(1, 2);
            c.number = number;
            c.isEnabled = true;
            RefreshModel2View();
        };

        state.addAction("1", delegate (FSMEvent evt)
        {
            Input(1, number);
            return "fall";
        });
        state.addAction("2", delegate (FSMEvent evt)
        {
            Input(2, number);
            return "fall";
        });
        state.addAction("3", delegate (FSMEvent evt)
        {
            Input(3, number);
            return "fall";
        });
        state.addAction("4", delegate (FSMEvent evt)
        {
            Input(4, number);
            return "fall";
        });

        return state;
    }


    private void Input(int column, int number)
    {
        Cube c = model.GetCube(1, 2);
        c.isEnabled = false;
        Cube newCube = model.GetCube(1, column);
        newCube.isEnabled = true;
        newCube.number = number;
        RefreshModel2View();
    }

    //下落状态
    private State FallState()
    {
        StateWithEventMap state = TaskState.Create(delegate
        {
            Task doFallTask = DoFallTask();
            return doFallTask;
        }, fsm, "remove");

        return state;
    }

    private Task DoFallTask()
    {
        TaskSet ts = new TaskSet();

        for (int i = model.height; i >= 1; i--)
        {
            for (int j = 1; j <= model.width; j++)
            {
                Cube c = model.GetCube(i, j);
                Cube end = null;
                int endY = 1;
                if (c.isEnabled == true)
                {
                    for (int k = i + 1; k <= model.height; k++)
                    {
                        Cube fall = model.GetCube(k, j);
                        if (fall == null || fall.isEnabled == true)
                        {
                            break;
                        }
                        else
                        {
                            end = fall;
                            endY = k;
                        }
                    }
                    if (end != null)
                    {
                        end.number = c.number;
                        end.isEnabled = true;
                        c.isEnabled = false;
                        ts.push(view.play.MoveTask(c.number, new Vector2(i, j), new Vector2(endY, j)));
                    }
                }
            }
        }

        TaskManager.PushBack(ts, delegate
        {
            RefreshModel2View();
        });

        return ts;
    }

    //消除状态
    private State RemoveState()
    {
        bool isFall = false;
        StateWithEventMap state = TaskState.Create(delegate
        {
            Task task = new Task();
            TaskManager.PushFront(task, delegate
            {
                isFall = CheckAndRemove();
            });
            return task;
        }, fsm, delegate
        {
            if (isFall == true)
            {
                return "fall";
            }
            else
            {
                return "input";
            }
        });

        return state;
    }

    //检查是否消除
    public bool CheckAndRemove()
    {
        bool isRemove = false;
        for (int i = 1; i <= model.height; i++)
        {
            for (int j = 1; j <= model.width; j++)
            {
                Cube c = model.GetCube(i, j);
                if (c.isEnabled == true)
                {
                    Cube up = model.GetCube(i - 1, j);                                    
                    if (up != null && up.isEnabled == true && up.number + c.number == 10)
                    {                        
                        c.isEnabled = false;
                        up.isEnabled = false;
                        isRemove = true;
                        break;
                    }
                    Cube down = model.GetCube(i + 1, j);                   
                    if (down != null && down.isEnabled == true && down.number + c.number == 10)
                    {                       
                        c.isEnabled = false;
                        down.isEnabled = false;
                        isRemove = true;
                        break;
                    }
                    Cube left = model.GetCube(i, j - 1);                   
                    if (left != null && left.isEnabled == true && left.number + c.number == 10)
                    {
                        c.isEnabled = false;
                        left.isEnabled = false;
                        isRemove = true;
                        break;
                    }
                    Cube right = model.GetCube(i, j + 1);                   
                    if (right != null && right.isEnabled == true && right.number + c.number == 10)
                    {
                        c.isEnabled = false;
                        right.isEnabled = false;
                        isRemove = true;
                        break;
                    }
                }
            }
        }
        RefreshModel2View();//这里有必要刷新吗？
        return isRemove;
    }

    //开始状态
    private State BeginState()
    {
        StateWithEventMap state = new StateWithEventMap();
        state.onStart += delegate
        {
            view.begin.gameObject.SetActive(true);
        };
        state.onOver += delegate
        {
            view.begin.gameObject.SetActive(false);
        };
        state.addEvent("begin", "play");
        return state;
    }

    //玩的状态
    private State PlayState()
    {
        StateWithEventMap state = new StateWithEventMap();//带消息映射表的状态类。  
        state.onStart += delegate
        {
            view.play.gameObject.SetActive(true);
            RefreshModel2View();
        };
        state.onOver += delegate
        {
            view.play.gameObject.SetActive(false);
        };
        return state;
    }


    private void RefreshModel2View()
    {
        for (int i = 1; i <= model.height; i++)
        {
            for (int j = 1; j <= model.width; j++)
            {
                Square s = view.play.GetSquare(i, j);
                Cube c = model.GetCube(i, j);

                if (c.isEnabled)
                {
                    //Debug.Log(i.ToString() + "," + j.ToString());
                    s.Number = c.number;
                    s.Show();
                }
                else
                {
                    s.Hide();
                }
            }
        }

    }

    //结束状态
    private State EndState()
    {
        StateWithEventMap state = new StateWithEventMap();//带消息映射表的状态类。
        state.onStart += delegate
        {
            view.end.gameObject.SetActive(true);
        };
        state.onOver += delegate
        {
            view.end.gameObject.SetActive(false);
        };
        state.addEvent("end", "begin");
        return state;
    }

    public void FSMPost(string msg)
    {
        //Debug.Log(msg);
        fsm.post(msg);
    }

}
