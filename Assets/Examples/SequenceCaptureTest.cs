using UnityEngine;
using System.Collections;

namespace SequenceCaptureSystem {

    public class SequenceCaptureTest : MonoBehaviour {
        public const float SEED = 1000f;
        public const float DEG = 360f;
        public float scale = 1f;
        public float freq = 0.1f;
        public float angularFreq = 0.1f;

        Vector3 _seed;

        void Start() {
            _seed = SEED * new Vector3 (Random.value, Random.value, Random.value);
        }
    	void Update () {
            var tpos = Time.timeSinceLevelLoad * freq;
            transform.position = scale * new Vector3 (
                Noise (tpos + _seed.x, _seed.y), Noise (tpos + _seed.y, _seed.z), Noise (tpos + _seed.z, _seed.x));

            var trot = Time.timeSinceLevelLoad * angularFreq;
            transform.localRotation = Quaternion.Euler (
                DEG * Noise (_seed.y, trot + _seed.z), DEG * Noise (_seed.z, trot + _seed.x), DEG * Noise (_seed.x, trot + _seed.y));
    	}

        float Noise(float x, float y) {
            return 2f * Mathf.PerlinNoise (x, y) - 1f;
        }
    }
}