using UnityEngine;

public class CharacterPush : MonoBehaviour
{
    [SerializeField] float pushPower = 2.5f;   

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic) return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);

        if (pushDir.sqrMagnitude < 0.0001f) return;

        body.AddForce(pushDir.normalized * pushPower, ForceMode.Impulse);
    }
}
