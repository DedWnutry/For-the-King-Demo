using UnityEngine;

public interface IHitable
{
    void GetHit(float amount, DamageType type, Transform part);

    SoundManager SM { get; set; }
}
