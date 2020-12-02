using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleGravity : ForceGenerator
{
    public Vector3 gravity;

    public ParticleGravity(Vector3 gravity)
    {
        this.gravity = gravity;
    }

    public override void UpdateForce(ref Particle particle, float dt)
    {
        if (particle.HasInfiniteMass())
        {
            return;
        }

        particle.AddForce(gravity);
    }
}
