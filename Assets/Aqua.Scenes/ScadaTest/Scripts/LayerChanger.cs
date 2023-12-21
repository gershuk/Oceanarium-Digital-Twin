#nullable enable

using Aqua.Scada;
using Aqua.UIBaseElements;

using UnityEngine;

namespace Aqua.Scenes.ScadaTest
{
    public class LayerChanger : MonoBehaviour
    {
        private bool _isInited = false;

        [SerializeField]
        private VisitorCreater[] _visitorCreaters;

        [SerializeField] private SimpleToggleViewModel ����1;
        [SerializeField] private SimpleToggleViewModel ����2;
        [SerializeField] private SimpleToggleViewModel �����1;
        [SerializeField] private SimpleToggleViewModel �����2;
        [SerializeField] private SimpleToggleViewModel ��_�����;
        [SerializeField] private SimpleToggleViewModel �������������;
        [SerializeField] private SimpleToggleViewModel �����������;
        [SerializeField] private SimpleToggleViewModel FWS1;
        [SerializeField] private SimpleToggleViewModel FWS2;
        [SerializeField] private SimpleToggleViewModel CA;
        [SerializeField] private SimpleToggleViewModel BWR;

        private void DisableVistorCreater (VisitorCreater visitorCreater)
        {
            visitorCreater.gameObject.SetActive(false);
        }

        private void DisableAllVistorCreaters ()
        {
            foreach (var visitor in _visitorCreaters)
                DisableVistorCreater(visitor);
        }

        private void EnableVistorCreater (VisitorCreater visitorCreater)
        {
            visitorCreater.gameObject.SetActive(true);
            visitorCreater.RespawnVisitors();
        }

        private void Awake ()
        {
            ForceInit();
        }

        public void ForceInit ()
        {
            if (_isInited)
                return;

            foreach (var visitorCreater in _visitorCreaters)
            {
                visitorCreater.ForceInit();
                DisableVistorCreater(visitorCreater);
            }

            _isInited = true;
        }

        public void Update ()
        {
            switch (����1.State, 
                    ����2.State, 
                    �����1.State, 
                    �����2.State, 
                    ��_�����.State, 
                    �������������.State, 
                    �����������.State, 
                    FWS1.State,
                    FWS2.State,
                    CA.State,
                    BWR.State)
            {
                case (false, false, false, true, false, false, true, false, false, false, false):
                    if (_visitorCreaters[0].gameObject.activeSelf)
                        return;
                    DisableAllVistorCreaters();
                    EnableVistorCreater(_visitorCreaters[0]);
                    break;
                case (false, false, true, false, false, false, false, false, true, false, false):
                    if (_visitorCreaters[1].gameObject.activeSelf)
                        return;
                    DisableAllVistorCreaters();
                    EnableVistorCreater(_visitorCreaters[1]);
                    break;
                case (false, true, true, false, true, false, true, true, false, true, false):
                    if (_visitorCreaters[2].gameObject.activeSelf)
                        return;
                    DisableAllVistorCreaters();
                    EnableVistorCreater(_visitorCreaters[2]);
                    break;
                case (false, false, false, true, false, false, false, true, false, true, true):
                    if (_visitorCreaters[3].gameObject.activeSelf)
                        return;
                    DisableAllVistorCreaters();
                    EnableVistorCreater(_visitorCreaters[3]);
                    break;
                case (true, false, true, false, false, true, true, true, true, true, false):
                    if (_visitorCreaters[4].gameObject.activeSelf)
                        return;
                    DisableAllVistorCreaters();
                    EnableVistorCreater(_visitorCreaters[4]);
                    break;
                case (_, _, _, _, _, _, _, _, _, _, _):
                    DisableAllVistorCreaters();
                    break;
            }
        }
    }
}