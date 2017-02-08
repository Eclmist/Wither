using UnityEngine;
using System.Collections;

public class ParticleToggle : MonoBehaviour {

    public void ToggleParticles(bool active)
    {
        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ParticleSystem.EmissionModule em = ps.emission;
            em.enabled = active;
        }
    }

}
