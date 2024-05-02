using System.Collections;
using System.Collections.Generic;
using System;
using FuzzyLogic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

/*
    Autor: Humberto De Jesus Jimenez Gutierrez
    Fecha de creacion: Marzo 2, 2023
    Ultima moficacion: Marzo 15, 2023
*/

namespace FuzzyLogic
{
    public class FuzzySystem
    {
        string name;
        Dictionary<string, FuzzyVariable> independientVariables;
        Dictionary<string, FuzzyVariable> dependientVariables;
        Dictionary<string, List<Rule>> rules;

        /*
            FuzzySystem :: This is the constructor for the Fuzzy system class, the fuzzy system is a high level class used to make all fuzzy logic decisions.
                We use this to declare our independent, dependent variables. We also use those variables to declare rules. We also fuzzify, inference and defuzzify in this class.
            Parameters :: 
                name :: Name of this fuzzy set
            Other ::
                independientVariables :: The independent variable is the variable thats going to change or determine the outcome of the dependent variables
                dependientVariables :: The dependent variable is the one that's going to be affected by the values of the independent variables
                rules :: These are the rules that it will use to make desicions. 
                    The key of the dictionary is the name of a dependent variable
                    The value of the dictionary is the list of rules that will affect that dependent variable.
            Return :: 
                None
        */
        public FuzzySystem(string name)
        {
            this.name = name;
            independientVariables = new Dictionary<string, FuzzyVariable>();
            dependientVariables = new Dictionary<string, FuzzyVariable>();
            rules = new Dictionary<string, List<Rule>>();
        }

        /*
        AddIndependentVariable :: This fucntion adds a variable into our independentVariables dictionary, unless its already there
            The independent variable is the variable thats going to change or determine the outcome of the dependent variables
            We declare how they're going to change the dependant vairables using fuzzy rules
        Parameters :: 
            variable :: This is the variable we'll try to add to our IndependentVariable dictionary
        Return ::
            None
        */
        public void AddIndependientVariable(FuzzyVariable variable)
        {
            if (independientVariables.ContainsKey(variable.Name()))
            {
                throw new ArgumentException("Independient Fuzzy Set '" + variable.Name() + "' already added");
            }
            independientVariables.Add(variable.Name(), variable);
        }

        /*For when you want to check if you can replace an independentVariable with another one, this checks if it has the same variable name and Fuzzy set names
            ??How do I replace?
        */
        public void ReplaceIndependientVariable(FuzzyVariable variable)
        {
            if (independientVariables.ContainsKey(variable.Name()))
            {
                List<string> sets = variable.GetFuzzySetNames();
                foreach (string set in sets)
                {
                    if (!independientVariables[variable.Name()].FuzzySetExists(set))
                    {
                        throw new ArgumentException("Independient Fuzzy Variable '" + variable.Name() + "' does not contain fuzzySet with name '" + set + "', but you are trying to replace it with a new variable that contains the strange fuzzySet");
                    }
                }
                sets = independientVariables[variable.Name()].GetFuzzySetNames();
                foreach (string set in sets)
                {
                    if (!variable.FuzzySetExists(set))
                    {
                        throw new ArgumentException("Independient Fuzzy Variable '" + variable.Name() + "' contains fuzzySet with name '" + set + "', but you are trying to replace it with a new variable that does not contain the named fuzzySet");
                    }
                }
            }
            throw new ArgumentException("Independient Fuzzy Variable '" + variable.Name() + "' does not exist, but you are trying to replace it");
        }

        /*
        ?? Dependent is written wrong here?
        AddDependientVariable :: This function adds a variable into the DependientVariable dictionary 
            The dependent variable is the one that's going to be affected by the values of the independent variables
            By who or by how much they're going to be changed depends on the Fuzzy rules declared in the fuzzy system
        Parameters ::
            variable :: This is the variable that we want to add to our DependientVariable dictionary
        Return ::
            None
        */
        public void AddDependientVariable(FuzzyVariable variable)
        {
            if (dependientVariables.ContainsKey(variable.Name()))
            {
                throw new ArgumentException("Dependient Fuzzy Variable '" + variable.Name() + "' already added");
            }
            dependientVariables.Add(variable.Name(), variable);
            rules.Add(variable.Name(), new List<Rule>());
        }

