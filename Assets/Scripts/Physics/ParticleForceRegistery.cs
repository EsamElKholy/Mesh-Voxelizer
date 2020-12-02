using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleForceRegistery 
{
    protected struct ParticleForceRegisteration
    {
        public Particle particle;
        public ForceGenerator forceGenerator;
    }

    private List<ParticleForceRegisteration> registerations = new List<ParticleForceRegisteration>();

    public void Add(ref Particle particle, ForceGenerator forceGenerator)
    {
        ParticleForceRegisteration forceRegisteration = new ParticleForceRegisteration();
        forceRegisteration.particle = particle;
        forceRegisteration.forceGenerator = forceGenerator;

        registerations.Add(forceRegisteration);
    }

    public void Remove(Particle particle, ForceGenerator forceGenerator)
    { }

    public void Clear()
    {
        registerations.Clear();
    }

    public void UpdateForces(float dt)
    {
        for (int i = 0; i < registerations.Count; i++)
        {
            var registery = registerations[i];
            registery.forceGenerator.UpdateForce(ref registery.particle, dt);
            registerations[i] = registery;
        }
    }
}
