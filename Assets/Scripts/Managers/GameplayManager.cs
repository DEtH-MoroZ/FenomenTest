using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager instance;

    [SerializeField] private Transform NodePrefab;
    [SerializeField] private Transform ConnectionLinePrefab;
    [SerializeField] private Transform TokenPrefab;
    [SerializeField] private Transform TokenFinalPositionPrefab;

    [SerializeField] private Camera gameCamera;

    public int TokenLayer = 6;
    public int NodeLayer = 7;

    public Node[] Nodes;
    private Connection[] Connections;
    private Token[] Tokens;
    private TokenFinalPosition[] TokenFinalPositions;

    private bool isFinalStageShowing = true;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Debug.Log("[GameplayManager] Loaded data:\n" + PuzzleDataManager.instance.currentPuzzleData.ToString());

        CreatePuzzleRepresentation();
        CalculateCameraPositionAndSize();
        ShowFinalStage();
    }

    #region PlayfieldCreation

    private void CreatePuzzleRepresentation()
    {
        CreateNodes();
        CreateConnections();
        SpawnTokens();
        SpawnFinalPositions();
    }

    private void CreateNodes()
    {
        Nodes = new Node[PuzzleDataManager.instance.currentPuzzleData.NodeCount];
        for (int a = 0; a < Nodes.Length; a++)
        {
            Nodes[a] = Instantiate(NodePrefab, PuzzleDataManager.instance.currentPuzzleData.NodeCoords[a], Quaternion.identity, this.transform).GetComponent<Node>();
            Nodes[a].id = a;
        }
    }

    private void CreateConnections()
    {
        Connections = new Connection[PuzzleDataManager.instance.currentPuzzleData.NodeConnectionsCount];
        for (int a = 0; a < Connections.Length; a++)
        {
            Connections[a] = Instantiate(ConnectionLinePrefab, this.transform).GetComponent<Connection>();
            Connections[a].SetConnection(
                PuzzleDataManager.instance.currentPuzzleData.NodeConnections[2 * a],
                PuzzleDataManager.instance.currentPuzzleData.NodeConnections[2 * a + 1]);
            RegisterConnection(Connections[a]);
        }
    }
    private void RegisterConnection(Connection conToReg)
    {
        Nodes[conToReg.ConnectionStart].RegisterConnection(conToReg.ConnectionEnd);
        Nodes[conToReg.ConnectionEnd].RegisterConnection(conToReg.ConnectionStart);
    }

    private void SpawnTokens()
    {
        Tokens = new Token[PuzzleDataManager.instance.currentPuzzleData.StartTokenPositions.Length];
        for (int a = 0; a < PuzzleDataManager.instance.currentPuzzleData.StartTokenPositions.Length; a++)
        {
            Tokens[a] = Instantiate(
                TokenPrefab,
                Vector3.zero,
                Quaternion.identity,
                this.transform).GetComponent<Token>();
            Tokens[a].id = a;
            Tokens[a].SetVisualSign(a.ToString());
            Tokens[a].SetRandomColor();
            //this is made on purpose, for a starter effect
            Tokens[a].Move(
                PuzzleDataManager.instance.currentPuzzleData.NodeCoords
                [PuzzleDataManager.instance.currentPuzzleData.StartTokenPositions[a]],
                PuzzleDataManager.instance.currentPuzzleData.StartTokenPositions[a]);
            Nodes[Tokens[a].CurrentPosition].SetOccupied();
            Tokens[a].EndPosition = PuzzleDataManager.instance.currentPuzzleData.EndTokenPositions[a];
        }
    }
    private void SpawnFinalPositions()
    {
        TokenFinalPositions = new TokenFinalPosition[Tokens.Length];
        for (int a = 0; a < Tokens.Length; a++)
        {
            TokenFinalPositions[a] = Instantiate(TokenFinalPositionPrefab,
                PuzzleDataManager.instance.currentPuzzleData.NodeCoords
                [PuzzleDataManager.instance.currentPuzzleData.EndTokenPositions[a]],
                Quaternion.identity,
                this.transform).GetComponent<TokenFinalPosition>();

            TokenFinalPositions[a].SetVisualSign(a.ToString());

            TokenFinalPositions[a].SetColor(Tokens[a].GetColor());
        }
    }
    private void CalculateCameraPositionAndSize()
    {
        //works good with squred puzzleds, that positioned upright side of coords, doesnt fit random generation
        /*
        Vector3 center = Vector3.zero;
        for (int a = 0; a < Nodes.Length; a++)
        {
            center += Nodes[a].transform.position;
        }
        gameCamera.transform.position = center / Nodes.Length + (-Vector3.forward * 5f);
        gameCamera.orthographicSize = gameCamera.transform.position.x / 1.5f;
    */
        Vector3 center = Vector3.zero;
        float cameraSize = 0f;

        float MaxX = Nodes[0].transform.position.x;
        float MinX = Nodes[0].transform.position.x;
        float MaxY = Nodes[0].transform.position.y;
        float MinY = Nodes[0].transform.position.y;

        for (int a = 1; a < Nodes.Length; a++)
        {
            if (MaxX < Nodes[a].transform.position.x)
            {
                MaxX = Nodes[a].transform.position.x;
            }

            if (MinX > Nodes[a].transform.position.x)
            {
                MinX = Nodes[a].transform.position.x;
            }

            if (MaxY < Nodes[a].transform.position.y)
            {
                MaxY = Nodes[a].transform.position.y;
            }

            if (MinY > Nodes[a].transform.position.y)
            {
                MinY = Nodes[a].transform.position.y;
            }
        }

        center = new Vector3( (MaxX + MinX)/2f, (MaxY + MinY)/2f, -10f);

        cameraSize = 1.6f * (MaxY - MinY) /2f;  //1.6 is possible biggest horizontal aspect ratio for monitors

        gameCamera.transform.position = center;
        gameCamera.orthographicSize = cameraSize;
    }
    #endregion

    #region TokenControls
    //practiaclly we shoot raycast every fixed update
    //bump variables for update/controls
    Vector3 mousePos;
    Vector2 worldPosition;
    int selectedNodeId;
    [HideInInspector]
    public int selectedToken = -1;
    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {




            for (int a = 0; a < LatestHits.Length; a++)
            {

                if (selectedToken == -1) //if token is not selected, then try to select token
                {
                    if (LatestHits[a].gameObject.layer == TokenLayer)
                    {
                        LatestHits[a].gameObject.BroadcastMessage("Select", SendMessageOptions.DontRequireReceiver);
                        return;
                    }
                }

                else  //if we got selected token, then try to find where to move it
                {
                    if (LatestHits[a].gameObject.layer == NodeLayer)
                    {
                        selectedNodeId = LatestHits[a].gameObject.GetComponent<Node>().id;

                        if (ActualPath.Length > 1) //there is a path and we can move to it
                        {
                            //MoveToken(selectedToken, selectedNodeId);
                            MoveTokenByPath(selectedToken, ActualPath);
                            ResetOccupation();
                            HightlightPathDisable();
                            return;
                        }
                        else //the case when we try to move token, but there is no path - then we deselect token
                        {
                           
                            DeselectAllTokens();
                        }
                    }

                    else if (LatestHits[a].gameObject.layer == TokenLayer) //if we hit another token, while previouse is choosen
                    {
                        DeselectAllTokens();
                        LatestHits[a].gameObject.BroadcastMessage("Select", SendMessageOptions.DontRequireReceiver);
                        continue;
                    }
                }
            }
            //and if none of this works - deselect token in any case
            if (LatestHits.Length == 0)
            {
                DeselectAllTokens();
            }
        }

    }
    //pathfinding visualization
    private int prevTargetNodeId = -1;
    private Node targetNode;
    Collider2D[] LatestHits;
    int[] ActualPath;
    private void FixedUpdate()
    {
        mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

        LatestHits = Physics2D.OverlapBoxAll(worldPosition, Vector2.one, 0f);
        if (LatestHits.Length == 0)
        {

            prevTargetNodeId = -1;
            HightlightPathDisable();

            return;
        }
        if (selectedToken == -1) return; //no selected token - then we just do nothing

        for (int a = 0; a < LatestHits.Length; a++)
        {
            if (LatestHits[a].gameObject.layer == NodeLayer) //found free node, occupied nodes have disabled colliders
            {
                targetNode = LatestHits[a].gameObject.GetComponent<Node>();


                if (targetNode.id != prevTargetNodeId) { prevTargetNodeId = targetNode.id; }
                else { return; }



                ActualPath = FindPath(selectedToken, targetNode.id);
                

                HightlightPath(ActualPath);
                return;

            }
        }



    }
    private void DeselectAllTokens()
    {
        for (int a = 0; a < Tokens.Length; a++)
        {
            Tokens[a].DeSelect();
        }
        selectedToken = -1;
    }



    //bump variables for pathfinding
    private List<List<int>> CurrentPaths;
    List<int> bumpConnections;
    List<int> path;
    private int[] FindPath(int _TokenId, int target)
    {

        Debug.Log(target);
        path = new List<int>();
        CurrentPaths = new List<List<int>>();
        CurrentPaths.Add(new List<int>());
        CurrentPaths[0].Add(Tokens[_TokenId].CurrentPosition); //adding starting node

        for (int a = 0; a < Nodes.Length; a++)
        {
            Nodes[a].SetUnchecked();
        }

        bool pathExists = true;
        int currentStep = 0;
        int CurrentCheckedNodeId;

        while (pathExists)  //looping untill we check everything
        {

            for (int a = 0; a < CurrentPaths.Count; a++) //checking every existing possible path, at the begining its just one
            {
                bumpConnections = new List<int>();

                for (int b = 0; b < Nodes[CurrentPaths[a][currentStep]].connectedId.Count; b++) //there we get all connections
                {
                    CurrentCheckedNodeId = Nodes[CurrentPaths[a][currentStep]].connectedId[b];

                    if (Nodes[CurrentCheckedNodeId].isOccupied == true) continue; //first we check if there is another token
                    if (Nodes[CurrentCheckedNodeId].isChecked == true) continue; //then we check if we check
                    else (Nodes[CurrentCheckedNodeId]).SetChecked(); //if not checked before - then we set it checked


                    bumpConnections.Add(CurrentCheckedNodeId); // all checks are made, we add new node to nodes possible for use

                    if (CurrentCheckedNodeId == target) // we found target node
                    {
                        CurrentPaths[a].Add(CurrentCheckedNodeId);
                        path = CurrentPaths[a]; //save


                        return path.ToArray();

                    }

                    
                }
                if (bumpConnections.Count == 0) //if we didnt find new connections and we didnt find target - remove this possible path
                {
                    CurrentPaths.RemoveAt(a);
                    continue;
                }
                for (int c = 0; c < bumpConnections.Count; c++) //othervise we add first node to this path, and clone this path with other new node
                {
                    if (c == 0) { CurrentPaths[a].Add(bumpConnections[0]); }
                    else
                    {
                        CurrentPaths.Add(CurrentPaths[a]);
                        CurrentPaths[CurrentPaths.Count - 1].Add(bumpConnections[c]);
                    }
                }
            }

            if (CurrentPaths.Count == 0) //everything checked, no paths
            {

                pathExists = false;
                                
            }
            currentStep++;
        }


        

        path = new List<int>();
        path.Add(Tokens[_TokenId].CurrentPosition);

        return path.ToArray();

    }

    private void ResetOccupation()
    {
        for (int a = 0; a < Nodes.Length; a++)
        {
            Nodes[a].SetNotOccupied();
            for (int b = 0; b < Tokens.Length; b++)
            {
                if (Tokens[b].CurrentPosition == a)
                {
                    Nodes[a].SetOccupied();
                    continue;
                }
                
            }
        }
    }

    private void HightlightPath(int[] path)
    {
        
        for (int a = 0; a < path.Length; a++)
        {
            
            Nodes[path[a]].EnableHightlight();
           
            
            if (a + 1 < path.Length)
            {
                for (int b = 0; b < Connections.Length; b++)
                {
                    if ( (Connections[b].ConnectionStart == Nodes[path[a]].id ) && (Connections[b].ConnectionEnd == Nodes[path[a + 1]].id))
                    {
                        Connections[b].EnableHightlight();
                    }
                    if ((Connections[b].ConnectionStart == Nodes[path[a+1]].id) && (Connections[b].ConnectionEnd == Nodes[path[a]].id))
                    {
                        Connections[b].EnableHightlight();
                    }
                }
            }
        }
        
    }
    private void HightlightPathDisable()
    {
        for (int a = 0; a < Nodes.Length; a++)
        {
            Nodes[a].DisableHightlight();
        }
        for (int b = 0; b < Connections.Length; b++)
        {
            Connections[b].DisableHightlight();
        }
    }



    #endregion

    #region TokenMovement
    private void MoveToken(int tokenId, int targetNodeId)
    {
        if (Nodes[selectedNodeId].isOccupied == true) { return; }
        Nodes[Tokens[tokenId].CurrentPosition].SetNotOccupied();
        Nodes[targetNodeId].SetOccupied();
        Tokens[tokenId].Move(Nodes[targetNodeId].transform.position, targetNodeId);
        CheckForGameEnd();
    }
    private void MoveTokenByPath(int tokenId, int[] path)
    {
        Nodes[Tokens[tokenId].CurrentPosition].SetNotOccupied();
        Nodes[ path[path.Length - 1] ].SetOccupied();

        Tokens[tokenId].MoveByTraectory(PathToTraectory(path), path[path.Length - 1]);
        CheckForGameEnd();
    }

    private Vector3[] PathToTraectory(int[] path)
    {
        Vector3[] traectory = new Vector3[path.Length];
        for (int a = 0; a < path.Length; a++)
        {
            traectory[a] = Nodes[path[a]].transform.position;
        }
        return traectory;
    }

    #endregion

    #region GameEnd

    void CheckForGameEnd()
    {
        int positionsCounter = 0;
        for (int a = 0; a < Tokens.Length; a++)
        {
            if (Tokens[a].CurrentPosition == Tokens[a].EndPosition)
            {
                positionsCounter++;
            }
        }
        if (positionsCounter == Tokens.Length)
        {
            GameEnd();
        }
    }

    public void GameEnd()
    {
        LoadingManager.instance.LoadMainMenu();
    }

    public void ShowFinalStage()
    {
        isFinalStageShowing = !isFinalStageShowing;
        for (int a = 0; a < TokenFinalPositions.Length; a++)
        {
            TokenFinalPositions[a].SpRendererSwitch(isFinalStageShowing);
            
            Tokens[a].SpRendererSwitch(!isFinalStageShowing);
        }
    }

    #endregion
}
