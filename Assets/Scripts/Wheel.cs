using UnityEngine;

namespace Burnout
{
    public class Wheel : Object
    {
        public Vehicle Vehicle;
        public Spring Spring;

        private RaycastHit Hit;
        private bool IsGrounded;

        public Wheel(Vehicle obj)
        {
            Vehicle = obj;
        }

        public void Simulate()
        {
            Vector3 velocity = Vehicle.Body.velocity;
            Vector3 force = Vector3.zero;

            force += SpringForce(velocity);

            //force.x -= (Quaternion.Euler(Vehicle.transform.forward) * velocity).x * 0.25f;

            if (IsGrounded) ApplyForceAtPosition(force);
        }

        public Vector3 SpringForce(Vector3 velocity)
        {
            IsGrounded = false;

            RaycastHit[] contacts = Physics.SphereCastAll(Position, 0, Vector3.down, Vehicle.WheelRadius);

            float distance = float.MaxValue;
            for(int i = 0; i < contacts.Length; i++)
            {
                if (contacts[i].distance < distance)
                {
                    distance = contacts[i].distance;

                    Hit = contacts[i];
                    IsGrounded = true;
                }
            }

            if (IsGrounded)
            {
                Spring.Offset = Vehicle.Spring.RestLength - Hit.distance;

                float force = (Spring.Offset * Vehicle.Spring.Strength) - (velocity.y * Vehicle.Spring.Damping);
                return new Vector3(0, force, 0);
            }
            else
            {
                return Vector3.zero;
            }
        }

        public void ApplyForceAtPosition(Vector3 force) => Vehicle.Body.AddForceAtPosition(Vehicle.transform.TransformVector(force), Position, ForceMode.Acceleration);

        public void DrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(Position, Vehicle.WheelRadius);

            if (IsGrounded)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(Position, Vehicle.transform.up - Hit.normal * 2);
            }
        }
    }
}