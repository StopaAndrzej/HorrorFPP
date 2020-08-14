using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenWindowScript : MonoBehaviour
{
    [SerializeField] private Window1_Manager window;
    [SerializeField] private List<GameObject> kitchenSmokeParticles;

    public void KillParticles()
    {
        foreach(GameObject el in kitchenSmokeParticles)
        {
            el.GetComponent<ParticleSystem>().loop = false;
        }
    }

}
