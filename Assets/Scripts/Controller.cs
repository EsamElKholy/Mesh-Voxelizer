using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Transform voxelHightLighter;
    public Transform aimTarget;

    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private Camera mainCamera;

    private VoxelManager manager;
    private Ray cameraRay;

    // Start is called before the first frame update
    void Start()
    {
        cameraRay = new Ray();
       
        manager = GameObject.FindObjectOfType<VoxelManager>();
        mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInput();
        UpdateCameraRay();

        transform.Rotate(Vector3.up * mouseHorizontal);

        mainCamera.transform.Rotate(Vector3.right * -mouseVertical);

        if (Input.GetKey(KeyCode.R))
        {
            var nodes = manager.voxelOctree2.CheckRay(cameraRay, aimTarget);
            if (nodes.Count > 0)
            {
                var closestNode = GetClosestNode(ref nodes);

                if (closestNode.Index >= 0)
                {
                    var pos = aimTarget.TransformPoint(closestNode.Position);

                    voxelHightLighter.position = pos;
                    voxelHightLighter.gameObject.SetActive(true);

                    if (Input.GetMouseButtonUp(0))
                    {
                        manager.voxelOctree2.Nodes[closestNode.Index].Value = 0;
                        manager.UpdateFilledNodes();
                        manager.UpdateMesh(aimTarget.GetChild(0).gameObject);
                    }
                }
                else
                {
                    voxelHightLighter.gameObject.SetActive(false);
                }
            }
            else
            {
                voxelHightLighter.gameObject.SetActive(false);
            }
        }
        else 
        {
            voxelHightLighter.gameObject.SetActive(false);
        }
    }

    private Node GetClosestNode(ref List<Node> nodes) 
    {
        if (nodes.Count == 1)
        {
            return nodes[0];
        }
        else if (nodes.Count > 1)
        {
            float distance = float.MaxValue;
            int index = -1;

            for (int i = 0; i < nodes.Count; i++)
            {
                float dist = Vector3.Distance(transform.position, nodes[i].Position);

                if (dist < distance)
                {
                    distance = dist;
                    index = i;
                }
            }

            if (index >= 0)
            {
                return nodes[index];
            }
        }

        Node nullNode = new Node();
        nullNode.Index = -1;

        return nullNode;
    }

    private void UpdateCameraRay() 
    {
        cameraRay.origin = mainCamera.transform.parent.position;
        cameraRay.direction = mainCamera.transform.forward;
    }

    private void UpdateInput() 
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");
    }
}
