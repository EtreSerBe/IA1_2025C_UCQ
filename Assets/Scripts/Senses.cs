using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Lo vamos a usar para poner todo lo que tiene que ver con detección, visión, y si da tiempo, tacto y oído.
/// </summary>
public class Senses : MonoBehaviour
{

    // Opciones para remplazar valores hardcodeados:
    // 1) hacer una variable que se puede cambiar desde el editor.
    public float radioDeDeteccion = 20.0f;
    // 1.A) hacer una variable no-pública que se puede cambiar desde el editor
    [SerializeField] private float radioDeDeteccionPrivado = 2.0f;

    // 2) variable pública estática.
    // La variable static solo una vez se le puede asignar valor y después ya nunca puede cambiar.
    public static float RadioDeDeteccionStatic = 20.0f;
    
    // 2.A) variable const
    // es muy parecida a la static, pero NO se le puede asignar un valor ya en la ejecución.
    public const float RADIO_DE_DETECCION_CONST = 20.0f;
    
    // 3) Scriptable Objects
    // es un tipo de clase especial que sirve principalmente para guardar datos, pero también puede tener funciones.
    // Solo se instancía una vez, y todos los que referencíen a ese scriptableObject pueden acceder a esa única instancia.
    // Ayuda muchísimo a reducir el uso de memoria cuando A) se va a remplazar muchos datos de una clase y
    // B) cuando va a haber muchos que usen esos datos
    
    // 4) Un archivo de configuración.
    
    
    // Lista de GameObjects encontrados este frame
    private GameObject[] _foundGameObjects ;
    public GameObject[] foundGameObjects => _foundGameObjects;

    
    public static Vector3 PuntaMenosCola(Vector3 punta, Vector3 cola)
    {
        float x = punta.x - cola.x;
        float y = punta.y - cola.y;
        float z = punta.z - cola.z;
        return new Vector3(x, y, z);
        
        // Internamente, esta línea hace lo que las 4 líneas de arriba harían.
        // return punta - cola;
    }

    public static float Pitagoras(Vector3 vector3)
    {
        // hipotenusa = raíz cuadrada de a^2 + b^2 + c^2
        float hipotenusa = math.sqrt(vector3.x * vector3.x +
                                     vector3.y * vector3.y +
                                     vector3.z * vector3.z);
        return hipotenusa;

        // return vector3.magnitude;
    }
        
    
    // Vamos a detectar cosas que estén en un radio determinado.
    void DetectarTodosLosGameObjects()
    {
        // Esta obtiene TODOS los gameObjects en la escena.
         _foundGameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

         // Después los filtramos para que solo nos dé los que sí están dentro del radio determinado.
         foreach (var foundGameObject in _foundGameObjects)
         {
             // Primero hacemos punta menos cola entre la posición de este GameObject y la del foundGameObject,
             // esto nos da la flecha que va del uno al otro,
             Vector3 puntaMenosCola = PuntaMenosCola(foundGameObject.transform.position, gameObject.transform.position);

             // Y ya con esa flecha, usamos el teorema de Pitágoras, para calcular la distancia entre este gameObject
             // que es dueño de este script Senses y el foundGameObject.
             float distancia = Pitagoras(puntaMenosCola);

             // ya con la distancia calculada, la comparamos contra este radio que determinamos.
             if (distancia < radioDeDeteccion)
             {
                 // Sí está dentro del radio
                 Debug.Log($"El objeto: {foundGameObject.name} sí está dentro del radio.");
             }
             else
             {
                 // no está dentro del radio.
                 Debug.Log($"El objeto: {foundGameObject.name} No está dentro del radio.");
             }
         }
    }

    public List<GameObject> GetPlayers()
    {
        List<GameObject> players = new List<GameObject>();
        foreach (var foundObject in _foundGameObjects)
        {
            if (foundObject.tag != "Player")
                continue; // continue es: vete a la siguiente iteración del ciclo en donde estás.
                // break; // break es: salte del ciclo donde estés.
            
            // Si sí es un player, le digo que lo meta en la lista
            players.Add(foundObject);
        }

        return players;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DetectarTodosLosGameObjects();
    }

    // Update is called once per frame
    void Update()
    {
        DetectarTodosLosGameObjects();
    }

    // OnDrawGizmos Se manda a llamar cada que la pestaña de escena se actualiza. Se actualiza incluso cuando no estás en play mode.
    // OnDrawGizmosSelected hace lo mismo, pero solo cuando el gameObject con este script esté seleccionado en la escena.
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radioDeDeteccion);

        if (Application.isPlaying)
        {
            // Después los filtramos para que solo nos dé los que sí están dentro del radio determinado.
            foreach (var foundGameObject in _foundGameObjects)
            {
                // Primero hacemos punta menos cola entre la posición de este GameObject y la del foundGameObject,
                // esto nos da la flecha que va del uno al otro,
                Vector3 puntaMenosCola = PuntaMenosCola(foundGameObject.transform.position, gameObject.transform.position);

                // Y ya con esa flecha, usamos el teorema de Pitágoras, para calcular la distancia entre este gameObject
                // que es dueño de este script Senses y el foundGameObject.
                float distancia = Pitagoras(puntaMenosCola);

                // ya con la distancia calculada, la comparamos contra este radio que determinamos.
                if (distancia < radioDeDeteccion)
                {
                    Gizmos.color = Color.green;
                    // Sí está dentro del radio
                    Gizmos.DrawWireCube(foundGameObject.transform.position, Vector3.one);
                }
                else
                {
                    Gizmos.color = Color.red;
                    // no está dentro del radio.
                    Gizmos.DrawWireCube(foundGameObject.transform.position, Vector3.one);
                }
            }
        }
    }
}

/*
 * Un valor hardcodeado (hardcoded) es un valor alfanumérico que está en el código, pero se necesita para ajustar el
 * funcionamiento de las cosas.
 *
 * El problema de los valores hardcodeados se vuelve más grande entre más veces se utilice dicho valor.
 * 
 */