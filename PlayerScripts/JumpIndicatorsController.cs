using System.Collections.Generic;
using UnityEngine;

public class JumpIndicatorsController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    [SerializeField] private GameObject angleIndicator1;
    [SerializeField] private GameObject angleIndicator2;
    [SerializeField] private GameObject powerIndicator1;
    [SerializeField] private GameObject powerIndicator2;

    [SerializeField] private float maxAngleRotationZ = 0f;
    [SerializeField] private float minAngleRotationZ = -76f;

    [SerializeField] private float maxPowerPositionY = 1.4f;
    [SerializeField] private float minPowerPositionY = 0f;

    [SerializeField] private float angleChoosingSpeed = 1f;
    [SerializeField] private float powerChoosingSpeed = 1f;

    private enum State
    {
        Hidden,
        ChoosingAngle,
        ChoosingPower
    }

    private State state = State.Hidden;


    private float angleValue = 0f;
    private float powerValue = 0f;
    private bool changeUp = false;


    private void Update()
    {
        if (state == State.Hidden) return;
        else if (state == State.ChoosingAngle)
        {
            float angleChange = angleChoosingSpeed * Time.deltaTime;
            if (changeUp)
            {
                angleValue += angleChange;
                if (angleValue > 1.0f)
                {
                    angleValue = 1.0f;
                    changeUp = false;
                }
            }
            else
            {
                angleValue -= angleChange;
                if (angleValue < 0.0f)
                {
                    angleValue = 0.0f;
                    changeUp = true;
                }
            }
            Quaternion indicatorRotation;
            if (playerTransform.localScale.x > .8f)
            {
                indicatorRotation = Quaternion.Euler(0, 0, minAngleRotationZ + (angleValue * (maxAngleRotationZ - minAngleRotationZ)) + playerTransform.eulerAngles.z);
            }
            else
            {
                indicatorRotation = Quaternion.Euler(0, 0, -minAngleRotationZ - (angleValue * (maxAngleRotationZ - minAngleRotationZ)) + playerTransform.eulerAngles.z);
            }
            angleIndicator2.GetComponent<RectTransform>().rotation = indicatorRotation;
        }
        else if (state == State.ChoosingPower)
        {
            float powerChange = powerChoosingSpeed * Time.deltaTime;
            if (changeUp)
            {
                powerValue += powerChange;
                if (powerValue > 1.0f)
                {
                    powerValue = 1.0f;
                    changeUp = false;
                }
            }
            else
            {
                powerValue -= powerChange;
                if (powerValue < 0.0f)
                {
                    powerValue = 0.0f;
                    changeUp = true;
                }
            }
            var indicatorPosition = new Vector3(0, minPowerPositionY + (powerValue * (maxPowerPositionY - minPowerPositionY)), 0);
            powerIndicator2.GetComponent<RectTransform>().anchoredPosition = indicatorPosition;
        }
    }


    private void HideIndicators()
    {
        angleIndicator1.SetActive(false);
        angleIndicator2.SetActive(false);

        powerIndicator1.SetActive(false);
        powerIndicator2.SetActive(false);
    }

    private void ShowAngleIndicators()
    {
        angleIndicator1.SetActive(true);
        angleIndicator2.SetActive(true);
    }

    private void ShowPowerIndicators()
    {
        powerIndicator1.SetActive(true);
        powerIndicator2.SetActive(true);
    }



    public bool ActivateAndReturnTrueIfFinishedPicking()
    {
        if (state == State.Hidden)
        {
            angleValue = Random.value;
            changeUp = Random.Range(0, 2) == 0;
            ShowAngleIndicators();
            state = State.ChoosingAngle;
            return false;
        }
        else if (state == State.ChoosingAngle)
        {
            powerValue = Random.value;
            changeUp = Random.Range(0, 2) == 0;
            ShowPowerIndicators();
            state = State.ChoosingPower;
            return false;
        }
        else
        {
            HideIndicators();
            state = State.Hidden;
            return true;
        }
    }

    public void CancelPicking()
    {
        HideIndicators();
        state = State.Hidden;
    }

    public Dictionary<string, float> GetAngleAndPowerValues()
    {
        return new Dictionary<string, float> { { "angle", angleValue }, { "power", powerValue } };
    }
}
