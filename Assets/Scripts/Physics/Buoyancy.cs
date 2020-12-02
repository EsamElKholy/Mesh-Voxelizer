using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoyancy : ForceGenerator
{
    private float maxDepth;
    private float volume;
    private float liquidHeight;
    private float density;

    public Buoyancy(float maxDepth, float volume, float liquidHeight, float density = 1000)
    {
        this.maxDepth = maxDepth;
        this.volume = volume;
        this.liquidHeight = liquidHeight;
        this.density = density;
    }

    public override void UpdateForce(ref Particle particle, float dt)
    {
        float depth = particle.Position.y;

        if (depth >= liquidHeight + maxDepth)
        {
            return;
        }

        Vector3 force = Vector3.zero;

        if (depth <= liquidHeight - maxDepth)
        {
            force.y = density * volume;

            particle.AddForce(force);

            return;
        }

        force.y = density * volume * (depth - maxDepth - liquidHeight) / 2 * maxDepth;
        particle.AddForce(force);
    }
}