        /* For when you want to check if you can replace an dependientVariable with another one, this checks if it has the same variable name and Fuzzy set names
            ??How do I replace?
        */
        public void ReplaceDependientVariable(FuzzyVariable variable)
        {
            if (dependientVariables.ContainsKey(variable.Name()))
            {
                List<string> sets = variable.GetFuzzySetNames();
                foreach (string set in sets)
                {
                    if (!dependientVariables[variable.Name()].FuzzySetExists(set))
                    {
                        throw new ArgumentException("Dependient Fuzzy Variable '" + variable.Name() + "' does not contain fuzzySet with name '" + set + "', but you are trying to replace it with a new variable that contains the strange fuzzySet");
                    }
                }
                sets = dependientVariables[variable.Name()].GetFuzzySetNames();
                foreach (string set in sets)
                {
                    if (!variable.FuzzySetExists(set))
                    {
                        throw new ArgumentException("Dependient Fuzzy Variable '" + variable.Name() + "' contains fuzzySet with name '" + set + "', but you are trying to replace it with a new variable that does not contain the named fuzzySet");
                    }
                }
            }
            throw new ArgumentException("Dpendient Fuzzy Variable '" + variable.Name() + "' does not exist, but you are trying to replace it");
        }

        /*
        AddRule :: This function verifies if the consequent and antecedent(s) of a rule have valid Fuzzy Variables and if they contain valid Fuzzy sets for those variables.
            The rule is added on a dictionary where the kay to look it up is the name of the fuzzy variable that the consecuent is related to.
        Parameters ::
            rule (Rule) :: Contains the rule (IF THEN...) that you want to add
        Return ::
            None
        */
        public void AddRule(Rule rule)
        {
            if (dependientVariables.ContainsKey(rule.Consequent().Key))
            {
                if (!dependientVariables[rule.Consequent().Key].FuzzySetExists(rule.Consequent().Value))
                {
                    throw new ArgumentException("Dependient Fuzzy Variable '" + rule.Consequent().Key + "' does not contain a fuzzySet with name'" + rule.Consequent().Value + "'");
                }
            }
            else
            {
                throw new ArgumentException("Fuzzy System '" + name + "' does not contain a Dependient FuzzyVariable with name '" + rule.Consequent().Key + "'");
            }
            foreach (KeyValuePair<string, string> condition in rule.Antecedents())
            {
                if (independientVariables.ContainsKey(condition.Key))
                {
                    if (!independientVariables[condition.Key].FuzzySetExists(condition.Value))
                    {
                        throw new ArgumentException("Independient Fuzzy Set '" + condition.Key + "' does not contain rule '" + condition.Value + "'");
                    }
                }
                else
                {
                    throw new ArgumentException("Fuzzy System '" + name + "' does not contain Independient Fuzzy Variable '" + condition.Key + "'");
                }
            }
            rules[rule.Consequent().Key].Add(rule);
        }

        /*
        Fuzzify :: This function recieves the name of a variable and the current value of it. Afterward, it looks up the 
            fuzzy variable model in the the independent variables dictionary. Once it finds that variable it calculates the degree of membership 
            in which the variable's value is aligned with for each fuzzy set of that fuzzy variable model.
        Parameters ::
            inputs (dictionary<string,float>):: This dictionary contains a variable and the current numeric value of it
                (Ex. "Hunger", 1) 
        Return ::
            fuzzyInputs (Dictionary<string, Dictionary<string,float>>) :: This variable contains a dictionary inside another dictionary, this is composed of the outside key and an inside key and value pair
                1. Name of the fuzzy variable (string)
                    1a. Name of the Fuzzy set (string)
                    1b. Degree of membership in that fuzzy set 
        */
        public Dictionary<string, Dictionary<string, float>> Fuzzify(Dictionary<string, float> inputs)
        {
            Dictionary<string, Dictionary<string, float>> fuzzyInputs = new Dictionary<string, Dictionary<string, float>>();

            foreach (KeyValuePair<string, FuzzyVariable> fuzzyVariable in independientVariables)
            {
                if (inputs.ContainsKey(fuzzyVariable.Key))
                {
                    fuzzyInputs.Add(fuzzyVariable.Key, fuzzyVariable.Value.Fuzzification(inputs[fuzzyVariable.Key]));
                }
                else
                {
                    throw new ArgumentException("Inputs does not contain value for FuzzyVariable with name '" + fuzzyVariable.Key + "'");
                }
            }

            return fuzzyInputs;
        }

