using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolController : MonoBehaviour
{
    [Header("Dependencies")]
    public FrameOptimationManager frameManager;

    [Header("Spawn Settings")]
    [SerializeField] protected GameObject fishPrefab;
    public int maxPopulation = 60;
    public int initialPopulation = 40;
    //[DisplayWithoutEdit]
    public int currentPopulation;
    public float spawnRange = 6f;
    protected bool spawnInitialFish = true;

    [Header("Couzin Settings")]
    public float p = 0.1f;
    public float alpha = 2f;
    public float rho = 6f;
    [SerializeField] protected float delta_t = 0.2f;

    [Header("Omega Settings")]
    public float maxOmega = 2f;
    public float initialOmega = 0.5f;
    public float omegaIncrement = 0.012f;
    public float omegaDecrement = 0.0008f;
    public float omegaFeedbackTime = 10f;
    public bool allowOmegaFeedback = false;

    [Header("Known Routes Settings")]
    [SerializeField] protected int nRoutes = 3;
    [SerializeField] protected List<Vector3> knownRoutes;
    public float minRouteUpdateTime = 10;
    public float maxRouteUpdateTime = 20;
    [SerializeField] protected float knownRouteChangeRate = 0.5f;
    [SerializeField] protected bool updateKnownRoutes = true;
    [SerializeField] protected bool enableRouteConsensus = true;

    [Header("Fuzzy School")]
    public bool isFuzzy = false;

    //proportion:: This variable is used to make variables go faster (used for testing)
    //  For example, I could use it to make the age of a fish last 120s without changing the original variable
    //  Its better to have it as a seperate variable because then I can also use it for other variables like hunger or energy
    //  Default value (1): This means that it doesn't change the value
    //  fixedAge/fixedAgeValue :: The variables let you know if the age will never change (fixedAge), and if so what age it would be (fixedAgeValue)
    //  fixedEnergy/fixedEnergyValue :: The variables let you know if the energy will never change (fixedEnergy), and if so what energy it would be (fixedEnergyValue)
    //  fixedAgeValue/fixedEnergyValue :: Both of these float variables that range from [0-1] should represent the % of the max value that they represent (lifeExpectency/maxEnergy)
    [Header("Testing Variables")]
    public float proportion = 1f;
    public bool fixedAge = false;
    public float fixedAgeValue = 0.5f;
    public bool fixedEnergy = false;
    public float fixedEnergyValue = 0.5f;

    [Header("Hunger Settings")]
    public float maxHunger = 1;
    public float initialHunger = 0.65f;
    public float hungerThreshold = 0.7f;
    public float hungerIncrement = 0.005f;
    public float hungerRecoveryNoise = 0.2f;
    [SerializeField] protected bool randomInitialHunger = false;

    [Header("Energy Settings")]
    public float maxEnergy = 1;
    public float initialEnergy = 0.5f;
    public float energyThreshold = 0.2f;
    public float energyIncrement = 0.01f;
    public float energyDecrement = 0.005f;
    [SerializeField] protected bool randomInitialEnergy = false;

    [Header("Age Settings")]
    [SerializeField] public int lifeExpectation = 1200;
    [SerializeField] protected float deathFunctionCallTime = 30;
    [SerializeField] protected bool enableDeathByAge = false;

    [SerializeField] protected bool randomAge = false;

    [Header("Size Settings")]
    public float bodySize = 2; // tamano del cuerpo en metros cubicos
    public float minSizeScale = 0.5f; // escala minima del tamaño del pez, cuando el pez acaba de nacer
    public float maxSizeScale = 1.5f; // escala maxima del tamaño pez, cuando la edad del pez es igual o mayor a ageToReachMaxSize
    public float ageToReachMaxSize = 600;
    //[DisplayWithoutEdit]
    public Vector3 originalSizeScale;

    [Header("Regeneration Settings")]
    public float regenerationTime = 30;
    public float regenerationCoef = 0.5f; // Regenera 50% de la poblacion actual
    [SerializeField] protected bool enableRegeneration = true;

    [Header("Regrouping Settings")]
    public float maxCentroidDistance = 30;
    public float minCentroidDistance = 10;
    //[DisplayWithoutEdit]
    public Vector3 centroid;

    [Header("Foreign Fish Interaction Settings")]
    public List<string> othersTag;
    [SerializeField] protected bool autoIncludeOwnTag = true;
    public float othersRelevantDistance = 15;
    public float otherFishSearchingRange = 10;
    public LayerMask fishLayer;
    public bool enableAvoidanceWithOthers = true;
    public bool enableAlignmentWithOthers = true;
    [HideInInspector] public List<GameObject> othersList;

    [Header("Predator Avoidance Settings")]
    public List<string> predatorsTag;
    public float predatorRelevantDistance = 15;
    [SerializeField] protected float predatorRelevantDistanceToSchool = 30;
    public float newPredatorLearningRate = 0.05f;
    private Dictionary<string, float> newPredatorLearningRateMemory;
    public float escapeRouteNoiseWeight = 0.5f; // Variable para cambiar la influencia de la ruta de ruido
    protected List<GameObject> predators;

    [Header("Prey Following Settings")]
    public List<string> preysTag;
    public float preferedHuntingRange = 5;
    public float maxHuntingRange = 20f;
    public float followingPreyMaxTime = 15f;
    public float huntingSuccessRate = 0.6f;

    [Header("Hiding Location Settings")]
    public LayerMask hidingLayer;

    [Header("Obtacles Avoidance Settings")]
    public LayerMask obstacleLayer;
    public float obstacleSearchingRange = 3;
    public float obstacleRelevantDistance = 6;

    //Used to combine the obstacle and hiding layers
    [Header("Combined Layers")]
    public LayerMask combinedLayer;

    [Header("Swimming Depth Settings")]
    public float preferedSwimmingDepth = -30;
    public float preferedSwimmingDepthRange = 10;

    [Header("Direction Weight Settings")]
    public float couzinDirectionWeight = 1;
    public float centroidFollowingDirectionWeight = 2f;
    public float obstacleAvoidanceDirectionWeight = 8f;
    public float predatorAvoidanceDirectionWeight = 1;
    public float preyFollowingDirectionWeight = 4f;
    public float swimmingDepthDirectionWeight = 2;
    public float hidingDirectionWeight = 100f; //This valuew was for testing

    [Header("Speed Settings")]
    public float normalSpeed = 2;
    public float escapeSpeed = 2f;
    public float huntingSpeed = 2f;
    public float acceleration = 1f;
    public float animationSpeed = 2f;
    public float rotationSpeed = 3.2f;

    protected List<FishController> fishList;
    private List<FishController> informedFishList;

    protected virtual void Start()
    {
        // Comprobaciones necesarias para evitar que algunos valores minimos sean mayores que los maximos.
        if (initialOmega > maxOmega)
        {
            initialOmega = maxOmega;
        }
        if( nRoutes < 1)
        {
            nRoutes = 1;
        }
        if (minRouteUpdateTime > maxRouteUpdateTime)
        {
            maxRouteUpdateTime = minRouteUpdateTime;
        }
        if (minSizeScale > maxSizeScale)
        {
            maxSizeScale = minSizeScale;
        }
        if (minCentroidDistance >= maxCentroidDistance)
        {
            maxCentroidDistance = minCentroidDistance * 2;
        }
        if (preferedHuntingRange > maxHuntingRange)
        {
            maxHuntingRange = preferedHuntingRange;
        }
        if (obstacleSearchingRange >= obstacleRelevantDistance)
        {
            obstacleRelevantDistance = obstacleSearchingRange * 2;
        }
        if (hungerThreshold > maxHunger)
        {
            hungerThreshold = maxHunger;
        }
        else if (hungerThreshold < 0)
        {
            hungerThreshold = maxHunger;
        }
        if (energyThreshold > maxEnergy)
        {
            energyThreshold = maxEnergy;
        }
        else if (energyThreshold < 0)
        {
            energyThreshold = maxEnergy/2f;
        }

        //Adding proportions
        lifeExpectation = (int)((float)lifeExpectation/proportion);
        ageToReachMaxSize = (int)((float)ageToReachMaxSize/proportion);

        // Inicializacion de la escala original
        originalSizeScale = fishPrefab.transform.localScale;

        //Initialize the combined layer, obstacle + hiding layer
        combinedLayer = obstacleLayer | hidingLayer;

        // Inicializacion de listas
        fishList = new List<FishController>();
        informedFishList = new List<FishController>();
        othersList = new List<GameObject>();
        predators = new List<GameObject>();

        // Inicializacion de la memoria del banco
        newPredatorLearningRateMemory = new Dictionary<string, float>();
        if (autoIncludeOwnTag)
        {
            if (!othersTag.Contains(fishPrefab.tag))
            {
                othersTag.Add(fishPrefab.tag);
            }
        }

        // Inicializacion de las rutas conocidas por el cardumen
        knownRoutes = new List<Vector3>();
        for (int i = 0; i < nRoutes; i++)
        {
            knownRoutes.Add((new Vector3(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360))).normalized);
        }

        // Invoke a las funciones periodicas
        Invoke("UpdateKnownRoutes", Random.Range(minRouteUpdateTime, maxRouteUpdateTime));
        Invoke("FeedbackOmega", omegaFeedbackTime);
        Invoke("RegenerateFish", regenerationTime);
        Invoke("KillFish", deathFunctionCallTime);
        Invoke("UpdateFishInformation", delta_t);
    }

    protected virtual void Update()
    {
        // Actualizar el centroide del cardumen
        centroid = GetSchoolCentroid();

        // Actualizar la poblacion para verla en el inspector
        currentPopulation = fishList.Count;

        // Si no se han generado todos los individuos iniciales, generar un individuo por frame
        if (spawnInitialFish)
        {
            // Se genera un nuevo individuo y se añade a la lista fishList
            FishController fish = GenerateFish();
            if (fish == null)
            {
                spawnInitialFish = false;
            }
            else
            {
                fishList.Add(fish);
                MakeFishInformed();
                if (fishList.Count >= initialPopulation)
                {
                    spawnInitialFish = false;
                }
            }
        }
    }

    void UpdateFishInformation()
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
        // Esta funcion se manda a llamar a si misma cada delta_t segundos
        for (int i = 0; i < fishList.Count; i++)
        {
            // Para cada pez, se asigna la nueva direccion preferida
            FishInformation info = GetFishInformation(i);
            fishList[i].SetInformation(info);
        }
        // Vuelve actualizar la direccion dentro de delta_t segundos
        Invoke("UpdateFishInformation", delta_t);
    }

    protected virtual FishInformation GetFishInformation(int i)
    {
        // Calcular velocidad
        float speed = normalSpeed;
        if (preyFollowingDirectionWeight > 0)
        {
            float meanMaxHunger = (hungerThreshold + maxHunger) / 2;
            if (fishList[i].hunger > meanMaxHunger)
            {
                speed += huntingSpeed * ((fishList[i].hunger - meanMaxHunger) / (maxHunger - meanMaxHunger));
            }
        }
        if (predatorAvoidanceDirectionWeight > 0)
        {
            if (predators.Count > 0)
            {
                GameObject predator = GetCloserPredator(i);
                float w = 1;
                float distance = (predator.transform.position - fishList[i].transform.position).magnitude;
                if (distance > predatorRelevantDistance)
                {
                    w = 0;
                }
                else if (distance > predatorRelevantDistance / 2)
                {
                    w = 1 - (2 * distance - predatorRelevantDistance) / predatorRelevantDistance;
                }
                speed += escapeSpeed * w;
            }
        }
        if (fishList[i].energy < energyThreshold)
        {
            speed *= fishList[i].energy / energyThreshold;
        }

        //Calcular el rango de caza
        float preySearchingRange = preferedHuntingRange;

        float meanHunger = (hungerThreshold + maxHunger) / 2;
        if (fishList[i].hunger > meanHunger)
        {
            preySearchingRange += (maxHuntingRange - preferedHuntingRange) * ((fishList[i].hunger - meanHunger) / (maxHunger - meanHunger));
        }

        // Calcular la direccion
        Vector3 couzinDirection = GetCouzinDirection(i);
        Vector3 knownRoute = GetKnownRoute(i);
        Vector3 obstacleAvoidanceDirection = GetObstacleAvoidanceDirection(i);
        Vector3 hidingLocationDirection = GetHidingLocationDirection(i);
        Vector3 predatorAvoidanceDirection = GetPredatorAvoidanceDirection(i);
        Vector3 preyFollowingDirection = GetPreyFollowingDirection(i);
        Vector3 centroidFollowingDirection = GetCentroidFollowingDirection(i);
        Vector3 swimmingDepthDirection = GetSwimmingDepthDirection(i);

        Vector3 preferedDirection = couzinDirection * couzinDirectionWeight
                                  + knownRoute * fishList[i].omega
                                  + obstacleAvoidanceDirection * obstacleAvoidanceDirectionWeight
                                  //+ hidingLocationDirection * hidingDirectionWeight
                                  + predatorAvoidanceDirection * predatorAvoidanceDirectionWeight
                                  + preyFollowingDirection * preyFollowingDirectionWeight
                                  + centroidFollowingDirection * centroidFollowingDirectionWeight
                                  + swimmingDepthDirection * swimmingDepthDirectionWeight;

        if (preferedDirection == Vector3.zero)
            preferedDirection = fishList[i].transform.forward;
        else
            preferedDirection /= preferedDirection.magnitude;

        return new FishInformation(preferedDirection, speed, preySearchingRange);
    }

    protected Vector3 GetObstacleAvoidanceDirection(int i)
    {
        Vector3 obstacleAvoidanceDirection = Vector3.zero;
        if (obstacleAvoidanceDirectionWeight > 0)
        {
            if (fishList[i].pointList.Count > 0)
            {
                // Suma las diferencia entre la pocision del individuo y el punto de colision
                // Cada diferencia se normaliza antes de anadirse a la suma, y representa la direccion del pez apuntando hacia el obstaculo
                for (int j = 0; j < fishList[i].pointList.Count; j++)
                {
                    if ((fishList[i].transform.position - fishList[i].pointList[j]).magnitude < obstacleRelevantDistance)
                    {
                        obstacleAvoidanceDirection += (fishList[i].transform.position - fishList[i].pointList[j]).normalized;
                    }
                    else
                    {
                        // Elimina los puntos de colision que estan demasiado lejos
                        fishList[i].pointList.RemoveAt(j);
                        j--;
                    }
                }
                // Del vector que contiene la suma de todas las diferencias, se calcula el vector unitario
                if (obstacleAvoidanceDirection != Vector3.zero)
                    obstacleAvoidanceDirection /= obstacleAvoidanceDirection.magnitude;
            }
        }
        return obstacleAvoidanceDirection;
    }

    protected Vector3 GetHidingLocationDirection(int i)
    {
        //Calcula la direccion de la localizacion de escondite detectado y ve hacia el. (Solamente se esta escogiendo la ultima localizacion que ha visto)
        Vector3 hidingDirection = Vector3.zero;
        if (hidingDirectionWeight > 0)
        {
            if (fishList[i].hideLocation != Vector3.zero){
                hidingDirection = (fishList[i].hideLocation - fishList[i].transform.position).normalized;
                fishList[i].hideLocation = Vector3.zero;
            }

            if (hidingDirection != Vector3.zero)
                hidingDirection /= hidingDirection.magnitude;
        }
        return hidingDirection;
    }

    protected Vector3 GetPredatorAvoidanceDirection(int i)
    {
        Vector3 predatorAvoidanceDirection = Vector3.zero;

        if (predatorAvoidanceDirectionWeight > 0)
        {
            if (predators.Count > 0)
            {
                float w = 1;
                GameObject predator = GetCloserPredator(i);
                float distance = (predator.transform.position - fishList[i].transform.position).magnitude;
                if (distance > predatorRelevantDistance)
                {
                    w = 0;
                }
                else if (distance > predatorRelevantDistance / 2)
                {
                    w = 1 - (2 * distance - predatorRelevantDistance) / predatorRelevantDistance;
                }
                //Si hay un depredador, la direccion del pez se calcula tomando en cuenta tres direcciones
                // 1. La direccion contraria al depredador
                predatorAvoidanceDirection = (fishList[i].transform.position - predator.transform.position).normalized;
                // 3. Una direccion ruido, calculada de manera aleatoria
                predatorAvoidanceDirection += escapeRouteNoiseWeight * fishList[i].escapeRouteNoise;
                predatorAvoidanceDirection /= predatorAvoidanceDirection.magnitude;
                predatorAvoidanceDirection *= w;
            }
        }

        return predatorAvoidanceDirection;
    }

    protected Vector3 GetPreyFollowingDirection(int i)
    {
        Vector3 preyFollowingDirection = Vector3.zero;

        if (preyFollowingDirectionWeight > 0)
        {

            if (fishList[i].prey)
            {
                preyFollowingDirection = (fishList[i].prey.transform.position - fishList[i].transform.position).normalized;
            }
        }

        return preyFollowingDirection;
    }

    protected virtual Vector3 GetCentroidFollowingDirection(int i)
    {
        Vector3 centroidFollowingDirection = Vector3.zero;

        if (centroidFollowingDirectionWeight > 0)
        {
            if ((centroid - fishList[i].transform.position).magnitude > maxCentroidDistance)
            {
                fishList[i].gotoCentroid = true;
                centroidFollowingDirection = (centroid - fishList[i].transform.position).normalized;
            }
            else if (fishList[i].gotoCentroid && (centroid - fishList[i].transform.position).magnitude > minCentroidDistance)
            {
                centroidFollowingDirection = (centroid - fishList[i].transform.position).normalized;
            }
            else
            {
                fishList[i].gotoCentroid = false;
            }
        }

        return centroidFollowingDirection;
    }

    protected Vector3 GetCouzinDirection(int i)
    {
        Vector3 couzinDirection = Vector3.zero;

        if (couzinDirectionWeight > 0)
        {
            // COUZIN 2002
            // c, una variable foo
            Vector3 c;
            // Para mayor explicacion sobre las ecuaciones, revisar en: https://drive.google.com/file/d/14z2cNMCAOx63Hk-FZquXGO4KMJ247oHZ/view?usp=sharing

            // Primeramente, se verifica si el pez tiene tiene individuos dentro de la region alfa
            // De ser asi, calcula la nueva direccion intentando alejarse de sus vecinos, para mantener su espacio libre
            if (AreNeighborsWithinARegion(i, alpha, enableAvoidanceWithOthers))
            {
                // neighbors, lista de los indices de los vecinos que se encuentran dentro de la region alpha
                List<Transform> neighbors = GetNeighborsWithinARegion(i, alpha, enableAvoidanceWithOthers);

                // Ecuacion (1) en el articulo de couzin:
                foreach (Transform j in neighbors)
                {
                    c = j.position - fishList[i].transform.position;
                    couzinDirection += c / c.magnitude;
                }
                couzinDirection = -couzinDirection;

                // Del vector restultante, se calcula su vector unitario que representa la direccion preferida
                couzinDirection /= couzinDirection.magnitude;
            }
            else if (AreNeighborsWithinARegion(i, rho, enableAlignmentWithOthers, othersTag))
            {
                // v, tomado del nombre que le da Couzin a la direccion de un individuo. Guarda la sumatoria de las direcciones unitarias de los vecinos dentro de rho
                Vector3 v = Vector3.zero;
                // neighbors, lista de los indices de los vecinos que se encuentran dentro de la region rho
                List<Transform> neighbors = GetNeighborsWithinARegion(i, rho, enableAlignmentWithOthers, othersTag);

                if (neighbors.Count > 0) // Si hay vecinos dentro de la region rho, entonces usa la siguiente ecuacion...
                {
                    // Ecuacion (2) en el articulo de couzin:
                    foreach (Transform j in neighbors)
                    {
                        c = j.position - fishList[i].transform.position;
                        couzinDirection += c / c.magnitude;
                        v += j.forward / j.forward.magnitude;
                    }
                    couzinDirection = couzinDirection + v;
                }

                // Del vector restultante, se calcula su vector unitario que representa la direccion preferida
                couzinDirection /= couzinDirection.magnitude;
            }
            // END COUZIN
        }

        return couzinDirection;
    }

    protected Vector3 GetKnownRoute(int i)
    {
        Vector3 knownRoute = Vector3.zero;

        // Si el individuo es un individuo informado
        if (fishList[i].isInformed)
        {
            // Ecuacion (3) en el articulo de couzin:
            if (enableRouteConsensus)
            {
                knownRoute = GetConsensus_g();
            }
            else
            {
                knownRoute = fishList[i].g;
            }
        }

        return knownRoute;
    }

    protected Vector3 GetSwimmingDepthDirection(int i)
    {
        Vector3 swimmingDepthDirection = Vector3.zero;

        if (fishList[i].transform.position.y > (preferedSwimmingDepth + preferedSwimmingDepthRange))
        {
            swimmingDepthDirection = new Vector3(0, -1, 0);
        }
        else if (fishList[i].transform.position.y > preferedSwimmingDepth)
        {
            swimmingDepthDirection = new Vector3(0, -1, 0) * (fishList[i].transform.position.y - preferedSwimmingDepth) / preferedSwimmingDepthRange;
        }
        else if (fishList[i].transform.position.y < preferedSwimmingDepth)
        {
            swimmingDepthDirection = new Vector3(0, 1, 0) * (preferedSwimmingDepth - fishList[i].transform.position.y) / preferedSwimmingDepthRange;
        }
        else if (fishList[i].transform.position.y < (preferedSwimmingDepth - preferedSwimmingDepthRange))
        {
            swimmingDepthDirection = new Vector3(0, 1, 0);
        }

        return swimmingDepthDirection;
    }

    bool AreNeighborsWithinARegion(int i, float region, bool includeOthers, List<string> filterOthersTag = null)
    {
        // Esta funcion verifica si el pez i tiene otros peces j dentro de la region indicada en los parametros.
        // Si hay cuando menos un pez dentro de la region indicada, retorna true
        // Caso contrario, retorna false
        for (int j = 0; j < fishList.Count; j++)
        {
            if (j != i)
            {
                if ((fishList[j].transform.position - fishList[i].transform.position).magnitude < region)
                {
                    return true;
                }
            }
        }
        if (includeOthers)
        {
            if (othersTag == null)
            {
                for (int j = 0; j < othersList.Count; j++)
                {
                    if ((othersList[j].transform.position - fishList[i].transform.position).magnitude < region)
                    {
                        return true;
                    }
                }
            }
            else
            {
                for (int j = 0; j < othersList.Count; j++)
                {
                    if (othersTag.Contains(othersList[j].tag))
                    {
                        if ((othersList[j].transform.position - fishList[i].transform.position).magnitude < region)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    List<Transform> GetNeighborsWithinARegion(int i, float region, bool includeOthers, List<string> filterOthersTag = null)
    {
        // Retorna una lista con los indices de los vecinos de i que se encuentran dentro de la region indicada en los parametros
        List<Transform> neighbors = new List<Transform>();
        for (int j = 0; j < fishList.Count; j++)
        {
            if (i != j)
            {
                if ((fishList[j].transform.position - fishList[i].transform.position).magnitude < region)
                {
                    neighbors.Add(fishList[j].transform);
                }
            }
        }
        if (includeOthers)
        {
            if (filterOthersTag == null)
            {
                for (int j = 0; j < othersList.Count; j++)
                {
                    if ((othersList[j].transform.position - fishList[i].transform.position).magnitude < region)
                    {
                        neighbors.Add(othersList[j].transform);
                    }
                }
            }
            else
            {
                for (int j = 0; j < othersList.Count; j++)
                {
                    if (othersTag.Contains(othersList[j].tag))
                    {
                        if ((othersList[j].transform.position - fishList[i].transform.position).magnitude < region)
                        {
                            neighbors.Add(othersList[j].transform);
                        }
                    }
                }
            }
        }
        return neighbors;
    }

    protected FishController GenerateFish()
    {
        // Solo se generan N peces, si se trata de generar otro, la funcion GenerateFish() retorna null
        if (fishList.Count >= maxPopulation)
            return null;

        // Primero, se genera de manera aleatoria la posicion del individuo
        // La posicion generada es aleatoria, con centro en trasform.position y un radio igual a "instantiateRange"
        Vector3 position = centroid + new Vector3(
                Random.Range(-spawnRange, spawnRange),
                Random.Range(-spawnRange, spawnRange),
                Random.Range(-spawnRange, spawnRange));

        // Segundo, se genera de manera aleatoria la rotacion inicial del individuo
        Quaternion rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), 0);

        // Tercero, creamos una variable que almacenara el pez
        FishController fish;

        // Se asigna a la variable fish la referencia al componente FishController, del nuevo individuo
        fish = Instantiate(fishPrefab, position, rotation).GetComponent<FishController>();
        fish.isInformed = false;

        // Se asigna el SchoolController como padre del individuo generado
        fish.transform.SetParent(transform);
        fish.assignedFrame = frameManager.RegisterFish();
        if (fixedAge)
        {
            fish.age = lifeExpectation * fixedAgeValue;
        }
        else if (randomAge){
            fish.age = Random.Range(0, lifeExpectation);
        }
        else
        {
            fish.age = 0;
        }
        if (randomInitialHunger)
            fish.hunger = Random.Range(0, maxHunger);
        else
            fish.hunger = initialHunger;

        if (randomInitialEnergy)
        {
            fish.energy = Random.Range(0, maxEnergy);
            fish.recoveringEnergy = Random.Range(0, 2) == 1;
        }
        else if (fixedEnergy)
        {
            fish.energy = maxEnergy * fixedEnergyValue;
            fish.recoveringEnergy = false;
        }
        else
        {
            fish.energy = initialEnergy;
            fish.recoveringEnergy = true;
        }



        // Se asigna la referencia del school controller
        fish.schoolController = this;
        return fish;
    }

    protected void MakeFishInformed()
    {
        if (fishList.Count > 0)
        {
            int i = 0;
            while ((float)((float)informedFishList.Count / fishList.Count) < p && i < fishList.Count)
            {
                if (!fishList[i].isInformed)
                {
                    fishList[i].isInformed = true;
                    fishList[i].omega = initialOmega;
                    fishList[i].g = knownRoutes[Random.Range(0, knownRoutes.Count)];
                    informedFishList.Add(fishList[i]);
                }
                i++;
            }
        }
    }

    void KillFish()
    {
        if (enableDeathByAge)
        {
            List<int> deathFishIndex = new List<int>();
            for (int i = 0; i < fishList.Count; i++)
            {
                if ( (fishList[i].age / (fishList[i].age + lifeExpectation)) > Random.Range(0f, 1f) )
                {
                    deathFishIndex.Add(i);
                }
            }
            for (int i = deathFishIndex.Count; i > 0; i--)
            {
                KillFish(fishList[deathFishIndex[i - 1]]);
            }
            MakeFishInformed();
        }
        Invoke("KillFish", deathFunctionCallTime);
    }

    public void KillFish(FishController fish)
    {
        if (fishList.Contains(fish))
        {
            fishList.Remove(fish);
        }
        if (informedFishList.Contains(fish))
        {
            informedFishList.Remove(fish);
        }
        Destroy(fish.gameObject);
    }

    public void KillFish(FishController fish, GameObject predator)
    {
        if (fishList.Contains(fish))
        {
            fishList.Remove(fish);
        }
        if (informedFishList.Contains(fish))
        {
            informedFishList.Remove(fish);
        }
        Destroy(fish.gameObject);
        SetPredator(predator);
    }

    public virtual void ResetSchool()
    {
        while (fishList.Count > 0)
        {
            KillFish(fishList[0]);
        }
        spawnInitialFish = true;
    }

    void RegenerateFish()
    {
        if (enableRegeneration && fishList.Count < maxPopulation)
        {
            int n = (int)(regenerationCoef * fishList.Count);
            for (int i = 0; i < n; i++)
            {
                FishController fish = GenerateFish();
                if (fish == null)
                {
                    break;
                }
                else
                {
                    fishList.Add(fish);
                }
            }
            MakeFishInformed();
        }
        Invoke("RegenerateFish", regenerationTime);
    }

    public void SetPredator(GameObject predator)
    {
        if (!predatorsTag.Contains(predator.tag))
        {
            if (Random.Range(0f, 1f) < newPredatorLearningRate)
            {
                predatorsTag.Add(predator.tag);
                if (newPredatorLearningRateMemory.ContainsKey(predator.tag))
                {
                    newPredatorLearningRateMemory.Remove(predator.tag);
                }
            }
            else
            {
                if (!newPredatorLearningRateMemory.ContainsKey(predator.tag))
                {
                    newPredatorLearningRateMemory.Add(predator.tag, 0);
                }
                newPredatorLearningRateMemory[predator.tag] += newPredatorLearningRate;
            }

        }
        if (!predators.Contains(predator))
        {
            predators.Add(predator);

            foreach (FishController fish in fishList)
            {
                fish.escapeRouteNoise = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                fish.escapeRouteNoise = fish.escapeRouteNoise.normalized;
            }
        }
    }

    public GameObject GetCloserPredator(int i)
    {
        GameObject predator = null;
        float maxDistance = float.MaxValue;
        foreach (GameObject p in predators)
        {
            if ((p.transform.position - fishList[i].transform.position).magnitude < maxDistance)
            {
                predator = p;
            }
        }
        return predator;
    }

    Vector3 GetConsensus_g()
    {
        Vector3 g = Vector3.zero;
        // Primero, verificar si hay individuos con informacion, en caso contrario se retorna el vector zero
        if (informedFishList.Count > 0)
        {
            List<Vector3> directions = new List<Vector3>();
            List<int> counter = new List<int>();
            foreach (FishController fish in informedFishList)
            {
                if (directions.Contains(fish.g))
                {
                    counter[directions.IndexOf(fish.g)]++;
                }
                else
                {
                    directions.Add(fish.g);
                    counter.Add(1);
                }
            }
            if (directions.Count == 1)
            {
                g = directions[0];
            }
            else
            {
                bool thereIsMajority = false;
                int max = counter[0];
                g = directions[0];
                for (int i = 1; i < directions.Count; i++)
                {
                    if (max != counter[i])
                    {
                        thereIsMajority = true;
                    }
                    if (counter[i] > max)
                    {
                        g = directions[i];
                        max = counter[i];
                    }
                }
                if (!thereIsMajority)
                {
                    g = directions[0];
                }
            }
        }
        return g;
    }

    void FeedbackOmega()
    {
        if (allowOmegaFeedback)
        {
            // variable que guarda la direccion global del banco de peces
            Vector3 u = GetGlobalDirection();
            foreach (FishController fish in informedFishList)
            {
                float teta = Tools.AngleBetween2Vectors(u, fish.transform.forward);

                if (teta <= 20)
                {
                    fish.omega += omegaIncrement;
                    if (fish.omega > maxOmega)
                    {
                        fish.omega = maxOmega;
                    }
                }
                else
                {
                    fish.omega -= omegaDecrement;
                    if (fish.omega < 0)
                    {
                        fish.omega = 0;
                    }
                }
            }
        }
        // La funcion se invoca a si misma para volver a retroalimentar la variable omega, luego de delta_t_omega segundos
        Invoke("FeedbackOmega", omegaFeedbackTime);
    }

    void UpdateKnownRoutes()
    {
        if (updateKnownRoutes)
        {
            knownRoutes = new List<Vector3>();
            for (int i = 0; i < nRoutes; i++)
            {
                knownRoutes.Add((new Vector3(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360))).normalized);
            }

            foreach (FishController fish in informedFishList)
            {
                if (knownRouteChangeRate >= Random.Range(0f, 1f))
                {
                    fish.g = knownRoutes[Random.Range(0, knownRoutes.Count)];
                    fish.omega = initialOmega;
                }
            }
        }
        Invoke("UpdateKnownRoutes", Random.Range(minRouteUpdateTime, maxRouteUpdateTime));
    }

    public Vector3 GetSchoolCentroid()
    {
        if (fishList.Count < 1)
        {
            return transform.position;
        }

        Vector3 centroid = Vector3.zero;

        foreach (FishController fish in fishList)
        {
            centroid += fish.transform.position;
        }

        return centroid / fishList.Count;
    }

    Vector3 GetGlobalDirection()
    {
        Vector3 direction = Vector3.zero;
        foreach (FishController fish in fishList)
        {
            direction += fish.transform.forward;
        }
        return direction / direction.magnitude;
    }

    public int CountPopulation()
    {
        return fishList.Count;
    }

    public List<FishController> GetFishList()
    {
        return fishList;
    }

    public GameObject GetFirstFish()
    {
        if( fishList.Count > 0)
        {
            return fishList[0].gameObject;
        }
        return null;
    }
}
