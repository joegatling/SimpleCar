using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cars
{
    public class Wheel : MonoBehaviour
    {
        public enum WheelPosition
        {
            Front,
            Rear
        }

        [SerializeField] private Transform _geometry;

        [SerializeField] WheelPosition _position = WheelPosition.Front;

        public WheelPosition wheelPosition => _position;

        public Transform geometry => _geometry;

        public bool IsDriveWheel(Cars.Car.DriveTrain driveTrain)
        {
            if(_position == WheelPosition.Front)
            {
                return driveTrain == Car.DriveTrain.FrontWheelDrive || driveTrain == Car.DriveTrain.AllWheelDrive;
            }
            else
            {
                return driveTrain == Car.DriveTrain.RearWheelDrive || driveTrain == Car.DriveTrain.AllWheelDrive;
            }
        }
    }

}