using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;

namespace SequenceCaptureSystem {

    [RequireComponent(typeof(Camera))]
    public class SequenceCapture : MonoBehaviour {
        public enum FormatEnum { JPEG = 0, PNG }

        public System.Environment.SpecialFolder saveFolder = System.Environment.SpecialFolder.MyPictures;
        public int fps = 30;
        public int limitImageCount = -1;
        public FormatEnum format;

        Texture2D _tex;
        int imageCounter = 0;
        ITextureSerializer serializer;

        void Awake() {
            _tex = new Texture2D (0, 0, TextureFormat.ARGB32, false);

            var folder = System.Environment.GetFolderPath (saveFolder);
            switch (format) {
            case FormatEnum.JPEG:
                serializer = new JpegSerializer (folder);
                break;
            default:
                serializer = new PngSerializer (folder);
                break;
            }
        }
        void OnEnable() {
            Time.captureFramerate = fps;
            imageCounter = limitImageCount;
        }
        void OnDisable() {
            Time.captureFramerate = 0;
        }
        void OnDestroy() {
            Destroy (_tex);
        }
    	void OnPostRender() {
            if (imageCounter > 0 && --imageCounter == 0)
                enabled = false;

            try {
                var w = Screen.width;
                var h = Screen.height;
                _tex.Resize (w, h);
                _tex.ReadPixels (new Rect (0f, 0f, w, h), 0, 0);
                serializer.Serialize(_tex);
            } catch (System.Exception e) {
                Debug.LogError (e);
            }
    	}

        public interface ITextureSerializer : System.IDisposable {
            bool Serialize (Texture2D tex);
        }
        public class JpegSerializer : ITextureSerializer {
            const string FORMAT_FILE = "{0}_{{0:D5}}.jpg";
            readonly string formatPath;

            public JpegSerializer(string folder) {
                formatPath = string.Format(
                    Path.Combine(folder, FORMAT_FILE), 
                    SceneManager.GetActiveScene().name);
            }

            #region TextureSerializer implementation
            public bool Serialize (Texture2D tex) {
                try {
                    var path = string.Format(formatPath, Time.frameCount);
                    File.WriteAllBytes (path, tex.EncodeToJPG ());
                    return true;
                } catch (System.Exception e) {
                    Debug.LogError (e);
                }
                return false;
            }
            #endregion
            #region IDisposable implementation
            public void Dispose () { }
            #endregion
        }
        public class PngSerializer : ITextureSerializer {
            const string FORMAT_FILE = "{0}_{{0:D5}}.png";
            readonly string formatPath;

            public PngSerializer(string folder) {
                formatPath = string.Format(
                    Path.Combine(folder, FORMAT_FILE), 
                    SceneManager.GetActiveScene().name);
            }

            #region TextureSerializer implementation
            public bool Serialize (Texture2D tex) {
                try {
                    var path = string.Format(formatPath, Time.frameCount);
                    File.WriteAllBytes (path, tex.EncodeToPNG ());
                    return true;
                } catch (System.Exception e) {
                    Debug.LogError (e);
                }
                return false;
            }
            #endregion
            #region IDisposable implementation
            public void Dispose () { }
            #endregion
        }
    }
}
