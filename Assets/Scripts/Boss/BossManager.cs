using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    Transform head;
    Transform rotto;

    Transform[] layers = new Transform[4];
    //True means open, as in core exposed
    public bool[] layerStates { get; private set; } = new bool[4];

    BossButtonController[] buttons = new BossButtonController[4];

    [SerializeField]
    float headTurnRate;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            switch (child.name)
            {
                case "Head":
                    head = child;
                    break;
                case "Layer0":
                    layers[0] = child;
                    break;
                case "Layer1":
                    layers[1] = child;
                    break;
                case "Layer2":
                    layers[2] = child;
                    break;
                case "Layer3":
                    layers[3] = child;
                    break;
                default:
                    break;
            }
        }

        for (int i = 0; i < 4; ++i)
            buttons[i] = GameObject.Find("Button " + i.ToString()).GetComponent<BossButtonController>();

        for (int i = 0; i < layerStates.Length; ++i)
            layerStates[i] = false;
        layers[0].localRotation = Quaternion.Euler(0, 45, 0);
        layers[1].localRotation = Quaternion.Euler(0, -45, 0);
        layers[2].localRotation = Quaternion.Euler(0, 45, 0);
        layers[3].localRotation = Quaternion.Euler(0, -45, 0);
        rotto = GameObject.Find("Rotto").GetComponent<Transform>();
    }

    public void TurnHead()
    {
        Vector3 directionVector = head.position - rotto.position;
        directionVector.y = 0;
        float angleBetween = Vector3.SignedAngle(Vector3.forward, directionVector, Vector3.up);
        head.rotation = Quaternion.Slerp(head.rotation, Quaternion.Euler(0, angleBetween, 0), headTurnRate);
    }

    public IEnumerator TurnLayer(int layerNumber, float turnRate)
    {
        //if layer0 or 2, then it closes to the left.
        bool closedLeft = ((layerNumber == 0 || layerNumber == 2) ? true : false);

        layerStates[layerNumber] = !layerStates[layerNumber];
        bool open = layerStates[layerNumber];

        Transform layer = layers[layerNumber];
        //Overaiming by 5 degrees so the Slerp hit it faster
        float goalAngle = open ? 0 : (closedLeft ? 45 : -45);
        while (true)
        {
            float currentAngle = layer.rotation.eulerAngles.y;
            if (currentAngle > 180)
                currentAngle -= 360;

            if (open && closedLeft && currentAngle < goalAngle)
                break;
            else if (!open && closedLeft && currentAngle > goalAngle)
                break;
            else if (open && !closedLeft && currentAngle > goalAngle)
                break;
            else if (!open && !closedLeft && currentAngle < goalAngle)
                break;
            layer.rotation = Quaternion.Slerp(layer.rotation, Quaternion.Euler(0, goalAngle + 5 * (open ? -1 : 1) * (closedLeft ? 1 : -1), 0), turnRate);
            yield return new WaitForFixedUpdate();
        }
        layer.rotation = Quaternion.Euler(0, goalAngle, 0);
    }

    public void ResetButton(int button)
    {

    }

    public void DeployEnemy(GameObject clawPrefab, GameObject enemyPrefab, Vector3 position)
    {
        float verticalDisplacement = 10f;
        //This might need to be changed for each enemy type
        float clawDisplacement = 1.8f;
        Transform pivot = Instantiate(new GameObject(), position + Vector3.up * verticalDisplacement, Quaternion.identity).GetComponent<Transform>();
        Transform claw = Instantiate(clawPrefab, position + Vector3.up * (verticalDisplacement + clawDisplacement), Quaternion.Euler(0, 90, 90), pivot).GetComponent<Transform>();
        claw.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        claw.name = "Arm";
        Transform enemy = Instantiate(enemyPrefab, position + Vector3.up * verticalDisplacement, Quaternion.identity, pivot).GetComponent<Transform>();
        enemy.name = enemyPrefab.name;
        enemy.GetComponent<BaseEnemy>().SetTarget(null);
        StartCoroutine(claw.GetComponent<ClawManager>().PlaceEnemy(enemy, verticalDisplacement));
    }
}
