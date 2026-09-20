using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ShanHaiSpiritTrail.Editor
{
    // All gameplay UI is created at runtime, so scene dependency collection
    // cannot discover its default material. Refuse builds that would strip it.
    public sealed class RuntimeUiShaderCheck : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;
        public void OnPreprocessBuild(BuildReport report) => Validate();

        public static void Validate()
        {
            var material = Canvas.GetDefaultCanvasMaterial();
            var shader = material ? material.shader : null;
            if (!shader)
                throw new BuildFailedException("The default Canvas UI shader is unavailable.");

            var settings = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset");
            var serialized = new SerializedObject(settings[0]);
            var included = serialized.FindProperty("m_AlwaysIncludedShaders");
            for (int index = 0; index < included.arraySize; index++)
                if (included.GetArrayElementAtIndex(index).objectReferenceValue == shader) return;

            throw new BuildFailedException("Runtime UI requires " + shader.name
                + " in Graphics Settings > Always Included Shaders. Restore it before building.");
        }
    }
}
