using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlphaWork
{
    public class Utils
    {

        /// <summary>
        /// Editor下绘制调试bounds，
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="color"></param>
        public static void DrawBounds(Bounds bounds, Color color)
        {
            Vector3 v3FrontTopLeft;
            Vector3 v3FrontTopRight;
            Vector3 v3FrontBottomLeft;
            Vector3 v3FrontBottomRight;
            Vector3 v3BackTopLeft;
            Vector3 v3BackTopRight;
            Vector3 v3BackBottomLeft;
            Vector3 v3BackBottomRight;

            Vector3 v3Center = bounds.center;
            Vector3 v3Extents = bounds.extents;

            v3FrontTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top left corner
            v3FrontTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top right corner
            v3FrontBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom left corner
            v3FrontBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom right corner
            v3BackTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top left corner
            v3BackTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top right corner
            v3BackBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom left corner
            v3BackBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom right corner

            Debug.DrawLine(v3FrontTopLeft, v3FrontTopRight, color);
            Debug.DrawLine(v3FrontTopRight, v3FrontBottomRight, color);
            Debug.DrawLine(v3FrontBottomRight, v3FrontBottomLeft, color);
            Debug.DrawLine(v3FrontBottomLeft, v3FrontTopLeft, color);

            Debug.DrawLine(v3BackTopLeft, v3BackTopRight, color);
            Debug.DrawLine(v3BackTopRight, v3BackBottomRight, color);
            Debug.DrawLine(v3BackBottomRight, v3BackBottomLeft, color);
            Debug.DrawLine(v3BackBottomLeft, v3BackTopLeft, color);

            Debug.DrawLine(v3FrontTopLeft, v3BackTopLeft, color);
            Debug.DrawLine(v3FrontTopRight, v3BackTopRight, color);
            Debug.DrawLine(v3FrontBottomRight, v3BackBottomRight, color);
            Debug.DrawLine(v3FrontBottomLeft, v3BackBottomLeft, color);
        }
        public static Transform FindTransformByName(Transform t, string name)
        {
            Transform[] ts = t.GetComponentsInChildren<Transform>();
            foreach (var tran in ts)
            {
                if (tran.name == name)
                {
                    return tran;
                }
            }
            return null;
        }

        

        /// <summary>
        /// Combine SkinnedMeshRenderers together and share one skeleton.
        /// Merge materials will reduce the drawcalls, but it will increase the size of memory. 
        /// </summary>
        /// <param name="skeleton">combine meshes to this skeleton(a gameobject)</param>
        /// <param name="meshes">meshes need to be merged</param>
        /// <param name="combine">merge materials or not</param>    
        public static void CombineObject(GameObject skeleton, SkinnedMeshRenderer[] meshes, bool combine = false)
        {

            // Fetch all bones of the skeleton
            List<Transform> transforms = new List<Transform>();
            transforms.AddRange(skeleton.GetComponentsInChildren<Transform>(true));

            List<Material> materials = new List<Material>();//the list of materials
            List<CombineInstance> combineInstances = new List<CombineInstance>();//the list of meshes
            List<Transform> bones = new List<Transform>();//the list of bones
            List<BoneWeight> boneWeights = new List<BoneWeight>();

            int boneOffset = 0;

            // Collect information from meshes
            for (int i = 0; i < meshes.Length; i++)
            {
                SkinnedMeshRenderer smr = meshes[i];

                if (smr.sharedMesh == null)
                    continue;

                materials.AddRange(smr.sharedMaterials); // Collect materials

                int subMeshIndex = 0;
                for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
                {

                    CombineInstance ci = new CombineInstance();
                    ci.mesh = smr.sharedMesh;
                    ci.subMeshIndex = subMeshIndex;
                    combineInstances.Add(ci);
                    subMeshIndex++;


                    BoneWeight[] meshBoneweight = smr.sharedMesh.boneWeights;

                    foreach (BoneWeight bw in meshBoneweight)
                    {
                        BoneWeight bWeight = bw;

                        bWeight.boneIndex0 += boneOffset;
                        bWeight.boneIndex1 += boneOffset;
                        bWeight.boneIndex2 += boneOffset;
                        bWeight.boneIndex3 += boneOffset;
                        boneWeights.Add(bWeight);
                    }


                    int boneAddCnt = 0;
                    // Collect bones
                    for (int j = 0; j < smr.bones.Length; j++)
                    {
                        Transform hit = null;
                        int tBase = 0;
                        for (tBase = 0; tBase < transforms.Count; tBase++)
                        {
                            if (smr.bones[j].name.Equals(transforms[tBase].name))
                            {
                                hit = transforms[tBase];

                                break;
                            }
                        }
                        if (hit)
                        {
                            bones.Add(hit);
                            boneAddCnt++;
                        }
                        else
                        {
                            Debug.LogError("failed to find bone " + smr.bones[j].name);
                        }
                    }
                    boneOffset += boneAddCnt;
                }

            }

            // Create a new SkinnedMeshRenderer
            SkinnedMeshRenderer oldSKinned = skeleton.GetComponent<SkinnedMeshRenderer>();
            if (oldSKinned != null)
            {
                GameObject.DestroyImmediate(oldSKinned);
            }
            SkinnedMeshRenderer r = skeleton.AddComponent<SkinnedMeshRenderer>();
            r.sharedMesh = new Mesh();
            r.sharedMesh.CombineMeshes(combineInstances.ToArray(), combine, false);// Combine meshes            

            r.bones = bones.ToArray();
            r.sharedMesh.boneWeights = boneWeights.ToArray();
            r.sharedMesh.RecalculateBounds();

            if (combine)
            {
                // Below informations only are used for merge materilas(bool combine = true)
                List<Vector2[]> oldUV = null;
                Material newMaterial = null;
                Texture2D newDiffuseTex = null;

                newMaterial = new Material(Shader.Find("Mobile/Diffuse"));
                oldUV = new List<Vector2[]>();

                const int COMBINE_TEXTURE_MAX = 512;
                const string COMBINE_DIFFUSE_TEXTURE = "_MainTex";


                // merge the texture
                List<Texture2D> Textures = new List<Texture2D>();
                for (int i = 0; i < materials.Count; i++)
                {
                    Textures.Add(materials[i].GetTexture(COMBINE_DIFFUSE_TEXTURE) as Texture2D);
                }

                newDiffuseTex = new Texture2D(COMBINE_TEXTURE_MAX, COMBINE_TEXTURE_MAX, TextureFormat.RGBA32, true);
                Rect[] uvs = newDiffuseTex.PackTextures(Textures.ToArray(), 0);
                newMaterial.mainTexture = newDiffuseTex;

                // reset uv
                Vector2[] uva, uvb;
                for (int j = 0; j < combineInstances.Count; j++)
                {
                    uva = (Vector2[])(combineInstances[j].mesh.uv);
                    uvb = new Vector2[uva.Length];
                    for (int k = 0; k < uva.Length; k++)
                    {
                        uvb[k] = new Vector2((uva[k].x * uvs[j].width) + uvs[j].x, (uva[k].y * uvs[j].height) + uvs[j].y);
                    }
                    oldUV.Add(combineInstances[j].mesh.uv);
                    combineInstances[j].mesh.uv = uvb;
                }
                r.material = newMaterial;
                for (int i = 0; i < combineInstances.Count; i++)
                {
                    combineInstances[i].mesh.uv = oldUV[i];
                }
            }
            else
            {
                r.materials = materials.ToArray();
            }
        }
    }
}