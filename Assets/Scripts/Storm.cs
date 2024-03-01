using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storm : LightFlickering
{
    [SerializeField] private bool _delay;
    [SerializeField] private AudioSource _lightningStrike;

    public override void Update()
    {
        if (_delay) LightSource.intensity = MinGlow;

        Timer += Time.deltaTime;

        if (Timer > Random.Range(IntervalMin, IntervalMax))
        {
            _delay = false;

            if (!_delay) StartCoroutine(Lightning());

            Timer = 0;
        }
    }

    IEnumerator Lightning()
    {
        LightSource.intensity = MaxGlow;

        StartCoroutine(StrikeSoundDelay());

        yield return new WaitForSeconds(1f);

        _delay = true;
    }

    IEnumerator StrikeSoundDelay()
    {
        yield return new WaitForSeconds(Random.Range(1, 5));

        _lightningStrike.Play(0);
    }
}