        /*
        FuzzyOperator :: This method runs an implication method on each rule, where many antecedents affect with a consecuent.
            The implication method changes our resulting consequent fuzzy sets, it limits or changes it depending on the degree of membership of the antecedents. 
        Parameters :: 
            fuzzyInputs (Dictionary<string, Dictionary<string,float>>) :: This variable contains a dictionary inside another dictionary that contains the degree of membership of each fuzzy set in each variable, this is composed of the outside key and an inside key and value pair
                1. Name of the fuzzy variable (string)
                    1a. Name of the Fuzzy set (string), for all fuzzy sets in that variable
                    1b. Degree of membership in each fuzzy set (float)
            andOperatorMethod (string) ("min") :: Declare the type of and operator you want to use, there's only two types ("min", "prod")
        Results ::
            operatorResults (Dictionary<string, List<KeyValuePair<string,float>>>) :: This variable contains the result of implication function.
                Implication will limit the consecuent functions (the ones used for the results)
                This consists of a dictionary that contains lists of key value pairs that contains the implication values for each consequent fuzzy sets declared in the rules for all unique consequent. 
                    1. Consequent variable name (string)
                        1a. Consequent Fuzzy set name (string)
                        1b. Implication values (float)
        */
        public Dictionary<string, List<KeyValuePair<string, float>>> FuzzyOperator(Dictionary<string, Dictionary<string, float>> fuzzyInputs, string andOperatorMethod = "min")
        {
            Dictionary<string, List<KeyValuePair<string, float>>> operator_results = new Dictionary<string, List<KeyValuePair<string, float>>>();
            List<KeyValuePair<string, float>> operator_result;
            KeyValuePair<string, float> result;
            List<float> mu_s;
            foreach (KeyValuePair<string, FuzzyVariable> set in dependientVariables)
            {
                operator_result = new List<KeyValuePair<string, float>>();
                foreach (Rule rule in rules[set.Key])                               //Rules that affect that dependentVariable
                {
                    mu_s = new List<float>();
                    foreach (KeyValuePair<string, string> antecedent in rule.Antecedents())
                    {
                        mu_s.Add(fuzzyInputs[antecedent.Key][antecedent.Value]);                    //Add the degree of membership of each antecedent fuzzy set
                    }
                    result = new KeyValuePair<string, float>(rule.Consequent().Value, AndOperator(mu_s, andOperatorMethod) * rule.Weight()); // Multiply by weight as the process of implication method
                    operator_result.Add(result);
                }
                operator_results.Add(set.Key, operator_result);     //Do AndOperator for each rule and store them here
            }
            return operator_results;
        }

