using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerBar : MonoBehaviour
{
    public static PowerBar Instance { get; private set; }

    [SerializeField] float _barSpeed = 2f;

    [SerializeField] float _powerBarFreezeTimer = 5f;

    [SerializeField] Image _powerBarMask;

    public bool BPowerBarFrozen;

    public float CurrentPower { get; private set; }

    float _currentPower;
    float _maxPower = 100f;

    bool _bPowerIncreasing;
    bool _bPowerBarOn;

    void Awake()
    {
        HandleSingleton();
    }

    void Start()
    {
        _currentPower = _maxPower;

        _powerBarMask = transform.GetChild(0).GetComponent<Image>();

        _bPowerIncreasing = false;
        _bPowerBarOn = true;
        BPowerBarFrozen = false;

        StartCoroutine(UpdatePowerBar());
    }

    void HandleSingleton()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public IEnumerator UpdatePowerBar()
    {
        while (_bPowerBarOn && !BPowerBarFrozen)
        {
            if (_bPowerIncreasing)
            {
                _currentPower += _barSpeed;
                if (_currentPower >= _maxPower)
                    _bPowerIncreasing = false;
            }
            else if (!_bPowerIncreasing)
            {
                _currentPower -= _barSpeed;
                if (_currentPower <= 0)
                    _bPowerIncreasing = true;
            }
            CurrentPower = _currentPower;
            float fill = _currentPower / _maxPower;
            _powerBarMask.fillAmount = fill;
            yield return new WaitForSeconds(.02f);
        }
        yield return null;
    }

    public void SetPowerBarEnabled(bool toggle)
    {
        Image[] imageComponents = GetComponentsInChildren<Image>();

        for (int i = 0; i < imageComponents.Length; i++)
            imageComponents[i].enabled = toggle;

        _bPowerBarOn = toggle;

        if (toggle == true)
        {
            BPowerBarFrozen = false;
            StartCoroutine(UpdatePowerBar());
        }
    }

    public void FreezePowerBar()
    {
        StartCoroutine(FreezePower());
    }

    IEnumerator FreezePower()
    {
        StopCoroutine(UpdatePowerBar());
        BPowerBarFrozen = true;

        yield return new WaitForSeconds(_powerBarFreezeTimer);

        SetPowerBarEnabled(true);
    }
}
