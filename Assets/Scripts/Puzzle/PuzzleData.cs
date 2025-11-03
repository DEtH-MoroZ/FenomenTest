using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/*
 1 строка - количество фишек;
2 строка - количество точек, куда можно поместить фишку;
3-11 строка - координаты точек, куда можно поместить фишку;
12 строка - начальное расположение фишек по точкам;
13 строка - выигрышное положение фишек по точкам;
14 строка - количество соединений;
15-22 строка - список соединений между парами точек
*/

//nodes ids and connectinon ids are started from one. lol?

public class PuzzleData
{

    public int TokenCount;
    public int NodeCount;
    public Vector2[] NodeCoords;
    public int[] StartTokenPositions;
    public int[] EndTokenPositions;
    public int NodeConnectionsCount;
    public int[] NodeConnections; // power of two, n0 start n1 end

    public PuzzleData(string PuzzleLocationPath)
    {

        TextAsset txtAsset = (TextAsset)Resources.Load(PuzzleLocationPath, typeof(TextAsset));
        string tileFile = txtAsset.text;

        string[] newLineSplittedStrings = tileFile.Split('\n');

        bool success;
        int intNumber;
        float floatNumber;

        int CurrentLine = 0;

        //token count
        success = int.TryParse(newLineSplittedStrings[CurrentLine], out intNumber);
        if (success)
        {
            TokenCount = intNumber;
        }
        else
        {
            Debug.LogError("[PuzzleData] Something wrong with puzzle data; Token Count.");
            return;
        }
        CurrentLine++;
        //Node count
        success = int.TryParse(newLineSplittedStrings[CurrentLine], out intNumber);
        if (success)
        {
            NodeCount = intNumber;
        }
        else
        {
            Debug.LogError("[PuzzleData] Something wrong with puzzle data; Node Count.");
            return;
        }

        CurrentLine++;

        //Node Coords

        NodeCoords = new Vector2[NodeCount];


        for (int a = 0; a < NodeCount; a++)
        {
            float[] coordsFloats = new float[2];
            string[] coordsStrings = newLineSplittedStrings[CurrentLine + a].Split(',');
            for (int b = 0; b < 2; b++)
            {
                success = float.TryParse(coordsStrings[b], out floatNumber);
                if (success)
                {
                    coordsFloats[b] = floatNumber;
                }
                else
                {
                    Debug.LogError("[PuzzleData] Something wrong with puzzle data; Node Coords," + a + "; " + b + ";");
                    return;
                }
            }

            NodeCoords[a] = new Vector2(coordsFloats[0], coordsFloats[1]);
        }

        CurrentLine += NodeCount;

        //Start Token Positions
        string[] tokenPositionsStrings = newLineSplittedStrings[CurrentLine].Split(',');
        StartTokenPositions = new int[tokenPositionsStrings.Length];
        for (int a = 0; a < StartTokenPositions.Length; a++)
        {
            success = int.TryParse(tokenPositionsStrings[a], out intNumber);
            if (success)
            {
                StartTokenPositions[a] = intNumber-1;
            }
            else
            {
                Debug.LogError("[PuzzleData] Something wrong with puzzle data; Start Token Positions," + a + ";");
                return;
            }
        }
        CurrentLine++;

        //End Token Positions

        string[] endTokenPositionsStrings = newLineSplittedStrings[CurrentLine].Split(',');
        EndTokenPositions = new int[endTokenPositionsStrings.Length];
        for (int a = 0; a < EndTokenPositions.Length; a++)
        {
            success = int.TryParse(endTokenPositionsStrings[a], out intNumber);
            if (success)
            {
                EndTokenPositions[a] = intNumber-1;
            }
            else
            {
                Debug.LogError("[PuzzleData] Something wrong with puzzle data; End Token Positions," + a + ";");
                return;
            }
        }

        CurrentLine++;

        //Node Connections Count
        success = int.TryParse(newLineSplittedStrings[CurrentLine], out intNumber);
        if (success)
        {
            NodeConnectionsCount = intNumber;
        }
        else
        {
            Debug.LogError("[PuzzleData] Something wrong with puzzle data; Node Connections Count;");
            return;
        }

        CurrentLine++;

        //Node Connections

        NodeConnections = new int[NodeConnectionsCount * 2];
        int NodeConnectionsIterator = 0;
        for (int a = CurrentLine; a < CurrentLine + NodeConnectionsCount; a++) {

            string[] connectionString = newLineSplittedStrings[a].Split(',');

            for (int b = 0; b < connectionString.Length; b++)
            {
                success = int.TryParse(connectionString[b], out intNumber);
                if (success)
                {                    
                    NodeConnections[NodeConnectionsIterator] = intNumber-1;
                    NodeConnectionsIterator++;
                }
                else
                {
                    Debug.LogError("[PuzzleData] Something wrong with puzzle data; Node Connections," + a + "; " + b + ";" );
                    return;
                }
            }
        }

        Debug.Log("[PuzzleData] Imported; [SanityTest] = " + this.SanityTest());
    }

