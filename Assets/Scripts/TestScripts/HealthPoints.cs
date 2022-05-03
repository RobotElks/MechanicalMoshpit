using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPoints : MonoBehaviour
{
    public int healthPoints = 100;




    public void getHit(int damage){
        if((healthPoints - damage) > 0){
            healthPoints = healthPoints - damage;
        }
        else{
            killed();
        }
    }

    public void healPowerUp(int heal){
        if((healthPoints + heal) < 100){
            healthPoints = healthPoints + heal;
        }
        else{
            healthPoints = 100;
        }
    }

    public void killed(){
        MonoBehaviour[] comps = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour c in comps)
        {
            if (c.GetType() != typeof(HealthPoints)) 
            {
                c.enabled = false;
            }
        }
        //this.renderer.material.color = Color.red;
    }
}
