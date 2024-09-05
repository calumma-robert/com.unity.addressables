using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.SceneManagement;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
    /// <summary>
    /// Basic implementation of IInstanceProvider.
    /// </summary>
    public class InstanceProvider : IInstanceProvider
    {
        Dictionary<GameObject, AsyncOperationHandle<GameObject>> m_InstanceObjectToPrefabHandle = new Dictionary<GameObject, AsyncOperationHandle<GameObject>>();

        /// <inheritdoc/>
        public GameObject ProvideInstance(ResourceManager resourceManager, AsyncOperationHandle<GameObject> prefabHandle, InstantiationParameters instantiateParameters)
        {
            GameObject result = instantiateParameters.Instantiate(prefabHandle.Result);
            m_InstanceObjectToPrefabHandle.Add(result, prefabHandle);
            return result;
        }

        /// <inheritdoc/>
        public void ReleaseInstance(ResourceManager resourceManager, GameObject instance)
        {
            // I dont know why the dude before thought the guard is great, as it was basically throwing exceptions at the TryGetValue as it uses the '==' operator...
            if (instance == null)
                return;

            AsyncOperationHandle<GameObject> resource;
            if (!m_InstanceObjectToPrefabHandle.TryGetValue(instance, out resource))
            {
                Debug.LogWarningFormat("Releasing unknown GameObject {0} to InstanceProvider.", instance);
            }
            else
            {
                resource.Release();
                m_InstanceObjectToPrefabHandle.Remove(instance);
            }

            if (Application.isPlaying)
                Object.Destroy(instance);
            else
                Object.DestroyImmediate(instance);
        }
    }
}
