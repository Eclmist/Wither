using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

public static class LevelLoader {

    
    public static void LoadNextLevel()
    {
        EditorSceneManager.LoadScene(EditorSceneManager.GetActiveScene().buildIndex + 1, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public static void ReloadCurrentLevel()
    {
        EditorSceneManager.LoadScene(EditorSceneManager.GetActiveScene().buildIndex, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public static void LoadLevel(int index)
    {
        EditorSceneManager.LoadScene(index, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }


}
