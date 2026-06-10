using UnityEngine;
using System.Net.Sockets;
using System.Text;
using UnityEngine.InputSystem;

public class SendMultiCubeDataOverWiFi : MonoBehaviour
{
    [Header("=== Cubes ===")]
    public Transform cube1;
    public Transform cube2;
    public Transform cube3;

    [Header("=== Network ===")]
    private TcpClient client;
    private NetworkStream stream;
    private string serverIP = "10.143.112.218";
    private int serverPort = 8080;

    [Header("=== XRI Input (Grippers) ===")]
    public InputActionReference rightIndexTrigger;
    public InputActionReference leftIndexTrigger;

    private bool isConnected = false;
    private float reconnectTimer = 0f;

    private Vector3 lastRot1, lastPos2, lastPos3, lastRot2, lastRot3;
    private float lastRightGrip = 0f;
    private float lastLeftGrip = 0f;

    void Start()
    {
        Debug.Log("🚀 Daiybotics Sender Started");
        TouchScreenKeyboard.Open(serverIP, TouchScreenKeyboardType.URL);
        StartCoroutine(WaitForIPInput());
    }

    private System.Collections.IEnumerator WaitForIPInput()
    {
        var keyboard = TouchScreenKeyboard.Open(serverIP, TouchScreenKeyboardType.URL);
        while (!keyboard.done) yield return null;

        if (keyboard.status == TouchScreenKeyboard.Status.Done && !string.IsNullOrEmpty(keyboard.text))
        {
            serverIP = keyboard.text.Trim();
        }

        Debug.Log($"📡 Target IP set to: {serverIP}");
        ConnectToPC();
    }

    private void ConnectToPC()
    {
        try
        {
            if (client != null)
            {
                client.Close();
                client = null;
            }

            client = new TcpClient();
            client.SendTimeout = 500;
            client.ReceiveTimeout = 500;

            Debug.Log($"🔄 Trying to connect to {serverIP}:{serverPort}...");

            var result = client.BeginConnect(serverIP, serverPort, null, null);
            bool success = result.AsyncWaitHandle.WaitOne(6000); // 6 seconds timeout

            if (success && client.Connected)
            {
                stream = client.GetStream();
                isConnected = true;
                Debug.Log($"✅ SUCCESSFULLY CONNECTED to {serverIP}:{serverPort}");
            }
            else
            {
                Debug.LogError($"❌ Connection FAILED / Timeout to {serverIP}");
                isConnected = false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Connection Exception: {e.Message}");
            isConnected = false;
        }
    }

    void Update()
    {
        // Try to reconnect if lost
        if (!isConnected)
        {
            reconnectTimer += Time.deltaTime;
            if (reconnectTimer > 5f)
            {
                reconnectTimer = 0f;
                ConnectToPC();
            }
            return;
        }

        string data = BuildDataString();
        if (!string.IsNullOrEmpty(data))
        {
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            try
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"⚠️ Send failed: {e.Message}");
                isConnected = false;
            }
        }
    }

    private string BuildDataString()
    {
        var sb = new System.Text.StringBuilder();

        if (cube1 != null)
        {
            var rot = GetNormalizedRotation(cube1);
            int rx = Mathf.RoundToInt(rot.x);
            int ry = Mathf.RoundToInt(rot.y);
            if (Mathf.Abs(rx - lastRot1.x) > 1 || Mathf.Abs(ry - lastRot1.y) > 1)
            {
                sb.Append($"C1R,{rx},{ry}\n");
                lastRot1 = new Vector3(rx, ry, 0);
            }
        }

        if (cube2 != null) AppendPosRot(sb, "C2", cube2, ref lastPos2, ref lastRot2);
        if (cube3 != null) AppendPosRot(sb, "C3", cube3, ref lastPos3, ref lastRot3);

        // Grippers
        float rightGrip = GetGripperValue(rightIndexTrigger);
        float leftGrip = GetGripperValue(leftIndexTrigger);

        if (Mathf.Abs(rightGrip - lastRightGrip) > 0.02f)
        {
            sb.Append($"GRIPR,{rightGrip:F3}\n");
            lastRightGrip = rightGrip;
        }
        if (Mathf.Abs(leftGrip - lastLeftGrip) > 0.02f)
        {
            sb.Append($"GRIPL,{leftGrip:F3}\n");
            lastLeftGrip = leftGrip;
        }

        return sb.ToString();
    }

    private void AppendPosRot(System.Text.StringBuilder sb, string prefix, Transform t, ref Vector3 lastPos, ref Vector3 lastRot)
    {
        Vector3 pos = t.position;
        Vector3 rot = GetNormalizedRotation(t);

        if (Vector3.Distance(pos, lastPos) > 0.01f || Vector3.Distance(rot, lastRot) > 1f)
        {
            sb.Append($"{prefix}P,{pos.x:F3},{pos.y:F3},{pos.z:F3}\n");
            sb.Append($"{prefix}R,{rot.x:F1},{rot.y:F1},{rot.z:F1}\n");
            lastPos = pos;
            lastRot = rot;
        }
    }

    private Vector3 GetNormalizedRotation(Transform t)
    {
        Vector3 e = t.localEulerAngles;
        float x = e.x > 180 ? e.x - 360 : e.x;
        float y = e.y > 180 ? e.y - 360 : e.y;
        float z = e.z > 180 ? e.z - 360 : e.z;

        x = Mathf.Clamp(x, -90f, 90f) + 90f;
        y = Mathf.Clamp(y, -90f, 90f) + 90f;
        z = Mathf.Clamp(z, -90f, 90f) + 90f;

        return new Vector3(x, y, z);
    }

    private float GetGripperValue(InputActionReference actionRef)
    {
        if (actionRef?.action == null) return 0f;
        return actionRef.action.ReadValue<float>();
    }

    void OnApplicationQuit()
    {
        if (stream != null) stream.Close();
        if (client != null) client.Close();
    }
}