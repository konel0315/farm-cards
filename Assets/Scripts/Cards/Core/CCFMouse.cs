using DG.Tweening;
using UnityEngine;

public class ChoiceCardTilt : MonoBehaviour
{
    [SerializeField] private float maxRot = 15f;
    [SerializeField] private float followSpeed = 0.4f;
    
    private void Update()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float deltaX = mouseWorld.x - Camera.main.transform.position.x;
        
        float moveRatio = Mathf.Clamp(-deltaX / 5f, -1f, 1f);
        float targetRotZ = moveRatio * maxRot;
        
        transform.DOLocalRotate(new Vector3(0, 0, targetRotZ), followSpeed).SetUpdate(true);
    }

    public float GetCurrentZRotation()
    {
        float z = transform.localEulerAngles.z;
        return z > 180f ? z - 360f : z;
    }
}