        /*
        Inference :: Inference is the last step of Defuzzification, of converting a fuzzy value into a crisp value (number) to return back to the program
            For each variable, we have multiple fuzzy sets of possible values, which was changed with the implication value. In this function we'll try to pick a singluar crisp value based on these fuzzy sets.
        Parameters ::
            outputs (Dictionary<string, List<KeyValuePair<string, float>>>) :: This comes from the fuzzyOperator function
                This variable contains the result of implication function.
                This consists of a dictionary that contains lists of key value pairs that contains the implication values for each consequent fuzzy sets declared in the rules for all unique consequent. 
                    1. Consequent variable name (string)
                        1a. Consequent Fuzzy set name (string)
                        1b. Implication values (float)
            Inference method (string) :: This is the method used to select a crisp value based on the fuzzy sets given
                The methods implemented here are:
                "last_of_maxima" :: Look at the highest degrees in the fuzzy distributions in the fuzzy consequent space and return the numeric value at those degree
                    if there are two or more instances of that degree then pick the last one
                "first of maxima" :: Look at the highest degrees in the fuzzy distributions in the fuzzy consequent space and return the numeric value at those degree
                    if there are two or more instances of that degree then pick the first one
        Return ::
            defuzzifiedOuputs (Dictionary<string, float>) :: This is a dictionary that has:
                1. (key) Name of each dependent variable
                2. (value) Resulting output of that dependent variable, this is the number we'll give at the end of the Fuzzy logic System
        */
        public Dictionary<string, float> Inference(Dictionary<string, List<KeyValuePair<string, float>>> outputs, string inferenceMethod = "last_of_maxima")
        {
            Dictionary<string, float> defuzziedOutputs = new Dictionary<string, float>();
            if (inferenceMethod == "last_of_maxima")
            {
                string maxima_fuzzy_set;
                float maxima_mu;
                float maxima_intersection;
                foreach (KeyValuePair<string, FuzzyVariable> set in dependientVariables)
                {
                    maxima_fuzzy_set = "";
                    maxima_mu = 0;
                    maxima_intersection = 0;
                    foreach (KeyValuePair<string, float> output in outputs[set.Key])            //Check all fuzzy sets and implication values of that Variable
                    {
                        if (maxima_mu < output.Value)               //if that Fuzzy set has the biggest degree
                        {
                            maxima_fuzzy_set = output.Key;
                            maxima_mu = output.Value;
                            maxima_intersection = dependientVariables[set.Key].LastIntersection(maxima_fuzzy_set, maxima_mu);               //Crisp numeric value that you get at that implication value of the set
                        }
                        else if (maxima_mu == output.Value && output.Value > 0)                 //If this set has the same degree somewhere else (assuming its not 0)
                        {
                            if (maxima_intersection < dependientVariables[set.Key].LastIntersection(output.Key, output.Value))              //
                            {
                                maxima_fuzzy_set = output.Key;
                                maxima_mu = output.Value;
                                maxima_intersection = dependientVariables[set.Key].LastIntersection(maxima_fuzzy_set, maxima_mu);
                            }
                        }
                    }
                    defuzziedOutputs.Add(set.Key, maxima_intersection);
                }
            }
            else if (inferenceMethod == "first_of_maxima") //?? This inference method has the same code as the previous one.
            {
                string maxima_fuzzy_set;
                float maxima_mu;
                float maxima_intersection;
                foreach (KeyValuePair<string, FuzzyVariable> set in dependientVariables)
                {
                    maxima_fuzzy_set = "";
                    maxima_mu = 0;
                    maxima_intersection = 0;
                    foreach (KeyValuePair<string, float> output in outputs[set.Key])
                    {
                        if (maxima_mu < output.Value)
                        {
                            maxima_fuzzy_set = output.Key;
                            maxima_mu = output.Value;
                            maxima_intersection = dependientVariables[set.Key].LastIntersection(maxima_fuzzy_set, maxima_mu);
                        }
                        else if (maxima_mu == output.Value && output.Value > 0)
                        {
                            if (maxima_intersection < dependientVariables[set.Key].LastIntersection(output.Key, output.Value))
                            {
                                maxima_fuzzy_set = output.Key;
                                maxima_mu = output.Value;
                                maxima_intersection = dependientVariables[set.Key].LastIntersection(maxima_fuzzy_set, maxima_mu);
                            }
                        }
                    }
                    defuzziedOutputs.Add(set.Key, maxima_intersection);
                }
            }
            return defuzziedOutputs;
        }

        /*
        FuzzyInferenceSystem :: This is the function that is used to run the fuzzy system. It fuzzifies the input values, 
            it runs an implication method and it runs the inference method to then output the dependent variable and their values calculated form this process.
        Parameters :: 
            crispInputs (Dictionary<string,float>) :: This has a dictionary that contains ::
                inputVariableName (string) :: This is the name of the independent variables (The ones that we'll use for the output) 
                    (ex. "Hunger", "Energy", "Age")
                value (float) :: This is the current crisp numeric value of this variable.
        Return ::
            crispOutputs (Dictionary<string, float>) :: This is a dictionary that contains ::
                dependentVariableName (string) :: This is the name of the output variable
                value (float) :: Value of that dependent vairable
        */
        public Dictionary<string, float> FuzzyInferenceSystem(Dictionary<string, float> crispInputs, string andOperatorMethod = "min", string inferenceMethod = "last_of_maxima")
        {
            Dictionary<string, Dictionary<string, float>> fuzzyInputs = Fuzzify(crispInputs);

            Dictionary<string, List<KeyValuePair<string, float>>> fuzzyOutputs = FuzzyOperator(fuzzyInputs, andOperatorMethod);

            Dictionary<string, float> crispOutputs = Inference(fuzzyOutputs, inferenceMethod);

            return crispOutputs;
        }

        /*
        AndOperator :: This runs the implication function on a list of degrees of memberships
            The implication function is used to limit the consecuent function
            "min" :: Also konwn as Mamdani's implication, it clips the consecuent function up to the minimum value
            "prod" :: Also known as Larsen's implicatoin, it scales down the consecuent function up to the minimum value
        Parameters :: 
            values (List<float>) :: List of degrees of memberships (0,1)
            method (string) ("min") :: This is the method that the user wants to use to calculate the AND operation
        Return ::
            result (float) :: Returns the value calculated by this operation. 
        */
        public float AndOperator(List<float> values, string method = "min")
        {
            if (values.Count > 0)
            {
                if (method == "min")
                {
                    float min = 1;
                    foreach (float value in values)
                    {
                        if (min > value)
                            min = value;
                    }
                    return min;
                }
                else if (method == "prod")
                {
                    float result = 1;
                    foreach (float value in values)
                    {
                        result *= value;
                    }
                    return result;
                }
                else
                {
                    throw new ArgumentException("Function '" + method + "' does not exist as an And Operator");
                }
            }
            return 0;
        }

