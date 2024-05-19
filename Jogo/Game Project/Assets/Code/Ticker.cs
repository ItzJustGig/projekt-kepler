using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    private void OnEnable()
    {
        Ticker.OnTickAction += Tick;
    } 
    
    private void OnDisable()
    {
        Ticker.OnTickAction -= Tick;
    }

    private void Tick(){
        
    }
*/

public class Ticker : MonoBehaviour
{
    public static float tickTime = 0.2f;
    //public static float tickTime_075 = 0.75f;

    private float _tickTimer;

    public delegate void TickAction();
    public static event TickAction OnTickAction;

    //public delegate void Tick075Action();
    //public static event Tick075Action OnTick075Action;

    private void Update()
    {
        _tickTimer += Time.deltaTime;
        //_tickTimer_075 += Time.deltaTime;

        if (_tickTimer >= tickTime)
        {
            _tickTimer = 0;
            TickEvent();
        }

        /*if (_tickTimer_075 >= tickTime_075)
        {
            _tickTimer_075 = 0;
            TickEvent();
        }*/
    }

    private void TickEvent()
    {
        OnTickAction?.Invoke();
    }

    /*private void TickEvent075()
    {
        OnTick075Action?.Invoke();
    }*/
}
