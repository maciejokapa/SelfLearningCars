using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network : MonoBehaviour {

    public int generation;                      //numer generacji
    public GameObject player;                   //prefab auta do postawienia
    private PlayerController playerController;  //odwołanie do skryptu
    private float[,] cars = new float[39,20];   //tablica zbiorcza do przechowywania w i b aktualnych samochodów
    //Inicjalizacja
    public void Start()
    {
        Init();
    }

    public int[] scores = new int[20];   //tablica do pzrechowywania fitnesow

    private void Update()
    {
        CheckIfCarsAreDead();
    }


    // postawienie pierwszych 10 samochodów
    public void Init () 
    {
        generation = 1;
        for (int i = 0; i < cars.GetLength(1); i++)
        {
            GameObject car = Instantiate(player, Vector3.zero, Quaternion.identity);  //postawienie samochodu
            playerController = car.GetComponent<PlayerController>();    //chwycenie odniesienia do skrytpu
            playerController.ID = i;                                    //nadanie ID -> zapisywanie wyniku i znajdowanie najlepszych

            for (int j = 0; j < cars.GetLength(0); j++)
            {
                cars[j, i] = Random.value * 2 - 1;                      //nadanie wartości w i b w tablicy zbiorczej

                if (j <= 29)
                    playerController.W[j] = cars[j, i];                 //nadanie wartości w w kontrolerze
                else
                    playerController.B[j - 30] = cars[j, i];            //nadanie wartości b w kontrolerze
            }
        }
    }


    float[] father = new float[39];   //tablice z w i b dwoch najlepszych
    float[] mother = new float[39];
    //jesli wszytskie samochody umarly znajdz najlepsze dwa w tablicy zbiorczej i stworz nowe pokolenie
    public void CheckIfCarsAreDead ()
    {
        if (FindObjectsOfType<PlayerController>().Length == 0)  //jesli nie ma zadnego kontrolera na scenie
        {
            father = SearchForWinner();
            mother = SearchForWinner();

            CreateNewGen();
            InstantiateNewGen();
        }
    }


    //znajdz pozycje najlepszego w tablicy zbiorczej 
    public float[] SearchForWinner ()
    {
        int bestScore = Mathf.Max(scores);     //zapamietanie najlepszego wyniku
        for (int i = 0; i < scores.Length; i++)  
        {
            if (bestScore==scores[i])          //odnalezienie pozycji najleszego wyniku
            {
               scores[i] = 0;                    //wyzerowanie wyniku zeby znalezc za chwile drugiego rodzica
               return  GetWinnerWeights(i);               
            }
        }
        return new float[26];
    }


    //zwroc w i b najlepszego z tablicy zbiorczej
    public float[] GetWinnerWeights (int i)
    {
        float[] parentWeights = new float[39];
        for (int j = 0; j < cars.GetLength(0); j++)
        {
            parentWeights[j] = cars[j, i];
        }
        return parentWeights;
    }


    //wygeneruj nowe pokolenie na podstawie dwoch najlepszych
    public void CreateNewGen ()
    {
        
        for (int i = 0; i < cars.GetLength(0); i++)  //pierwsze dwa samochody takie jak najlepsze
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


    //stworz dzieci jako mieszanka rodzicow
    public void CrossingOver()
    {             
        for (int i = 2; i < cars.GetLength(1); i++)  //indeks 0 i 1 zajete przez rodzicow
        {
            for (int j = 0; j < cars.GetLength(0); j++)
            {
                float dice = Random.value;
                if (dice>0.5f)                    //wartosc od jednego albo drugiego rodzica losowo
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

    //zmutuj wartosci dzieci
    public  void Mutate ()      //indeks 0 i 1 zajete przez rodzicow a 2, 3, 4, 5 przez tych po crossingover bez mutacji
    {          
        for (int i = 6; i < 10; i++)  //w i b czterech dzieci zmienione o 10%
        {
            for (int j = 0; j < cars.GetLength(0); j++)
            {
                if (Random.value > 0.5f)
                    cars[j, i] *= 1.1f;
                else
                    cars[j, i] *= -1.1f;
            }
        }
        for (int i = 10; i < 14; i++)  //w i b czterech dzieci zmienione o 50%
        {
            for (int j = 0; j < cars.GetLength(0); j++)
            {
                if (Random.value > 0.5f)
                    cars[j, i] *= 1.5f;
                else
                    cars[j, i] *= -1.5f;
            }
        }
        for (int i = 14; i < cars.GetLength(1); i++)  //w i b pozostałych przypadkowe
        {
            for (int j = 0; j < cars.GetLength(0); j++)
            {
                
                    cars[j, i] *= Random.Range(-5.0f,5.0f);
            }
        }
    }


    //postaw nastepne pokolenie
    public void InstantiateNewGen ()
    {
        generation++;
        for (int i = 0; i < cars.GetLength(1); i++)
        {
            GameObject car = Instantiate(player, Vector3.zero, Quaternion.identity);  //postawienie samochodu
            playerController = car.GetComponent<PlayerController>();    //chwycenie odniesienia do skrytpu
            playerController.ID = i;                                    //nadanie ID -> zapisywanie wyniku i znajdowanie najlepszych

            for (int j = 0; j < cars.GetLength(0); j++)
            {
                if (j <= 29)
                    playerController.W[j] = cars[j, i];                 //nadanie wartości w w kontrolerze
                else
                    playerController.B[j - 30] = cars[j, i];            //nadanie wartości b w kontrolerze
            }
        }
    }
}
