using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDrag : ForceGenerator
{
    private float K1;
    private float K2;

    public ParticleDrag(float k1, float k2)
    {
        K1 = k1;
        K2 = k2;
    }

    public override void UpdateForce(ref Particle particle, float dt)
    {
        Vector3 force = particle.Velocity;

        float dragCoeff = force.magnitude;
        dragCoeff = K1 * dragCoeff + K2 * dragCoeff * dragCoeff;

        force = force.normalized;
        force *= -dragCoeff;

        particle.AddForce(force);
    }
}
