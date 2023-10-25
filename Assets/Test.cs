using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

using Aqua.SocketSystem;
using Aqua.TanksSystem;
using Aqua.TanksSystem.Models;

using UniRx;

using UnityEngine;

public class Test : MonoBehaviour
{
    void Start ()
    {
        //var pos1 = new ReactiveProperty<Vector3>(new());
        //var scale = new ReactiveProperty<float>(new float());

        //Observable.CombineLatest(pos1, scale, (pos, scale) => (pos, scale)).Subscribe(v => Debug.Log($"{Time.time} {v.pos} {v.scale}"));
        //pos1.Value = Vector3.right;
        //scale.Value = 20;

        var flow = new UniversalSocket<int,int>();
        var valve = new UniversalSocket<float, float>();

        var tube = new OneWayTubeModel<int,string>(static(number)=> number.ToString());
        tube.InSocket.SubscribeTo(flow);

        var cSocket = new ÑombiningSocket<string, float, string>(combineFunction: static (s,f) => $"str:'{s}' float:{f}");
        cSocket.SubscribeTo(tube.OutSocket);
        cSocket.SubscribeTo(valve);
        cSocket.ReadOnlyProperty.Subscribe(Debug.Log);

        var t = flow.TrySetValue(20);
    }
}
