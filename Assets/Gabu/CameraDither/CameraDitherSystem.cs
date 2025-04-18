using UnityEngine;

public class CameraDitherSystem : MonoBehaviour
{
    [SerializeField]
    private Material _material;
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private string _cameraPositionValue = "Point A";
    [SerializeField]
    private string _targetPositionValue = "Point B";

    private void Start()
    {
        if (_material == null)
        {
            Debug.LogError("Material is not assigned.");
            return;
        }
        if (_target == null)
        {
            _target = transform;
        }
    }

    void LateUpdate()
    {
        Debug.Log($"やってるよ");
        _material.SetVector(_cameraPositionValue, transform.position);
        _material.SetVector(_targetPositionValue, _target.position);
    }
}
