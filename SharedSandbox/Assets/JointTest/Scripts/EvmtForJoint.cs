using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;
using SpatialSlur.SlurUnity;
using Random = System.Random;

public class EvmtForJoint : MonoBehaviour
{

    // VARIABLES
#region variables
    // Texture to be used as start of CA input
    public Texture2D[] seedImageStore = new Texture2D[27];
    private GOLRule[] ruleStore = new GOLRule[13];

    

    public GameObject TargetBox;

    public Color[] Spectrum;

    // Number of frames to run which is also the height of the CA
   

    int currentFrame = 0;
    private int frameThershold = 0;
     
    private int width;
    private int length;
    private int height;
    private float spacing = 1.0f;

    //Max Age
    int maxAge = 0;

    //Layer Density
    private float layerdensity = 0;

    private float[] layerDensities;

    private float[] conditionDensity;

    private float maxlayerdensity = 0;
    private float minlayerdensity = 10000;

    int LayerCount=0;

    int totalAliveCells = 0;

    int maxDensity3dMO = 0;

    private int[,,] Density3dVN;

    public Rigidbody Structure;

    public GameObject visibleVoxel;

    Rigidbody[] StructureGrid;

    GameObject [] visibleGrid;


    private Material[] _materials;
    private FixedJoint[][] StructureJoints;

    private float BreakForce = Mathf.Infinity;
    private float BreakTorque = Mathf.Infinity;
    [Range(0.0f, 10000.0f)]
    public float MaxForce = 1000.0f;

    [Range(0.0f, 10000.0f)]
    public float MaxTorque = 1000.0f;

    public int[,,] VoxelStateStore;

    public int[,,] VoxelAge;


    private int[,] FutureStructureState;

    
   
    //Defualt Condition
    private Texture2D seedImage;
    private GOLRule currentRule;
    public int imgIndex = 0;
    public int ruleIndex = 0;
    
    //Condition Change On Frame 1
    public bool c1 = false;
    public  int cdnFrm1 = 0;
    public int cdnRule1 = 0;

    //Condition Change On Frame 2
    public bool c2 = false;
    public int cdnFrm2 = 0;
    public int cdnRule2 = 0;

    //Condition Change On Density 3
    public bool c3 = false;
    public int cdnDst3 = 0;
    public int cdnRule3 = 0;

    //Condition Change On Density 4
    public bool c4 = false;
    public int cdnDst4 = 0;
    public int cdnRule4 = 0;


    //Condition Change On Age 5
    public bool c5 = false;
    public int cdnAge5 = 0;
    public int cdnRule5 = 0;
   
    //Time End
    public int timeEnd = 2;
    //Age Limit
    public  int ageLimit = 0;


    //Layer Densities

    // Setup Different Game of Life Rules

    GOLRule rule1 = new GOLRule();
    GOLRule rule2 = new GOLRule();
    GOLRule rule3 = new GOLRule();
    GOLRule rule4 = new GOLRule();
    GOLRule rule5 = new GOLRule();
    GOLRule rule6 = new GOLRule();
    GOLRule rule7 = new GOLRule();
    GOLRule rule8 = new GOLRule();
    GOLRule rule9 = new GOLRule();
    GOLRule rule10 = new GOLRule();
    GOLRule rule11 = new GOLRule();
    GOLRule rule12 = new GOLRule();
    GOLRule rule13 = new GOLRule();

    List<GameObject> disconnectedvoxelobjs = new List<GameObject>();

    private bool[] Conditons = new bool[5];

#endregion 
    // Use this for initialization
    void Start()
    {
       
        //Setup GOL Rules

        conditionDensity = new float[12];
        for (int i = 0; i < 4; i++)
        {
            conditionDensity[i] = 0.01f * (i + 1);
        }

        for (int i = 4; i < 12; i++)
        {
            conditionDensity[i] = 0.05f * (i - 3);
        }

        rule1.setupRule(1, 2, 2, 2);
        rule2.setupRule(1, 2, 3, 3);
        rule3.setupRule(1, 2, 3, 4);
        rule4.setupRule(1, 3, 3, 3);
        rule5.setupRule(1, 3, 3, 6);
        rule6.setupRule(2, 3, 3, 3);
        rule7.setupRule(2, 3, 3, 4);
        rule8.setupRule(3, 3, 2, 2);
        rule9.setupRule(3, 4, 1, 1);
        rule10.setupRule(3, 4, 3, 4);
        rule11.setupRule(3, 6, 3, 3);
        rule12.setupRule(4, 5, 2, 2);
        rule13.setupRule(5, 5, 1, 1);

        ruleStore[0] = rule1;
        ruleStore[1] = rule2;
        ruleStore[2] = rule3;
        ruleStore[3] = rule4;
        ruleStore[4] = rule5;
        ruleStore[5] = rule6;
        ruleStore[6] = rule7;
        ruleStore[7] = rule8;
        ruleStore[8] = rule9;
        ruleStore[9] = rule10;
        ruleStore[10] = rule11;
        ruleStore[11] = rule12;
        ruleStore[12] = rule13;

        getStructureData();
       

        CreateInvisibleStructure();
        CreateVisibleStructure();
        ConnectJoints();
        CacheMaterials();
        StartCoroutine(UpdateBodyColors());
        
    }

