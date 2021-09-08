using System.Collections;
using System.Collections.Generic;


public static class Utility
{
    //Funtion return an array
    public static T[] ShuffleArray<T>(T[] array, int seed) //Take in un-shuffled array and a seed to make the array always shuffle in the same order
    {
        System.Random prng = new System.Random(seed); //psedo random number generator

        for (int i = 0; i < array.Length - 1; i++) //Ignore the last iteration of the loop
        {
            int randomIndex = prng.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }
        return array;
    }
}
