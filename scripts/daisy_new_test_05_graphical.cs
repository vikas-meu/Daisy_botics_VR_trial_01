using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine.XR;

public class SendCubeDataOverWiFi : MonoBehaviour
{
    [Header("Objects to Send")]
    public Transform cube;              // Main reference cube (World space)
    public Transform cube1;
    public Transform cube2;

    [Header("Reference Object")]
    public Transform referenceObject;   // ← New: This will be used as reference for cube1 & cube2

    [Header("TCP Settings")]
    public string serverIP = "192.168.1.13";
    public int serverPort = 8080;

    private TcpClient client;
    private NetworkStream stream;

    [Serializable]
    public class TransformData
    {
        public float x, y, z;
        public TransformData(float x, float y, float z)
        {
            this.x = x; this.y = y; this.z = z;
        }
    }

    [Serializable]
    public class CubeData
    {
        public TransformData position;
        public TransformData rotation;
    }

    [Serializable]
    public class PacketData
    {
        public CubeData cube;
        public CubeData cube1;
        public CubeData cube2;
        public float trig1;
        public float trig2;
    }

    void Start() => ConnectToServer();

    void ConnectToServer()
    {
        try
        {
            client = new TcpClient(serverIP, serverPort);
            stream = client.GetStream();
            Debug.Log("✅ Connected to PC receiver");
        }
        catch (Exception e)
        {
            Debug.LogError("❌ Could not connect: " + e.Message);
        }
    }

    void Update()
    {
        if (stream == null || !stream.CanWrite) return;

        // Check all required objects
        if (cube == null || cube1 == null || cube2 == null || referenceObject == null)
        {
            if (Time.frameCount % 60 == 0)
                Debug.LogWarning("Some objects are not assigned in the inspector!");
            return;
        }

        float trig1Value = GetTriggerValue(XRNode.LeftHand);
        float trig2Value = GetTriggerValue(XRNode.RightHand);

        PacketData packet = new PacketData
        {
            cube = CreateWorldCubeData(cube),
            cube1 = CreateRelativeToReference(cube1, referenceObject),
            cube2 = CreateRelativeToReference(cube2, referenceObject),
            trig1 = trig1Value,
            trig2 = trig2Value
        };

        string json = JsonUtility.ToJson(packet) + "\n";
        byte[] data = Encoding.UTF8.GetBytes(json);

        try
        {
            stream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to send data: " + e.Message);
        }
    }

    // Main cube → World space
    private CubeData CreateWorldCubeData(Transform t)
    {
        return new CubeData
        {
            position = new TransformData(t.position.x, t.position.y, t.position.z),
            rotation = new TransformData(t.eulerAngles.x, t.eulerAngles.y, t.eulerAngles.z)
        };
    }

    // Cube1 & Cube2 → Position + Rotation relative to referenceObject
    private CubeData CreateRelativeToReference(Transform target, Transform reference)
    {
        // Position relative to reference object
        Vector3 localPos = reference.InverseTransformPoint(target.position);

        // Proper relative rotation using Quaternion (much more accurate)
        Quaternion relativeRot = Quaternion.Inverse(reference.rotation) * target.rotation;
        Vector3 localEuler = relativeRot.eulerAngles;

        return new CubeData
        {
            position = new TransformData(localPos.x, localPos.y, localPos.z),
            rotation = new TransformData(localEuler.x, localEuler.y, localEuler.z)
        };
    }

    private float GetTriggerValue(XRNode node)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(node);
        if (device.isValid && device.TryGetFeatureValue(CommonUsages.trigger, out float value))
            return value;
        return 0f;
    }

    void OnApplicationQuit()
    {
        stream?.Close();
        client?.Close();
    }
}