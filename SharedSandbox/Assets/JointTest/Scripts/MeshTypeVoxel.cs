using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTypeVoxel : MonoBehaviour {

	// VARIABLES
    public MeshFilter  [] meshTube = new MeshFilter  [64];
    private MeshFilter  [,,,,,] meshCode = new MeshFilter  [2, 2, 2, 2, 2, 2];

    public void setupMesh(int _f, int _b , int _l, int _r, int _u, int _d)
    {
        MeshFilter meshType = gameObject.AddComponent<MeshFilter>();
        meshCode[0,0,0,0,0,0] = meshTube[0];//
        meshCode[0,0,0,0,0,1] = meshTube[1];//
        meshCode[0,0,0,0,1,0] = meshTube[2];//
        meshCode[0,0,0,1,0,0] = meshTube[3];//
        meshCode[0,0,1,0,0,0] = meshTube[4];//
        meshCode[0,1,0,0,0,0] = meshTube[5];//
        meshCode[1,0,0,0,0,0] = meshTube[6];//
        meshCode[1,0,1,0,0,0] = meshTube[7];//
        meshCode[1,1,0,0,0,0] = meshTube[8];//
        meshCode[1,0,0,1,0,0] = meshTube[9];//
        meshCode[0,1,0,1,0,0] = meshTube[10];//
        meshCode[0,1,1,0,0,0] = meshTube[11];//
        meshCode[0,1,0,0,1,0] = meshTube[12];//
        meshCode[0,1,0,0,0,1] = meshTube[13];//
        meshCode[1,0,0,0,0,1] = meshTube[14];//
        meshCode[1,0,0,0,1,0] = meshTube[15];//
        meshCode[0,0,0,1,1,0] = meshTube[16];//
        meshCode[0,0,0,1,0,1] = meshTube[17];//
        meshCode[0,0,1,0,0,1] = meshTube[18];//
        meshCode[0,0,1,0,1,0] = meshTube[19];//
        meshCode[0,0,0,0,1,1] = meshTube[20];//
        meshCode[0,0,1,1,0,0] = meshTube[21];//
        meshCode[1,0,1,1,0,0] = meshTube[22];//
        meshCode[1,0,1,0,1,0] = meshTube[23];//
        meshCode[1,0,1,0,0,1] = meshTube[24];//
        meshCode[0,1,1,0,0,1] = meshTube[25];//
        meshCode[0,1,1,0,1,0] = meshTube[26];//
        meshCode[0,1,0,1,1,0] = meshTube[27];//
        meshCode[1,0,0,1,1,0] = meshTube[28];//
        meshCode[1,0,0,1,0,1] = meshTube[29];//
        meshCode[0,1,0,1,0,1] = meshTube[30];//
        meshCode[1,1,0,1,0,0] = meshTube[31];//
        meshCode[1,1,1,0,0,0] = meshTube[32];//
        meshCode[0,0,0,1,1,1] = meshTube[33];//
        meshCode[0,0,1,0,1,1] = meshTube[34];
        meshCode[0,0,1,1,1,0] = meshTube[35];
        meshCode[0,0,1,1,0,1] = meshTube[36];
        meshCode[1,1,0,0,0,1] = meshTube[37];
        meshCode[1,1,0,0,1,0] = meshTube[38];
        meshCode[0,1,0,0,1,1] = meshTube[39];
        meshCode[1,0,0,0,1,1] = meshTube[40];
        meshCode[0,1,1,1,0,0] = meshTube[41];
        meshCode[1,0,1,0,1,1] = meshTube[42];
        meshCode[1,1,1,1,0,0] = meshTube[43];
        meshCode[0,0,1,1,1,1] = meshTube[44];
        meshCode[1,1,0,0,1,1] = meshTube[45];
        meshCode[1,0,1,1,0,1] = meshTube[46];
        meshCode[1,0,1,1,1,0] = meshTube[47];
        meshCode[0,1,1,1,0,1] = meshTube[48];
        meshCode[0,1,1,1,1,0] = meshTube[49];
        meshCode[1,1,1,0,1,0] = meshTube[50];
        meshCode[1,1,1,0,0,1] = meshTube[51];
        meshCode[1,1,0,1,0,1] = meshTube[52];
        meshCode[1,1,0,1,1,0] = meshTube[53];
        meshCode[0,1,1,0,1,1] = meshTube[54];
        meshCode[0,1,0,1,1,1] = meshTube[55];
        meshCode[1,0,0,1,1,1] = meshTube[56];
        meshCode[1,0,1,1,1,1] = meshTube[57];
        meshCode[1,1,1,0,1,1] = meshTube[58];
        meshCode[1,1,0,1,1,1] = meshTube[59];
        meshCode[1,1,1,1,1,0] = meshTube[60];
        meshCode[1,1,1,1,0,1] = meshTube[61];
        meshCode[0,1,1,1,1,1] = meshTube[62];
        meshCode[1,1,1,1,1,1] = meshTube[63];
        
        if (meshCode[_f, _b, _l, _r, _u, _d] != null)
        {
            Mesh _mesh = meshCode[_f, _b, _l, _r, _u, _d].sharedMesh;
            meshType.sharedMesh = _mesh;
        }
    }

 

 



    
}
