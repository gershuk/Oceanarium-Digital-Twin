using Aqua.TickSystem;

using UnityEngine;

[RequireComponent(typeof(ClockGenerator))]
public class ClockController : MonoBehaviour
{
    [SerializeField]
    private MonoBehaviour[] _initTickList;

    [SerializeField]
    private ClockGenerator _clockGenerator;

    private void Awake ()
    {
        if (_clockGenerator == null)
            _clockGenerator = GetComponent<ClockGenerator>();

        foreach (var monoBehaviour in _initTickList)
        {
            if (monoBehaviour is ITickObject tickObject)
            {
                _clockGenerator.AddToEnd(tickObject);
            }
            else
            {
                Debug.LogWarning($"{monoBehaviour} is not ITickObject.");
            }
        }
    }

    private void Start ()
    {
        _clockGenerator.Init();
    }

    private void Update ()
    {
        _clockGenerator.Tick();
    }
}
