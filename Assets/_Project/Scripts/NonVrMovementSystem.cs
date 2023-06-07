using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Management;

namespace Assets.Scripts.Player.WindowsMovement
{
    public class NonVrMovementSystem : MonoBehaviour
    {
        [SerializeField, Range(1f, 3f)] 
        private float MovementSpeed = 1.5f;

        private Transform _cameraRig;
        private CharacterController _characterController;

        private float _vertical;
        private float _horizontal;
        private float xRot;

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

        private Vector3 DirectionCalculation()
        {
            _horizontal = Input.GetAxis("Horizontal");
            _vertical = Input.GetAxis("Vertical");

            Vector3 direction = _characterController.transform.right * _horizontal * MovementSpeed +
                                _characterController.transform.forward * _vertical * MovementSpeed;
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