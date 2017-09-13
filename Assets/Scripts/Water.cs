using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Water : MonoBehaviour {
    
    // density is tightly related to buoyancy, this game uses depth to calculate buoyancy, approximately
    public float Density;
    public Vector3 FlowDirection;
    public float FlowForce;

    private Player player;

    private void Start() {
        player = GetterUtility.GetPlayer();
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            // provide buoyancy
            RaycastHit hitInfo;
            Physics.Raycast(player.GetLowestPoint(), Vector3.up, out hitInfo, 100f, LayerMask.GetMask("Water Surface"));

            float buoyancyMagnitude = hitInfo.distance * Density;
            player.ReceiveBuoyancy(buoyancyMagnitude);

            // provide flow force
            player.ReceiveForce(FlowDirection * FlowForce);
        }

        if (other.CompareTag("Energy")) {
            PickUp pickUp = other.transform.parent.GetComponent<PickUp>();
            // provide buoyancy
            RaycastHit hitInfo;
            Physics.Raycast(pickUp.GetLowestPoint(), Vector3.up, out hitInfo, 100f, LayerMask.GetMask("Water Surface"));

            float buoyancyMagnitude = hitInfo.distance * Density;
            pickUp.ReceiveBuoyancy(buoyancyMagnitude);
        }
        
        if (other.CompareTag("Charge")) {
            Charge charge = other.transform.parent.GetComponent<Charge>();
            // provide buoyancy
            RaycastHit hitInfo;
            Physics.Raycast(charge.GetLowestPoint(), Vector3.up, out hitInfo, 100f, LayerMask.GetMask("Water Surface"));

            float buoyancyMagnitude = hitInfo.distance * Density;
            charge.ReceiveBuoyancy(buoyancyMagnitude);
        }

        if (other.CompareTag("Friendly")) {
            Friendly friendly = other.GetComponent<Friendly>();
            if (!friendly.IsSunk) {
                // provide buoyancy
                RaycastHit hitInfo;
                Physics.Raycast(friendly.GetLowestPoint(), Vector3.up, out hitInfo, 100f,
                    LayerMask.GetMask("Water Surface"));

                float buoyancyMagnitude = hitInfo.distance * Density;
                friendly.ReceiveBuoyancy(buoyancyMagnitude);
            }
        }
        
        if (other.CompareTag("Thorn")) {
            Thorn thorn = other.transform.parent.GetComponent<Thorn>();
            if (!thorn.IsSunk) {
                // provide buoyancy
                RaycastHit hitInfo;
                Physics.Raycast(thorn.transform.position, Vector3.up, out hitInfo, 100f,
                    LayerMask.GetMask("Water Surface"));

                float buoyancyMagnitude = hitInfo.distance * Density / 20f;
                thorn.ReceiveBuoyancy(buoyancyMagnitude);
            }
        }
    }
}