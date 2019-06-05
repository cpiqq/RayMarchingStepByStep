using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class RayMarchCamera : MonoBehaviour
{
    [SerializeField]
    private Shader _shader;

    private Material _rayMarchMat;

    public Material rayMarchMat
    {
        get
        {
            if (this._rayMarchMat == null && this._shader != null)
            {
                this._rayMarchMat = new Material(this._shader);
                this._rayMarchMat.hideFlags = HideFlags.HideAndDontSave;
            }

            return this._rayMarchMat;
        }
    }

    private Camera _camera;

    public Camera camera
    {
        get
        {
            if (this._camera == null)
            {
                this._camera = GetComponent<Camera>();
            }

            return this._camera;
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (!rayMarchMat)
        {
            Graphics.Blit(src, dest);
            return;
        }
        
        rayMarchMat.SetMatrix("_CamFrustum", this.CamFrustum(this.camera));
        rayMarchMat.SetMatrix("_CamToWorld", this.camera.cameraToWorldMatrix );
        rayMarchMat.SetVector("_CamWorldSpace", this.camera.transform.position);
 


        RenderTexture.active = dest;
        GL.PushMatrix();
        GL.LoadOrtho();
        rayMarchMat.SetPass(0);
        GL.Begin(GL.QUADS);
        
        //BL
        GL.MultiTexCoord2(0, 0.0f,0.0f);
        GL.Vertex3(0.0f, 0.0f, 3.0f);
        //BR
        GL.MultiTexCoord2(0, 1.0f,0.0f);
        GL.Vertex3(1.0f, 0.0f, 2.0f);
        //TR
        GL.MultiTexCoord2(0, 1.0f,1.0f);
        GL.Vertex3(1.0f, 1.0f, 1.0f);
        //TL
        GL.MultiTexCoord2(0, 0.0f,1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.0f);
        
        GL.End();
        GL.PopMatrix();


    }

    private Matrix4x4 CamFrustum(Camera cam)
    {
        Matrix4x4 frustum = Matrix4x4.identity;

        float fov = Mathf.Tan((cam.fieldOfView * 0.5f) * Mathf.Deg2Rad );
        Vector3 goUp = Vector3.up * fov;
        Vector3 goRight = Vector3.right * fov * cam.aspect;

        Vector3 TL = -Vector3.forward - goRight + goUp;
        Vector3 TR = -Vector3.forward + goRight + goUp;
        Vector3 BR = -Vector3.forward + goRight - goUp;
        Vector3 BL = -Vector3.forward - goRight - goUp;
        
        frustum.SetRow(0, TL);
        frustum.SetRow(1, TR);
        frustum.SetRow(2, BR);
        frustum.SetRow(3, BL);
        

        return frustum;
    }
}
















