using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deform : MonoBehaviour
{
    public float speed = 100;
    public float amount = 0.1f;
    public float rotationSpeed = 100;
    private new Renderer renderer;
    private Material material;
    public Vector4 pivot;
    public float deformFactor = 0;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();

        if (renderer)
        {
            material = renderer.sharedMaterial;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.X))
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                ModifyX(amount * speed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                ModifyX(-amount * speed * Time.deltaTime);
            }
        }

        if (Input.GetKey(KeyCode.Y))
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                ModifyY(amount * speed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                ModifyY(-amount * speed * Time.deltaTime);
            }
        }

        if (Input.GetKey(KeyCode.Z))
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                ModifyZ(amount * speed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                ModifyZ(-amount * speed * Time.deltaTime);
            }
        }

        if (Input.GetKey(KeyCode.F))
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                ModifyFactor(amount * speed * Time.deltaTime);
                Camera.main.transform.Translate(0, 0, -amount * speed *2* Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                ModifyFactor(-amount * speed * Time.deltaTime);
                if (deformFactor > 0)
                {
                    Camera.main.transform.Translate(0, 0, amount * speed *2 * Time.deltaTime);
                }
            }
        }

        if (Input.GetKey(KeyCode.C))
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                Camera.main.transform.Translate(0, 0, -amount * speed * 5 * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Camera.main.transform.Translate(0, 0, amount * speed * 5 * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                Camera.main.transform.Translate(0, amount * speed * 5 * Time.deltaTime, 0);
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                Camera.main.transform.Translate(0, -amount * speed * 5 * Time.deltaTime, 0);
            }
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.parent.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.parent.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }

        if (!renderer)
        {
            renderer = GetComponent<Renderer>();
        }

        if (!material && renderer)
        {
            material = renderer.sharedMaterial;
        }

        UpdateMaterial();
    }

    public void UpdateMaterial()
    {
        if (material)
        {
            material.SetVector("_CenterPivot", pivot);
            material.SetFloat("_DeformFactor", deformFactor);
        }
    }

    private void ModifyX(float amount)
    {
        pivot.x += amount;
    }

    private void ModifyY(float amount)
    {
        pivot.y += amount;
    }

    private void ModifyZ(float amount)
    {
        pivot.z += amount;
    }

    private void ModifyFactor(float amount)
    {
        deformFactor += amount;

        if (deformFactor < 0)
        {
            deformFactor = 0;
        }
    }
}
