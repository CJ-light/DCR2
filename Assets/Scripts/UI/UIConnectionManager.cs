using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIConnectionManager : MonoBehaviour
{
    [Header("General - General")]
    public GameObject initialN;
    public GameObject maxN;
    public GameObject regenTime;
    public GameObject prefSwimDepth;
    [Header("General - Advanced")]
    public GameObject spawnRange;
    public GameObject regenCoef;
    public GameObject minRouteUpdTime;
    public GameObject maxRouteUpdTime;
    public GameObject minCentroidDist;
    public GameObject maxCentroidDist;
    public GameObject prefSwimDepthRange;
    public GameObject swimDepthDirectionW;
    public GameObject normalSpeed;
    [Header("Couzin - General")]
    public GameObject alpha;
    public GameObject rho;
    public GameObject p;
    [Header("Couzin - Advanced")]
    public GameObject couzinDirectionW;
    public GameObject centroidFollowDirectionW;
    public GameObject allowFeedback;
    public GameObject omegaInitial;
    public GameObject omegaMax;
    public GameObject omegaInc;
    public GameObject omegaDec;
    [Header("Hunting - General")]
    public GameObject initialHunger;
    public GameObject hungerThreshold;
    public GameObject initialEnergy;
    public GameObject energyThreshold;
    public GameObject huntingSpeed;
    [Header("Hunting - Advanced")]
    public GameObject hungerInc;
    public GameObject hungerRegenNoise;
    public GameObject energyInc;
    public GameObject energyDec;
    public GameObject prefHuntRange;
    public GameObject maxHuntRange;
    public GameObject huntSuccessRate;
    public GameObject preyFollowDirectionW;
    [Header("Predator Avoidance - General")]
    public GameObject predatorRelevantDistance;
    public GameObject newPredatorLearnRate;
    public GameObject escapeRouteNoise;
    public GameObject escapeSpeed;
    [Header("Predator Avoidance - Advanced")]
    public GameObject predatorAvoidDirectionW;
    [Header("Evade - General")]
    public GameObject othersRelevantDistance;
    public GameObject enableAvoidOthers;
    public GameObject enableAlignOthers;
    public GameObject obstacleSearchingRange;
    public GameObject obstacleRelevantDistance;
    [Header("Evade - Advanced")]
    public GameObject obstacleAvoidDirectionW;

    [Header("UI Manager")]
    //dropdown
    public Transform dropdownSpecies;
    protected int dpdIndex;
    protected List<Dropdown.OptionData> dpdOptions;
    protected string dpdValue;
    //controller
    [SerializeField] protected GameObject selectedSpecies;
    [SerializeField] protected FuzzySchoolController speciesVariablesFuzzy;
    [SerializeField] protected SchoolController speciesVariablesNonFuzzy;
    public Vector3 offset;
    public Transform parentPreys;
    public Transform parentPredators;
    public Transform parentFuzzyPredators;
    public FrameOptimationManager selectedFrameManager;

    [Header("Species Controllers")]
    public GameObject speciesReefFish8;
    public GameObject speciesReefFish4;
    public GameObject speciesTinyYellow;
    public GameObject speciesBlueTang;
    public GameObject speciesSeaHorse;
    public GameObject speciesLionFish;
    public GameObject speciesTurtle;
    public GameObject speciesStingray;
    public GameObject speciesHammerHead;

    [Header("Species Prefabs")]
    public GameObject prefabReefFish8;
    public GameObject prefabReefFish4;
    public GameObject prefabTinyYellow;
    public GameObject prefabBlueTang;
    public GameObject prefabSeaHorse;
    public GameObject prefabLionFish;
    public GameObject prefabTurtle;
    public GameObject prefabStingray;
    public GameObject prefabHammerHead;

    // Start is called before the first frame update
    void Start()
    {
        Dropdown dpd = dropdownSpecies.GetComponent<Dropdown>();
        dpd.onValueChanged.AddListener(delegate {
            DropdownValueChanged(dpd);
        });
    }

    void DropdownValueChanged(Dropdown change)
    {
        SetCurrentParametersOfSelectedSpeciesOnUI_Fuzzy();
    }

    void GetSelectedSpecies()
    {
        dpdIndex = dropdownSpecies.GetComponent<Dropdown>().value;
        dpdOptions = dropdownSpecies.GetComponent<Dropdown>().options;
        dpdValue = dpdOptions[dpdIndex].text;
        // Debug.Log(dpdValue);
    }

    void PickGameObjectOfSelectedSpecies(bool restaurar = false)
    {
        if (dpdValue == "ReefFish8")
        {
            if (restaurar)
            {
                Destroy(speciesReefFish8);
                speciesReefFish8 = Instantiate(prefabReefFish8,parentPreys);
            }
            selectedSpecies = speciesReefFish8;
        }
        else if (dpdValue == "ReefFish4")
        {
            if (restaurar)
            {
                Destroy(speciesReefFish4);
                speciesReefFish4 = Instantiate(prefabReefFish4,parentPreys);
            }
            selectedSpecies = speciesReefFish4;
        }
        else if (dpdValue == "TinyYellow")
        {
            if (restaurar)
            {
                Destroy(speciesTinyYellow);
                speciesTinyYellow = Instantiate(prefabTinyYellow,parentPreys);
            }
            selectedSpecies = speciesTinyYellow;
        }
        else if (dpdValue == "BlueTang")
        {
            if (restaurar)
            {
                Destroy(speciesBlueTang);
                speciesBlueTang = Instantiate(prefabBlueTang,parentPreys);
            }
            selectedSpecies = speciesBlueTang;
        }
        else if (dpdValue == "SeaHorse")
        {
            if (restaurar)
            {
                Destroy(speciesSeaHorse);
                speciesSeaHorse = Instantiate(prefabSeaHorse,parentPreys);
            }
            selectedSpecies = speciesSeaHorse;
        }
        else if (dpdValue == "LionFish")
        {
            if (restaurar)
            {
                Destroy(speciesLionFish);
                speciesLionFish = Instantiate(prefabLionFish,parentFuzzyPredators);
            }
            selectedSpecies = speciesLionFish;
        }
        else if (dpdValue == "Turtle")
        {
            if (restaurar)
            {
                Destroy(speciesTurtle);
                speciesTurtle = Instantiate(prefabTurtle,parentPredators);
            }
            selectedSpecies = speciesTurtle;
        }
        else if (dpdValue == "Stingray")
        {
            if (restaurar)
            {
                Destroy(speciesStingray);
                speciesStingray = Instantiate(prefabStingray,parentPredators);
            }
            selectedSpecies = speciesStingray;
        }
        else if (dpdValue == "HammerHead")
        {
            if (restaurar)
            {
                Destroy(speciesHammerHead);
                speciesHammerHead = Instantiate(prefabHammerHead,parentPredators);
            }
            selectedSpecies = speciesHammerHead;
        }

    }

    public void SetCurrentParametersOfSelectedSpeciesOnUI_Fuzzy()
    {
        float currentModifier;

        GetSelectedSpecies();

        PickGameObjectOfSelectedSpecies();
        speciesVariablesFuzzy = selectedSpecies.GetComponent<FuzzySchoolController>();
        if (speciesVariablesFuzzy == null)
        {
            SetCurrentParametersOfSelectedSpeciesOnUI_NonFuzzy();
            return;
        }

        //General - General
        currentModifier = initialN.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        initialN.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.initialPopulation / currentModifier;

        currentModifier = maxN.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        maxN.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.maxPopulation / currentModifier;

        currentModifier = regenTime.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        regenTime.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.regenerationTime / currentModifier;

        currentModifier = prefSwimDepth.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        prefSwimDepth.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.preferedSwimmingDepth / currentModifier;

        //General - Advanced
        currentModifier = spawnRange.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        spawnRange.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.spawnRange / currentModifier;

        currentModifier = regenCoef.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        regenCoef.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.regenerationCoef / currentModifier;

        currentModifier = minRouteUpdTime.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        minRouteUpdTime.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.minRouteUpdateTime / currentModifier;

        currentModifier = maxRouteUpdTime.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        maxRouteUpdTime.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.maxRouteUpdateTime / currentModifier;

        currentModifier = minCentroidDist.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        minCentroidDist.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.minCentroidDistance / currentModifier;

        currentModifier = maxCentroidDist.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        maxCentroidDist.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.maxCentroidDistance / currentModifier;

        currentModifier = prefSwimDepthRange.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        prefSwimDepthRange.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.preferedSwimmingDepthRange / currentModifier;

        currentModifier = swimDepthDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        swimDepthDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.swimmingDepthDirectionWeight / currentModifier;

        currentModifier = normalSpeed.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        normalSpeed.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.normalSpeed / currentModifier;

        //Couzin - General
        currentModifier = alpha.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        alpha.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.alpha / currentModifier;

        currentModifier = rho.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        rho.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.rho / currentModifier;

        currentModifier = p.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        p.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.p / currentModifier;

        //Couzin - Advanced
        currentModifier = couzinDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        couzinDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.couzinDirectionWeight / currentModifier;

        currentModifier = centroidFollowDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        centroidFollowDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.centroidFollowingDirectionWeight / currentModifier;

        allowFeedback.transform.Find("Toggle").gameObject.GetComponent<Toggle>().isOn = speciesVariablesFuzzy.allowOmegaFeedback;

        currentModifier = omegaInitial.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        omegaInitial.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.initialOmega / currentModifier;

        currentModifier = omegaMax.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        omegaMax.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.maxOmega / currentModifier;

        currentModifier = omegaInc.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        omegaInc.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.omegaIncrement / currentModifier;

        currentModifier = omegaDec.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        omegaDec.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.omegaDecrement / currentModifier;

        //Hunting - General
        currentModifier = initialHunger.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        initialHunger.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.initialHunger / currentModifier;

        currentModifier = hungerThreshold.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        hungerThreshold.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.hungerThreshold / currentModifier;

        currentModifier = initialEnergy.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        initialEnergy.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.initialEnergy / currentModifier;

        currentModifier = energyThreshold.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        energyThreshold.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.energyThreshold / currentModifier;

        currentModifier = huntingSpeed.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        huntingSpeed.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.huntingSpeed / currentModifier;

        //Hunting - Advanced
        currentModifier = hungerInc.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        hungerInc.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.hungerIncrement / currentModifier;

        currentModifier = hungerRegenNoise.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        hungerRegenNoise.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.hungerRecoveryNoise / currentModifier;

        currentModifier = energyInc.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        energyInc.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.energyIncrement / currentModifier;

        currentModifier = energyDec.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        energyDec.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.energyDecrement / currentModifier;

        currentModifier = prefHuntRange.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        prefHuntRange.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.preferedHuntingRange / currentModifier;

        currentModifier = maxHuntRange.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        maxHuntRange.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.maxHuntingRange / currentModifier;

        currentModifier = huntSuccessRate.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        huntSuccessRate.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.huntingSuccessRate / currentModifier;

        currentModifier = preyFollowDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        preyFollowDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.preyFollowingDirectionWeight / currentModifier;

        //PredatorAvoidance - General
        currentModifier = predatorRelevantDistance.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        predatorRelevantDistance.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.predatorRelevantDistance / currentModifier;

        currentModifier = newPredatorLearnRate.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        newPredatorLearnRate.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.newPredatorLearningRate / currentModifier;

        currentModifier = escapeRouteNoise.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        escapeRouteNoise.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.escapeRouteNoiseWeight / currentModifier;

        currentModifier = escapeSpeed.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        escapeSpeed.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.escapeSpeed / currentModifier;

        //PredatorAvoidance - Advanced
        currentModifier = predatorAvoidDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        predatorAvoidDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.predatorAvoidanceDirectionWeight / currentModifier;

        //Evade - General
        currentModifier = othersRelevantDistance.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        othersRelevantDistance.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.othersRelevantDistance / currentModifier;

        enableAvoidOthers.transform.Find("Toggle").gameObject.GetComponent<Toggle>().isOn = speciesVariablesFuzzy.enableAvoidanceWithOthers;

        enableAlignOthers.transform.Find("Toggle").gameObject.GetComponent<Toggle>().isOn = speciesVariablesFuzzy.enableAlignmentWithOthers;

        currentModifier = obstacleSearchingRange.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        obstacleSearchingRange.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.obstacleSearchingRange / currentModifier;

        currentModifier = obstacleRelevantDistance.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        obstacleRelevantDistance.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.obstacleRelevantDistance / currentModifier;

        //Evade - Advanced
        currentModifier = obstacleAvoidDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        obstacleAvoidDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesFuzzy.obstacleAvoidanceDirectionWeight / currentModifier;

    }

    void SetCurrentParametersOfSelectedSpeciesOnUI_NonFuzzy()
    {
        float currentModifier;

        speciesVariablesNonFuzzy = selectedSpecies.GetComponent<SchoolController>();

        //General - General
        currentModifier = initialN.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        initialN.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.initialPopulation / currentModifier;

        currentModifier = maxN.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        maxN.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.maxPopulation / currentModifier;

        currentModifier = regenTime.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        regenTime.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.regenerationTime / currentModifier;

        currentModifier = prefSwimDepth.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        prefSwimDepth.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.preferedSwimmingDepth / currentModifier;

        //General - Advanced
        currentModifier = spawnRange.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        spawnRange.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.spawnRange / currentModifier;

        currentModifier = regenCoef.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        regenCoef.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.regenerationCoef / currentModifier;

        currentModifier = minRouteUpdTime.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        minRouteUpdTime.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.minRouteUpdateTime / currentModifier;

        currentModifier = maxRouteUpdTime.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        maxRouteUpdTime.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.maxRouteUpdateTime / currentModifier;

        currentModifier = minCentroidDist.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        minCentroidDist.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.minCentroidDistance / currentModifier;

        currentModifier = maxCentroidDist.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        maxCentroidDist.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.maxCentroidDistance / currentModifier;

        currentModifier = prefSwimDepthRange.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        prefSwimDepthRange.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.preferedSwimmingDepthRange / currentModifier;

        currentModifier = swimDepthDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        swimDepthDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.swimmingDepthDirectionWeight / currentModifier;

        currentModifier = normalSpeed.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        normalSpeed.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.normalSpeed / currentModifier;

        //Couzin - General
        currentModifier = alpha.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        alpha.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.alpha / currentModifier;

        currentModifier = rho.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        rho.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.rho / currentModifier;

        currentModifier = p.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        p.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.p / currentModifier;

        //Couzin - Advanced
        currentModifier = couzinDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        couzinDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.couzinDirectionWeight / currentModifier;

        currentModifier = centroidFollowDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        centroidFollowDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.centroidFollowingDirectionWeight / currentModifier;

        allowFeedback.transform.Find("Toggle").gameObject.GetComponent<Toggle>().isOn = speciesVariablesNonFuzzy.allowOmegaFeedback;

        currentModifier = omegaInitial.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        omegaInitial.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.initialOmega / currentModifier;

        currentModifier = omegaMax.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        omegaMax.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.maxOmega / currentModifier;

        currentModifier = omegaInc.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        omegaInc.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.omegaIncrement / currentModifier;

        currentModifier = omegaDec.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        omegaDec.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.omegaDecrement / currentModifier;

        //Hunting - General
        currentModifier = initialHunger.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        initialHunger.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.initialHunger / currentModifier;

        currentModifier = hungerThreshold.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        hungerThreshold.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.hungerThreshold / currentModifier;

        currentModifier = initialEnergy.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        initialEnergy.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.initialEnergy / currentModifier;

        currentModifier = energyThreshold.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        energyThreshold.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.energyThreshold / currentModifier;

        currentModifier = huntingSpeed.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        huntingSpeed.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.huntingSpeed / currentModifier;

        //Hunting - Advanced
        currentModifier = hungerInc.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        hungerInc.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.hungerIncrement / currentModifier;

        currentModifier = hungerRegenNoise.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        hungerRegenNoise.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.hungerRecoveryNoise / currentModifier;

        currentModifier = energyInc.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        energyInc.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.energyIncrement / currentModifier;

        currentModifier = energyDec.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        energyDec.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.energyDecrement / currentModifier;

        currentModifier = prefHuntRange.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        prefHuntRange.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.preferedHuntingRange / currentModifier;

        currentModifier = maxHuntRange.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        maxHuntRange.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.maxHuntingRange / currentModifier;

        currentModifier = huntSuccessRate.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        huntSuccessRate.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.huntingSuccessRate / currentModifier;

        currentModifier = preyFollowDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        preyFollowDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.preyFollowingDirectionWeight / currentModifier;

        //PredatorAvoidance - General
        currentModifier = predatorRelevantDistance.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        predatorRelevantDistance.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.predatorRelevantDistance / currentModifier;

        currentModifier = newPredatorLearnRate.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        newPredatorLearnRate.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.newPredatorLearningRate / currentModifier;

        currentModifier = escapeRouteNoise.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        escapeRouteNoise.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.escapeRouteNoiseWeight / currentModifier;

        currentModifier = escapeSpeed.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        escapeSpeed.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.escapeSpeed / currentModifier;

        //PredatorAvoidance - Advanced
        currentModifier = predatorAvoidDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        predatorAvoidDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.predatorAvoidanceDirectionWeight / currentModifier;

        //Evade - General
        currentModifier = othersRelevantDistance.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        othersRelevantDistance.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.othersRelevantDistance / currentModifier;

        enableAvoidOthers.transform.Find("Toggle").gameObject.GetComponent<Toggle>().isOn = speciesVariablesNonFuzzy.enableAvoidanceWithOthers;

        enableAlignOthers.transform.Find("Toggle").gameObject.GetComponent<Toggle>().isOn = speciesVariablesNonFuzzy.enableAlignmentWithOthers;

        currentModifier = obstacleSearchingRange.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        obstacleSearchingRange.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.obstacleSearchingRange / currentModifier;

        currentModifier = obstacleRelevantDistance.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        obstacleRelevantDistance.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.obstacleRelevantDistance / currentModifier;

        //Evade - Advanced
        currentModifier = obstacleAvoidDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        obstacleAvoidDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value = speciesVariablesNonFuzzy.obstacleAvoidanceDirectionWeight / currentModifier;

    }

    public void ApplyChangesOnButtonPress_Fuzzy()
    {
        float currentModifier;
        if (speciesVariablesFuzzy == null)
        {
            ApplyChangesOnButtonPress_NonFuzzy();
            return;
        }

        //General - General
        currentModifier = initialN.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.initialPopulation = (int)(initialN.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier);

        currentModifier = maxN.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.maxPopulation = (int)(maxN.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier);

        currentModifier = regenTime.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.regenerationTime = regenTime.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = prefSwimDepth.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.preferedSwimmingDepth = prefSwimDepth.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        //General - Advanced
        currentModifier = spawnRange.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.spawnRange = spawnRange.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = regenCoef.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.regenerationCoef = regenCoef.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = minRouteUpdTime.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.minRouteUpdateTime = minRouteUpdTime.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = maxRouteUpdTime.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.maxRouteUpdateTime = maxRouteUpdTime.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = minCentroidDist.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.minCentroidDistance = minCentroidDist.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = maxCentroidDist.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.maxCentroidDistance = maxCentroidDist.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = prefSwimDepthRange.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.preferedSwimmingDepthRange = prefSwimDepthRange.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = swimDepthDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.swimmingDepthDirectionWeight = swimDepthDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = normalSpeed.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.normalSpeed = normalSpeed.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        //Couzin - General
        currentModifier = alpha.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.alpha = alpha.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = rho.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.rho = rho.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = p.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.p = p.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        //Couzin - Advanced
        currentModifier = couzinDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.couzinDirectionWeight = couzinDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = centroidFollowDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.centroidFollowingDirectionWeight = centroidFollowDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        speciesVariablesFuzzy.allowOmegaFeedback = allowFeedback.transform.Find("Toggle").gameObject.GetComponent<Toggle>().isOn;

        currentModifier = omegaInitial.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.initialOmega = omegaInitial.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = omegaMax.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.maxOmega = omegaMax.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = omegaInc.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.omegaIncrement = omegaInc.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = omegaDec.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.omegaDecrement = omegaDec.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        //Hunting - General
        currentModifier = initialHunger.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.initialHunger = initialHunger.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = hungerThreshold.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.hungerThreshold = hungerThreshold.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = initialEnergy.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.initialEnergy = initialEnergy.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = energyThreshold.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.energyThreshold = energyThreshold.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = huntingSpeed.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.huntingSpeed = huntingSpeed.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        //Hunting - Advanced
        currentModifier = hungerInc.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.hungerIncrement = hungerInc.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = hungerRegenNoise.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.hungerRecoveryNoise = hungerRegenNoise.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = energyInc.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.energyIncrement = energyInc.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = energyDec.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.energyDecrement = energyDec.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = prefHuntRange.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.preferedHuntingRange = prefHuntRange.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = maxHuntRange.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.maxHuntingRange = maxHuntRange.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = huntSuccessRate.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.huntingSuccessRate = huntSuccessRate.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = preyFollowDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.preyFollowingDirectionWeight = preyFollowDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        //PredatorAvoidance - General
        currentModifier = predatorRelevantDistance.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.predatorRelevantDistance = predatorRelevantDistance.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = newPredatorLearnRate.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.newPredatorLearningRate = newPredatorLearnRate.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = escapeRouteNoise.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.escapeRouteNoiseWeight = escapeRouteNoise.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = escapeSpeed.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.escapeSpeed = escapeSpeed.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        //PredatorAvoidance - Advanced
        currentModifier = predatorAvoidDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.predatorAvoidanceDirectionWeight = predatorAvoidDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        //Evade - General
        currentModifier = othersRelevantDistance.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.othersRelevantDistance = othersRelevantDistance.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        speciesVariablesFuzzy.enableAvoidanceWithOthers = enableAvoidOthers.transform.Find("Toggle").gameObject.GetComponent<Toggle>().isOn;

        speciesVariablesFuzzy.enableAlignmentWithOthers = enableAlignOthers.transform.Find("Toggle").gameObject.GetComponent<Toggle>().isOn;

        currentModifier = obstacleSearchingRange.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.obstacleSearchingRange = obstacleSearchingRange.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = obstacleRelevantDistance.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.obstacleRelevantDistance = obstacleRelevantDistance.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        //Evade - Advanced
        currentModifier = obstacleAvoidDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesFuzzy.obstacleAvoidanceDirectionWeight = obstacleAvoidDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        speciesVariablesFuzzy.ResetSchool();
    }

    void ApplyChangesOnButtonPress_NonFuzzy()
    {
        float currentModifier;
        //General - General
        currentModifier = initialN.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.initialPopulation = (int)(initialN.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier);

        currentModifier = maxN.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.maxPopulation = (int)(maxN.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier);

        currentModifier = regenTime.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.regenerationTime = regenTime.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = prefSwimDepth.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.preferedSwimmingDepth = prefSwimDepth.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        //General - Advanced
        currentModifier = spawnRange.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.spawnRange = spawnRange.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = regenCoef.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.regenerationCoef = regenCoef.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = minRouteUpdTime.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.minRouteUpdateTime = minRouteUpdTime.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = maxRouteUpdTime.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.maxRouteUpdateTime = maxRouteUpdTime.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = minCentroidDist.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.minCentroidDistance = minCentroidDist.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = maxCentroidDist.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.maxCentroidDistance = maxCentroidDist.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = prefSwimDepthRange.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.preferedSwimmingDepthRange = prefSwimDepthRange.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = swimDepthDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.swimmingDepthDirectionWeight = swimDepthDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = normalSpeed.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.normalSpeed = normalSpeed.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        //Couzin - General
        currentModifier = alpha.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.alpha = alpha.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = rho.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.rho = rho.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = p.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.p = p.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        //Couzin - Advanced
        currentModifier = couzinDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.couzinDirectionWeight = couzinDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = centroidFollowDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.centroidFollowingDirectionWeight = centroidFollowDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        speciesVariablesNonFuzzy.allowOmegaFeedback = allowFeedback.transform.Find("Toggle").gameObject.GetComponent<Toggle>().isOn;

        currentModifier = omegaInitial.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.initialOmega = omegaInitial.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = omegaMax.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.maxOmega = omegaMax.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = omegaInc.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.omegaIncrement = omegaInc.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = omegaDec.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.omegaDecrement = omegaDec.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        //Hunting - General
        currentModifier = initialHunger.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.initialHunger = initialHunger.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = hungerThreshold.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.hungerThreshold = hungerThreshold.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = initialEnergy.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.initialEnergy = initialEnergy.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = energyThreshold.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.energyThreshold = energyThreshold.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = huntingSpeed.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.huntingSpeed = huntingSpeed.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        //Hunting - Advanced
        currentModifier = hungerInc.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.hungerIncrement = hungerInc.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = hungerRegenNoise.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.hungerRecoveryNoise = hungerRegenNoise.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = energyInc.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.energyIncrement = energyInc.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = energyDec.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.energyDecrement = energyDec.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = prefHuntRange.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.preferedHuntingRange = prefHuntRange.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = maxHuntRange.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.maxHuntingRange = maxHuntRange.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = huntSuccessRate.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.huntingSuccessRate = huntSuccessRate.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = preyFollowDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.preyFollowingDirectionWeight = preyFollowDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        //PredatorAvoidance - General
        currentModifier = predatorRelevantDistance.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.predatorRelevantDistance = predatorRelevantDistance.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = newPredatorLearnRate.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.newPredatorLearningRate = newPredatorLearnRate.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = escapeRouteNoise.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.escapeRouteNoiseWeight = escapeRouteNoise.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = escapeSpeed.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.escapeSpeed = escapeSpeed.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        //PredatorAvoidance - Advanced
        currentModifier = predatorAvoidDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.predatorAvoidanceDirectionWeight = predatorAvoidDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        //Evade - General
        currentModifier = othersRelevantDistance.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.othersRelevantDistance = othersRelevantDistance.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        speciesVariablesNonFuzzy.enableAvoidanceWithOthers = enableAvoidOthers.transform.Find("Toggle").gameObject.GetComponent<Toggle>().isOn;

        speciesVariablesNonFuzzy.enableAlignmentWithOthers = enableAlignOthers.transform.Find("Toggle").gameObject.GetComponent<Toggle>().isOn;

        currentModifier = obstacleSearchingRange.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.obstacleSearchingRange = obstacleSearchingRange.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        currentModifier = obstacleRelevantDistance.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.obstacleRelevantDistance = obstacleRelevantDistance.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        //Evade - Advanced
        currentModifier = obstacleAvoidDirectionW.transform.Find("Slider").gameObject.GetComponent<SliderValueText>().modificador;
        speciesVariablesNonFuzzy.obstacleAvoidanceDirectionWeight = obstacleAvoidDirectionW.transform.Find("Slider").gameObject.GetComponent<Slider>().value * currentModifier;

        speciesVariablesNonFuzzy.ResetSchool();
    }

    public void ToggleSpeciesOnButtonClick()
    {
        selectedSpecies.SetActive(!selectedSpecies.activeSelf);
    }

    public void MoveCameraToFishFromSpecies()
    {
        GameObject fish;
        if (speciesVariablesFuzzy == null)
        {
            fish = speciesVariablesNonFuzzy.GetFirstFish();

        }
        else
        {
            fish = speciesVariablesFuzzy.GetFirstFish();
        }
        if (fish)
        {
            Camera.main.transform.position = fish.transform.position + offset;
            Camera.main.transform.LookAt(fish.transform);
        }
    }

    public void RestoreDefaultValuesOfASpecies()
    {
        bool restaurar = true;
        PickGameObjectOfSelectedSpecies(restaurar);
        SetCurrentParametersOfSelectedSpeciesOnUI_Fuzzy();
        if (speciesVariablesFuzzy == null)
        {
            speciesVariablesNonFuzzy.frameManager = selectedFrameManager;
        }
        else
        {
            speciesVariablesFuzzy.frameManager = selectedFrameManager;
        }
    }

}
