using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

using Aqua.SocketSystem;
using Aqua.TanksSystem;

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

        var number = new ReactiveProperty<int>();
        var sockIn = new UniversalSocket<int,int>(number);

        var tube = new OneWayTubeModel<int,string>(static(number)=> number.ToString());
        tube.InSocket.SubscribeTo(sockIn);
        tube.OutSocket.ReadOnlyProperty.Subscribe(v=>Debug.Log(v));
        number.Value = 1;
        number.Value = 2;
        number.Value = 3;
        number.Value = 4;
    }
}
