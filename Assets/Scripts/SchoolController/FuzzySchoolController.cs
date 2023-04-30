using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FuzzyLogic;

public class FuzzySchoolController : SchoolController
{
    [Header("Fuzzy Speed Settings")]
    [SerializeField] protected float minSpeed = 0.5f;
    [SerializeField] protected float maxSpeed = 5f;

    protected FuzzySystem fuzzySystem;
    Dictionary<string, float> variables;

    protected override void Start()
    {
        base.Start();

        if (minSpeed > maxSpeed)
        {
            maxSpeed = minSpeed;
        }

        SetFuzzySystem();

        variables = new Dictionary<string, float>();
        variables.Add("Hunger", 0);
        variables.Add("Size", 0);
        variables.Add("Age", 0);
        variables.Add("Energy", 0);
        variables.Add("CentroidDistance", 0);
        variables.Add("PredatorDistance", 0);
        variables.Add("PreyDistance", 0);
    }

    protected void SetFuzzySystem()
    {
        fuzzySystem = new FuzzySystem("FishFuzzySystem");
        SetVariables();
        SetRules();
    }

    protected virtual void SetVariables()
    {

        /* INDEPENDIENT FUZZY VARIABLES */

        FuzzyVariable hungerVariable = new FuzzyVariable("Hunger");
        hungerVariable.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: maxHunger / 2));
        hungerVariable.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: maxHunger * 0.15f, one: maxHunger * 0.5f, max_zero: maxHunger * 0.85f));
        hungerVariable.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: maxHunger * 0.5f, one: maxHunger));

        FuzzyVariable sizeVariable = new FuzzyVariable("Size");
        sizeVariable.AddFuzzySet(new DiagonalLineFuzzySet("Small", one: minSizeScale, zero: minSizeScale + (maxSizeScale - minSizeScale) * 0.5f));
        sizeVariable.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: minSizeScale + (maxSizeScale - minSizeScale) * 0.15f, one: minSizeScale + (maxSizeScale - minSizeScale) * 0.5f, max_zero: minSizeScale + (maxSizeScale - minSizeScale) * 0.85f));
        sizeVariable.AddFuzzySet(new DiagonalLineFuzzySet("Big", zero: minSizeScale + (maxSizeScale - minSizeScale) * 0.5f, one: maxSizeScale));

        FuzzyVariable ageVariable = new FuzzyVariable("Age");
        ageVariable.AddFuzzySet(new DiagonalLineFuzzySet("Fry", one: 0, zero: lifeExpectation * 0.2f));
        ageVariable.AddFuzzySet(new TriangularFuzzySet("Young", min_zero: lifeExpectation * 0.1f, one: lifeExpectation * 0.4f, max_zero: lifeExpectation * 0.7f));
        ageVariable.AddFuzzySet(new TriangularFuzzySet("Adult", min_zero: lifeExpectation * 0.4f, one: lifeExpectation * 0.7f, max_zero: lifeExpectation));
        ageVariable.AddFuzzySet(new DiagonalLineFuzzySet("Old", zero: lifeExpectation * 0.7f, one: lifeExpectation));

        FuzzyVariable energyVariable = new FuzzyVariable("Energy");
        energyVariable.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: maxEnergy * 0.5f));
        energyVariable.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: maxEnergy * 0.15f, one: maxEnergy * 0.5f, max_zero: maxEnergy * 0.85f));
        energyVariable.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: maxEnergy * 0.5f, one: maxEnergy));

        FuzzyVariable centroidDistanceVariable = new FuzzyVariable("CentroidDistance");
        centroidDistanceVariable.AddFuzzySet(new DiagonalLineFuzzySet("Close", one: rho, zero: maxCentroidDistance * 0.5f));
        centroidDistanceVariable.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: rho, one: maxCentroidDistance * 0.5f, max_zero: maxCentroidDistance*0.8f));
        centroidDistanceVariable.AddFuzzySet(new DiagonalLineFuzzySet("Far", zero: maxCentroidDistance * 0.5f, one: maxCentroidDistance));

        FuzzyVariable predatorDistanceVariable = new FuzzyVariable("PredatorDistance");
        predatorDistanceVariable.AddFuzzySet(new DiagonalLineFuzzySet("Close", one: predatorRelevantDistance * 0.25f, zero: predatorRelevantDistance * 0.5f));
        predatorDistanceVariable.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: predatorRelevantDistance * 0.25f, one: predatorRelevantDistance * 0.5f, max_zero: predatorRelevantDistance * 0.8f));
        predatorDistanceVariable.AddFuzzySet(new DiagonalLineFuzzySet("Far", zero: predatorRelevantDistance * 0.5f, one: predatorRelevantDistance));

        FuzzyVariable preyDistanceVariable = new FuzzyVariable("PreyDistance");
        preyDistanceVariable.AddFuzzySet(new DiagonalLineFuzzySet("Close", one: 3, zero: 7));
        preyDistanceVariable.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 3, one: 7, max_zero: 11));
        preyDistanceVariable.AddFuzzySet(new DiagonalLineFuzzySet("Far", zero: 7, one: 11));



        /* DEPENDIENT FUZZY VARIABLES */

        FuzzyVariable speedVariable = new FuzzyVariable("Speed");
        speedVariable.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: minSpeed, zero: minSpeed + (maxSpeed - minSpeed) * 0.5f));
        speedVariable.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: minSpeed + (maxSpeed - minSpeed) * 0.15f, one: minSpeed + (maxSpeed - minSpeed) * 0.5f, max_zero: minSpeed + (maxSpeed - minSpeed) * 0.85f));
        speedVariable.AddFuzzySet(new DiagonalLineFuzzySet("Fast", zero: minSpeed + (maxSpeed - minSpeed) * 0.5f, one: maxSpeed));

        FuzzyVariable swimmingDepthVariable = new FuzzyVariable("SwimmingDepth");
        swimmingDepthVariable.AddFuzzySet(new DiagonalLineFuzzySet("Low", zero: -20f, one: 0));
        swimmingDepthVariable.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: -30, one: -20, max_zero: -10));
        swimmingDepthVariable.AddFuzzySet(new TriangularFuzzySet("High", min_zero: -40, one: -30, max_zero: -20));
        swimmingDepthVariable.AddFuzzySet(new DiagonalLineFuzzySet("VeryHigh", zero: -35, one: -45));

        FuzzyVariable preySearchingRangeVariable = new FuzzyVariable("PreySearchingRange");
        preySearchingRangeVariable.AddFuzzySet(new DiagonalLineFuzzySet("Small", one: preferedHuntingRange * 0.5f, zero: preferedHuntingRange));
        preySearchingRangeVariable.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: preferedHuntingRange * 0.5f, one: maxHuntingRange * 0.5f, max_zero: maxHuntingRange*0.75f));
        preySearchingRangeVariable.AddFuzzySet(new DiagonalLineFuzzySet("Big", zero: maxHuntingRange * 0.5f, one: maxHuntingRange));

        FuzzyVariable couzinDirectionWeightVariable = new FuzzyVariable("CouzinDirectionWeight");
        couzinDirectionWeightVariable.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: 0.5f));
        couzinDirectionWeightVariable.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 0.15f, one: 0.5f, max_zero: 0.85f));
        couzinDirectionWeightVariable.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: 0.5f, one: 1));

        FuzzyVariable predatorAvoidanceDirectionWeightVariable = new FuzzyVariable("PredatorAvoidanceDirectionWeight");
        predatorAvoidanceDirectionWeightVariable.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: 0.5f));
        predatorAvoidanceDirectionWeightVariable.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 0.15f, one: 0.5f, max_zero: 0.85f));
        predatorAvoidanceDirectionWeightVariable.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: 0.5f, one: 1));

        FuzzyVariable preyFollowingDirectionWeightVariable = new FuzzyVariable("PreyFollowingDirectionWeight");
        preyFollowingDirectionWeightVariable.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: 0.5f));
        preyFollowingDirectionWeightVariable.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 0.15f, one: 0.5f, max_zero: 0.85f));
        preyFollowingDirectionWeightVariable.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: 0.5f, one: 1));

        FuzzyVariable centroidFollowingDirectionWeightVariable = new FuzzyVariable("CentroidFollowingDirectionWeight");
        centroidFollowingDirectionWeightVariable.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: 0.5f));
        centroidFollowingDirectionWeightVariable.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 0.15f, one: 0.5f, max_zero: 0.85f));
        centroidFollowingDirectionWeightVariable.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: 0.5f, one: 1));



        /* ADD VARIABLES TO FUZZY SYSTEM */

        fuzzySystem.AddIndependientVariable(hungerVariable);
        fuzzySystem.AddIndependientVariable(sizeVariable);
        fuzzySystem.AddIndependientVariable(ageVariable);
        fuzzySystem.AddIndependientVariable(energyVariable);
        fuzzySystem.AddIndependientVariable(centroidDistanceVariable);
        fuzzySystem.AddIndependientVariable(predatorDistanceVariable);
        fuzzySystem.AddIndependientVariable(preyDistanceVariable);

        fuzzySystem.AddDependientVariable(speedVariable);
        fuzzySystem.AddDependientVariable(swimmingDepthVariable);
        fuzzySystem.AddDependientVariable(preySearchingRangeVariable);
        fuzzySystem.AddDependientVariable(couzinDirectionWeightVariable);
        fuzzySystem.AddDependientVariable(predatorAvoidanceDirectionWeightVariable);
        fuzzySystem.AddDependientVariable(preyFollowingDirectionWeightVariable);
        fuzzySystem.AddDependientVariable(centroidFollowingDirectionWeightVariable);
    }

    protected virtual void SetRules()
    {
        // RULES
        Dictionary<string, string> antecedents;
        KeyValuePair<string, string> consequent;

        // SPEED
        antecedents = new Dictionary<string, string>();
        antecedents.Add("Energy", "Low");
        consequent = new KeyValuePair<string, string>("Speed", "Low");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Energy", "Medium");
        consequent = new KeyValuePair<string, string>("Speed", "Medium");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Energy", "High");
        consequent = new KeyValuePair<string, string>("Speed", "Fast");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Fry");
        consequent = new KeyValuePair<string, string>("Speed", "Low");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Young");
        consequent = new KeyValuePair<string, string>("Speed", "Medium");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Adult");
        consequent = new KeyValuePair<string, string>("Speed", "Medium");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Old");
        consequent = new KeyValuePair<string, string>("Speed", "Low");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("PredatorDistance", "Close");
        consequent = new KeyValuePair<string, string>("Speed", "Fast");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        /* SwimmingDepth */
        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Fry");
        consequent = new KeyValuePair<string, string>("SwimmingDepth", "VeryHigh");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Young");
        consequent = new KeyValuePair<string, string>("SwimmingDepth", "High");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Adult");
        consequent = new KeyValuePair<string, string>("SwimmingDepth", "Low");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Old");
        consequent = new KeyValuePair<string, string>("SwimmingDepth", "Medium");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));


        /* PreySearchingRange */
        antecedents = new Dictionary<string, string>();
        antecedents.Add("Hunger", "Low");
        consequent = new KeyValuePair<string, string>("PreySearchingRange", "Medium");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Hunger", "Medium");
        consequent = new KeyValuePair<string, string>("PreySearchingRange", "Medium");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Hunger", "High");
        consequent = new KeyValuePair<string, string>("PreySearchingRange", "Big");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));


        /*CouzinDirectionWeight*/
        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Fry");
        consequent = new KeyValuePair<string, string>("CouzinDirectionWeight", "High");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Young");
        consequent = new KeyValuePair<string, string>("CouzinDirectionWeight", "Medium");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Adult");
        consequent = new KeyValuePair<string, string>("CouzinDirectionWeight", "Medium");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Old");
        consequent = new KeyValuePair<string, string>("CouzinDirectionWeight", "Medium");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        /* PredatorAvoidanceDirectionWeight */
        antecedents = new Dictionary<string, string>();
        antecedents.Add("PredatorDistance", "Close");
        consequent = new KeyValuePair<string, string>("PredatorAvoidanceDirectionWeight", "High");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("PredatorDistance", "Medium");
        consequent = new KeyValuePair<string, string>("PredatorAvoidanceDirectionWeight", "Medium");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("PredatorDistance", "Far");
        consequent = new KeyValuePair<string, string>("PredatorAvoidanceDirectionWeight", "Low");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));


        /* PreyFollowingDirectionWeight */
        antecedents = new Dictionary<string, string>();
        antecedents.Add("PreyDistance", "Close");
        consequent = new KeyValuePair<string, string>("PreyFollowingDirectionWeight", "High");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("PreyDistance", "Medium");
        consequent = new KeyValuePair<string, string>("PreyFollowingDirectionWeight", "High");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("PreyDistance", "Far");
        consequent = new KeyValuePair<string, string>("PreyFollowingDirectionWeight", "Medium");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));


        /* CentroidFollowingDirectionWeight */
        antecedents = new Dictionary<string, string>();
        antecedents.Add("CentroidDistance", "Close");
        consequent = new KeyValuePair<string, string>("CentroidFollowingDirectionWeight", "Low");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("CentroidDistance", "Medium");
        consequent = new KeyValuePair<string, string>("CentroidFollowingDirectionWeight", "Medium");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("CentroidDistance", "Far");
        consequent = new KeyValuePair<string, string>("CentroidFollowingDirectionWeight", "High");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Fry");
        consequent = new KeyValuePair<string, string>("CentroidFollowingDirectionWeight", "High");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Young");
        consequent = new KeyValuePair<string, string>("CentroidFollowingDirectionWeight", "High");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Adult");
        consequent = new KeyValuePair<string, string>("CentroidFollowingDirectionWeight", "Medium");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Old");
        consequent = new KeyValuePair<string, string>("CentroidFollowingDirectionWeight", "Medium");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));
    }

    protected override FishInformation GetFishInformation(int i)
    {
        variables["Hunger"] = fishList[i].hunger;
        variables["Energy"] = fishList[i].energy;
        variables["Size"] = fishList[i].sizeScale;
        variables["Age"] = fishList[i].age;
        variables["PredatorDistance"] = float.MaxValue;

        GameObject predator = GetCloserPredator(i);
        if (predatorAvoidanceDirectionWeight > 0 && predator)
            variables["PredatorDistance"] = (predator.transform.position - fishList[i].transform.position).magnitude;
        variables["PreyDistance"] = float.MaxValue;
        if (preyFollowingDirectionWeight > 0 && fishList[i].prey)
            variables["PreyDistance"] = (fishList[i].prey.transform.position - fishList[i].transform.position).magnitude;
        variables["CentroidDistance"] = float.MaxValue;

        Dictionary<string, float> output_variables = fuzzySystem.FuzzyInferenceSystem(variables);

        preferedSwimmingDepth = output_variables["SwimmingDepth"];
        if (preferedSwimmingDepth > -preferedSwimmingDepthRange)
        {
            preferedSwimmingDepth = -preferedSwimmingDepthRange;
        }

        // Calcular la direccion
        Vector3 obstacleAvoidanceDirection = GetObstacleAvoidanceDirection(i);
        Vector3 predatorAvoidanceDirection = GetPredatorAvoidanceDirection(i);
        Vector3 preyFollowingDirection = GetPreyFollowingDirection(i);
        Vector3 centroidFollowingDirection = GetCentroidFollowingDirection(i);
        Vector3 couzinDirection = GetCouzinDirection(i);
        Vector3 knownRoute = GetKnownRoute(i);
        Vector3 swimmingDepthDirection = GetSwimmingDepthDirection(i);


        Vector3 preferedDirection = couzinDirection * couzinDirectionWeight * output_variables["CouzinDirectionWeight"]
                                  + knownRoute * fishList[i].omega
                                  + obstacleAvoidanceDirection * obstacleAvoidanceDirectionWeight
                                  + predatorAvoidanceDirection * predatorAvoidanceDirectionWeight * output_variables["PredatorAvoidanceDirectionWeight"]
                                  + preyFollowingDirection * preyFollowingDirectionWeight * output_variables["PreyFollowingDirectionWeight"]
                                  + centroidFollowingDirection * centroidFollowingDirectionWeight * output_variables["CentroidFollowingDirectionWeight"]
                                  + swimmingDepthDirection * swimmingDepthDirectionWeight;

        if (preferedDirection == Vector3.zero)
        {
            preferedDirection = fishList[i].transform.forward;
        }
        else
        {
            preferedDirection /= preferedDirection.magnitude;
        }

        return new FishInformation(preferedDirection, output_variables["Speed"], output_variables["PreySearchingRange"]);
    }

    protected override Vector3 GetCentroidFollowingDirection(int i)
    {
        Vector3 centroidFollowingDirection = Vector3.zero;

        if (centroidFollowingDirectionWeight > 0)
        {
            centroidFollowingDirection = (centroid - fishList[i].transform.position).normalized;
        }

        return centroidFollowingDirection;
    }
    
    public override void ResetSchool()
    {
        base.ResetSchool();
        SetFuzzySystem();
    }
}