        /*
            GetIndependientFuzzyVariable:: Function that gets the name of a variable and returns that variable
        */
        public FuzzyVariable GetIndependientFuzzyVariable(string variableName){
            if (independientVariables.ContainsKey(variableName))
            {
                return independientVariables[variableName];
            }
            else
            {
                return null;
            }
        }

        /*
            GetDependientFuzzyVariable:: Fucntion that gets the name of the variable and returns that variable
        */
        public FuzzyVariable GetDependientFuzzyVariable(string variableName){
            if(dependientVariables.ContainsKey(variableName)){
                return dependientVariables[variableName];
            }
            else{
                return null;
            }
        }

        public string Name()
        {
            return name;
        }
    }

    /*
    Rules are used to model the relationships between inputs and outputs
    This is usually composed of a number of fuzzy implications (also known as IF THEN rules), which is called the knowledge base
    (Ex. If hunger is high then look for prey should then be high) (I made the example up...)
    This class is used to store the necesary information and return it
    */
    public class Rule
    {
        Dictionary<string, string> antecedents;
        KeyValuePair<string, string> consequent;
        float weight;

        /*
        Rule :: This is a constructor for a rule, it stores the necesary information that it needs
        Parameters :: 
            antecedents :: Acting as the IF part of the rule, it states the condition at which a variable needs to be in order for the consequent to happen
                This contains a dictionary that contains the (variable name: fuzzy set name) of the antecedent
                (Ex. If Hunger is in the "High" hunger fuzzy set)
            consequent :: Acting as the ELSE part of the rule, it states whats going to happen
                This contains a keyValuePair that contians the (variable name: fuzzy set name) of the consequent
                (Ex. Try to activate the "High" look for prey fuzzy set)
                Different rule antecedents can affect one consequent variable, in the Defuzzification step we combine the multiple consequences that affect one variable and join them into one function.
            ??weight ::
        Return ::
            None 
        */
        public Rule(Dictionary<string, string> antecedents, KeyValuePair<string, string> consequent, float weight = 1f)
        {
            this.antecedents = antecedents;
            this.consequent = consequent;
            this.weight = weight; //??What does weight do?
        }

        /*
        Consequent :: This function returns the consequent composed of the:
            1. variable name (string)
            2. fuzzy set name (string)
        */
        public KeyValuePair<string, string> Consequent()
        {
            return consequent;
        }

        /*
        Antecedent :: This function returns the antecedent composed of the:
            1. variable name (string)
            2. fuzzy set name (string)
        */
        public Dictionary<string, string> Antecedents()
        {
            return antecedents;
        }

        //?? This is how much that rule is going to be followed?
        public float Weight()
        {
            return weight;
        }
    }

    /*
    Fuzzy Variable :: This is the variable we're going to model, each variable contains a name and Fuzzy sets in certain ranges
    (ex. Hunger variable, with the fuzzy sets of "Low", "Medium", "High")
    */
    public class FuzzyVariable
    {
        protected Dictionary<string, FuzzySet> fuzzySets;
        string name;

        /*
        FuzzyVariable :: Constructor for the fuzzy variable, stores the name of the Variable and creates a dictionrary to store the Fuzzy Sets
        Parameters :: 
            name :: Name of this variable
        Variable ::
            fuzzySets :: Dictionary where the fuzzy sets are going to be stored
        Return ::
            None
        */
        public FuzzyVariable(string name)
        {
            fuzzySets = new Dictionary<string, FuzzySet>();
            this.name = name;
        }

        /*
        Name :: Function that returns the name of this variable
        Return :: 
            name (string) :: name of this variable
        */
        public string Name()
        {
            return name;
        }

        /*
        GetFuzzySetNames :: Get the names of the Fuzzy sets in this variable
        Parameters :: 
            None
        Variables ::
            names (List of strings) :: Used to store the names of the Fuzzy Sets
        Return ::
            names (List of Strings) :: return the names of the Fuzzy Sets 
        */
        public List<string> GetFuzzySetNames()
        {
            List<string> names = new List<string>();
            foreach (KeyValuePair<string, FuzzySet> set in fuzzySets)
            {
                names.Add(set.Key);
            }
            return names;
        }

