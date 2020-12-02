using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle
{
    public Vector3 Position;
    public Vector3 Velocity;
    public Vector3 Acceleration;

    public float Damping;
    private float InverseMass;
    private Vector3 ForceAccumulator;

    public Particle(Vector3 pos, float mass)
    {
        Position = pos;
        Velocity = Vector3.zero;
        Acceleration = Vector3.zero;
        SetMass(mass);
        Damping = 1;
    }

    public void SetMass(float mass)
    {
        if (mass > 0)
        {
            InverseMass = 1 / mass;
        }
        else
        {
            InverseMass = float.MaxValue;
        }
    }

    public void Integrate(float dt)
    {
        Position = Position + Velocity * dt;

        Vector3 resultingAcceleration = Acceleration;

        //Vector3 gravityForce = new Vector3(0, -5, 0);
        resultingAcceleration = resultingAcceleration + ForceAccumulator * InverseMass;

        Velocity = Velocity + resultingAcceleration * dt;
        Velocity = Velocity * Damping;
        //Velocity = Velocity * Mathf.Pow(Damping, dt);

        ClearForceAccumulator();
    }

    public void AddForce(Vector3 force)
    {
        ForceAccumulator += force;
    }

    public void ClearForceAccumulator()
    {
        ForceAccumulator = Vector3.zero;
    }

    public bool HasInfiniteMass()
    {
        if (InverseMass == float.MaxValue)
        {
            return true;
        }

        return false;
    }
}
