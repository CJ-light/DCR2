using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FuzzyLogic;
using TMPro;
using Info;
using System;


/*
FuzzySchoolController
*/
public class FuzzySchoolController : SchoolController
{
    [Header("Fuzzy Speed Settings")]
    [SerializeField] protected float minSpeed = 0.5f;
    [SerializeField] protected float maxSpeed = 5f;

    [Header("Testing")]
    protected bool isTesting = false;

    protected FuzzySystem fuzzySystem;
    Dictionary<string, float> variables;

    TextMeshPro m_textMeshPro;

    int schoolID;
    int isFuzzyAge = 1;
    int isFuzzyEnergy = 1;
    int swimmingDepthRules2 = 1;
    int centroidFollowingDirectionWeightRules2 = 1;
    int couzinDirectionWeightRules2 = 1;

    Dictionary<string, float> output_variables;

    Info.FuzzySchoolControllerInfo infoTool;


    protected override void Start()
    {
        isFuzzy = true;
        
        //Create the class that will store the information of this class
        if (isTesting)
        {
            schoolID = gameObject.GetInstanceID();
            infoTool = new Info.FuzzySchoolControllerInfo(schoolID, minSpeed, maxSpeed, lifeExpectation);
            enableRegeneration = infoTool.enableRegeneration;
            randomInitialEnergy = Convert.ToBoolean(infoTool.startRandomEnergy);
            randomAge = Convert.ToBoolean(infoTool.startRandomAge);
            isFuzzyAge = infoTool.isFuzzyAge;
            isFuzzyEnergy = infoTool.isFuzzyEnergy;
            proportion = infoTool.proportion;
            fixedAge = Convert.ToBoolean(infoTool.fixedAge);
            fixedAgeValue = infoTool.fixedAgeValue;
            fixedEnergy = Convert.ToBoolean(infoTool.fixedEnergy);
            fixedEnergyValue = infoTool.fixedEnergyValue;
            swimmingDepthRules2 = infoTool.swimmingDepthRules2;
            centroidFollowingDirectionWeightRules2 = infoTool.centroidFollowingDirectionWeightRules2;
            couzinDirectionWeightRules2 = infoTool.couzinDirectionWeightRules2;
        }

        base.Start();   //Runs the start function of SchoolController.cs

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

        GameObject newTextObject = new GameObject("NewTextObject");
        m_textMeshPro = newTextObject.AddComponent<TextMeshPro>();
        if (isTesting)
        {
            m_textMeshPro.text = "Speed";
            m_textMeshPro.fontSize = 100;
        }
        
    }

    /*
    SetFuzzySystem :: This prepares the Fuzzy System, the fuzzy system is the high level representation of the Fuzzy logic processes.
        It prepares the independent and dependent fuzzy variables that we're going to use, and then we declare how they're going to affect each other with the rules. 
    Parameters ::
        None
    Return ::
        None
    */
    protected void SetFuzzySystem()
    {
        fuzzySystem = new FuzzySystem("FishFuzzySystem");
        SetVariables();
        SetRules();
    }

    /*
    SetVariables :: This function will prepare the independent and dependent fuzzy variables.
        This includes declaring the name of each variable. It also includes the definition of the fuzzy sets in each variable.
        
    */
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
        switch (isFuzzyAge){
            case 1:
                //FuzzyLogic Age 1 : Original
                ageVariable.AddFuzzySet(new DiagonalLineFuzzySet("Fry", one: 0, zero: lifeExpectation * 0.2f));
                ageVariable.AddFuzzySet(new TriangularFuzzySet("Young", min_zero: lifeExpectation * 0.1f, one: lifeExpectation * 0.4f, max_zero: lifeExpectation * 0.7f));
                ageVariable.AddFuzzySet(new TriangularFuzzySet("Adult", min_zero: lifeExpectation * 0.4f, one: lifeExpectation * 0.7f, max_zero: lifeExpectation));
                ageVariable.AddFuzzySet(new DiagonalLineFuzzySet("Old", zero: lifeExpectation * 0.7f, one: lifeExpectation));   
                break;
            case 0:
                //FuzzyLogic Age Default (No Fuzzy)
                ageVariable.AddFuzzySet(new TrapezoidFuzzySet("Fry", min_zero: lifeExpectation * 0f, min_one: lifeExpectation * 0.0008f, max_one: lifeExpectation * 0.166f, max_zero: lifeExpectation * 0.167f));
                ageVariable.AddFuzzySet(new TrapezoidFuzzySet("Young", min_zero: lifeExpectation * 0.167f, min_one: lifeExpectation * 0.168f, max_one: lifeExpectation * 0.541f, max_zero: lifeExpectation * 0.542f));
                ageVariable.AddFuzzySet(new TrapezoidFuzzySet("Adult", min_zero: lifeExpectation * 0.542f, min_one: lifeExpectation * 0.543f, max_one: lifeExpectation * 0.874f, max_zero: lifeExpectation * 0.875f));
                ageVariable.AddFuzzySet(new TrapezoidFuzzySet("Old", min_zero: lifeExpectation * 0.875f, min_one: lifeExpectation * 0.876f, max_one: lifeExpectation * 0.999f, max_zero: lifeExpectation * 1.0f)); 
                break;      
        }