    public bool SanityTest ()
    {
        
        if (TokenCount != StartTokenPositions.Length) 
        {
            Debug.LogWarning("[PuzzleData] Token count doesnt fit start token positions");
            return false;
        }
        if (TokenCount != EndTokenPositions.Length)
        {
            Debug.LogWarning("[PuzzleData] Token count doesnt fit end token positions");
            return false;
        }
        if (NodeCount+1 < TokenCount)
        {
            Debug.LogWarning("[PuzzleData] Node count is not enought to resolve puzzle");
            return false;
        }
        if (NodeCount-1 > NodeConnectionsCount)
        {
            Debug.LogWarning("[PuzzleData] Node connections count doesn't fit node count");
            return false;
        }

        return true;
    }

   
    public PuzzleData (int seed)
    {
        UnityEngine.Random.InitState(seed);

        TokenCount = 1 + Mathf.RoundToInt( 10 * UnityEngine.Random.value );
        NodeCount = Mathf.Max(TokenCount+3, 1 + Mathf.RoundToInt(10 * UnityEngine.Random.value));
        NodeCoords = new Vector2[NodeCount];

        float randomfl;

        for (int a = 0; a < NodeCoords.Length; a++)
        {
            randomfl = UnityEngine.Random.Range(0f, 260f);
            NodeCoords[a] = new Vector2(Mathf.Cos(randomfl), Mathf.Sin(randomfl)) * UnityEngine.Random.Range(0,300); //spherical variaton, prefere this more
            NodeCoords[a] += Vector2.one * 300f;
        }

        

        List<int> BumpStartTokenPositions = new List<int>();
        //int randomint;
        
        //bool theTest;
        
        //random positions of tokens, needs some more work. swithching to straiforward aproach
        for (int b = 0; b < TokenCount; b++)
        {
            /*
            int inCaseCounter = 0;
            theTest = true;           
            while (theTest == true)
            {
                randomint = UnityEngine.Random.Range(0, NodeCount);
               
                theTest = BumpStartTokenPositions.Contains(randomint);
                if (theTest == false)
                {
                    BumpStartTokenPositions.Add(randomint);
                    break;
                }
                inCaseCounter++;
                if (inCaseCounter == 100) { break;  }
            } */

            BumpStartTokenPositions.Add(b);
        }
        
        
        StartTokenPositions = BumpStartTokenPositions.ToArray();

        EndTokenPositions = new int[TokenCount];
        /*
        int tempInt;
        for (int i = 0; i < StartTokenPositions.Length; i++)
        {
            int rnd = UnityEngine.Random.Range(0, StartTokenPositions.Length);
            tempInt = StartTokenPositions[rnd];
            EndTokenPositions[rnd] = StartTokenPositions[i];
            EndTokenPositions[i] = tempInt;
        }
        */
        for (int i = 0; i < StartTokenPositions.Length;i++)
        {
            EndTokenPositions[StartTokenPositions.Length - i - 1] = i;
        }


        NodeConnectionsCount = UnityEngine.Random.Range(NodeCount+3, NodeCount * 2);
        NodeConnections = new int[NodeConnectionsCount * 2];

        for (int a = 0; a < NodeCount-1; a++)
        {
            NodeConnections[a] = a+1;
             
        }

        NodeConnections[NodeCount] = 0;

        for (int b = NodeCount+1; b < NodeConnections.Length; b++)
        {
            NodeConnections[b] = UnityEngine.Random.Range(0, NodeCount - 1);
        }


        SanityTest();
    }

    public override string ToString() {
        string finalString = "";
        
        finalString += "[TokenCount] = " + TokenCount + ";\n";
        finalString += "[NodeCount] = " + NodeCount + ";\n";
        finalString += "[NodeCoords] \n";

        if (NodeCoords == null) { Debug.LogError("[NoodCoords] array is not initializated"); }
        else
        {
            for (int a = 0; a < NodeCoords.Length; a++)
            {
                finalString += NodeCoords[a].ToString() + ";\n";
            }
        }
        finalString += "[StartTokenPositions] = ";
        if (StartTokenPositions == null) { Debug.LogError("[StartTokenPositions] array is not initializated"); }
        else
        {
            for (int a = 0; a < StartTokenPositions.Length; a++)
            {
                finalString += StartTokenPositions[a] + ",";
                
            }
        }
        finalString += ";\n";
        finalString += "[EndTokenPositions] = ";
        if (EndTokenPositions == null) { Debug.LogError("[EndTokenPositions] array is not initializated"); }
        else
        {
            for (int a = 0; a < EndTokenPositions.Length; a++)
            {
                finalString += EndTokenPositions[a] + ",";
            }
        }
        finalString += ";\n";

        finalString += "[NodeConnectionsCount] = " + NodeConnectionsCount + ";\n";

        finalString += "[NodeCoords] \n";
        if (NodeConnections == null) { Debug.LogError("[NodeConnections] array is not initializated"); }
        else
        {
            for (int a = 0; a < NodeConnections.Length; a++)
            {
                finalString += NodeConnections[a];
                if (a%2 == 1)
                {
                    finalString += "\n";
                }
                else
                {
                    finalString += ",";
                }
            }
        }

        return finalString;
    }

}
