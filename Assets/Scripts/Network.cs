using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network : MonoBehaviour {

    public int generation;                      
    public GameObject player;                   
    private PlayerController playerController;  
    private float[,] cars = new float[39,20];  
    //Inicjalizacja
    public void Start()
    {
        Init();
    }

    public int[] scores = new int[20];   

    private void Update()
    {
        CheckIfCarsAreDead();
    }


    
    public void Init () 
    {
        generation = 1;
        for (int i = 0; i < cars.GetLength(1); i++)
        {
            GameObject car = Instantiate(player, Vector3.zero, Quaternion.identity);  
            playerController = car.GetComponent<PlayerController>();    
            playerController.ID = i;                                    

            for (int j = 0; j < cars.GetLength(0); j++)
            {
                cars[j, i] = Random.value * 2 - 1;                      

                if (j <= 29)
                    playerController.W[j] = cars[j, i];                 
                else
                    playerController.B[j - 30] = cars[j, i];            
            }
        }
    }


    float[] father = new float[39];   
    float[] mother = new float[39];
  
    public void CheckIfCarsAreDead ()
    {
        if (FindObjectsOfType<PlayerController>().Length == 0)
        {
            father = SearchForWinner();
            mother = SearchForWinner();

            CreateNewGen();
            InstantiateNewGen();
        }
    }


    public float[] SearchForWinner ()
    {
        int bestScore = Mathf.Max(scores); 
        for (int i = 0; i < scores.Length; i++)  
        {
            if (bestScore==scores[i])         
            {
               scores[i] = 0;                  
               return  GetWinnerWeights(i);               
            }
        }
        return new float[26];
    }


    public float[] GetWinnerWeights (int i)
    {
        float[] parentWeights = new float[39];
        for (int j = 0; j < cars.GetLength(0); j++)
        {
            parentWeights[j] = cars[j, i];
        }
        return parentWeights;
    }


    public void CreateNewGen ()
    {
        
        for (int i = 0; i < cars.GetLength(0); i++)  
        {
            cars[i, 0] = father[i];
        }
        for (int i = 0; i < cars.GetLength(0); i++)
        {
            cars[i, 1] = mother[i];
        }
        
        CrossingOver();
        Mutate();
    }


    public void CrossingOver()
    {             
        for (int i = 2; i < cars.GetLength(1); i++)  /
        {
            for (int j = 0; j < cars.GetLength(0); j++)
            {
                float dice = Random.value;
                if (dice>0.5f)                  
                {
                    cars[j, i] = father[j];
                }
                else
                {
                    cars[j, i] = mother[j];
                }
            }
        }
    }


    public  void Mutate ()   
    {          
        for (int i = 6; i < 10; i++)
        {
            for (int j = 0; j < cars.GetLength(0); j++)
            {
                if (Random.value > 0.5f)
                    cars[j, i] *= 1.1f;
                else
                    cars[j, i] *= -1.1f;
            }
        }
        for (int i = 10; i < 14; i++) 
        {
            for (int j = 0; j < cars.GetLength(0); j++)
            {
                if (Random.value > 0.5f)
                    cars[j, i] *= 1.5f;
                else
                    cars[j, i] *= -1.5f;
            }
        }
        for (int i = 14; i < cars.GetLength(1); i++)  
        {
            for (int j = 0; j < cars.GetLength(0); j++)
            {
                
                    cars[j, i] *= Random.Range(-5.0f,5.0f);
            }
        }
    }


    public void InstantiateNewGen ()
    {
        generation++;
        for (int i = 0; i < cars.GetLength(1); i++)
        {
            GameObject car = Instantiate(player, Vector3.zero, Quaternion.identity);  
            playerController = car.GetComponent<PlayerController>();    
            playerController.ID = i;                                    

            for (int j = 0; j < cars.GetLength(0); j++)
            {
                if (j <= 29)
                    playerController.W[j] = cars[j, i];                
                else
                    playerController.B[j - 30] = cars[j, i];      
            }
        }
    }
}
