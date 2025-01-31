using UnityEngine;

namespace FastReset {
    public class JointData {
        public GameObject jointObj { get; }
        private Rigidbody jointRb;
        private Quaternion initialRotation;

        public JointData(GameObject jointObj) {
            this.jointObj = jointObj;
            jointRb = jointObj.GetComponent<Rigidbody>();
            initialRotation = jointObj.transform.rotation;
        }

        public void Reset() {
            jointRb.angularVelocity = Vector3.zero;
            jointRb.velocity = Vector3.zero;
            jointObj.transform.rotation = initialRotation;
        }
    }
}
