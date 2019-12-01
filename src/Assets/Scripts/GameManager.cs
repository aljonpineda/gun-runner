using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static bool isInputEnabled = true;

    public static void EnableInputs()
    {
        isInputEnabled = true;
    }

    public static void DisableInputs()
    {
        isInputEnabled = false;
    }

    public static bool InputEnabled()
    {
        return isInputEnabled;
    }
}
