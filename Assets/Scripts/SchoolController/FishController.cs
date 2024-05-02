using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : MonoBehaviour
{

    [Header("Dependencies")]
    public Animation anim;
    public GameObject[] obstacleDetector;
    [SerializeField] ParticleSystem bloodParticle;
    public SchoolController schoolController;

    [Header("Informed Settings")]
    //[DisplayWithoutEdit]
    public bool isInformed;
    //[DisplayWithoutEdit]
    public float omega;
    //[DisplayWithoutEdit]
    public Vector3 g;

    [Header("FishInformation")]
    //[DisplayWithoutEdit]
    public float speed;
    float currentSpeed;
    //[DisplayWithoutEdit]
    public Vector3 direction;
    //[DisplayWithoutEdit]
    public float preySearchingRange;

    [Header("Regrouping")]
    //[DisplayWithoutEdit]
    public bool gotoCentroid;


    [Header("Obstacles Avoidance")]
    public List<Vector3> pointList;

    [Header("Predator Avoidance")]
    //[DisplayWithoutEdit]
    public Vector3 escapeRouteNoise;

    [Header("Prey Following")]
    //[DisplayWithoutEdit]
    public bool searchingPrey;
    public GameObject prey;
    float followingTimeCounter;

    [Header("Attributes")]
    public float age;
    public float hunger;
    public float energy;
    //[DisplayWithoutEdit]
    public bool recoveringEnergy = true;
    //[DisplayWithoutEdit]
    public float sizeScale;
    bool updateSizeScale;

    [Header("Optimation Settings")]
    //[DisplayWithoutEdit]
    public int assignedFrame;

    private void Start()
    {
        // Reproducir la animacion del SwimOne
        if (anim)
        {
            anim.Play("SwimOne");
            anim["SwimOne"].speed = schoolController.animationSpeed;
        }

        // Establecer tamaño inicial
        if (age < schoolController.ageToReachMaxSize)
        {
            updateSizeScale = true;
            sizeScale = schoolController.minSizeScale + (schoolController.maxSizeScale - schoolController.minSizeScale) * ((float)age / (float)schoolController.ageToReachMaxSize);
        }
        else
        {
            updateSizeScale = false;
            sizeScale = schoolController.maxSizeScale;
        }
        transform.localScale = schoolController.originalSizeScale * sizeScale;

        // Establecer la velocidad inicial
        currentSpeed = schoolController.normalSpeed;

        /*if (schoolController.fixedAge)
        {
            age = schoolController.lifeExpectation * schoolController.fixedAgeValue;
        }

        if(schoolController.fixedEnergy)
        {
            energy = schoolController.maxEnergy * schoolController.fixedEnergyValue;
        }*/
    }

    private void FixedUpdate()
    {
        // Solo se ejecuta si el numero de frame actual es igual al frame asignado del individuo
        if (assignedFrame == schoolController.frameManager.currentFrame)
        {
            // Fragmento de codigo que se ejecuta si el individuo es informado
            if (isInformed)
            {
                // Se obtiene un arreglo de todos los peces que se encuentren en un radio determinado
                Collider[] allColliders = Physics.OverlapSphere(transform.position, schoolController.otherFishSearchingRange, schoolController.fishLayer);

                // Anadimos los peces a la lista de otros peces y seleccionamos a los depredadores del banco de pez
                List<Collider> predatorsCollider = new List<Collider>();
                foreach (Collider collider in allColliders)
                {
                    if (schoolController.predatorsTag.Contains(collider.tag))
                    {
                        predatorsCollider.Add(collider);
                    }
                    else if (schoolController != collider.GetComponent<FishController>().schoolController && !schoolController.othersList.Contains(collider.gameObject))
                    {
                        schoolController.othersList.Add(collider.gameObject);
                    }
                }

                // Si se ha encontrado al menos un depredador, se le informa al SchoolController
                foreach(Collider predator in predatorsCollider)
                {
                    schoolController.SetPredator(predator.gameObject);
                }
            }

            // Deteccion de obstaculos, para cada detector se lanza un Raycast
            foreach (GameObject detector in obstacleDetector)
            {
                RaycastHit hit;
                if (Physics.Raycast(detector.transform.position, detector.transform.forward, out hit, schoolController.obstacleSearchingRange, schoolController.obstacleLayer))
                {
                    pointList.Add(hit.point);
                }
            }

            // Se actualiza el tamano del pez
            if (updateSizeScale)
            {
                if (age < schoolController.ageToReachMaxSize)
                {
                    sizeScale = schoolController.minSizeScale + (schoolController.maxSizeScale - schoolController.minSizeScale) * ((float)age / (float)schoolController.ageToReachMaxSize);
                }
                else
                {
                    updateSizeScale = false;
                    sizeScale = schoolController.maxSizeScale;
                }
                transform.localScale = schoolController.originalSizeScale * sizeScale;
            }

            // Busqueda de presa
            if (searchingPrey && !prey)
            {
                FindPrey();
            }
        }
    }

    private void Update()
    {
        //Actualizar edad
        
        if (schoolController.fixedAge == false)
        {
            age += Time.deltaTime;
        }

        // Actualizar hambre
        hunger += schoolController.hungerIncrement * Time.deltaTime;
        if (hunger > schoolController.maxHunger)
        {
            hunger = schoolController.maxHunger;
        }
        // Activar bandera para buscar presas
        else if (hunger > schoolController.hungerThreshold)
        {
            if (!searchingPrey)
            {
                searchingPrey = true;
            }
        }

        // Actualizar energia
        // If the energy is fixed then don't change the energy
        if (schoolController.fixedEnergy == false)
        {
            if (recoveringEnergy)
            {
                energy += schoolController.energyIncrement * Time.deltaTime;
                if (energy > schoolController.maxEnergy)
                {
                    recoveringEnergy = false;
                    energy = schoolController.maxEnergy;
                }
            }
            else
            {
                energy -= schoolController.energyDecrement * Time.deltaTime;
                if (energy < 0)
                {
                    recoveringEnergy = true;
                    energy = 0;
                }
            }
        }

        // Si se ha seguido demasiado tiempo una presa, buscar otra
        if( prey )
        {
            followingTimeCounter += Time.deltaTime;
            if (followingTimeCounter > schoolController.followingPreyMaxTime)
            {
                FindPrey(prey); // FindTarget se encarga de resetear followingTimeCounter
            }
        }

        // Actualizar la rapidez
        currentSpeed = Mathf.Lerp(currentSpeed, speed, schoolController.acceleration * Time.deltaTime);

        // Actualizar la rotacion
        Vector3 delayDirection = Vector3.Lerp(transform.forward, direction, schoolController.rotationSpeed * Time.deltaTime);
        transform.LookAt(transform.position + delayDirection);

        // Calcula la nueva posicion
        transform.position += currentSpeed * Time.deltaTime * transform.forward;
    }

    public void Dead()
    {
        // Elimina al individuo de la escena de la manera correcta,
        // Eliminando todas las referencias que existan de el en las listas de SchoolController
        schoolController.KillFish(this);
    }

    public void Dead(GameObject predator)
    {
        // Elimina al individuo de la escena de la manera correcta.
        // A su vez, indica al controlador del banco que pez que lo comio
        schoolController.KillFish(this, predator);
    }

    public void FindPrey(int method = 0)
    {
        // Busca una presa con uno de dos metodos

        // Resetea el contador para saber cuanto tiempo se ha seguido a la misma presa
        followingTimeCounter = 0;

        if (method == 0) // Selecciona la presa mas cercana, sin importar la distancia a la que se encuentre
        {
            float minDistance = float.MaxValue;
            foreach (string tag in schoolController.preysTag)
            {
                GameObject[] preys = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject prey in preys)
                {
                    if ((transform.position - prey.transform.position).magnitude < minDistance)
                    {
                        this.prey = prey;
                        minDistance = (transform.position - prey.transform.position).magnitude;
                    }
                }
            }
        }
        else if (method == 1) // Selecciona a la presa mas cercana dentro de un rango
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, preySearchingRange, schoolController.fishLayer);

            float minDistance = float.MaxValue;
            foreach (Collider collider in colliders)
            {
                if (schoolController.preysTag.Contains(collider.tag) && (collider.transform.position - transform.position).magnitude < minDistance)
                {
                    prey = collider.gameObject;
                    minDistance = (prey.transform.position - transform.position).magnitude;
                }
            }
        }
    }

    public void FindPrey(GameObject exclude, int method = 0)
    {
        // Busca una presa con uno de dos metodos, excluyendo a determinado individuo

        // Resetea el contador para saber cuanto tiempo se ha seguido a la misma presa
        followingTimeCounter = 0;
        if (method == 0) // Selecciona la presa mas cercana, sin importar la distancia a la que se encuentre
        {

            float minDistance = float.MaxValue;
            foreach (string tag in schoolController.preysTag)
            {
                GameObject[] preys = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject prey in preys)
                {
                    if (prey != exclude && (transform.position - prey.transform.position).magnitude < minDistance)
                    {
                        this.prey = prey;
                        minDistance = (transform.position - prey.transform.position).magnitude;
                    }
                }
            }
        }
        else if (method == 1) // Selecciona a la presa mas cercana dentro de un rango
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, preySearchingRange, schoolController.fishLayer);

            float minDistance = float.MaxValue;
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject != exclude && schoolController.preysTag.Contains(collider.tag) && (collider.transform.position - transform.position).magnitude < minDistance)
                {
                    prey = collider.gameObject;
                    minDistance = (prey.transform.position - transform.position).magnitude;
                }
            }
        }
    }

    public float GetSize()
    {
        // Devuelve el tamano del pez
        return sizeScale * schoolController.bodySize;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Comprueba que el objeto con el que colisiono es igual a la presa
        if (other.gameObject == prey)
        {
            // En base a una probabilidad, se define si la caza es exitosa o no
            if (Random.Range(0f, 1f) < schoolController.huntingSuccessRate)
            {
                // Se obtiene la referencia al FishController de la presa
                FishController prey = other.GetComponent<FishController>();

                // Se calcula el hambre recuperado por devorar al pez
                float hungerRecovery = prey.GetSize() / GetSize() * schoolController.maxHunger;

                // Se resta el hambre recuperado con un ruido aleatorio
                hunger -= hungerRecovery + hungerRecovery * UnityEngine.Random.Range(
                                             -schoolController.hungerRecoveryNoise,
                                              schoolController.hungerRecoveryNoise);

                // Si el hambre llega a cero, se deja de cazar
                if (hunger <= 0)
                {
                    hunger = 0;
                    searchingPrey = false;
                }

                // La referencia a la presa se iguala a null
                this.prey = null;

                // Se elimina a la presa
                prey.Dead(gameObject);

                // Se activa temporalmente el sistema de particulas de la sangre
                if (bloodParticle)
                {
                    CancelInvoke("StopParticle");
                    Invoke("StopParticle", 2f);
                    bloodParticle.Play();
                }
            }
            else // Si la caza no es exitosa
            {
                // Se busca otra presa
                FindPrey(other.gameObject);
            }
        }
    }

    public void SetInformation(FishInformation info)
    {
        // Se asignan la rapidez, la direccion y el rango de busqueda de presas
        speed = info.Speed;
        direction = info.Direction;
        preySearchingRange = info.PreySearchingRange;
    }

    void StopParticle()
    {
        if (bloodParticle)
            bloodParticle.Stop();
    }
}
