using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FuzzyLogic;

public class FuzzyLogicTest : MonoBehaviour
{
    public int n_operation_by_frame = 50;
    public bool test;
    FuzzySystem fuzzySystem;

    private void Start()
    {
        /* INDEPENDIENT FUZZY VARIABLES */
        FuzzyVariable hungerSet = new FuzzyVariable("Hunger");
        hungerSet.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: 0.5f));
        hungerSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 0.1f, one: 0.5f, max_zero: 0.9f));
        hungerSet.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: 0.5f, one: 1));

        FuzzyVariable sizeSet = new FuzzyVariable("Size");
        sizeSet.AddFuzzySet(new DiagonalLineFuzzySet("Small", one: 0, zero: 0.5f));
        sizeSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 0.1f, one: 0.5f, max_zero: 0.9f));
        sizeSet.AddFuzzySet(new DiagonalLineFuzzySet("Big", zero: 0.5f, one: 1));

        FuzzyVariable ageSet = new FuzzyVariable("Age");
        ageSet.AddFuzzySet(new DiagonalLineFuzzySet("Fry", one: 0, zero: 20));
        ageSet.AddFuzzySet(new TriangularFuzzySet("Young", min_zero: 10, one: 40, max_zero: 70));
        ageSet.AddFuzzySet(new TriangularFuzzySet("Adult", min_zero: 40, one: 70, max_zero: 100));
        ageSet.AddFuzzySet(new DiagonalLineFuzzySet("Old", zero: 70, one: 100));

        FuzzyVariable energySet = new FuzzyVariable("Energy");
        energySet.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: 0.5f));
        energySet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 0.1f, one: 0.5f, max_zero: 0.9f));
        energySet.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: 0.5f, one: 1));

        FuzzyVariable obstacleDistanceSet = new FuzzyVariable("ObstacleDistance");
        obstacleDistanceSet.AddFuzzySet(new DiagonalLineFuzzySet("Close", one: 2, zero: 6));
        obstacleDistanceSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 5, one: 8, max_zero: 11));
        obstacleDistanceSet.AddFuzzySet(new DiagonalLineFuzzySet("Far", zero: 8, one: 12));

        FuzzyVariable centroidDistanceSet = new FuzzyVariable("CentroidDistance");
        centroidDistanceSet.AddFuzzySet(new DiagonalLineFuzzySet("Close", one: 10, zero: 20));
        centroidDistanceSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 10, one: 20, max_zero: 30));
        centroidDistanceSet.AddFuzzySet(new DiagonalLineFuzzySet("Far", zero: 20, one: 40));

        FuzzyVariable predatorDistanceSet = new FuzzyVariable("PredatorDistance");
        predatorDistanceSet.AddFuzzySet(new DiagonalLineFuzzySet("Close", one: 2, zero: 6));
        predatorDistanceSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 5, one: 8, max_zero: 11));
        predatorDistanceSet.AddFuzzySet(new DiagonalLineFuzzySet("Far", zero: 8, one: 12));

        FuzzyVariable preyDistanceSet = new FuzzyVariable("PreyDistance");
        preyDistanceSet.AddFuzzySet(new DiagonalLineFuzzySet("Close", one: 0.5f, zero: 3));
        preyDistanceSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 2, one: 5, max_zero: 8));
        preyDistanceSet.AddFuzzySet(new DiagonalLineFuzzySet("Far", zero: 7, one: 12));

        /* DEPENDIENT FUZZY VARIABLES */

        FuzzyVariable speedSet = new FuzzyVariable("Speed");
        speedSet.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: 2.5f));
        speedSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 0.5f, one: 2.5f, max_zero: 4.5f));
        speedSet.AddFuzzySet(new DiagonalLineFuzzySet("Fast", zero: 2.5f, one: 5));

        FuzzyVariable swimmingDepthSet = new FuzzyVariable("SwimmingDepth");
        swimmingDepthSet.AddFuzzySet(new DiagonalLineFuzzySet("Low", zero: -20f, one: 0));
        swimmingDepthSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: -30, one: -20, max_zero: -10));
        swimmingDepthSet.AddFuzzySet(new TriangularFuzzySet("High", min_zero: -40, one: -30, max_zero: -20));
        swimmingDepthSet.AddFuzzySet(new DiagonalLineFuzzySet("VeryHigh", zero: -35, one: -45));

        FuzzyVariable preySearchingRangeSet = new FuzzyVariable("PreySearchingRange");
        preySearchingRangeSet.AddFuzzySet(new DiagonalLineFuzzySet("Small", one: 0, zero: 2f));
        preySearchingRangeSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 1, one: 3, max_zero: 5));
        preySearchingRangeSet.AddFuzzySet(new DiagonalLineFuzzySet("Big", zero: 4, one: 20));

        FuzzyVariable couzinDirectionWeightSet = new FuzzyVariable("CouzinDirectionWeight");
        couzinDirectionWeightSet.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: 0.35f));
        couzinDirectionWeightSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 0.15f, one: 0.5f, max_zero: 0.85f));
        couzinDirectionWeightSet.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: 0.65f, one: 1));

        FuzzyVariable obstacleAvoidanceDirectionWeightSet = new FuzzyVariable("ObstacleAvoidanceDirectionWeight");
        obstacleAvoidanceDirectionWeightSet.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: 0.35f));
        obstacleAvoidanceDirectionWeightSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 0.15f, one: 0.5f, max_zero: 0.85f));
        obstacleAvoidanceDirectionWeightSet.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: 0.65f, one: 1));

        FuzzyVariable predatorAvoidanceDirectionWeightSet = new FuzzyVariable("PredatorAvoidanceDirectionWeight");
        predatorAvoidanceDirectionWeightSet.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: 0.35f));
        predatorAvoidanceDirectionWeightSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 0.15f, one: 0.5f, max_zero: 0.85f));
        predatorAvoidanceDirectionWeightSet.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: 0.65f, one: 1));

        FuzzyVariable preyFollowingDirectionWeightSet = new FuzzyVariable("PreyFollowingDirectionWeight");
        preyFollowingDirectionWeightSet.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: 0.35f));
        preyFollowingDirectionWeightSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 0.15f, one: 0.5f, max_zero: 0.85f));
        preyFollowingDirectionWeightSet.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: 0.65f, one: 1));

        FuzzyVariable centroidFollowingDirectionWeightSet = new FuzzyVariable("CentroidFollowingDirectionWeight");
        centroidFollowingDirectionWeightSet.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: 0.35f));
        centroidFollowingDirectionWeightSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 0.15f, one: 0.5f, max_zero: 0.85f));
        centroidFollowingDirectionWeightSet.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: 0.65f, one: 1));


        /* FUZZY SYSTEM */
        FuzzySystem fishFuzzySystem = new FuzzySystem("FishFuzzySystem");

        fishFuzzySystem.AddIndependientVariable(hungerSet);
        fishFuzzySystem.AddIndependientVariable(sizeSet);
        fishFuzzySystem.AddIndependientVariable(ageSet);
        fishFuzzySystem.AddIndependientVariable(energySet);
        fishFuzzySystem.AddIndependientVariable(obstacleDistanceSet);
        fishFuzzySystem.AddIndependientVariable(centroidDistanceSet);
        fishFuzzySystem.AddIndependientVariable(predatorDistanceSet);
        fishFuzzySystem.AddIndependientVariable(preyDistanceSet);

        fishFuzzySystem.AddDependientVariable(speedSet);
        fishFuzzySystem.AddDependientVariable(swimmingDepthSet);
        fishFuzzySystem.AddDependientVariable(preySearchingRangeSet);
        fishFuzzySystem.AddDependientVariable(couzinDirectionWeightSet);
        fishFuzzySystem.AddDependientVariable(obstacleAvoidanceDirectionWeightSet);
        fishFuzzySystem.AddDependientVariable(predatorAvoidanceDirectionWeightSet);
        fishFuzzySystem.AddDependientVariable(preyFollowingDirectionWeightSet);
        fishFuzzySystem.AddDependientVariable(centroidFollowingDirectionWeightSet);

        // RULES
        Dictionary<string, string> antecedents;
        KeyValuePair<string, string> consequent;

        // SPEED
        antecedents = new Dictionary<string, string>();
        antecedents.Add("Energy", "Low");
        consequent = new KeyValuePair<string, string>("Speed", "Low");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Energy", "Medium");
        consequent = new KeyValuePair<string, string>("Speed", "Medium");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Energy", "High");
        consequent = new KeyValuePair<string, string>("Speed", "Fast");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Fry");
        consequent = new KeyValuePair<string, string>("Speed", "Low");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Young");
        consequent = new KeyValuePair<string, string>("Speed", "Medium");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Adult");
        consequent = new KeyValuePair<string, string>("Speed", "Medium");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Old");
        consequent = new KeyValuePair<string, string>("Speed", "Low");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("PredatorDistance", "Close");
        consequent = new KeyValuePair<string, string>("Speed", "Fast");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("PreyDistance", "Close");
        consequent = new KeyValuePair<string, string>("Speed", "Fast");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));


        /* SwimmingDepth */
        antecedents = new Dictionary<string, string>();
        antecedents.Add("Hunger", "Low");
        consequent = new KeyValuePair<string, string>("SwimmingDepth", "Medium");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Hunger", "Medium");
        consequent = new KeyValuePair<string, string>("SwimmingDepth", "High");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Hunger", "High");
        consequent = new KeyValuePair<string, string>("SwimmingDepth", "VeryHigh");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));


        /* PreySearchingRange */
        antecedents = new Dictionary<string, string>();
        antecedents.Add("Hunger", "Low");
        consequent = new KeyValuePair<string, string>("PreySearchingRange", "Small");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Hunger", "Medium");
        consequent = new KeyValuePair<string, string>("PreySearchingRange", "Medium");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Hunger", "High");
        consequent = new KeyValuePair<string, string>("PreySearchingRange", "Big");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));


        /*CouzinDirectionWeight*/
        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Fry");
        consequent = new KeyValuePair<string, string>("CouzinDirectionWeight", "High");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Young");
        consequent = new KeyValuePair<string, string>("CouzinDirectionWeight", "Medium");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Adult");
        consequent = new KeyValuePair<string, string>("CouzinDirectionWeight", "Low");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Old");
        consequent = new KeyValuePair<string, string>("CouzinDirectionWeight", "Low");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));


        /* ObstacleAvoidanceDirectionWeight */
        antecedents = new Dictionary<string, string>();
        antecedents.Add("ObstacleDistance", "Close");
        consequent = new KeyValuePair<string, string>("ObstacleAvoidanceDirectionWeight", "High");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("ObstacleDistance", "Medium");
        consequent = new KeyValuePair<string, string>("ObstacleAvoidanceDirectionWeight", "Medium");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("ObstacleDistance", "Far");
        consequent = new KeyValuePair<string, string>("ObstacleAvoidanceDirectionWeight", "Low");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));


        /* PredatorAvoidanceDirectionWeight */
        antecedents = new Dictionary<string, string>();
        antecedents.Add("PredatorDistance", "Close");
        consequent = new KeyValuePair<string, string>("PredatorAvoidanceDirectionWeight", "High");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("PredatorDistance", "Medium");
        consequent = new KeyValuePair<string, string>("PredatorAvoidanceDirectionWeight", "Medium");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("PredatorDistance", "Far");
        consequent = new KeyValuePair<string, string>("PredatorAvoidanceDirectionWeight", "Low");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));


        /* PreyFollowingDirectionWeight */
        antecedents = new Dictionary<string, string>();
        antecedents.Add("PreyDistance", "Close");
        consequent = new KeyValuePair<string, string>("PreyFollowingDirectionWeight", "High");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("PreyDistance", "Medium");
        consequent = new KeyValuePair<string, string>("PreyFollowingDirectionWeight", "Medium");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("PreyDistance", "Far");
        consequent = new KeyValuePair<string, string>("PreyFollowingDirectionWeight", "Low");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));


        /* CentroidFollowingDirectionWeight */
        antecedents = new Dictionary<string, string>();
        antecedents.Add("CentroidDistance", "Close");
        consequent = new KeyValuePair<string, string>("CentroidFollowingDirectionWeight", "Low");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("CentroidDistance", "Medium");
        consequent = new KeyValuePair<string, string>("CentroidFollowingDirectionWeight", "Medium");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("CentroidDistance", "Far");
        consequent = new KeyValuePair<string, string>("CentroidFollowingDirectionWeight", "High");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Fry");
        consequent = new KeyValuePair<string, string>("CentroidFollowingDirectionWeight", "High");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Young");
        consequent = new KeyValuePair<string, string>("CentroidFollowingDirectionWeight", "Medium");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Adult");
        consequent = new KeyValuePair<string, string>("CentroidFollowingDirectionWeight", "Low");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Old");
        consequent = new KeyValuePair<string, string>("CentroidFollowingDirectionWeight", "Low");
        fishFuzzySystem.AddRule(new Rule(antecedents, consequent));

        fuzzySystem = fishFuzzySystem;
    }

    private void Update()
    {
        if (test)
        {
            string text = "";
            for (int i = 0; i < n_operation_by_frame; i++)
            {
                Dictionary<string, float> variables = new Dictionary<string, float>();
                variables.Add("Hunger", 0.5f);
                variables.Add("Size", 0.5f);
                variables.Add("Age", 75);
                variables.Add("Energy", 0.3f);
                variables.Add("ObstacleDistance", 20);
                variables.Add("CentroidDistance", 30);
                variables.Add("PredatorDistance", float.MaxValue);
                variables.Add("PreyDistance", float.MaxValue);

                Dictionary<string, float> outputs = fuzzySystem.FuzzyInferenceSystem(variables);
                foreach (var output in outputs)
                {
                    text += output.Key + ": " + output.Value + "; ";
                }
                text += "\n";
            }
            if (text != "")
                Debug.Log(text);
        }
    }
}

