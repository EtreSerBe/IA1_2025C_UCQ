using System.Collections.Generic;
using UnityEngine;


// las structs son POD (Plain Old Data), se manejan como si fueran int, float, etc. y no como punteros internamente.
// En contraste, las class siempre se manejan como punteros.


public class Node
{
    public Node()
    {
        Parent = null;
    }
        
    public Node(int x, int y)
    {
        X = x;
        Y = y;
        
        // Hay que tener la certeza de que inicializa en null, para hacer el pathfinding bien.
        Parent = null; 
    }
    
    // Tienen información de algo relevante en un grafo... ¿pero qué?
    public int X { get; }

    public int Y { get; }

    // aristas que nos dicen a qué nodos puede visitar este nodo.
    // private List<Edge> edges = new List<Edge>();
    /*
     * public edge UpEdge = this, upNode;
     * public edge RightEdge = this, RightNode;
     * public edge DownEdge = this, downNode;
     * public edge LeftEdge = this, leftNode;
     * 

     * public Node UpNode;
     * public Node RightNode;
     * public Node DownNode;
     * public Node LeftNode;
     *
     * Ni siquiera necesitamos esos Node de arriba, abajo, derecha, izquierda, porque los índices en la cuadrícula
     * ya nos dicen cuáles serían dichos vecinos arriba (-1 en Y), abajo (+1 en Y), derecha (+1 en X), izquierda (-1 en X)
     */
    
    // Tiene una referencia al nodo Parent, es el que lo metió al árbol. (El que lo metió a la lista abierta).
    public Node Parent;
}

// No la vamos a usar en este caso, porque están implícitas las aristas en la estructura del grid.
// Si tuviéramos otro tipo de representación, como un grafo no-grid, ahí sí necesitaríamos tener aristas explícitas.
// public class Edge
// {
//     public Node A;
//     public Node B;
//     // podríamos poner que tiene un costo moverse entre ellos o algo.
//     // Pero ahorita lo voy a dejar así.
// }


public class Pathfinding : MonoBehaviour
{
    [SerializeField] private int height = 5;
    [SerializeField] private int width = 5;
    
    // Alguien que contenga todos los Nodos.
    // Esos nodos van a estar en forma de Grid/Cuadrícula, entonces podemos usar un array bidimensional.
    private Node[][] _grid;
    
    // Algoritmo para inicializar el grid de width*height
    void InitializeGrid()
    {
        _grid = new Node[height][];
        // Primero eje vertical, 
        for (int i = 0; i < height; i++)
        {
            _grid[i] = new Node[width];
            // después el eje horizontal
            for (int j = 0; j < width; j++)
            {
                _grid[j][i] = new Node(j, i);
            }
        }
    }

    /// <summary>
    /// Algoritmo que nos dice si hay un camino desde un Nodo origen hacia un Nodo objetivo.
    /// </summary>
    /// <returns></returns>
    private bool DepthFirstSearchRecursive(Node origin, Node goal)
    {
        // Los algoritmos de pathfinding construyen un árbol con la información del grafo.
        // Lo primero que tenemos que establecer es ¿Cuál es la raíz del árbol?
        // La raíz del árbol siempre va a ser el punto de origen del algoritmo.
        Node root = origin; // su padre tiene que ser null
        
        // si esto es recursivo, ¿Qué valor es el que vamos "avanzando"?
        // la meta no va a cambiar, entonces lo único que podemos avanzar es el origen
        
        // cuándo se detiene esta recursión? Hay dos:
        // A) cuando llegamos a la meta.
        if (origin == goal)
            return true;
        
        // B) cuando no hay camino.
        // DepthFirstSearchRecursive()

        // si se acabó la recursión y nunca llegaste a la meta, es que no hay camino.
        return false;
    }
    
    // recursivo VS iterativo
    // Recursivo es que se manda a llamar a sí mismo dentro del cuerpo de la función.
    public int RestaHastaCero(int value)
    {
        // stopping condition. La condición que hace que la recursión se detenga.
        if (value < 0)
            return value; // aquí ya no va a volver a mandar a llamarse, y entonces se acaba la recursión.
        
        Debug.Log($"Conteo regresivo: {value}");
        return RestaHastaCero(value - 1);
    }
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RestaHastaCero(10);
        // Iterativo:
        for (int i = 10; i < 0; i--)
        {
            Debug.Log($"Conteo regresivo: {i}");
        }
        
        // Todo lo que se puede hacer iterativo se puede hacer recursivo y viceversa.
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
