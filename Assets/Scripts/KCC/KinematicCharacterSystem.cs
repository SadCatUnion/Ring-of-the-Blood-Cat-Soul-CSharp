using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KCC
{
    public class KinematicCharacterSystem : Singleton<KinematicCharacterSystem>
    {
        public List<KinematicCharacterMotor> characterMotors = new List<KinematicCharacterMotor>();
        public List<PhysicsMover> physicsMovers = new List<PhysicsMover>();

        public void RegisterCharactorMotor(KinematicCharacterMotor characterMotor)
        {
            characterMotors.Add(characterMotor);
        }
        public void UnregisterCharactorMotor(KinematicCharacterMotor characterMotor)
        {
            characterMotors.Remove(characterMotor);
        }
        public void RegisterPhysicsMover(PhysicsMover physicsMover)
        {
            physicsMovers.Add(physicsMover);
            physicsMover.rigidbody.interpolation = RigidbodyInterpolation.None;
        }
        public void UnregisterPhysicsMover(PhysicsMover physicsMover)
        {
            physicsMovers.Remove(physicsMover);
        }

        private void FixedUpdate()
        {
            var deltaTime = Time.deltaTime;
            PreSimulationInterpolationUpdate(deltaTime);
            Simulate(deltaTime);
            PostSimulationInterpolationUpdate(deltaTime);
        }
        private void LateUpdate()
        {
            CustomInterpolationUpdate();
        }
        public void PreSimulationInterpolationUpdate(float deltaTime)
        {
            foreach (var motor in characterMotors)
            {
                motor.initialTickPosition = motor.transientPosition;
                motor.initialTickRotation = motor.transientRotation;
                motor.transform.SetPositionAndRotation(motor.transientPosition, motor.transientRotation);
            }
            foreach (var mover in physicsMovers)
            {
                mover.initialTickPosition = mover.transientPosition;
                mover.initialTickRotation = mover.transientRotation;
                mover.transform.SetPositionAndRotation(mover.transientPosition, mover.transientRotation);
                mover.rigidbody.transform.SetPositionAndRotation(mover.transientPosition, mover.transientRotation);
            }
        }
        public void Simulate(float deltaTime)
        {
            foreach(var mover in physicsMovers)
            {
                mover.UpdateVelocity(deltaTime);
            }
            foreach(var motor in characterMotors)
            {
                motor.UpdatePhase1(deltaTime);
            }
            foreach(var mover in physicsMovers)
            {
                mover.transform.SetPositionAndRotation(mover.transientPosition, mover.transientRotation);
                mover.rigidbody.transform.SetPositionAndRotation(mover.transientPosition, mover.transientRotation);
            }
            foreach(var motor in characterMotors)
            {
                motor.UpdatePhase2(deltaTime);
                motor.transform.SetPositionAndRotation(motor.transientPosition, motor.transientRotation);
            }
        }    
        public void PostSimulationInterpolationUpdate(float deltaTime)
        {
            foreach(var motor in characterMotors)
            {
                motor.transform.SetPositionAndRotation(motor.initialTickPosition, motor.initialTickRotation);
            }
            foreach(var mover in physicsMovers)
            {
                if (mover.moveWithPhysics)
                {
                    mover.rigidbody.transform.SetPositionAndRotation(mover.initialTickPosition, mover.initialTickRotation);
                    mover.rigidbody.MovePosition(mover.transientPosition);
                    mover.rigidbody.MoveRotation(mover.transientRotation);
                }
                else
                {
                    mover.rigidbody.transform.SetPositionAndRotation(mover.transientPosition, mover.transientRotation);
                }
            }
        }
        public void CustomInterpolationUpdate()
        {

        }
    }
}