        //FuzzyLogic Age Default (No Fuzzy)
        /*
        FuzzyVariable ageVariable = new FuzzyVariable("Age");
        ageVariable.AddFuzzySet(new TrapezoidFuzzySet("Fry", min_zero: lifeExpectation * 0f, min_one: lifeExpectation * 0.0008f, max_one: lifeExpectation * 0.166f, max_zero: lifeExpectation * 0.167f));
        ageVariable.AddFuzzySet(new TrapezoidFuzzySet("Young", min_zero: lifeExpectation * 0.167f, min_one: lifeExpectation * 0.168f, max_one: lifeExpectation * 0.541f, max_zero: lifeExpectation * 0.542f));
        ageVariable.AddFuzzySet(new TrapezoidFuzzySet("Adult", min_zero: lifeExpectation * 0.542f, min_one: lifeExpectation * 0.543f, max_one: lifeExpectation * 0.874f, max_zero: lifeExpectation * 0.875f));
        ageVariable.AddFuzzySet(new TrapezoidFuzzySet("Old", min_zero: lifeExpectation * 0.875f, min_one: lifeExpectation * 0.876f, max_one: lifeExpectation * 0.999f, max_zero: lifeExpectation * 1.0f));
        */

        //FuzzyLogic Age 1 : Original
        /*
        FuzzyVariable ageVariable = new FuzzyVariable("Age");
        ageVariable.AddFuzzySet(new DiagonalLineFuzzySet("Fry", one: 0, zero: lifeExpectation * 0.2f));
        ageVariable.AddFuzzySet(new TriangularFuzzySet("Young", min_zero: lifeExpectation * 0.1f, one: lifeExpectation * 0.4f, max_zero: lifeExpectation * 0.7f));
        ageVariable.AddFuzzySet(new TriangularFuzzySet("Adult", min_zero: lifeExpectation * 0.4f, one: lifeExpectation * 0.7f, max_zero: lifeExpectation));
        ageVariable.AddFuzzySet(new DiagonalLineFuzzySet("Old", zero: lifeExpectation * 0.7f, one: lifeExpectation));
        */

        //FuzzyLogic Age 2 : They start going near the water surface, and then stay there going in circles in a group 
        /*
        FuzzyVariable ageVariable = new FuzzyVariable("Age");
        ageVariable.AddFuzzySet(new DiagonalLineFuzzySet("Fry", one: 0, zero: lifeExpectation * 0.0073f));
        ageVariable.AddFuzzySet(new TrapezoidFuzzySet("Young", min_zero: lifeExpectation * 0.0033f, min_one: lifeExpectation * 0.05f, max_one: lifeExpectation * 0.056f, max_zero: lifeExpectation * 0.075f));
        ageVariable.AddFuzzySet(new TrapezoidFuzzySet("Adult", min_zero: lifeExpectation * 0.056f, min_one: lifeExpectation * 0.067f, max_one: lifeExpectation * 0.67f, max_zero: lifeExpectation * 0.83f));
        ageVariable.AddFuzzySet(new DiagonalLineFuzzySet("Old", zero: lifeExpectation * 0.67f, one: lifeExpectation));
        */

