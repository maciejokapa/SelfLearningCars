using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Range(-1, 1)]
    public float rotate;                       //output  (rotacja)
    [Range(0, 1)]
    public float speed;                        //output  (predkosc)

    private float d1;                           //input  (odleglosci)
    private float d2;                           //input
    private float d3;                           //input


    private Network network;    //odwolanie do skrytpu sieci
    private void Start()
    {
        network = FindObjectOfType<Network>();
    }


    void Update()
    {
        Move();
        Draw();
        ComputeOutput();
    }


    public float[] W = new float[30];  //tablica wspolczynnikow
    public float[] B = new float[9];   //tablica przesuniec
    //obliczenie wartosci wyjsciowych na podstawie wspolczynikow
    public void ComputeOutput ()
    {       
        float x1 = Tanh(Mathf.Clamp((d1 * W[0] + d2 * W[4] + d3 * W[8] + B[0]), -80, 80));
        float x2 = Tanh(Mathf.Clamp((d1 * W[1] + d2 * W[5] + d3 * W[9] + B[1]), -80, 80));
        float x3 = Tanh(Mathf.Clamp((d1 * W[2] + d2 * W[6] + d3 * W[10] + B[2]), -80, 80));
        float x4 = Tanh(Mathf.Clamp((d1 * W[3] + d2 * W[7] + d3 * W[11] + B[3]), -80, 80));
        float x5 = Tanh(Mathf.Clamp((x1 * W[12] + x2 * W[15] + x3 * W[18] + x4 * W[21] + B[4]), -80, 80));
        float x6 = Tanh(Mathf.Clamp((x1 * W[13] + x2 * W[16] + x3 * W[19] + x4 * W[22] + B[5]), -80, 80));
        float x7 = Tanh(Mathf.Clamp((x1 * W[14] + x2 * W[17] + x3 * W[20] + x4 * W[23] + B[6]), -80, 80));

        float y1 = Tanh(Mathf.Clamp((x5 * W[24] + x6 * W[26] + x7 * W[28] + B[7]), -80, 80));
        float y2 = Tanh(Mathf.Clamp((x5 * W[25] + x6 * W[27] + x7 * W[29] + B[8]), -80, 80));

        speed = Sigm(Mathf.Clamp(y1, -80, 80));
        rotate = Tanh(Mathf.Clamp(y2, -80, 80));    
    }

    
    //funkcje aktywacyjne (tangens hiperboliczny i funkcja sigmoidalna)
    public float Tanh (float x)
    {
        float y = (Mathf.Exp(x) - Mathf.Exp(-x)) / (Mathf.Exp(x) + Mathf.Exp(-x));
        return y;
    }
    public float Sigm (float x)
    {
        float y = 1 / (1 + Mathf.Exp(-x));
        return y;
    }

    public int ID;      //numer nadany przez skrypt Network  (wykrywanie najlepszych)
    public int score;   //fitness score
    //wykrywanie kolizji
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))  //smierć i przeslanie fitnesu do sieci
        {
            Destroy(gameObject);
            network.scores[ID] = score;
        }

        if (collision.CompareTag("Checkpoint"))  //wzrost fitnesu
        { 
            score++;
        }
    }


    public float rotationFactor = 50;  //wspolczynniki maksymalne skretu i predkosci
    public float speedFactor = 20;
    public float timeLimit = 60;
    public float timeElapsed = 0;
    //poruszanie samochodu na podstawie obliczonych wartosci predkosci i rotacji
    public void Move()
    {
        transform.Rotate(0.0f, 0.0f, -rotate * rotationFactor * Time.deltaTime);

        Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.up);
        Vector3 movement = localForward.normalized;
        transform.Translate(movement * speed *speedFactor * Time.deltaTime);

        timeElapsed += Time.deltaTime;
        if (timeElapsed>timeLimit)
        {
            Destroy(gameObject);
            network.scores[ID] = score;
        }
    }


    public Transform leftRayOrigin;    //pozycje do stworzenia "promienia"
    public Transform leftRayDir;
    public Transform frontRayOrigin;
    public Transform frontRayDir;
    public Transform rightRayOrigin;
    public Transform rightRayDir;

    public LayerMask whatIsWall;    //maska do wykrywania kolizji tylko ze scianami
    //obliczanie wartosci wejsciowych (odleglosci) i rysowanie ich
    public void Draw()
    {
        RaycastHit2D leftRay = Physics2D.Raycast(leftRayOrigin.position, leftRayDir.position - leftRayOrigin.position,200,whatIsWall);
        if (leftRay.collider != null)
        {
            if (leftRay.collider.CompareTag("Wall"))
            {
                d1 = leftRay.distance;
                Vector3 drawLeft = leftRayDir.position - leftRayOrigin.position;
                drawLeft = drawLeft.normalized;
                drawLeft = drawLeft * leftRay.distance;
                Debug.DrawRay(leftRayOrigin.position, drawLeft, Color.yellow);
                //Debug.Log(d1);
            }
        }

        RaycastHit2D frontRay = Physics2D.Raycast(frontRayOrigin.position, frontRayDir.position - frontRayOrigin.position,200,whatIsWall);
        if (frontRay.collider != null)
        {
            if (frontRay.collider.CompareTag("Wall"))
            {
                d2 = frontRay.distance;
                Vector3 drawFront = frontRayDir.position - frontRayOrigin.position;
                drawFront = drawFront.normalized;
                drawFront = drawFront * frontRay.distance;
                Debug.DrawRay(frontRayOrigin.position, drawFront, Color.yellow);
                //Debug.Log(d2);
            }
        }

        RaycastHit2D rightRay = Physics2D.Raycast(rightRayOrigin.position, rightRayDir.position - rightRayOrigin.position,200,whatIsWall);
        if (rightRay.collider != null)
        {
            if (rightRay.collider.CompareTag("Wall"))
            {
                d3 = rightRay.distance;
                Vector3 drawRight = rightRayDir.position - rightRayOrigin.position;
                drawRight = drawRight.normalized;
                drawRight = drawRight * rightRay.distance;
                Debug.DrawRay(rightRayOrigin.position, drawRight, Color.yellow);
                //Debug.Log(d3);
            }
        }
    }
}
