using UnityEngine;

namespace GTS.Test
{
    public class TestClass : MonoBehaviour
    {
        #region Fields
        public Bounds aBounds;
        public BoundsInt aBoundsInt;
        public Color aColor;
        public AnimationCurve aCurve;
        public double aDouble;
        public AnEnum aanEnum; //enum
        public float aFloat;
        public GameObject aGameObject;
        public int aInt;
        public LayerMask aLayer;
        public long aLong;
        public UnityEngine.Object aUnityObject;
        public object aSystemObject;
        public string aString;
        public bool aBool;
        public Transform aTransform;
        public Vector2 aVector2;
        public Vector2Int aVector2Int;
        public Vector3 aVector3;
        public Vector3Int aVector3Int;
        public Vector4 aVector4;
        public Quaternion SomeQuaternion;
        public Gradient aGradient;
        #endregion

        #region Properties
        public int aIntProperty { get; set; }
        public string aStringProperty { get; set; }
        #endregion

        private void Start()
        {
            //Debug.Log(GenerateOnEnableBegin());
        }

        [ContextMenu("Do A Thing")]
        public int DoAThing()
        {
            Debug.Log("Doing that thing.");
            return 0;
        }
    }

    public enum AnEnum
    {
        ONE,
        TWO,
        THREE
    }
}
