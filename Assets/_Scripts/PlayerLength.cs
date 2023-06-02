using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerLength : NetworkBehaviour
{
    [SerializeField] private GameObject tailPrefab;

    public NetworkVariable<ushort> length = new(1);
    private Collider2D _collider2D;
    private Transform _lastTail;

    private List<GameObject> _tails;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _tails = new List<GameObject>();
        _lastTail = transform;
        _collider2D = GetComponent<Collider2D>();
        Debug.Log("Last tail set");
        if (!IsServer) length.OnValueChanged += LengthChanged;
    }

    [ContextMenu("Add Length")]
    public void AddLength()
    {
        length.Value += 1;
        InstantiateTail();
    }

    private void LengthChanged(ushort previousValue, ushort newValue)
    {
        InstantiateTail();
    }

    private void InstantiateTail()
    {
        var tailGameObject = Instantiate(tailPrefab, transform.position, Quaternion.identity);
        tailGameObject.GetComponent<SpriteRenderer>().sortingOrder = -length.Value;
        if (tailGameObject.TryGetComponent(out Tail tail))
        {
            tail.networkedOwner = transform;
            tail.followTransform = _lastTail;
            _lastTail = tailGameObject.transform;
            Physics2D.IgnoreCollision(tailGameObject.GetComponent<Collider2D>(), _collider2D);
        }

        _tails.Add(tailGameObject);
    }
}