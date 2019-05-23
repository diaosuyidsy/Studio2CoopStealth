using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class EnergyManager : MonoBehaviour
{
	public static EnergyManager instance;
	public int MaxEnergy = 3;
	public GameObject EnergyPrefab;

	private Transform EnergyBar;
	public int _currentEnergy;
	private List<GameObject> Energies;
	public RectTransform Kuang;
	public DOTweenAnimation Mask;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		/*else if (instance != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);*/
		_currentEnergy = MaxEnergy;
		EnergyBar = transform.GetChild(0).GetChild(0);
		Energies = new List<GameObject>();
		for (int i = 0; i < MaxEnergy; i++)
		{
			Energies.Add(Instantiate(EnergyPrefab, EnergyBar));
		}
	}

	public bool CanUseEnergy(int _energy)
	{
		if (_currentEnergy - _energy < 0) return false;
		if (!EnergyBar.gameObject.activeSelf) EnergyBar.gameObject.SetActive(true);
		Mask.DOPlayForward();
		_useEnergy(_energy);
		return true;
	}

	private void _useEnergy(int _energy)
	{
		int tempCur = _currentEnergy;
		_currentEnergy -= _energy;
		if (_currentEnergy <= 0) _currentEnergy = 0;
		if (_currentEnergy > MaxEnergy) _currentEnergy = MaxEnergy;
		if (tempCur > _currentEnergy)
		{
			for (int i = tempCur - 1; i > _currentEnergy - 1; i--)
			{
				Energies[i].GetComponentInChildren<DOTweenAnimation>().DOPlayForward();
			}
		}
		else
		{
			for (int i = tempCur; i < _currentEnergy; i++)
			{
				Energies[i].GetComponentInChildren<DOTweenAnimation>().DOPlayBackwards();
			}
		}
	}

	public void AddMaxEnergy(int _energyAmt)
	{
		// Restore all current energy
		_useEnergy(_currentEnergy - MaxEnergy);
		// Change the size of maxEnergy
		for (int i = 0; i < _energyAmt; i++)
		{
			Energies.Add(Instantiate(EnergyPrefab, EnergyBar));
		}
		MaxEnergy += _energyAmt;
		_currentEnergy = MaxEnergy;
		// Change the size of the Frame
		Kuang.sizeDelta = new Vector2(90f + 30f * MaxEnergy, Kuang.sizeDelta.y);
	}

	private void OnEnable()
	{
		EventManager.StartListening("PlayerDied", () =>
		{
			AddMaxEnergy(0);
		});
	}

	private void OnDisable()
	{
		EventManager.StopListening("PlayerDied", () =>
		{
			AddMaxEnergy(0);
		});
	}
}
