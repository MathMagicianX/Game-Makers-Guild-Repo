using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BasicCommandBuffer : MonoBehaviour
{
    // Cache a reference to main camera.
    [SerializeField] Camera camera;  
    CommandBuffer commandBuffer;

    // This keeps track of the rendering screen size.
    Vector2 screenResolution = Vector2.zero;

    public void OnEnable()
    {
        RemoveCommandBuffer();
        InitializeCommandBuffer();
    }

    public void OnDisable()
    {
        RemoveCommandBuffer();
    }

    public void RemoveCommandBuffer()
    {
        // If we did not initialize the command buffer, we cannot remove it to begin with!
        if (!Initialized())
        {
            return;
        }

        // Remove the command buffer from the camera and set the command buffer object to null.
        camera.RemoveCommandBuffer(CameraEvent.BeforeForwardAlpha, commandBuffer);
        commandBuffer = null;
    }

    public void OnPreRender()
    {
        // If the rendering screen size changes, we have to remove the old command buffer.
        if (screenResolution != new Vector2(Screen.width, Screen.height))
        {
            RemoveCommandBuffer();
        }

        // Initialize the command buffer.  More often than not, if already initialized, nothing will happen.
        InitializeCommandBuffer();
    }

    // Checks to ensure we initialized the command buffer.
    public bool Initialized()
    {
        return commandBuffer != null;
    }

    private void InitializeCommandBuffer()
    {
        // If we already initialized it, don't reinitialize it!
        if (Initialized())
        {
            return;
        }

        // Get the Command Buffer ready.
        commandBuffer = new CommandBuffer();
        commandBuffer.name = "MyCommandBuffer";


        // Create an ID for referencing our render texture, _screenRenderTexture.
        int screenRenderTextureID = Shader.PropertyToID("_screenRenderTexture");
        // Create a blank render texture.
        commandBuffer.GetTemporaryRT(screenRenderTextureID, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
        // Populate the render texture with what the camera sees.
        commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, screenRenderTextureID);


        // Save _screenRenderTexture result to a global texture object.
        commandBuffer.SetGlobalTexture("_GlobalScreenRenderTexture", screenRenderTextureID);

        // Add the Command Buffer to the main camera.  We will execute the command buffer just before foward rendering on the Transparency stage of the render pipeline occurs.
        camera.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, commandBuffer);

        // Record current size of the rendering screen.
        screenResolution = new Vector2(Screen.width, Screen.height);
    }
}

