using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainBox : MonoBehaviour
{

    private MeshFilter mf ;	
	private Vector3 defaultPosition;
	private Bounds bounds;

	private RainManager manager;
	
	private Transform cachedTransform;
	private float cachedMinY;
	private float cachedAreaHeight;
	private float cachedFallingSpeed;

    private Quaternion rotation;
    private Transform cam;

    [Range (1f, 5f)]
    public float rainSparsity = 1f; // larger the rainSparsity, lighter the rain

    void Awake()
    {
        rotation = Quaternion.identity;
        transform.rotation = rotation;
    }

    void Start()
    {
        manager = transform.parent.GetComponent<RainManager> ();

        bounds = new Bounds(new Vector3(transform.position.x, manager.minYPosition, transform.position.z),
                             new Vector3(manager.areaSize * 1.35f, Mathf.Max(manager.areaSize, manager.areaHeight) * 1.35f, manager.areaSize * 1.35f));

        mf = GetComponent<MeshFilter> ();
        mf.sharedMesh = manager.GetPreGennedMesh();

        cachedTransform = transform;
        cam = transform.parent.transform;
        cachedMinY = manager.minYPosition;
        cachedAreaHeight = manager.areaHeight;
        cachedFallingSpeed = manager.fallingSpeed;
    }


    void Update()
    {
        cachedTransform.position -= Vector3.up * Time.deltaTime * cachedFallingSpeed;

        if (cachedTransform.position.y  < cam.position.y - 5f)
        {
            cachedTransform.localPosition = Vector3.zero;
            mf.sharedMesh = manager.GetPreGennedMesh();//change rain pattern periodically
        }

        cachedTransform.rotation = rotation;//keep the rain always falling vertically instead of rotating with camera(parent)

        cachedTransform.localScale = new Vector3(rainSparsity, rainSparsity, rainSparsity);
    }

    void OnDrawGizmos()
    {
    #if UNITY_EDITOR
        // do not display a weird mesh in edit mode
        if (!Application.isPlaying)
        {
            mf = GetComponent< MeshFilter > ();
            mf.sharedMesh = null;
        }
    #endif

        if (transform.parent)
        {
            Gizmos.color =  new Color(0.2f, 0.3f, 3.0f, 0.35f);
            RainManager manager = transform.parent.GetComponent<RainManager>();
            if (manager)
                Gizmos.DrawWireCube(transform.position + transform.up * manager.areaHeight * 0.5f,
                                        new Vector3(manager.areaSize, manager.areaHeight, manager.areaSize));
        }
    }


}
