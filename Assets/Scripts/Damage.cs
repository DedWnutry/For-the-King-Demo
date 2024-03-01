using UnityEngine;

public enum DamageType { Slash, Blunt, Thrust, Fire, Cold, Poison, Light }

[System.Serializable]
public class Damage
{
    public int value;
    public DamageType dmgType;
}