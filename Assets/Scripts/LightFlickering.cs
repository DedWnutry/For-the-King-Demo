using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlickering : MonoBehaviour
{
    protected Light LightSource;
    [SerializeField] protected float IntervalMin;
    [SerializeField] protected float IntervalMax;
    protected float Timer;
    [SerializeField] protected float MaxGlow;
    [SerializeField] protected float MinGlow;

    void Start()
    {
        LightSource = GetComponent<Light>();
    }

    public virtual void Update()
    {
        Timer += Time.deltaTime;


        if (Timer > Random.Range(IntervalMin, IntervalMax))
        {
            LightSource.intensity = Random.Range(MinGlow, MaxGlow);

            Timer = 0;
        }
    }
}
