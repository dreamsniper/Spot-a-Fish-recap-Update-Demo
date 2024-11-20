using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Board_C : MonoBehaviour
{


    int gem_damage_opponent_max_value;

    void Find_gem_damage_opponent_max_value()
    {
        gem_damage_opponent_max_value = 0;

        for (int i = 0; i < myRuleset.gem_length; i++)
        {
            if (myRuleset.gemExplosionOutcomes[i].damageOpponent > gem_damage_opponent_max_value)
                gem_damage_opponent_max_value = myRuleset.gemExplosionOutcomes[i].damageOpponent;
        }
    }

    public void DamageActiveCharacter(int gemColor, int damage)
    {
        print("DamageActiveCharacter: " + damage);
        switch (activeCharacter.myCharacter.armor[gemColor])
        {
            case Character.gemColorArmor.weak:
                activeCharacter.myCharacter.currentHp -= damage * 2;
                break;


            case Character.gemColorArmor.normal:
                activeCharacter.myCharacter.currentHp -= damage;
                break;


            case Character.gemColorArmor.strong:
                activeCharacter.myCharacter.currentHp -= (int)(damage * 0.5f);
                break;


            case Character.gemColorArmor.immune:

                break;

            case Character.gemColorArmor.absorb:
                if ((activeCharacter.myCharacter.currentHp + damage) > activeCharacter.myCharacter.maxHp)
                    activeCharacter.myCharacter.currentHp = activeCharacter.myCharacter.maxHp;
                else
                    activeCharacter.myCharacter.currentHp += damage;
                break;

            case Character.gemColorArmor.repel:
                if ((passiveCharacter.myCharacter.currentHp - damage) < 0)
                    passiveCharacter.myCharacter.currentHp = 0;
                else
                    passiveCharacter.myCharacter.currentHp -= damage;
                break;
        }

        if (activeCharacter.myCharacter.currentHp < 0)
            activeCharacter.myCharacter.currentHp = 0;
    }

    public void DamagePassiveCharacter(int gemColor, int damage)
    {
        print("DamagePassiveCharacter: " + damage);
        switch (passiveCharacter.myCharacter.armor[gemColor])
        {
            case Character.gemColorArmor.weak:
                passiveCharacter.myCharacter.currentHp -= damage * 2;
                break;


            case Character.gemColorArmor.normal:
                passiveCharacter.myCharacter.currentHp -= damage;
                break;


            case Character.gemColorArmor.strong:
                passiveCharacter.myCharacter.currentHp -= (int)(damage * 0.5f);
                break;


            case Character.gemColorArmor.immune:

                break;

            case Character.gemColorArmor.absorb:
                if ((passiveCharacter.myCharacter.currentHp + damage) > passiveCharacter.myCharacter.maxHp)
                    passiveCharacter.myCharacter.currentHp = passiveCharacter.myCharacter.maxHp;
                else
                    passiveCharacter.myCharacter.currentHp += damage;
                break;

            case Character.gemColorArmor.repel:
                if ((activeCharacter.myCharacter.currentHp - damage) < 0)
                    activeCharacter.myCharacter.currentHp = 0;
                else
                    activeCharacter.myCharacter.currentHp -= damage;
                break;
        }

        if (passiveCharacter.myCharacter.currentHp < 0)
            passiveCharacter.myCharacter.currentHp = 0;
    }

}
