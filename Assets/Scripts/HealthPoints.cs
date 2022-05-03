using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPoints : MonoBehaviour
{
    public int healthPoints = 100;
    ParticleSystem smoke;

    public void getHit(int damage){
        if((healthPoints - damage) > 0){
            healthPoints = healthPoints - damage;
        }
        else{
            healthPoints = 0;
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
        //this.GetComponent<MeshRenderer>().material.color = Color.red;
        //this.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
        this.transform.Find("SmallTank_Base").GetComponent<MeshRenderer>().material.color = Color.black;
        this.transform.Find("SmallTank_Tower").GetComponent<MeshRenderer>().material.color = Color.black;
        //this.transform.Find("Smoke").SetActive(true);
        smoke = this.GetComponentInChildren<ParticleSystem>();
        smoke.enableEmission = true;
    }
}