        //FuzzyLogic Age 3: same problem as Fuzzy Logic Age 2
        /*
        FuzzyVariable ageVariable = new FuzzyVariable("Age");
        ageVariable.AddFuzzySet(new DiagonalLineFuzzySet("Fry", one: 0, zero: lifeExpectation * 0.0073f));
        ageVariable.AddFuzzySet(new TrapezoidFuzzySet("Young", min_zero: lifeExpectation * 0.0033f, min_one: lifeExpectation * 0.05f, max_one: lifeExpectation * 0.056f, max_zero: lifeExpectation * 0.075f));
        ageVariable.AddFuzzySet(new GaussianFuzzySet("Adult", centroid: lifeExpectation * 0.42f, stdDev: lifeExpectation * 0.17f));
        ageVariable.AddFuzzySet(new DiagonalLineFuzzySet("Old", zero: lifeExpectation * 0.67f, one: lifeExpectation));
        */

        FuzzyVariable energyVariable = new FuzzyVariable("Energy");
        switch (isFuzzyEnergy){
            case 1:
                //FuzzyLogic Energy (Original)
                energyVariable.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: maxEnergy * 0.5f));
                energyVariable.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: maxEnergy * 0.15f, one: maxEnergy * 0.5f, max_zero: maxEnergy * 0.85f));
                energyVariable.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: maxEnergy * 0.5f, one: maxEnergy));
                break;
            case 0:
                //FuzzyLogic Energy Default (No Fuzzy):
                energyVariable.AddFuzzySet(new TrapezoidFuzzySet("Low", min_zero: maxEnergy * 0f, min_one: maxEnergy * 0.001f, max_one: maxEnergy * 0.299f, max_zero: maxEnergy * 0.3f));
                energyVariable.AddFuzzySet(new TrapezoidFuzzySet("Medium", min_zero: maxEnergy * 0.3f, min_one: maxEnergy * 0.301f, max_one: maxEnergy * 0.699f, max_zero: maxEnergy * 0.7f));
                energyVariable.AddFuzzySet(new TrapezoidFuzzySet("High", min_zero: maxEnergy * 0.7f, min_one: maxEnergy * 0.701f, max_one: maxEnergy * 0.999f, max_zero: maxEnergy * 1.0f));
                break;
        }

        //FuzzyLogic Energy Default (No Fuzzy):
        /*
        FuzzyVariable energyVariable = new FuzzyVariable("Energy");
        energyVariable.AddFuzzySet(new TrapezoidFuzzySet("Low", min_zero: maxEnergy * 0f, min_one: maxEnergy * 0.001f, max_one: maxEnergy * 0.299f, max_zero: maxEnergy * 0.3f));
        energyVariable.AddFuzzySet(new TrapezoidFuzzySet("Medium", min_zero: maxEnergy * 0.3f, min_one: maxEnergy * 0.301f, max_one: maxEnergy * 0.699f, max_zero: maxEnergy * 0.7f));
        energyVariable.AddFuzzySet(new TrapezoidFuzzySet("High", min_zero: maxEnergy * 0.7f, min_one: maxEnergy * 0.701f, max_one: maxEnergy * 0.999f, max_zero: maxEnergy * 1.0f));
        */

        //FuzzyLogic Energy 1
        /*
        FuzzyVariable energyVariable = new FuzzyVariable("Energy");
        energyVariable.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: maxEnergy * 0.5f));
        energyVariable.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: maxEnergy * 0.15f, one: maxEnergy * 0.5f, max_zero: maxEnergy * 0.85f));
        energyVariable.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: maxEnergy * 0.5f, one: maxEnergy));
        */
        
        //FuzzyLogic Energy 2
        /*
        FuzzyVariable energyVariable = new FuzzyVariable("Energy");
        energyVariable.AddFuzzySet(new DiagonalLineFuzzySet("Low", one: 0, zero: maxEnergy * 0.2f));
        energyVariable.AddFuzzySet(new GaussianFuzzySet("Medium", centroid: maxEnergy * 0.4f, stdDev: maxEnergy * 0.2f));
        energyVariable.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: maxEnergy * 0.4f, one: maxEnergy));
        */

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

        FuzzyVariable hidingDirectionWeightVariable = new FuzzyVariable("HidingDirectionWeight");
        hidingDirectionWeightVariable.AddFuzzySet(new DiagonalLineFuzzySet("Low", one:0, zero: 0.5f));
        hidingDirectionWeightVariable.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 0.15f, one: 0.5f, max_zero: 0.85f));
        hidingDirectionWeightVariable.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: 0.5f, one: 1));

        FuzzyVariable energyIncrementWeightVariable = new FuzzyVariable("EnergyIncrementWeight");
        energyIncrementWeightVariable.AddFuzzySet(new DiagonalLineFuzzySet("Low", one:0, zero: 0.5f));
        energyIncrementWeightVariable.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 0.15f, one: 0.5f, max_zero: 0.85f));
        energyIncrementWeightVariable.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: 0.5f, one: 1));

        FuzzyVariable EnergyDecrementWeightVariable = new FuzzyVariable("EnergyDecrementWeight");
        EnergyDecrementWeightVariable.AddFuzzySet(new DiagonalLineFuzzySet("Low", one:0, zero: 0.5f));
        EnergyDecrementWeightVariable.AddFuzzySet(new TriangularFuzzySet("Medium", min_zero: 0.15f, one: 0.5f, max_zero: 0.85f));
        EnergyDecrementWeightVariable.AddFuzzySet(new DiagonalLineFuzzySet("High", zero: 0.5f, one: 1));



        /* ADD VARIABLES TO FUZZY SYSTEM */
        //?? Can a variable be both an Independent and Dependent variable

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
        fuzzySystem.AddDependientVariable(hidingDirectionWeightVariable);
        fuzzySystem.AddDependientVariable(energyIncrementWeightVariable);
        fuzzySystem.AddDependientVariable(EnergyDecrementWeightVariable);
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
        //Change how age affects the swimming depth
        switch(swimmingDepthRules2){
            //Original swimming depth rules
            case 0:
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
                break;
            //New rules: Their swimming depth is usually very high
            case 1:
                antecedents = new Dictionary<string, string>();
                antecedents.Add("Age", "Fry");
                consequent = new KeyValuePair<string, string>("SwimmingDepth", "VeryHigh");
                fuzzySystem.AddRule(new Rule(antecedents, consequent));

                antecedents = new Dictionary<string, string>();
                antecedents.Add("Age", "Young");
                consequent = new KeyValuePair<string, string>("SwimmingDepth", "VeryHigh");
                fuzzySystem.AddRule(new Rule(antecedents, consequent));

                antecedents = new Dictionary<string, string>();
                antecedents.Add("Age", "Adult");
                consequent = new KeyValuePair<string, string>("SwimmingDepth", "High");
                fuzzySystem.AddRule(new Rule(antecedents, consequent));

                antecedents = new Dictionary<string, string>();
                antecedents.Add("Age", "Old");
                consequent = new KeyValuePair<string, string>("SwimmingDepth", "High");
                fuzzySystem.AddRule(new Rule(antecedents, consequent));
                break;
        }

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
        //Change how age affects Couzin Direction Weight
        switch(couzinDirectionWeightRules2){
            //Original rules
            case 0:
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
                break;
            
            //Changed rules: They usually stop following others as they grow (they go in their own preffered direction)
            case 1:
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
                consequent = new KeyValuePair<string, string>("CouzinDirectionWeight", "Low");
                fuzzySystem.AddRule(new Rule(antecedents, consequent));

                antecedents = new Dictionary<string, string>();
                antecedents.Add("Age", "Old");
                consequent = new KeyValuePair<string, string>("CouzinDirectionWeight", "Low");
                fuzzySystem.AddRule(new Rule(antecedents, consequent));
                break;
        }

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

        //Change how age affects how if these fish stay together or not
        switch(centroidFollowingDirectionWeightRules2){
            //Original rules
            case 0:
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
                break;
            
            //Changed rules: These fish usually roam alone so they're usually not grouped together
            case 1:
                antecedents = new Dictionary<string, string>();
                antecedents.Add("Age", "Fry");
                consequent = new KeyValuePair<string, string>("CentroidFollowingDirectionWeight", "High");
                fuzzySystem.AddRule(new Rule(antecedents, consequent));

                antecedents = new Dictionary<string, string>();
                antecedents.Add("Age", "Young");
                consequent = new KeyValuePair<string, string>("CentroidFollowingDirectionWeight", "Medium");
                fuzzySystem.AddRule(new Rule(antecedents, consequent));

                antecedents = new Dictionary<string, string>();
                antecedents.Add("Age", "Adult");
                consequent = new KeyValuePair<string, string>("CentroidFollowingDirectionWeight", "Low");
                fuzzySystem.AddRule(new Rule(antecedents, consequent));

                antecedents = new Dictionary<string, string>();
                antecedents.Add("Age", "Old");
                consequent = new KeyValuePair<string, string>("CentroidFollowingDirectionWeight", "Low");
                fuzzySystem.AddRule(new Rule(antecedents, consequent));
                break;
            
            /* Hiding Direction Weight */
            antecedents = new Dictionary<string, string>();
            antecedents.Add("Age", "Fry");
            consequent = new KeyValuePair<string, string>("HidingDirectionWeight", "High");
            fuzzySystem.AddRule(new Rule(antecedents, consequent));

            antecedents = new Dictionary<string, string>();
            antecedents.Add("Age", "Young");
            consequent = new KeyValuePair<string, string>("HidingDirectionWeight", "High");
            fuzzySystem.AddRule(new Rule(antecedents, consequent));

            antecedents = new Dictionary<string, string>();
            antecedents.Add("Age", "Adult");
            consequent = new KeyValuePair<string, string>("HidingDirectionWeight", "Low");
            fuzzySystem.AddRule(new Rule(antecedents, consequent));

            antecedents = new Dictionary<string, string>();
            antecedents.Add("Age", "Old");
            consequent = new KeyValuePair<string, string>("HidingDirectionWeight", "Low");
            fuzzySystem.AddRule(new Rule(antecedents, consequent));

            antecedents = new Dictionary<string, string>();
            antecedents.Add("Hunger", "Low");
            consequent = new KeyValuePair<string, string>("HidingDirectionWeight", "High");
            fuzzySystem.AddRule(new Rule(antecedents, consequent));

            antecedents = new Dictionary<string, string>();
            antecedents.Add("Hunger", "Medium");
            consequent = new KeyValuePair<string, string>("HidingDirectionWeight", "Medium");
            fuzzySystem.AddRule(new Rule(antecedents, consequent));

            antecedents = new Dictionary<string, string>();
            antecedents.Add("Hunger", "High");
            consequent = new KeyValuePair<string, string>("HidingDirectionWeight", "Low");
            fuzzySystem.AddRule(new Rule(antecedents, consequent));

            /* Energy Increment Weight */
            antecedents = new Dictionary<string, string>();
            antecedents.Add("Hunger", "Low");
            consequent = new KeyValuePair<string, string>("EnergyIncrementWeight", "High");
            fuzzySystem.AddRule(new Rule(antecedents, consequent));

            antecedents = new Dictionary<string, string>();
            antecedents.Add("Hunger", "Medium");
            consequent = new KeyValuePair<string, string>("EnergyIncrementWeight", "High");
            fuzzySystem.AddRule(new Rule(antecedents, consequent));

            antecedents = new Dictionary<string, string>();
            antecedents.Add("Hunger", "High");
            consequent = new KeyValuePair<string, string>("EnergyIncrementWeight", "Low");
            fuzzySystem.AddRule(new Rule(antecedents, consequent));

            antecedents = new Dictionary<string, string>();
            antecedents.Add("Age", "Fry");
            consequent = new KeyValuePair<string, string>("EnergyIncrementWeight", "High");
            fuzzySystem.AddRule(new Rule(antecedents, consequent));

            antecedents = new Dictionary<string, string>();
            antecedents.Add("Age", "Young");
            consequent = new KeyValuePair<string, string>("EnergyIncrementWeight", "High");
            fuzzySystem.AddRule(new Rule(antecedents, consequent));

            antecedents = new Dictionary<string, string>();
            antecedents.Add("Age", "Adult");
            consequent = new KeyValuePair<string, string>("EnergyIncrementWeight", "Medium");
            fuzzySystem.AddRule(new Rule(antecedents, consequent));   

            antecedents = new Dictionary<string, string>();
            antecedents.Add("Age", "Old");
            consequent = new KeyValuePair<string, string>("EnergyIncrementWeight", "Low");
            fuzzySystem.AddRule(new Rule(antecedents, consequent)); 

            /*Energy Decrement Weight*/
            antecedents = new Dictionary<string, string>();
            antecedents.Add("Age", "Fry");
            consequent = new KeyValuePair<string, string>("EnergyDecrementWeight", "Low");
            fuzzySystem.AddRule(new Rule(antecedents, consequent));

            antecedents = new Dictionary<string, string>();
            antecedents.Add("Age", "Young");
            consequent = new KeyValuePair<string, string>("EnergyDecrementWeight", "Low");
            fuzzySystem.AddRule(new Rule(antecedents, consequent));

            antecedents = new Dictionary<string, string>();
            antecedents.Add("Age", "Adult");
            consequent = new KeyValuePair<string, string>("EnergyDecrementWeight", "Medium");
            fuzzySystem.AddRule(new Rule(antecedents, consequent));   

            antecedents = new Dictionary<string, string>();
            antecedents.Add("Age", "Old");
            consequent = new KeyValuePair<string, string>("EnergyDecrementWeight", "Medium");
            fuzzySystem.AddRule(new Rule(antecedents, consequent)); 
        }
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

        //Dictionary<string, float> output_variables = fuzzySystem.FuzzyInferenceSystem(variables);
        output_variables = fuzzySystem.FuzzyInferenceSystem(variables);

        preferedSwimmingDepth = output_variables["SwimmingDepth"];
        if (preferedSwimmingDepth > -preferedSwimmingDepthRange)
        {
            preferedSwimmingDepth = -preferedSwimmingDepthRange;
        }

        // Calcular la direccion
        Vector3 obstacleAvoidanceDirection = GetObstacleAvoidanceDirection(i);
        Vector3 hidingDirection = GetHidingLocationDirection(i);
        Vector3 predatorAvoidanceDirection = GetPredatorAvoidanceDirection(i);
        Vector3 preyFollowingDirection = GetPreyFollowingDirection(i);
        Vector3 centroidFollowingDirection = GetCentroidFollowingDirection(i);
        Vector3 couzinDirection = GetCouzinDirection(i);
        Vector3 knownRoute = GetKnownRoute(i);
        Vector3 swimmingDepthDirection = GetSwimmingDepthDirection(i);

        Vector3 preferedDirection = couzinDirection * couzinDirectionWeight * output_variables["CouzinDirectionWeight"]
                                  + knownRoute * fishList[i].omega
                                  + obstacleAvoidanceDirection * obstacleAvoidanceDirectionWeight
                                  //+ hidingDirection * hidingDirectionWeight * output_variables["HidingDirectionWeight"]
                                  + predatorAvoidanceDirection * predatorAvoidanceDirectionWeight * output_variables["PredatorAvoidanceDirectionWeight"]
                                  + preyFollowingDirection * preyFollowingDirectionWeight * output_variables["PreyFollowingDirectionWeight"]
                                  + centroidFollowingDirection * centroidFollowingDirectionWeight * output_variables["CentroidFollowingDirectionWeight"]
                                  + swimmingDepthDirection * swimmingDepthDirectionWeight;

        fishList[i].individualEnergyIncrement = energyIncrement * output_variables["EnergyIncrementWeight"];
        fishList[i].individualEnergyDecrement = energyDecrement * output_variables["EnergyDecrementWeight"];
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

    public void UpdateFishInformation()
    {
        // Remueve al depredador si esta muy lejos
        for (int i = 0; i < predators.Count; i++)
        {
            if (predators[i] == null || (predators[i].transform.position - centroid).magnitude > predatorRelevantDistanceToSchool)
            {
                predators.RemoveAt(i);
                i--;
            }
        }
        // Remueve los otros peces que estan muy lejos
        for (int j = 0; j < othersList.Count; j++)
        {
            if (othersList[j] == null || (othersList[j].transform.position - centroid).magnitude > othersRelevantDistance)
            {
                othersList.RemoveAt(j);
                j--;
            }
        }

        //These things are used to gather information for testing
        Dictionary<string, Dictionary<string, int>> countSets = new Dictionary<string, Dictionary<string, int>>();
        
        //Keeps count the amount of fish that are of a specific age
        countSets.Add("age", new Dictionary<string,int>(){
                                                        {"Fry",0},
                                                        {"Young",0},
                                                        {"Adult",0},
                                                        {"Old",0}
                                                        });
        
        //Keeps count of the amount of fish that fit into the different categories of energy
        countSets.Add("energy", new Dictionary<string,int>(){
                                                        {"Low",0},
                                                        {"Medium",0},
                                                        {"High",0}
                                                            });

        //Keeps count of the amount of fish that fit into the different categories of swimming Depth
        countSets.Add("swimmingDepth", new Dictionary<string,int>(){
                                                        {"Low",0},
                                                        {"Medium",0},
                                                        {"High",0},
                                                        {"VeryHigh",0}
                                                            });
        
        //Keeps count of the amount of fish that fit into the different categories of weight that tells them to stick close to the school or not
        countSets.Add("centroidFollowingDirectionWeight", new Dictionary<string,int>(){
                                                        {"Low",0},
                                                        {"Medium",0},
                                                        {"High",0}
                                                            });

        //Keeps count of the amount of fish that fit into the different categories of weight that tells them to follow the group or not
        countSets.Add("couzinDirectionWeight", new Dictionary<string,int>(){
                                                        {"Low",0},
                                                        {"Medium",0},
                                                        {"High",0}
                                                            });
        float totalCentroidDistance = 0;
        float totalSpeed = 0;

        for (int i = 0; i < fishList.Count; i++)
        {
            // Para cada pez, se asigna la nueva direccion preferida
            FishInformation info = GetFishInformation(i);
            fishList[i].SetInformation(info);

            if (isTesting){
                //Save information for testing
                totalSpeed = totalSpeed + info.Speed;
                totalCentroidDistance = totalCentroidDistance + Vector3.Distance(centroid, fishList[i].transform.position);
                
                countSets["swimmingDepth"][fuzzySystem.GetDependientFuzzyVariable("SwimmingDepth").FuzzificationClosestSet(output_variables["SwimmingDepth"])] += 1;
                countSets["couzinDirectionWeight"][fuzzySystem.GetDependientFuzzyVariable("CouzinDirectionWeight").FuzzificationClosestSet(output_variables["CouzinDirectionWeight"])] += 1;
                countSets["centroidFollowingDirectionWeight"][fuzzySystem.GetDependientFuzzyVariable("CentroidFollowingDirectionWeight").FuzzificationClosestSet(output_variables["CentroidFollowingDirectionWeight"])] += 1;
                countSets["age"][fuzzySystem.GetIndependientFuzzyVariable("Age").FuzzificationClosestSet(variables["Age"])] += 1;
                countSets["energy"][fuzzySystem.GetIndependientFuzzyVariable("Energy").FuzzificationClosestSet(variables["Energy"])] += 1;
            }
        }

        if (isTesting){
            float averageSpeed = totalSpeed/fishList.Count;
            float averageCentroidDistance = totalCentroidDistance/fishList.Count;
            //m_textMeshPro.rectTransform.anchoredPosition3D = centroid;
            //Debug.Log(centroid);
            //Debug.Log(averageSpeed);
            //Debug.Log(gameObject.transform.position);
            infoTool.addAvgSpeed(averageSpeed);
            infoTool.addAvgCentroidDistance(averageCentroidDistance);
            infoTool.addSwimmingDepthTypeCount(countSets["swimmingDepth"]);
            infoTool.addCentroidFollowingDirectionWeightTypeCount(countSets["centroidFollowingDirectionWeight"]);
            infoTool.addCouzinDirectionWeightTypeCount(countSets["couzinDirectionWeight"]);
            infoTool.addAgeTypeCount(countSets["age"]);
            infoTool.addEnergyTypeCount(countSets["energy"]);
            m_textMeshPro.transform.position = centroid;
            m_textMeshPro.text = string.Format("{0:N2}", averageSpeed);
        }
        // Vuelve actualizar la direccion dentro de delta_t segundos
        //Esta funcion se llama otra vez en delta_t segundos
        Invoke("UpdateFishInformation", delta_t);
    }
}

