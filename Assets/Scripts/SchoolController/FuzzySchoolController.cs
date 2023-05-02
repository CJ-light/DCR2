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

        FuzzyVariable ageSet = new FuzzyVariable("Age");
        ageSet.AddFuzzySet(new DiagonalLineFuzzySet("Fry", one: 0, zero: lifeExpectation * 0.2f));
        ageSet.AddFuzzySet(new TriangularFuzzySet("Young", min_zero: lifeExpectation * 0.1f, one: lifeExpectation * 0.4f, max_zero: lifeExpectation * 0.7f));
        ageSet.AddFuzzySet(new TriangularFuzzySet("Adult", min_zero: lifeExpectation * 0.4f, one: lifeExpectation * 0.7f, max_zero: lifeExpectation));
        ageSet.AddFuzzySet(new DiagonalLineFuzzySet("Old", zero: lifeExpectation * 0.7f, one: lifeExpectation));

        FuzzyVariable energySet = new FuzzyVariable("Energy");
        energySet.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: maxEnergy * 0.5f));
        energySet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: maxEnergy * 0.15f, one: maxEnergy * 0.5f, max_zero: maxEnergy * 0.85f));
        energySet.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: maxEnergy * 0.5f, one: maxEnergy));

        FuzzyVariable centroidDistanceSet = new FuzzyVariable("CentroidDistance");
        centroidDistanceSet.AddFuzzySet(new DiagonalLineFuzzySet("Close", one: rho, zero: maxCentroidDistance * 0.5f));
        centroidDistanceSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: rho, one: maxCentroidDistance * 0.5f, max_zero: maxCentroidDistance*0.8f));
        centroidDistanceSet.AddFuzzySet(new DiagonalLineFuzzySet("Far", zero: maxCentroidDistance * 0.5f, one: maxCentroidDistance));

        FuzzyVariable predatorDistanceSet = new FuzzyVariable("PredatorDistance");
        predatorDistanceSet.AddFuzzySet(new DiagonalLineFuzzySet("Close", one: predatorRelevantDistance * 0.25f, zero: predatorRelevantDistance * 0.5f));
        predatorDistanceSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: predatorRelevantDistance * 0.25f, one: predatorRelevantDistance * 0.5f, max_zero: predatorRelevantDistance * 0.8f));
        predatorDistanceSet.AddFuzzySet(new DiagonalLineFuzzySet("Far", zero: predatorRelevantDistance * 0.5f, one: predatorRelevantDistance));

        FuzzyVariable preyDistanceSet = new FuzzyVariable("PreyDistance");
        preyDistanceSet.AddFuzzySet(new DiagonalLineFuzzySet("Close", one: 3, zero: 7));
        preyDistanceSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 3, one: 7, max_zero: 11));
        preyDistanceSet.AddFuzzySet(new DiagonalLineFuzzySet("Far", zero: 7, one: 11));



        /* DEPENDIENT FUZZY VARIABLES */

        FuzzyVariable speedSet = new FuzzyVariable("Speed");
        speedSet.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: minSpeed, zero: minSpeed + (maxSpeed - minSpeed) * 0.5f));
        speedSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: minSpeed + (maxSpeed - minSpeed) * 0.15f, one: minSpeed + (maxSpeed - minSpeed) * 0.5f, max_zero: minSpeed + (maxSpeed - minSpeed) * 0.85f));
        speedSet.AddFuzzySet(new DiagonalLineFuzzySet("Fast", zero: minSpeed + (maxSpeed - minSpeed) * 0.5f, one: maxSpeed));

        FuzzyVariable swimmingDepthSet = new FuzzyVariable("SwimmingDepth");
        swimmingDepthSet.AddFuzzySet(new DiagonalLineFuzzySet("Low", zero: -20f, one: 0));
        swimmingDepthSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: -30, one: -20, max_zero: -10));
        swimmingDepthSet.AddFuzzySet(new TriangularFuzzySet("High", min_zero: -40, one: -30, max_zero: -20));
        swimmingDepthSet.AddFuzzySet(new DiagonalLineFuzzySet("VeryHigh", zero: -35, one: -45));

        FuzzyVariable preySearchingRangeSet = new FuzzyVariable("PreySearchingRange");
        preySearchingRangeSet.AddFuzzySet(new DiagonalLineFuzzySet("Small", one: preferedHuntingRange * 0.5f, zero: preferedHuntingRange));
        preySearchingRangeSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: preferedHuntingRange * 0.5f, one: maxHuntingRange * 0.5f, max_zero: maxHuntingRange*0.75f));
        preySearchingRangeSet.AddFuzzySet(new DiagonalLineFuzzySet("Big", zero: maxHuntingRange * 0.5f, one: maxHuntingRange));

        FuzzyVariable couzinDirectionWeightSet = new FuzzyVariable("CouzinDirectionWeight");
        couzinDirectionWeightSet.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: 0.5f));
        couzinDirectionWeightSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 0.15f, one: 0.5f, max_zero: 0.85f));
        couzinDirectionWeightSet.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: 0.5f, one: 1));

        FuzzyVariable predatorAvoidanceDirectionWeightSet = new FuzzyVariable("PredatorAvoidanceDirectionWeight");
        predatorAvoidanceDirectionWeightSet.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: 0.5f));
        predatorAvoidanceDirectionWeightSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 0.15f, one: 0.5f, max_zero: 0.85f));
        predatorAvoidanceDirectionWeightSet.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: 0.5f, one: 1));

        FuzzyVariable preyFollowingDirectionWeightSet = new FuzzyVariable("PreyFollowingDirectionWeight");
        preyFollowingDirectionWeightSet.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: 0.5f));
        preyFollowingDirectionWeightSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 0.15f, one: 0.5f, max_zero: 0.85f));
        preyFollowingDirectionWeightSet.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: 0.5f, one: 1));

        FuzzyVariable centroidFollowingDirectionWeightSet = new FuzzyVariable("CentroidFollowingDirectionWeight");
        centroidFollowingDirectionWeightSet.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: 0.5f));
        centroidFollowingDirectionWeightSet.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 0.15f, one: 0.5f, max_zero: 0.85f));
        centroidFollowingDirectionWeightSet.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: 0.5f, one: 1));



        /* ADD VARIABLES TO FUZZY SYSTEM */

        fuzzySystem.AddIndependientVariable(hungerVariable);
        fuzzySystem.AddIndependientVariable(sizeVariable);
        fuzzySystem.AddIndependientVariable(ageSet);
        fuzzySystem.AddIndependientVariable(energySet);
        fuzzySystem.AddIndependientVariable(centroidDistanceSet);
        fuzzySystem.AddIndependientVariable(predatorDistanceSet);
        fuzzySystem.AddIndependientVariable(preyDistanceSet);

        fuzzySystem.AddDependientVariable(speedSet);
        fuzzySystem.AddDependientVariable(swimmingDepthSet);
        fuzzySystem.AddDependientVariable(preySearchingRangeSet);
        fuzzySystem.AddDependientVariable(couzinDirectionWeightSet);
        fuzzySystem.AddDependientVariable(predatorAvoidanceDirectionWeightSet);
        fuzzySystem.AddDependientVariable(preyFollowingDirectionWeightSet);
        fuzzySystem.AddDependientVariable(centroidFollowingDirectionWeightSet);
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
