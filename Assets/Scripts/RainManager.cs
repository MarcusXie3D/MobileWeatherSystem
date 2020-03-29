using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainManager : MonoBehaviour
{

    public float minYPosition = 0.0f;
	
	public int numberOfParticles = 400;
    public float areaSize = 40.0f;
    public float areaHeight = 15.0f;
	public float fallingSpeed = 23.0f;
	public float particleSize = 0.2f;
	public float flakeRandom = 0.1f;
	
	public Mesh[] preGennedMeshes;
	private int preGennedIndex = 0;		

	public bool generateNewAssetsOnStart = false;	

	public void Start()
{
#if UNITY_EDITOR
    if (generateNewAssetsOnStart)
    {
            // create & save 3 meshes
            Mesh m1  = CreateMesh();
            Mesh m2 = CreateMesh();
            Mesh m3   = CreateMesh();
        //AssetDatabase.CreateAsset(m1, "Assets/Objects/RainFx/" + gameObject.name + "_LQ0.asset");
        //AssetDatabase.CreateAsset(m2, "Assets/Objects/RainFx/" + gameObject.name + "_LQ1.asset");
        //AssetDatabase.CreateAsset(m3, "Assets/Objects/RainFx/" + gameObject.name + "_LQ2.asset");
        Debug.Log("Created new rain meshes in Assets/Objects/RainFx/");
    }
#endif
}

    public Mesh GetPreGennedMesh() 
    {
		return preGennedMeshes[(preGennedIndex++) % preGennedMeshes.Length];
	}

    Mesh CreateMesh()
    {
		var mesh = new Mesh();

        Vector3 cameraRight = Camera.main.transform.right;
        Vector3 cameraUp = (Vector3.up);

        int particleNum = numberOfParticles / 2;

        Vector3[] verts = new Vector3[4 * particleNum];
        Vector2[] uvs = new Vector2[4 * particleNum];
        Vector2[] uvs2 = new Vector2[4 * particleNum];
        Vector3[] normals = new Vector3[4 * particleNum];

        int[] tris = new int[2 * 3 * particleNum];

        Vector3 position;
		for (int i  = 0; i<particleNum; i++)
		{
            int i4 = i* 4;
            int i6 = i* 6;

			position.x = areaSize* (Random.value - 0.5f);
			position.y = areaHeight* Random.value;
position.z = areaSize* (Random.value - 0.5f);

            float rand = Random.value;
            float widthWithRandom = particleSize* 0.215f;// + rand * flakeRandom;
            float heightWithRandom = particleSize + rand* flakeRandom;

verts[i4 + 0] = position - cameraRight* widthWithRandom - cameraUp* heightWithRandom;
verts[i4 + 1] = position + cameraRight* widthWithRandom - cameraUp* heightWithRandom;
verts[i4 + 2] = position + cameraRight* widthWithRandom + cameraUp* heightWithRandom;
verts[i4 + 3] = position - cameraRight* widthWithRandom + cameraUp* heightWithRandom;

normals[i4 + 0] = -Camera.main.transform.forward;
			normals[i4 + 1] = -Camera.main.transform.forward;
			normals[i4 + 2] = -Camera.main.transform.forward;
			normals[i4 + 3] = -Camera.main.transform.forward;

			uvs[i4 + 0] = new Vector2(0.0f, 0.0f);
uvs[i4 + 1] = new Vector2(1.0f, 0.0f);
uvs[i4 + 2] = new Vector2(1.0f, 1.0f);
uvs[i4 + 3] = new Vector2(0.0f, 1.0f);

uvs2[i4 + 0] = new Vector2(Random.Range(-2,2)*4.0f, Random.Range(-1,1)*1.0f);
			uvs2[i4 + 1] = new Vector2(uvs2[i4 + 0].x, uvs2[i4 + 0].y);
uvs2[i4 + 2] = new Vector2(uvs2[i4 + 0].x, uvs2[i4 + 0].y);
uvs2[i4 + 3] = new Vector2(uvs2[i4 + 0].x, uvs2[i4 + 0].y);

tris[i6 + 0] = i4 + 0;
			tris[i6 + 1] = i4 + 1;
			tris[i6 + 2] = i4 + 2;
			tris[i6 + 3] = i4 + 0;
			tris[i6 + 4] = i4 + 2;
			tris[i6 + 5] = i4 + 3;
		}

		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.normals = normals;
		mesh.uv = uvs;
		mesh.uv2 = uvs2;
		mesh.RecalculateBounds();
		
		return mesh;
	}	
}
