using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FuzzyLogic;

public class LionFishSchoolController : FuzzySchoolController
{
    protected override void SetVariables()
    {
        base.SetVariables();
    }

    protected override void SetRules()
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

        antecedents = new Dictionary<string, string>();
        antecedents.Add("PreyDistance", "Close");
        consequent = new KeyValuePair<string, string>("Speed", "Fast");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));


        /* SwimmingDepth */
        antecedents = new Dictionary<string, string>();
        antecedents.Add("Hunger", "Low");
        consequent = new KeyValuePair<string, string>("SwimmingDepth", "Medium");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Hunger", "Medium");
        consequent = new KeyValuePair<string, string>("SwimmingDepth", "High");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Hunger", "High");
        consequent = new KeyValuePair<string, string>("SwimmingDepth", "VeryHigh");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));


        /* PreySearchingRange */
        antecedents = new Dictionary<string, string>();
        antecedents.Add("Hunger", "Low");
        consequent = new KeyValuePair<string, string>("PreySearchingRange", "Small");
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
        consequent = new KeyValuePair<string, string>("CouzinDirectionWeight", "Low");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("Age", "Old");
        consequent = new KeyValuePair<string, string>("CouzinDirectionWeight", "Low");
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
        consequent = new KeyValuePair<string, string>("PreyFollowingDirectionWeight", "Medium");
        fuzzySystem.AddRule(new Rule(antecedents, consequent));

        antecedents = new Dictionary<string, string>();
        antecedents.Add("PreyDistance", "Far");
        consequent = new KeyValuePair<string, string>("PreyFollowingDirectionWeight", "Low");
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

    }
}
