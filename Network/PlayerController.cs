using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private GameObject cameraControllerPrefab;

    [SerializeField] private FrogVisualController frogVisualController;
    [SerializeField] private JumpIndicatorsController jumpIndicatorsController;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [SerializeField] private PolygonCollider2D sittingPolygonCollider;
    [SerializeField] private PolygonCollider2D flyingPolygonCollider;

    [SerializeField] private float maxJumpAngle = 90f;
    [SerializeField] private float minJumpAngle = 5f;
    [SerializeField] private float maxJumpPower = 1.0f;
    [SerializeField] private float minJumpPower = 0.1f;

    private CameraController cameraController;

    private bool masterControlsEnabled = false;
    private bool controlsEnabled = false;
    private bool isTurnedToTheRight = true;
    private bool justStartedJump = false;

    [Rpc(SendTo.Owner)]
    public void SetupPlayerControllerRpc()
    {
        cameraController = Instantiate(cameraControllerPrefab).GetComponent<CameraController>();
        cameraController.SetFollowedTransform(transform);

        Camera.main.gameObject.SetActive(false);

        var raceUIController = GameObject.Find("Canvas").GetComponent<RaceUIController>();

        raceUIController.OnJumpButtonPressed += RaceUIController_OnJumpButtonPressed;
        raceUIController.OnSwitchSideButtonPressed += RaceUIController_OnSwitchSideButtonPressed;
        raceUIController.OnZoomButtonPressed += RaceUIController_OnZoomButtonPressed;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetFrogColorAndDeactivateScriptIfNotOwnedRpc()
    {
        frogVisualController.SetFrogColor(LobbyManager.Instance.GetFrogColor(OwnerClientId));
        if (!IsServer && !IsOwner) this.enabled = false;
    }

    [Rpc(SendTo.Server)]
    private void SendFrogFlyingRpc(float angleValue, float powerValue)
    {
        if (!controlsEnabled) 
        {
            MakeControlsRpc(false);
            return;
        }
        controlsEnabled = false;
        MakeControlsRpc(false);
        float angle = minJumpAngle + (angleValue * (maxJumpAngle - minJumpAngle));
        float power = minJumpPower + (powerValue * (maxJumpPower - minJumpPower));

        Debug.Log("SendFrogFlying na serwerze. angle = " + angle.ToString() + ", power = " + power.ToString());


        if (isTurnedToTheRight)
        {
            var rot = Quaternion.Euler(0, 0, angle);
            rb.AddForce(rot * (transform.right * power));
        }
        else
        {
            var rot = Quaternion.Euler(0, 0, -angle);
            rb.AddForce(rot * (-transform.right * power));
        }

        animator.SetBool("IsFlying", true);
        sittingPolygonCollider.gameObject.SetActive(false);
        flyingPolygonCollider.gameObject.SetActive(true);
        justStartedJump = true;
        StartCoroutine(SwitchJustStartedJumpToFalseAfter(1f));
    }

    [Rpc(SendTo.Server)]
    private void SwitchSideRpc()
    {
        if (!controlsEnabled)
        {
            MakeControlsRpc(false);
            return;
        }

        if (isTurnedToTheRight)
        {
            transform.localScale = new Vector3(-1f, 1, 1);
            isTurnedToTheRight = false;
        }
        else
        {
            transform.localScale = Vector3.one;
            isTurnedToTheRight = true;
        }
    }


    [Rpc(SendTo.Owner)]
    private void MakeControlsRpc(bool active) 
    { 
        controlsEnabled = active;
        if (!controlsEnabled) jumpIndicatorsController.CancelPicking();
    }

    [Rpc(SendTo.Server)]
    public void MakeMasterControlsRpc(bool active)
    {
        masterControlsEnabled = active;
        controlsEnabled = active;
        MakeControlsRpc(active);
    }





    private void RaceUIController_OnZoomButtonPressed(object sender, System.EventArgs e)
    {
        cameraController.ChangeZoom();
    }

    private void RaceUIController_OnSwitchSideButtonPressed(object sender, System.EventArgs e)
    {
        if (!controlsEnabled) return;

        SwitchSideRpc();


    }

    private void RaceUIController_OnJumpButtonPressed(object sender, System.EventArgs e)
    {
        if (!controlsEnabled) return;

        if (jumpIndicatorsController.ActivateAndReturnTrueIfFinishedPicking())
        {
            var valuesDict = jumpIndicatorsController.GetAngleAndPowerValues();
            float angleValue = valuesDict["angle"];
            float powerValue = valuesDict["power"];

            Debug.Log("OnJumpButtonPressed zaraz przed wys³aniem SendFrogFlyingRpc. angleValue = " + angleValue.ToString() + ", powerValue = " + powerValue.ToString());
            SendFrogFlyingRpc(angleValue, powerValue);
        }
    }




    private void Update()
    {
        if (!IsServer || !masterControlsEnabled) return;

        if (!controlsEnabled)
        {
            if (!justStartedJump && rb.linearVelocity == Vector2.zero)
            {
                animator.SetBool("IsFlying", false);
                sittingPolygonCollider.gameObject.SetActive(true);
                flyingPolygonCollider.gameObject.SetActive(false);
                if (transform.rotation.eulerAngles.z > 180)
                {
                    if ((transform.rotation.eulerAngles.z % 180) - 180 < -80)
                    {
                        transform.rotation = Quaternion.identity;
                    }
                }
                else if (transform.rotation.eulerAngles.z > 80)
                {
                    transform.rotation = Quaternion.identity;
                }


                controlsEnabled = true;
                MakeControlsRpc(true);
            }
        }
        else
        {
            if (rb.linearVelocity != Vector2.zero)
            {
                controlsEnabled = false;
                MakeControlsRpc(false);
            }
        }
    }










    private IEnumerator SwitchJustStartedJumpToFalseAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        justStartedJump = false;
    }
}
