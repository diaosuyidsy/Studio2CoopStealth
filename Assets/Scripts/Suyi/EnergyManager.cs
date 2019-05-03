using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnergyManager : MonoBehaviour
{
	public static EnergyManager instance;
	public int MaxEnergy = 6;
	public GameObject EnergyPrefab;

	private Transform EnergyBar;
	public int _currentEnergy;
	private List<GameObject> Energies;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
		_currentEnergy = MaxEnergy;
		EnergyBar = transform.GetChild(0).GetChild(0);
		Energies = new List<GameObject>();
		for (int i = 0; i < _currentEnergy; i++)
		{
			Energies.Add(Instantiate(EnergyPrefab, EnergyBar));
		}
	}

	public bool CanUseEnergy(int _energy)
	{
		if (_currentEnergy - _energy < 0) return false;
		if (!EnergyBar.gameObject.activeSelf) EnergyBar.gameObject.SetActive(true);
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

}
