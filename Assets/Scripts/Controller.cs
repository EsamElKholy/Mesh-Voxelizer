using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Transform voxelHightLighter;
    public Transform voxelSphereHighLighter;

    public Transform aimTarget;
    public float sphereCastRaduis = 2f;

    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private Camera mainCamera;

    private VoxelManager manager;
    private Ray cameraRay;
    public enum EditModes
    {
        RAY,
        SPHERE
    }

    private EditModes currentEditMode = EditModes.SPHERE;

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
            if (Input.GetKeyUp(KeyCode.S))
            {
                if (currentEditMode == EditModes.RAY)
                {
                    voxelHightLighter.gameObject.SetActive(false);
                    voxelSphereHighLighter.gameObject.SetActive(false);
                    
                    currentEditMode = EditModes.SPHERE;
                }
                else
                {
                    voxelHightLighter.gameObject.SetActive(false);
                    voxelSphereHighLighter.gameObject.SetActive(false);

                    currentEditMode = EditModes.RAY;
                }
            }

            switch (currentEditMode)
            {
                case EditModes.RAY:
                    RaycastSelection();
                    break;
                case EditModes.SPHERE:
                    SpherecastSelection();
                    break;
                default:
                    break;
            }
        }
        else 
        {
            voxelHightLighter.gameObject.SetActive(false);
            voxelSphereHighLighter.gameObject.SetActive(false);
        }
    }

    private void RaycastSelection() 
    {
        var nodes = manager.voxelOctree2.CheckRay(cameraRay, aimTarget);
        if (nodes.Count > 0)
        {
            var closestNode = GetClosestNode(ref nodes, transform.position);

            if (closestNode.Index >= 0)
            {
                var pos = aimTarget.TransformPoint(closestNode.Position);

                voxelHightLighter.position = pos;
                voxelHightLighter.transform.localScale = Vector3.one * closestNode.Size;
                voxelHightLighter.gameObject.SetActive(true);

                if (Input.GetMouseButton(0))
                {
                    //FillIfNeeded(cameraRay, closestNode);
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

    private void SpherecastSelection() 
    {
        var nodes = manager.voxelOctree2.CheckRay(cameraRay, aimTarget);
        if (nodes.Count > 0)
        {
            var closestNode = GetClosestNode(ref nodes, transform.position);

            if (closestNode.Index >= 0)
            {
                var pos = aimTarget.TransformPoint(closestNode.Position);

                voxelSphereHighLighter.position = pos;
                voxelSphereHighLighter.transform.localScale = Vector3.one * sphereCastRaduis * 2;
                voxelSphereHighLighter.gameObject.SetActive(true);

                if (Input.GetMouseButtonUp(0))
                {
                    //manager.DisableNodesInSphere(closestNode.Position, sphereCastRaduis);
                    var spherecastNodes = manager.voxelOctree2.CastSphere(closestNode.Position, sphereCastRaduis);

                    if (spherecastNodes.Count > 0)
                    {
                        foreach (var node in spherecastNodes)
                        {
                            manager.voxelOctree2.Nodes[node.Index].Value = 0;
                        }
                        manager.voxelOctree2.Nodes[closestNode.Index].Value = 0;

                        manager.UpdateFilledNodes();
                        manager.UpdateMesh(aimTarget.GetChild(0).gameObject);
                    }                    
                }

                if (Input.GetMouseButtonUp(1))
                {
                    manager.EnableNodesInSphere(closestNode.Position, sphereCastRaduis);
                    manager.UpdateFilledNodes();
                    manager.UpdateMesh(aimTarget.GetChild(0).gameObject);
                }
            }
            else
            {
                voxelSphereHighLighter.gameObject.SetActive(false);
            }
        }
        else
        {
            voxelSphereHighLighter.gameObject.SetActive(false);
        }
    }

    private Node GetClosestNode(ref List<Node> nodes, Vector3 targetPosition) 
    {
        if (nodes.Count == 1)
        {
            return nodes[0];
        }
        else if (nodes.Count > 1)
        {
            float distance = 9999999999;
            int index = -1;

            for (int i = 0; i < nodes.Count; i++)
            {
                float dist = Vector3.Distance(targetPosition, aimTarget.TransformPoint(nodes[i].Position));

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

    private void FillIfNeeded(Ray aimRay, Node closestNode) 
    {
        List<Ray> rays = new List<Ray>();

        Vector3 newForward = aimTarget.TransformDirection(Vector3.forward);
        Vector3 newBack = aimTarget.TransformDirection(Vector3.back);
        Vector3 newRight = aimTarget.TransformDirection(Vector3.right);
        Vector3 newLeft = aimTarget.TransformDirection(Vector3.left);
        Vector3 newUp = aimTarget.TransformDirection(Vector3.up);
        Vector3 newDown = aimTarget.TransformDirection(Vector3.down);

        rays.Add(new Ray(closestNode.Position + (closestNode.Size / 1.9f * newForward), newForward));
        rays.Add(new Ray(closestNode.Position + (closestNode.Size / 1.9f * newBack), newBack));
        rays.Add(new Ray(closestNode.Position + (closestNode.Size / 1.9f * newRight), newRight));
        rays.Add(new Ray(closestNode.Position + (closestNode.Size / 1.9f * newLeft), newLeft));
        rays.Add(new Ray(closestNode.Position + (closestNode.Size / 1.9f * newUp), newUp));
        rays.Add(new Ray(closestNode.Position + (closestNode.Size / 1.9f * newDown), newDown));

        Ray rayFromClosestNode = new Ray(closestNode.Position + (closestNode.Size / 1.9f * aimRay.direction), aimRay.direction);

        List<List<Node>> nodes = new List<List<Node>>();

        for (int i = 0; i < rays.Count; i++)
        {
            Ray worldRay = new Ray((rays[i].origin), rays[i].direction);
            var filledNodes = manager.voxelOctree2.CheckRay(worldRay, aimTarget);
            if (filledNodes.Count > 1)
            {
                Vector3 position = rays[i].origin;
                nodes.Add(manager.voxelOctree2.GetNode(position));
            }
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = 0; j < nodes[i].Count; j++)
            {
                if (nodes[i][j].Index >= 0)
                {
                    manager.voxelOctree2.Nodes[nodes[i][j].Index].Value = 1;
                }
            }
        }
    }

    private void UpdateCameraRay() 
    {
        cameraRay.origin = mainCamera.transform.position;
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