    void SetTargetPos()
    {
        Vector3 TargetPoint = new Vector3(width / 2, height / 2, length / 2);
        TargetBox.GetComponent< Transform >().position= TargetPoint;
    }
    // Update is called once per frame
    void Update()
    {
        SetTargetPos();

       // CreateVisibleStructure();


    }

#region get structure data function
    //pure data store without model
    public void getStructureData()
    {
        seedImage = seedImageStore[imgIndex-1];
        currentRule = ruleStore[ruleIndex - 1];

        width = seedImage .width;
        length = seedImage .height;
        height = timeEnd;

        
        //create a database
        VoxelStateStore = new int[width, length, height];
        VoxelAge =new int[width ,length,height];
        FutureStructureState = new int[width, length];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                FutureStructureState[i, j] = 0;
                for (int k = 0; k < height; k++)
                {
                   
                    if (k == 0)
                    {
                        // Create a new state based on the input image
                        float t = seedImage.GetPixel(i, j).grayscale;
                        if (t > 0.8f)
                        {
                            VoxelStateStore[i, j, k] = 0;
                        }
                        else
                        {
                            VoxelStateStore[i, j, k] = 1;
                        }
                    }
                    else
                    {
                        // Set the state to death
                        VoxelStateStore[i, j, k] = 0;
                    }
                    VoxelAge[i, j, k] = 0;

                }
            }
        }//createDataGrid

        for (int a = 0; a <= timeEnd ; a++)//Calculate each frame
        {
            if (currentFrame < timeEnd )
            {
                CalculateCaData();

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < length; j++)
                    {
                        VoxelStateStore[i, j, 0] = FutureStructureState[i, j];
                       
                        if (VoxelStateStore[i, j, 0] == 1)
                        {
                            VoxelAge[i, j, 0]++;
                        }
                        if (VoxelStateStore[i, j, 0] == 0)
                        {
                            VoxelAge[i, j, 0] = 0;
                        }
                    }
                }
                SaveCaData();
                currentFrame ++;
            }
        }

        connect3dVn();
    }

    void CalculateCaData()
    {
        
        //calculate data
        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < length - 1; j++)
            {
                int currentVoxel= VoxelStateStore[i, j, 0];
                int CurrentStructureState = currentVoxel;
                int aliveNeighbours = 0;

                // Calculate how many alive neighbours are around the current voxel
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        int currentNeighbour= VoxelStateStore[i + x, j + y, 0];
                        int CurrentNeighourState = currentNeighbour;
                        aliveNeighbours += CurrentNeighourState;
                    }
                }
                aliveNeighbours -= CurrentStructureState;
             
                if (c1==true)
                {
                    RuleChangeOnFrm1();
                }
                if (c2 == true)
                {
                    RuleChangeOnFrm2();
                }
                if (c3 == true)
                {
                    RuleChangeOnDens1();
                }
                if (c4 == true)
                {
                    RuleChangeOnDens2();
                }
                if (c5 == true)
                {
                    if (VoxelAge[i, j, 0] > cdnAge5  && cdnAge5 > 0)
                    {
                        RuleChangeOnAge();
                    }
                }
              
                //get the instructions
                int inst0 = currentRule .getInstruction(0);
                int inst1 = currentRule .getInstruction(1);
                int inst2 = currentRule .getInstruction(2);
                int inst3 = currentRule .getInstruction(3);


                /////////////////////////////////////////////////////////////
                //Calculate state

                if (CurrentStructureState == 1)
                {
                    // If there are less than two neighbours I am going to die
                    if (aliveNeighbours < inst0)
                    {
                        FutureStructureState[i,j] = 0;
                    }
                    // If there are two or three neighbours alive I am going to stay alive
                    if (aliveNeighbours >= inst0 && aliveNeighbours <= inst1)
                    {
                        FutureStructureState[i,j] = 1;
                    }
                    // If there are more than three neighbours I am going to die
                    if (aliveNeighbours > inst1)
                    {
                        FutureStructureState[i,j] = 0;
                    }
                }
                // Rule Set 2: for voxels that are death
                if (CurrentStructureState == 0)
                {
                    // If there are exactly three alive neighbours I will become alive
                    if (aliveNeighbours >= inst2 && aliveNeighbours <= inst3)
                    {
                        FutureStructureState[i,j] = 1;
                    }
                }
                if (VoxelAge[i, j, 0] > ageLimit )
                {
                    FutureStructureState[i, j] = 0;
                }
            }
        }
    }

    void SaveCaData()
    {
        totalAliveCells = 0;
        layerDensities = new float[timeEnd ];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                VoxelStateStore[i, j, currentFrame] = VoxelStateStore[i, j, 0];
                int currentState = VoxelStateStore[i, j, 0];
                if (currentState == 1)
                {
                    int currentVoxelAge= VoxelAge[i, j, 0];
                    VoxelAge[i, j, currentFrame] = currentVoxelAge;
                    totalAliveCells++;
                    if (currentVoxelAge > maxAge)
                    {
                        maxAge = currentVoxelAge;
                    }


                }
            }
        }
        int totalCells = length * width;
        layerdensity = totalAliveCells / totalCells;
        layerDensities[currentFrame] = layerdensity;

        if (layerdensity > 0)
        {
            LayerCount++;
            if (layerdensity > maxlayerdensity)
            {
                maxlayerdensity = layerdensity;
            }

            if (layerdensity < minlayerdensity)
            {
                minlayerdensity = layerdensity;
            }
        }

       
    }

