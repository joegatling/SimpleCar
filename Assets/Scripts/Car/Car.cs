    using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cars
{
    [RequireComponent(typeof(Rigidbody))]
    public class Car : MonoBehaviour
    {
        public enum DriveTrain
        {
            FrontWheelDrive,
            RearWheelDrive,
            AllWheelDrive,
            NoWheelDrive
        }

        [SerializeField] private List<Wheel> _wheels = default;
        [SerializeField] private Rigidbody _rigidbody = default;

        [Header("Car Properties")]

        [SerializeField] private Vector3 _centerOfMass = Vector3.zero;

        [Header("Suspension")]
        //[SerializeField] private float _suspensionRestDistance = 0.5f;
        [SerializeField] private float _suspensionMaxDistance = 1.0f;
        [SerializeField] private float _suspensionStrength = 10;
        [SerializeField] private float _suspensionDampening = 1.0f;

        [Header("Tires")]        
        [SerializeField] private float _tireGrip = 0.5f;
        [SerializeField] private float _tireMass = 10;
        [SerializeField] private float _maxTurnAngle = 45;
        [SerializeField] private float _tireRadius = 0.25f;

        [Header("Engine")]
        [SerializeField] private float _topSpeed = 10;
        [SerializeField] AnimationCurve _torqueCurve = default;
        [SerializeField] private float _maxTorque = 10;

        [Space]
        [SerializeField] DriveTrain _driveTrain = DriveTrain.RearWheelDrive;

        [Space]
        [SerializeField] LayerMask _wheelPhysicsMask = default;


        private void Awake()
        {
            _rigidbody.centerOfMass = _centerOfMass;
        }


        private void FixedUpdate()
        {
            Ray wheelRay;
            RaycastHit wheelRayHit;

            // TODO - Consider wheel Radius!

            for(int i = 0; i < _wheels.Count; i++)
            {
                Wheel wheel = _wheels[i];
                if(wheel == null)
                {
                    continue;
                }

                Vector3 raycastOrigin = wheel.transform.position + wheel.transform.up * (_suspensionMaxDistance - _tireRadius);
                float raycastDistance = _suspensionMaxDistance * 2;
                // Step 1 - Calculate Suspension
                wheelRay = new Ray(raycastOrigin, wheel.transform.up * -1.0f);


                Debug.DrawRay(wheelRay.origin, wheelRay.direction * raycastDistance, Color.red);

                bool rayCastHit = Physics.Raycast(wheelRay, out wheelRayHit, raycastDistance, _wheelPhysicsMask);

                if (rayCastHit)
                {
                    Vector3 springDirection = wheel.transform.up;

                    Vector3 wheelWorldVelocity = _rigidbody.GetPointVelocity(wheel.transform.position);
                    float offset = _suspensionMaxDistance - wheelRayHit.distance;
                    float velocity = Vector3.Dot(springDirection, wheelWorldVelocity);
                    float force = (offset * _suspensionStrength) - (velocity * _suspensionDampening);

                    Debug.Log(force);

                   force = Mathf.Max(0, force);

                    wheel.geometry.position = wheelRay.GetPoint(wheelRayHit.distance - _tireRadius);

                    Debug.DrawRay(raycastOrigin, springDirection * force, Color.green);

                    _rigidbody.AddForceAtPosition(springDirection * force, wheel.transform.position);

                    // Step 2 - Calculate Steering
                    Vector3 steeringDirection = wheel.transform.right;
                    float steeringVelocity = Vector3.Dot(steeringDirection, wheelWorldVelocity);
                    float desiredChange = -steeringVelocity * _tireGrip;

                    float desiredAcceleration = desiredChange / Time.fixedDeltaTime;

                    _rigidbody.AddForceAtPosition(steeringDirection * _tireMass * desiredAcceleration, wheel.transform.position);

              

                    // Step 3 - Calcualte Acceleration

                    if (wheel.IsDriveWheel(_driveTrain))
                    {
                        Vector3 accelerationDirection = wheel.transform.forward;

                        if(Input.GetAxis("Vertical") > 0.0f)
                        {
                            float carSpeed = Vector3.Dot(transform.forward, _rigidbody.velocity);
                            float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / _topSpeed);
                            float torque = _torqueCurve.Evaluate(normalizedSpeed) * Input.GetAxis("Vertical") * _maxTorque;

                            Debug.Log(torque);

                            _rigidbody.AddForceAtPosition(accelerationDirection * torque, wheel.transform.position);
                        }
                    }
                }
                else
                {
                    wheel.geometry.position = wheelRay.GetPoint(_suspensionMaxDistance*2 - _tireRadius);
                    Debug.Log(0);
                }


                if (wheel.wheelPosition == Wheel.WheelPosition.Front)
                {
                    wheel.transform.localEulerAngles = new Vector3(0.0f, Input.GetAxis("Horizontal") * _maxTurnAngle, 0.0f);
                }

            }
        }

        void OnDrawGizmosSelected()
        {

            Gizmos.color = Color.cyan;
            Gizmos.matrix = transform.localToWorldMatrix;

            foreach(Wheel wheel in _wheels)
            {
                if(wheel != null)
                {
                    //Gizmos.DrawSphere(transform.InverseTransformPoint(wheel.transform.position), 0.2f);

                    Vector3 topPosition = transform.InverseTransformPoint(wheel.transform.position) + transform.up * _suspensionMaxDistance;
                    Vector3 restPosition = transform.InverseTransformPoint(wheel.transform.position);
                    Vector3 bottomPosition = transform.InverseTransformPoint(wheel.transform.position - wheel.transform.up * _suspensionMaxDistance);

                    Gizmos.DrawLine(topPosition, bottomPosition);
                    Gizmos.DrawWireSphere(topPosition, _tireRadius);
                    Gizmos.DrawWireSphere(restPosition, _tireRadius);
                    Gizmos.DrawWireSphere(bottomPosition, _tireRadius);
                }
            }

            Gizmos.color = Color.black;
            if (Application.IsPlaying(this))
            {
                Gizmos.DrawSphere(_rigidbody.centerOfMass, 0.2f); 
            }
            else
            {
                Gizmos.DrawSphere(_centerOfMass, 0.2f);
            }

        }
        private void OnValidate()
        {
            _wheels = GetComponentsInChildren<Wheel>().ToList();
            _rigidbody = GetComponent<Rigidbody>();

            if(Application.IsPlaying(this))
            {
                _rigidbody.centerOfMass = _centerOfMass;
            }

        }

    }
}