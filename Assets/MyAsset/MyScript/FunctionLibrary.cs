using static UnityEngine.Mathf;
using UnityEngine;

public static class FunctionLibrary
{
    public delegate Vector3 Function(float u, float v, float time); //この返り値と引数の関数の型を宣言


    public enum FunctionName {Wave, MultiWave, Ripple, Sphere , Torus}; // ただのｅｎｕｍ       
    public static Function[] functions = {Wave, MultiWave, Ripple, Sphere , Torus};
    public static int FunctionCount => functions.Length;

    public static Function GetFunction(FunctionName functionName)
    {
        return functions[(int)functionName];
    }
    public static FunctionName GetNextFnName(FunctionName name) {
		return ((int)name < functions.Length  - 1)? name + 1 : 0;
		}
    
    public static FunctionName GetOtherRandomFnName(FunctionName name) {
        var choice = (FunctionName)Random.Range(1, functions.Length);
        return choice == name ? 0: choice;
    }
    public static FunctionName GetRandomFnName(FunctionName name) {
        return (FunctionName)Random.Range(0, functions.Length);
    }

    public static Vector3 MultiWave(float u, float v, float time)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + 0.5f * time));
        p.y += 0.5f* Sin(2f * PI * (v+time));
        p.y += Sin(PI * (u + v + 0.25f * time));
        p.y += 1f/2.5f;
        p.z = v;
        return p;
    }

    public static Vector3 Wave(float u, float v, float time)
    {
        Vector3 p ;
        p.x= u;
        p.y = Sin(PI * (u + v + time));
        p.z = v ;
        return p;
    }

    public static Vector3 Ripple(float u, float v, float time)
    {
        float d = Sqrt(u * u + v * v);
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (4f * d - time));
        p.y /= 1f + 10f * d;
        p.z = v;
        return p;
    }

    public static Vector3 Sphere(float u, float v, float time)
    {
        Vector3 p;
        float r = 0.9f + Sin(PI * (6f * u+ 4f * v + time)) * 0.1f;
        float s = r * Cos(0.5f * PI * v);
        p.x = s * Sin(PI * u);
        p.y = r * Sin(PI * v);
        p.z = s * Cos(PI * u) * Cos(PI * v);
        return p;
    }

    public static Vector3 Torus (float u, float v, float time)
    {
        //float r = 1f;
        float r1  = 0.75f + 0.1f * Sin(PI * (6f * u+ 4f * v + time)); // + 0.1f ... からはねじれ
        float r2 = 0.25f+ 0.1f * Sin(PI * (8f * u+ 2f * v + time));
        float s = r1 + r2 * Cos(PI * v); // twist
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r2 * Sin( PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
    public static Vector3 Morph (
        float u , float v, float t, Function from, Function to , float progress
    ){
        return Vector3.LerpUnclamped(from(u,v,t), to(u,v,t), SmoothStep(0f, 1f,progress));
    } //clamp は最大、最小を設定してそれをはみ出さないようにする


}