        /*
        FuzzySetExists :: Given the name of a fuzzy set, tells you if that fuzzy set exists in this variable
        Parameters ::
            rule (string) :: This is the name of the fuzzy set
        Return ::
            containsFuzzySet (bool) :: Says if the fuzzy set is in this variable or not
        */
        public bool FuzzySetExists(string rule)
        {
            return fuzzySets.ContainsKey(rule);
        }

        /*
        AddFuzzySet :: Given a fuzzy set, adds that fuzzy set into this variable
        Parameter ::
            rule :: This is the fuzzySet that you want to add
        */
        public void AddFuzzySet(FuzzySet rule)
        {
            if (fuzzySets.ContainsKey(rule.Name()))
            {
                throw new ArgumentException("FuzzySet '" + rule.Name() + "' already added");
            }
            fuzzySets.Add(rule.Name(), rule);
        }

        /*
        Fuzzification :: This is the process in which we turn numeric crisp values into a degree of membership in a fuzzy subset
        Parameters :: 
            value :: This is the current numeric value of this variable
        Return::
            fuzziedSets (Dictionary<string, float>) :: For each Fuzzy set in the Fuzzy variable get a dictionary entry 
                containing the name of that set (string) and the degree of membership of that set (float)
        */
        public Dictionary<string, float> Fuzzification(float value)
        {
            Dictionary<string, float> fuzziedSets = new Dictionary<string, float>();

            foreach (KeyValuePair<string, FuzzySet> fuzzySet in fuzzySets)
            {
                fuzziedSets.Add(fuzzySet.Key, fuzzySet.Value.MembershipFunction(value));
            }

            return fuzziedSets;
        }

        /*
        FuzzificationClosest: This function returns the fuzzySet with the biggest degree of membership (The category that applies the most)
            This is used for the tests to have an idea of the current individuals that apply to one category the most (ex. Checking the amount of Young, Adult and Old fish at a certain time)
        */
        public string FuzzificationClosestSet(float value)
        {
            float maxMembership = -1;
            float curMembership;
            string maxMembershipName = "";

            foreach (KeyValuePair<string, FuzzySet> fuzzySet in fuzzySets)
            {
                curMembership = fuzzySet.Value.MembershipFunction(value);
                if (curMembership > maxMembership)
                {
                    maxMembership = curMembership;
                    maxMembershipName = fuzzySet.Key;
                }
            }
            return maxMembershipName;
        }

        /*
        FirstIntersection :: This is the numeric value of this variable at that degree of membership
        Parameters ::
            set (string) :: This is the name of the fuzzy set in that variable
            membership (float) :: This is the degree of membership one wants to look up in that set
        Result :: 
            valueAtIntersection :: This is the numeric value of this variable at that degree of membership
        */
        public float FirstIntersection(string set, float membership)
        {
            return fuzzySets[set].FirstIntersection(membership);
        }

        /*
        FirstIntersection :: This is the numeric value of this variable at that degree of membership
        Parameters ::
            set (string) :: This is the name of the fuzzy set in that variable
            membership (float) :: This is the degree of membership one wants to look up in that set
        Result :: 
            valueAtIntersection :: This is the numeric value of this variable at that degree of membership
        */
        public float LastIntersection(string set, float membership)
        {
            return fuzzySets[set].LastIntersection(membership);
        }
    }

    /*
    This is going to be the template for defining the fuzzy sets of our fuzzy variables?
    Working kind of like an interface, it defines the functions that we will use for each new Fuzzy set
    The only actual thing this base class will do is save the name of the Fuzzy set declared with it.

    The fuzzy sets declared in here are::
        1. Diagonal Line Fuzzy Set
        2. Triangular Fuzzy Set
        3. Trapezoid Fuzzy Set 
    */
    public class FuzzySet
    {
        string name;
        protected FuzzySet(string name)
        {
            this.name = name;
        }
        public virtual bool IndicatorFunction(float value)
        {
            return false;
        }

        public virtual float MembershipFunction(float value)
        {
            return 0;
        }

        public virtual float FirstIntersection(float value)
        {
            return 0;
        }

        public virtual float LastIntersection(float value)
        {
            return 0;
        }

        public string Name()
        {
            return name;
        }
    }

    /*
    Fuzzy sets made in a diagonal line
    */
    public class DiagonalLineFuzzySet : FuzzySet
    {

        float zero;
        float one;
        float m;

