using UnityEngine;

public class NumberGen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static int Generate(int inp)
    {
        int numBits = (inp * 8) / (inp + 5);
        int leftBit = (inp * 8) / (inp + 2);

        int outValue = 0;

        for (int i = 0; i < numBits; i++)
        {
            int randomBit = UnityEngine.Random.Range(0, numBits + 1);
            outValue |= 1 << randomBit;
        }

        return outValue;
    }

}
