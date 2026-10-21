using System.Linq;
using UnityEditor;
using UnityEngine;
namespace OpenWaters.Art.Editor {
public static class SocketDiagnostic {
public static void Run() {
HarborAssetBuilder.OpenPreview();
foreach(var t in Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Where(t=>t.name.Contains("Socket") || t.name=="FX_Wake" || t.name=="Cargo_Textiles")) {
Debug.Log($"SOCKET {t.name} world={t.position:F3} local={t.localPosition:F3} scale={t.lossyScale:F3} rotation={t.rotation.eulerAngles:F2} up={t.up:F3} forward={t.forward:F3}");
foreach(var r in t.GetComponentsInChildren<Renderer>())Debug.Log($"BOUNDS {r.name} {r.bounds}");
}
}
}}
