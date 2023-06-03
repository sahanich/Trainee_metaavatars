using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Management;

namespace Assets.Scripts.Player.WindowsMovement
{
    public class NonVrMovementSystem : MonoBehaviour
    {
        [SerializeField, Range(1f, 3f)] private float _movementSpeed = 1.5f;

        private Transform _cameraRig;
        private CharacterController _characterController;

        private float _vertical;
        private float _horizontal;
        private float xRot;
        //private TrackedPoseDriver _trackedCamera;

        public void Init(LocalPlayerRoot localPlayer)
        {
            _cameraRig = localPlayer.XrOrigin.Camera.transform;
            _characterController = localPlayer.CharacterController;
        }

        public void StartMovementControl()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR            
            StartCoroutine(MovementRoutine());
#endif
        }

        private IEnumerator MovementRoutine()
        {
            while (true)
            {
                yield return null;
                if (!XRGeneralSettings.Instance.Manager.isInitializationComplete)
                {
                    DoMovement();
                    DoRotate();
                }
            }

        }

//        void Start()
//        {
//#if UNITY_STANDALONE_WIN || UNITY_EDITOR
//            //_networkObject = GetComponent<NetworkObject>();
//            _characterController = GetComponent<CharacterController>();
//            //_characterController.enabled = true;
//            //_trackedCamera = _cameraRig.GetComponent<TrackedPoseDriver>();
//#endif
//        }

//        void Update()
//        {
////#if UNITY_STANDALONE_WIN || UNITY_EDITOR
//            //if (!_networkObject.IsOwner) return;
//            if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
//            {
//                //_trackedCamera.enabled = true;
//            }
//            else
//            {
//                //_trackedCamera.enabled = false;
//                DoMovement();
//                DoRotate();
//            }
////#endif
//        }

//        public void SetPosition(Vector3 position, Quaternion rotation)
//        {
//#if UNITY_STANDALONE_WIN || UNITY_EDITOR
//            if (_characterController != null)
//                _characterController.enabled = false;
//            transform.position = position;
//            transform.rotation = rotation;
//            if (_characterController != null)
//                _characterController.enabled = true;
//#endif
//        }

        private Vector3 DirectionCalculation()
        {
            _horizontal = Input.GetAxis("Horizontal");
            _vertical = Input.GetAxis("Vertical");

            Vector3 direction = _characterController.transform.right * _horizontal * _movementSpeed +
                                _characterController.transform.forward * _vertical * _movementSpeed;
            if (Input.GetKey(KeyCode.LeftShift)) direction = direction * 3;
            return direction;
        }

        private void DoMovement()
        {
            _characterController.SimpleMove(DirectionCalculation());
        }

        private void DoRotate()
        {
            float x = Input.GetAxis("Mouse X") * Time.deltaTime * 140;
            _characterController.transform.Rotate(transform.up * x);

            float y = Input.GetAxis("Mouse Y") * Time.deltaTime * 70;

            xRot = xRot - y;
            xRot = Mathf.Clamp(xRot, -90, 90);

            _cameraRig.localRotation = Quaternion.Euler(xRot, 0, 0);
        }
    }
}