using UnityEngine;
using System;
using System.Collections;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PointCloudView : MonoBehaviour
{
    public GameObject CoordinateMapper;
    private CoordinateMapperManager _CoordinateMapper;

    public GameObject BodySourceManager;
    private BodySourceManager _BodyManager;
    public GameObject UserInterfaceManager;
    private UserInterface _InterfaceManager;

    private Mesh mesh;
    int numPoints = 54272;
    public int startPoint;
    public int endPoint;

    private Vector3 pointPos;

    // Use this for initialization
    void Start()
    {
        mesh = new Mesh();

        GetComponent<MeshFilter>().mesh = mesh;
        //CreateMesh();
    }

    void Update()
    {
        if (CoordinateMapper == null || BodySourceManager == null || UserInterfaceManager == null)
        {
            return;
        }
        _CoordinateMapper = CoordinateMapper.GetComponent<CoordinateMapperManager>();
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        _InterfaceManager = UserInterfaceManager.GetComponent<UserInterface>();
        if (_CoordinateMapper == null || _BodyManager == null || _InterfaceManager == null)
        {
            return;
        }

        Vector3[] points = new Vector3[numPoints];
        int[] indices = new int[numPoints];
        Color[] colors = new Color[numPoints];

        Vector3 floorNormal = new Vector3(_BodyManager.Floor.X, _BodyManager.Floor.Y, _BodyManager.Floor.Z);
        var rotFromFloortoKinect = Quaternion.FromToRotation(floorNormal, Vector3.up);
        //Vector3 floorPos = new Vector3(_BodyManager.Floor.X * _BodyManager.Floor.W, _BodyManager.Floor.Y * _BodyManager.Floor.W, _BodyManager.Floor.Z * _BodyManager.Floor.W);

        int i = 0;
        for (int cameraPoints = startPoint; cameraPoints <= endPoint; ++cameraPoints)
        {
            //pointPos = new Vector3(-_CoordinateMapper.m_pCameraCoordinates[cameraPoints].X,
            //    _CoordinateMapper.m_pCameraCoordinates[cameraPoints].Y,
            //    _CoordinateMapper.m_pCameraCoordinates[cameraPoints].Z);
            //pointPos = pointPos + floorPos;
            //pointPos = rotFromFloortoKinect * pointPos;
            //points[i] = pointPos;

            points[i] = new Vector3(-_CoordinateMapper.m_pCameraCoordinates[cameraPoints].X,
                _CoordinateMapper.m_pCameraCoordinates[cameraPoints].Y,
                _CoordinateMapper.m_pCameraCoordinates[cameraPoints].Z);

            if (points[i].x < -4)
            {
                points[i].x = 0;
            }
            else if (points[i].x > 4)
            {
                points[i].x = 0;
            }

            if (points[i].y < -4)
            {
                points[i].y = 0;
            }
            else if (points[i].y > 4)
            {
                points[i].y = 0;
            }

            if (points[i].z < 0)
            {
                points[i].z = 8;
            }
            else if (points[i].z > 8)
            {
                points[i].z = 8;
            }

            indices[i] = i;
            colors[i] = HSVToRGB((points[i].z / 8), 1, 1);
            i++;
        }

        mesh.vertices = points;
        mesh.colors = colors;
        mesh.SetIndices(indices, MeshTopology.Points, 0);

        transform.position = new Vector3(0, _BodyManager.Floor.W, 0);
        transform.Translate(_InterfaceManager.stickmanRoot);
        transform.rotation = rotFromFloortoKinect;
    }

    public static Color HSVToRGB(float H, float S, float V)
    {
        if (S == 0f)
            return new Color(V,V,V);
        else if (V == 0f)
            return new Color(0,0,0);
        else
        {
            Color col = Color.black;
            float Hval = H * 6f;
            int sel = Mathf.FloorToInt(Hval);
            float mod = Hval - sel;
            float v1 = V * (1f - S);
            float v2 = V * (1f - S * mod);
            float v3 = V * (1f - S * (1f - mod));

            switch (sel + 1)
            {
                case 0:
                    col.r = V;
                    col.g = v1;
                    col.b = v2;
                    break;
                case 1:
                    col.r = V;
                    col.g = v3;
                    col.b = v1;
                    break;
                case 2:
                    col.r = v2;
                    col.g = V;
                    col.b = v1;
                    break;
                case 3:
                    col.r = v1;
                    col.g = V;
                    col.b = v3;
                    break;
                case 4:
                    col.r = v1;
                    col.g = v2;
                    col.b = V;
                    break;
                case 5:
                    col.r = v3;
                    col.g = v1;
                    col.b = V;
                    break;
                case 6:
                    col.r = V;
                    col.g = v1;
                    col.b = v2;
                    break;
                case 7:
                    col.r = V;
                    col.g = v3;
                    col.b = v1;
                    break;
            }
            col.r = Mathf.Clamp(col.r, 0f, 1f);
            col.g = Mathf.Clamp(col.g, 0f, 1f);
            col.b = Mathf.Clamp(col.b, 0f, 1f);
            return col;
        }
    }
}