        /*
        DiagonalLineFuzzySet :: This function defines the variables that we need in order to define this Fuzzy set
        Parameters:
            name :: This is going to be the name or category of the fuzzy set (ex. "Low", "Average", "Tall", "Fast")
            zero and one :: These will be the ranges at which the line will be defined in, where it will get a membership of 0 and where it will get a membership of 1
            zero :: This is the value at which the variable has a degree of membership of 0 (where its not part of the set)
            one :: This is the value at which the variable has a degree of membership of 1 (wher it is part of the set)
        Variables::
            m :: Slope of the diagonal line
        Return::
            none
        */

        //Recommendation :: It seems like the diagonal line fuzzy sets are only limited to starting and ending on one to zero, 
        //theres no option to make diagonal line functions that start and end in membership values between 0 and 1 
        //(ex. diagonal line starting from )
        public DiagonalLineFuzzySet(string name, float zero, float one) : base(name)
        {
            if (zero != one)
            {
                this.zero = zero;
                this.one = one;
                m = 1 / (one - zero); 
            }
            else
            {
                throw new ArgumentException("Parameter 'zero' must be diferent than parameter 'one'");
            }
        }

        /*
        IndicatorFunction :: This function lets us know if a value is a member (is part) of this Fuzzy Set or not.
        // (ex. If a fish is on a swimming depth of 0.1 then it will return True on the "High" swimming depth Fuzzy set and false on the "Low" swimming depth fuzzy set )
        Parameters :: 
            value :: This is a float value of the variable you're checking (ex. hunger, age, energy)
        Return ::
            is_member (bool) :: Binary value that tells you if that value is a member of this fuzzy set or not.
        */
        public override bool IndicatorFunction(float value)
        {
            if (zero < one)
            {
                if (value <= zero)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (value >= zero)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /*
        Membership function :: Based on the value from a variable, check the grade (also konwn as extent or degree) of membership of that value on this Fuzzy Set
        (ex. If energy is 1.0 (max) then it will have a grade of membership of 1.0 for the "High" energy Fuzzy set)
        Parameter ::
            value:: This is a float value of the variable you're checking (ex. hunger, age, energy)
        Return :: 
            degree_membership (float) :: A value in the range of [0,1] that tells us the degree of membership of a value in this Fuzzy Set
                0 means that this value is not part of this set
                0.5 means that this value is kind of part of this set
                1 means that this value is part of this set
        */ 
        public override float MembershipFunction(float value)
        {
            if (zero < one)
            {
                if (value <= zero)
                {
                    return 0;
                }
                else if (value >= one)
                {
                    return 1;
                }
                else
                {
                    return m * (value - zero);
                }
            }
            else
            {
                if (value >= zero)
                {
                    return 0;
                }
                else if (value <= one)
                {
                    return 1;
                }
                else
                {
                    return 1 + m * (value - one);
                }
            }
        }
        
        /*
        FirstIntersection :: This function recieves a degree of membership, returns the value at which this membership is seen in this fuzzy set (first intersection is from left to right, last intersection is from right to left)
        Parameter :: 
            value :: This is the degree of membership that this function wants to find the value of
        Return ::
            value (float) :: This is the value that has that degree of membership in this Fuzzy set. 
        */
        public override float FirstIntersection(float value)
        {
            if (value < 0)
            {
                value = 0;
            }
            else if (value > 1)
            {
                value = 1;
            }
            float intersection;
            if (zero < one)
            {
                intersection = value / m + zero; 
            }
            else
            {
                intersection = (value - 1) / m + one;
            }
            return intersection;
        }

        public override float LastIntersection(float value)
        {
            return FirstIntersection(value);
        }
    }

    public class TriangularFuzzySet : FuzzySet
    {
        float min_zero;
        float min_m;
        float one;
        float max_zero;
        float max_m;
        public TriangularFuzzySet(string name, float min_zero, float one, float max_zero) : base(name)
        {
            if (min_zero < one && one < max_zero)
            {
                this.min_zero = min_zero;
                if (min_zero != one)
                    min_m = 1 / (one - min_zero); // funcion de la pendiente, 1 porque 1-0 = 1 
                this.one = one;
                this.max_zero = max_zero;
                if (max_zero != one)
                    max_m = 1 / (one - max_zero); // funcion de la pendiente, 1 porque 1-0 = 1 
            }
            else
            {
                throw new ArgumentException("Parameter 'min_zero' must be less than parameter 'one', and parameter 'one' must be less than parameter 'max_zero'");
            }
        }

        public override bool IndicatorFunction(float value)
        {
            if (value <= min_zero || value >= max_zero)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override float MembershipFunction(float value)
        {
            if (value <= min_zero || value >= max_zero)
            {
                return 0;
            }
            if (value < one)
            {
                return min_m * (value - min_zero);
            }
            else
            {
                return 1 + max_m * (value - one);
            }
        }

        public override float FirstIntersection(float value)
        {
            if (value < 0)
            {
                value = 0;
            }
            else if (value > 1)
            {
                value = 1;
            }
            float intersection = value / min_m + min_zero;
            return intersection;
        }

        public override float LastIntersection(float value)
        {
            if (value < 0)
            {
                value = 0;
            }
            else if (value > 1)
            {
                value = 1;
            }
            float intersection = (value - 1) / max_m + one;
            return intersection;
        }
    }

    public class TrapezoidFuzzySet : FuzzySet
    {
        float min_zero;
        float min_m;
        float min_one;
        float max_one;
        float max_zero;
        float max_m;
        public TrapezoidFuzzySet(string name, float min_zero, float min_one, float max_one, float max_zero) : base(name)
        {
            if (min_zero < min_one && min_one < max_one && max_one < max_zero)
            {
                this.min_zero = min_zero;
                this.min_one = min_one;
                min_m = 1 / (min_one - min_zero); // funcion de la pendiente, 1 porque 1-0 = 1
                this.max_one = max_one;
                this.max_zero = max_zero;
                max_m = -1 / (max_one - max_zero);
            }
            else
            {
                throw new ArgumentException("Parameter 'min_zero' must be less than parameter 'min_one', parameter 'min_one' must be less than parameter 'max_one' and parameter 'max_one' must be less than parameter 'max_zero'");
            }
        }

        public override bool IndicatorFunction(float value)
        {
            if (value <= min_zero || value >= max_zero)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override float MembershipFunction(float value)
        {
            if (value <= min_zero || value >= max_zero)
            {
                return 0;
            }
            if (min_one <= value && value <= max_one)
            {
                return 1;
            }
            if (value < min_one)
            {
                return min_m * (value - min_zero);
            }
            else
            {
                return 1 + max_m * (value - max_one);
            }
        }

        public override float FirstIntersection(float value)
        {
            if (value < 0)
            {
                value = 0;
            }
            else if (value > 1)
            {
                value = 1;
            }
            float intersection = value / min_m + min_zero;
            return intersection;
        }

        public override float LastIntersection(float value)
        {
            if (value < 0)
            {
                value = 0;
            }
            else if (value > 1)
            {
                value = 1;
            }
            float intersection = (value - 1) / max_m + max_one;
            return intersection;
        }
    }

    /*
    This is the class for fuzzy sets using the gaussian distribution
    */
    public class GaussianFuzzySet : FuzzySet
    {
        string name;
        float centroid;
        float stdDev;
        float indicatorMult;

        public GaussianFuzzySet(string name, float centroid, float stdDev, float indicatorMult = 3) : base(name)
        {
            this.name = name;
            this.centroid = centroid;
            this.stdDev = stdDev;
            this.indicatorMult = indicatorMult; //This is the max amount of standard deviations from the center that it takes for a value to be declared to be in this fuzzy set or not
        }
        public override bool IndicatorFunction(float value)
        {
            if (value < indicatorMult * stdDev - centroid || indicatorMult * stdDev + centroid < value) {
                return false;
            }
            else{
                return true;
            }
        }

        public override float MembershipFunction(float value)
        {
            if (value < indicatorMult * stdDev - centroid || indicatorMult * stdDev + centroid < value) {
                return 0;
            }
            else{
                return MathF.Exp(-(MathF.Pow((value-centroid),2)/(2*MathF.Pow(stdDev,2))));
            }
        }

        public override float FirstIntersection(float value)
        {

            if (value <= 0){
                value = 0.1f;       //0.1 because 0 is undefined in this function so we can't define it as 0
            }
            else if (value > 1){
                value = 1;
            }

            return stdDev*MathF.Pow(2,0.5f)*MathF.Pow(-MathF.Log(value),0.5f)+centroid;             //Using the inverse function of the membership function
        }

        public override float LastIntersection(float value)
        {
            if (value <= 0){
                value = 0.1f;
            }
            else if (value > 1){
                value = 1;
            }
            return -stdDev*MathF.Pow(2,0.5f)*MathF.Pow(-MathF.Log(value),0.5f)+centroid;
        }
    }
}
