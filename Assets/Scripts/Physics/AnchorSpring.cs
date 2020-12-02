using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorSpring : ForceGenerator
{
    private Vector3 anchorPosition;
    private float springConstant;
    private float restLength;

    public AnchorSpring(Vector3 position, float constant, float length)
    {
        anchorPosition = position;
        springConstant = constant;
        restLength = length;
    }

    public override void UpdateForce(ref Particle particle, float dt)
    {
        Vector3 force = particle.Position;
        force -= anchorPosition;

        float magnitude = force.magnitude;
        magnitude = Mathf.Abs(magnitude - restLength);
        magnitude *= springConstant;

        force = force.normalized;
        force *= -magnitude;

        particle.AddForce(force);
    }
}
