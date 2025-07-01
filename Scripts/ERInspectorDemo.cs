using ERInspector;
using UnityEngine;

public class ERInspectorDemo : MonoBehaviour
{
    [Title("Object")]
    [SerializeField] private GameObject _character;

    [Space(16)]
    [InfoBox("The camera used by the character")]
    [SerializeField] private Camera _camera;

    [Space(16)]
    [ReadOnly]
    [SerializeField] private int _currentHealth;

    [Space(16)]
    [AssetOnly]
    [SerializeField] private Material _playerMaterial;

    [Space(16)]
    [SceneOnly]
    [SerializeField] private Rigidbody _rigidbody;

    [Space(16)]
    [SerializeField] private bool _isCanRun;

    [ShowIf("_isCanRun")]
    [SerializeField] private int _runSpeed;

    [Button]
    public void Jump()
    {

    }

    [Button]
    public void AddHealth(int health)
    {

    }
}