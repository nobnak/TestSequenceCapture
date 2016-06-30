using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;

namespace SequenceCaptureSystem {

    [RequireComponent(typeof(Camera))]
    public class SequenceCapture : MonoBehaviour {
        public System.Environment.SpecialFolder saveFolder = System.Environment.SpecialFolder.MyPictures;
        public int fps = 30;

        string _fileformat;
        Texture2D _tex;

        void OnEnable() {
            _tex = new Texture2D (0, 0, TextureFormat.ARGB32, false);
            Time.captureFramerate = fps;
            _fileformat = string.Format("{0}_{{0:D5}}.jpg", SceneManager.GetActiveScene().name);
        }
        void OnDisable() {
            Time.captureFramerate = 0;
            Destroy (_tex);
        }
    	void OnPostRender() {
            try {
                var w = Screen.width;
                var h = Screen.height;
                _tex.Resize (w, h);
                _tex.ReadPixels (new Rect (0f, 0f, w, h), 0, 0);
                var path = Path.Combine (
                    System.Environment.GetFolderPath (saveFolder),
                    string.Format (_fileformat, Time.frameCount));
                File.WriteAllBytes (path, _tex.EncodeToJPG ());
            } catch (System.Exception e) {
                Debug.LogError (e);
            }
    	}
    }
}
