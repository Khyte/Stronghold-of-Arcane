using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manager of the game, where all scripts can get essential functions
/// </summary>
public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public AudioController AudioController;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		else
			Instance = this;

		DontDestroyOnLoad(gameObject);
	}

	/// <summary>
	/// Complete a scene loading with a loading screen
	/// </summary>
	/// <param name="sceneName">The scene to load</param>
	/// <param name="loadingScreen">The loading screen</param>
	/// <param name="loadingProgress">The image to show the progress of the loading</param>
	/// <returns></returns>
	public IEnumerator LoadLevel(string sceneName, GameObject loadingScreen, Image loadingProgress)
	{
		loadingScreen.SetActive(true);
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

		while (!asyncLoad.isDone)
		{
			loadingProgress.fillAmount = asyncLoad.progress;
			yield return null;
		}

		AudioController.StopMusic();
	}

	/// <summary>
	/// Remap a value in an other range of floats
	/// </summary>
	/// <param name="from">Value to remap</param>
	/// <param name="fromMin">Minimum of the value to remap</param>
	/// <param name="fromMax">Maximum of the value to remap</param>
	/// <param name="toMin">New minimum value</param>
	/// <param name="toMax">New maximum value</param>
	/// <returns>The new remaped value</returns>
	public float Remap(float from, float fromMin, float fromMax, float toMin, float toMax)
	{
		var fromAbs = from - fromMin;
		var fromMaxAbs = fromMax - fromMin;

		var normal = fromAbs / fromMaxAbs;

		var toMaxAbs = toMax - toMin;
		var toAbs = toMaxAbs * normal;

		var to = toAbs + toMin;

		return to;
	}

	/// <summary>
	/// Quit the game
	/// </summary>
	public void Quit()
	{
		Application.Quit();
	}
}