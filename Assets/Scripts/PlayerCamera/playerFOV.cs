using UnityEngine;

public class playerFOV : MonoBehaviour
{
    [SerializeField] private float baseFOV = 60f; // Base FOV value for a 16:9 aspect ratio
    [SerializeField] private float aspectRatioFactor = 1f; // Factor to adjust FOV based on the current aspect ratio
    [SerializeField] private Camera cam; // Reference to the camera component

    private void Awake()
    {
        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }
    }

    private void Start()
    {
        // Calculate the current aspect ratio
        float currentAspectRatio = (float)Screen.width / Screen.height;

        // Calculate the FOV adjustment factor based on the current aspect ratio
        float fovAdjustmentFactor = aspectRatioFactor / currentAspectRatio;

        // Calculate the final FOV value based on the base FOV and the FOV adjustment factor
        float finalFOV = baseFOV * fovAdjustmentFactor;

        // Calculate the consistent horizontal FOV based on the final FOV and the current aspect ratio
        float hFOV = 2f * Mathf.Atan(Mathf.Tan(Mathf.Deg2Rad * finalFOV / 2f) * currentAspectRatio) * Mathf.Rad2Deg;

        // Set the camera's FOV to the final FOV value for consistent vertical FOV
        cam.fieldOfView = finalFOV;

        // Set the camera's horizontal FOV to the calculated hFOV for consistency across different aspect ratios
        cam.projectionMatrix = Matrix4x4.Perspective(hFOV, currentAspectRatio, cam.nearClipPlane, cam.farClipPlane);
    }
    //private void Update()
    //{
    //    FovUpdate();
    //}
    //void FovUpdate()
    //{
    //    // Calculate the current aspect ratio
    //    float currentAspectRatio = (float)Screen.width / Screen.height;

    //    // Calculate the FOV adjustment factor based on the current aspect ratio
    //    float fovAdjustmentFactor = aspectRatioFactor / currentAspectRatio;

    //    // Calculate the final FOV value based on the base FOV and the FOV adjustment factor
    //    float finalFOV = baseFOV * fovAdjustmentFactor;

    //    // Calculate the consistent horizontal FOV based on the final FOV and the current aspect ratio
    //    float hFOV = 2f * Mathf.Atan(Mathf.Tan(Mathf.Deg2Rad * finalFOV / 2f) * currentAspectRatio) * Mathf.Rad2Deg;

    //    // Set the camera's FOV to the final FOV value for consistent vertical FOV
    //    cam.fieldOfView = finalFOV;

    //    // Set the camera's horizontal FOV to the calculated hFOV for consistency across different aspect ratios
    //    cam.projectionMatrix = Matrix4x4.Perspective(hFOV, currentAspectRatio, cam.nearClipPlane, cam.farClipPlane);
    //}
}