#endregion 

    #region rule change condition function
    void RuleChangeOnFrm1()
    {
        if (currentFrame > cdnFrm1&&cdnRule1 >0)
        {
            currentRule = ruleStore[cdnRule1 - 1];
        }
        frameThershold = cdnFrm1;
    }
    void RuleChangeOnFrm2()
    {
        if (currentFrame > cdnFrm2 && cdnRule2>0)
        {
            currentRule = ruleStore[cdnRule2  - 1];
        }
    }
    void RuleChangeOnDens1()
    {
        if (layerdensity < conditionDensity[cdnDst3 - 1] && currentFrame > frameThershold&&cdnRule3>0)
        {
            currentRule = ruleStore[cdnRule3 - 1];
        }
    }
    void RuleChangeOnDens2()
    {
        if (layerdensity > conditionDensity[cdnDst4 - 1] && currentFrame > frameThershold&&cdnRule4>0)
        {
            currentRule = ruleStore[cdnRule4 - 1];
        }
    }
    void RuleChangeOnAge()
    {
        if ( cdnRule5  > 0)
        {
            currentRule = ruleStore[cdnRule5 - 1];
        }
    }
#endregion 


    void connect3dVn()
    {
        Density3dVN =new int[width,length ,height ];

        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < length - 1; j++)
            {
                for (int k = 1; k < height - 1; k++)
                {
                    int leftState = VoxelStateStore[i - 1, j, k];
                    int rightState = VoxelStateStore[i + 1, j, k];
                    int backState = VoxelStateStore[i, j - 1, k];
                    int frontState = VoxelStateStore[i, j + 1, k];
                    int belowState = VoxelStateStore[i, j, k - 1];
                    int aboveState = VoxelStateStore[i, j, k + 1];

                    Density3dVN[i, j, k] = leftState + rightState + backState + frontState + backState + belowState + aboveState;
                  

                    if (VoxelStateStore[i, j, k] == 1)
                    {

                        if (Density3dVN[i, j, k] < 3 && Density3dVN[i, j, k] > 0)
                        {
                            int _t = Density3dVN[i, j, k];

                            if (leftState == 0 && _t < 3)
                            {
                                VoxelStateStore[i - 1, j, k] = 1;
                                _t++;
                            }
                         
                            if (backState == 0 && _t < 3)
                            {
                                VoxelStateStore[i, j - 1, k] = 1;
                                _t++;
                            }
                           
                            if (belowState == 0 && _t < 3)
                            {
                                VoxelStateStore[i, j, k - 1] = 1;
                                _t++;
                            }
                           
                        }
                        if (Density3dVN[i, j, k] ==0)
                        {
                            VoxelStateStore[i, j, k] = 0;
                        }
                    }

                    if (VoxelStateStore[i, j, k] == 0)
                    {
                        if (Density3dVN[i,j,k] >= 3&&Density3dVN [i,j,k]<=3)
                        {
                            //VoxelStateStore[i, j, k] = 1;
                        }
                    }
                    int leftState2 = VoxelStateStore[i - 1, j, k];
                    int rightState2 = VoxelStateStore[i + 1, j, k];
                    int backState2 = VoxelStateStore[i, j - 1, k];
                    int frontState2 = VoxelStateStore[i, j + 1, k];
                    int belowState2 = VoxelStateStore[i, j, k - 1];
                    int aboveState2 = VoxelStateStore[i, j, k + 1];

                    Density3dVN[i, j, k] = leftState2 + rightState2 + backState2 + frontState2 + backState2 + belowState2 + aboveState2;
                    if (VoxelStateStore[i, j, k] != 0)
                    {
                        if (Density3dVN[i, j, k] < 3)
                        {
                            print("000");
                        }
                    }
                }
            }
        }
    }

    void CreateInvisibleStructure()
    {

        // Allocate space in memory for the array
        StructureGrid = new Rigidbody [width * length * height];
       
        // Populate the array with voxels from a base image

        int index = 0;
        for (int k = 0; k < height ; k++)
        {
            for (int j = 0; j < length; j++)
            {
                for (int i = 0; i < width ; i++,index++)
                {
                    if (VoxelStateStore[i, j, k] == 1)
                    {
                        // Create values for the transform of the new voxel
                        Vector3 currentVoxelPos = new Vector3(i * spacing, k * spacing, j * spacing);
                        Quaternion currentVoxelRot = Quaternion.identity;
                        //create the game object of the voxel
                        Rigidbody currentVoxelObj = Instantiate(Structure, currentVoxelPos, currentVoxelRot);

                        //currentVoxelObj.useGravity = false;

                        // Set the state of the voxels

                        StructureGrid[index] = currentVoxelObj;
                        // Attach the new voxel to the grid game object
                       
                        if (k<=1)
                        {
                            currentVoxelObj.isKinematic = true;
                        }
                    }
                }
            }
        }
    }

    void CreateVisibleStructure()
    {
        // Allocate space in memory for the array
        visibleGrid  = new GameObject [width * length * height];
        // Populate the array with voxels from a base image
        int index = 0;
        int Fr = 0;
        int Bc = 0;
        int Lt = 0;
        int Rt = 0;
        int Up = 0;
        int Dn = 0;
        for (int k = 0; k < height; k++)
        {
            for (int j = 0; j < length; j++)
            {
                for (int i = 0; i < width; i++, index++)
                {
                    if (VoxelStateStore[i, j, k] == 1)
                    {
                        if (j < length - 1)
                        {
                            Fr = VoxelStateStore[i, j + 1, k];
                        }
                        else if (j >= length - 1)
                        {
                            Fr = 0;
                        }
                        if (j > 0)
                        {
                            Bc = VoxelStateStore[i, j - 1, k];
                        }
                        else if (j <= 0)
                        {
                            Bc = 0;
                        }
                        if (i > 0)
                        {
                            Lt = VoxelStateStore[i - 1, j, k];
                        }
                        else if (i <= 0)
                        {
                            Lt = 0;
                        }
                        if (i < width - 1)
                        {
                            Rt = VoxelStateStore[i + 1, j, k];
                        }
                        else if (i >= width - 1)
                        {
                            Rt = 0;
                        }
                        if (k > 0)
                        {
                            Dn = VoxelStateStore[i, j, k - 1];
                        }
                        else if (k <= 0)
                        {
                            Up = 0;
                        }
                        if (k < height-1 )
                        {
                            Up = VoxelStateStore[i, j, k + 1];
                        }
                        else if (k >= height )
                        {
                            Dn = 0;
                        }
                        int currentVNDens = Fr + Bc + Lt + Rt + Up + Dn;
                    }
                    if (VoxelStateStore[i, j, k] == 1)
                    {
                        // Create values for the transform of the new voxel
                        Vector3 currentVoxelPos = new Vector3(i * spacing, k * spacing, j * spacing);
                        Quaternion currentVoxelRot = Quaternion.identity;

                        //create the game object of the voxel
                        GameObject  currentVoxelObj = Instantiate(visibleVoxel, currentVoxelPos, currentVoxelRot);

                         currentVoxelObj .GetComponent<MeshTypeVoxel>().setupMesh(Fr, Bc, Lt, Rt, Up, Dn);

                        //currentVoxelObj.useGravity = false;
                        // Set the state of the voxels
                        visibleGrid [index] = currentVoxelObj;
                    }
                }
            }
        }
    }
    private void ConnectJoints()
    {
        Rigidbody[] _bodies = new Rigidbody[width * length * height];
        StructureJoints = new FixedJoint[_bodies.Length][];

        for (int i = 0; i < _bodies.Length; i++)
            StructureJoints [i] = new FixedJoint[6];

        int index = 0;
        int Layercounts = width * length;
        
        for (int k = 0; k < height ; k++)
        {
            for (int j = 0; j < length; j++)
            {
                for (int i = 0; i < width; i++,index++)
                {
                    _bodies [index] = StructureGrid[index];
                    var bodyA = _bodies[index];
                    
                    if (bodyA == null)
                    {
                        continue;
                    }
                    var jointA = StructureJoints[index];

                    if (i > 0)
                    {
                        var bodyB = _bodies [index-1 ];
                        if (bodyB != null)
                        {
                            ConnectBodies(bodyA, jointA ,bodyB, StructureJoints [index-1],0);
                        }
                    }
                    if (j > 0)
                    {
                        var bodyB = _bodies [index-width];
                        if (bodyB != null)
                        {
                            ConnectBodies(bodyA, jointA, bodyB, StructureJoints[index - width], 1);
                        }
                    }
                    if (k > 0)
                    {
                        var bodyB = _bodies[index - Layercounts];
                        {
                            if (bodyB != null)
                            {
                                ConnectBodies(bodyA, jointA, bodyB, StructureJoints[index - Layercounts ], 2 );
                            }
                        }
                    }
                }
            }
        }

    }
    private void ConnectBodies(Rigidbody bodyA, FixedJoint[] jointsA, Rigidbody bodyB, FixedJoint[] jointsB, int index)
    {
        var joint = bodyA.gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = bodyB;

        // joint.breakForce = BreakForce;
        // joint.breakTorque = BreakTorque;

        jointsA[index] = jointsB[index + 3] = joint;
    }
    private void CacheMaterials()
    {
        _materials = new Material[visibleGrid .Length];

        for (int i = 0; i < visibleGrid  .Length; i++)
        {
            var b = visibleGrid [i];
            if (b == null) continue;

            var m = _materials[i] = b.gameObject.GetComponent<MeshRenderer>().material;
            m.color = Spectrum[0];
        }
    }
    IEnumerator UpdateBodyColors()
    {
        const float factor = 0.75f;

        while (true)
        {
            for (int i = 0; i < _materials.Length; i++)
            {
                var m = _materials[i];

                if (m != null)
                    m.color = Color.Lerp(m.color, GetTorqueColor(i), factor);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
    private Color GetTorqueColor(int index)
    {
        var joints = StructureJoints[index];

        float sum = 0.0f;
        int count = 0;

        foreach (var j in StructureJoints [index])
        {
            if (j != null)
            {
                sum += j.currentTorque.sqrMagnitude;
                count++;
            }
        }

        if (count == 0)
            return Spectrum[0];

        return Lerp(Spectrum, sum / (count * MaxTorque));
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private Color GetForceColor(int index)
    {
        var joints = StructureJoints [index];

        float sum = 0.0f;
        int count = 0;

        foreach (var j in StructureJoints [index])
        {
            if (j != null)
            {
                sum += j.currentForce.sqrMagnitude;
                count++;
            }
        }

        if (count == 0)
            return Spectrum[0];

        return Lerp(Spectrum, sum / (count * MaxTorque));
    }

    public static Color Lerp(IReadOnlyList< Color> colors, float factor)
    {
        int last = colors.Count - 1;
        int i;
        factor = SlurMathf.Fract(factor * last, out i);

        if (i < 0)
            return colors[0];
        else if (i >= last)
            return colors[last];

        return Color.LerpUnclamped(colors[i], colors[i + 1], factor);
    }


}
