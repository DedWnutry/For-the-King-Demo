using System.Collections.Generic;


public interface ICharStats
{
    void ChangeHealth(float value);

    void ChangeStamina(float value);

    void ChangeMana(float value);

    void ChangeExperience(int value);

    void ChangePoise(float value);

    int ChangeEquipload(float value);

    int lvl { get; set; }
    int exp { get; set; }
    int nextLvlExp { get; set; }
    int STR { get; set; }
    int DEX { get; set; }
    int INT { get; set; }
    float maxHP { get; set; }
    float maxSP { get; set; }
    float maxMP { get; set; }
    float curHP { get; set; }
    float curSP { get; set; }
    float curMP { get; set; }
    float healthRegen { get; set; }
    float staminaRegen { get; set; }
    float manaRegen { get; set; }
    int loadStage { get; set; }
    float maxOxygen { get; set; }
    float curOxygen { get; set; }
}
